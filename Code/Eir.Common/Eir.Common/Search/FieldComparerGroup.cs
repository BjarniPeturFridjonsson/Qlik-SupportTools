using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eir.Common.Common;
using Eir.Common.Extensions;

namespace Eir.Common.Search
{
    /// <summary>
    /// A nested combination of FieldComparers and other FieldComparerGroups.
    /// Serializable.
    /// </summary>
    public class FieldComparerGroup
    {
        public FieldComparerGroup()
        {
            FieldComparers = new List<FieldComparer>();
            FieldComparerGroups = new List<FieldComparerGroup>();
        }

        public List<FieldComparer> FieldComparers { get; }

        public List<FieldComparerGroup> FieldComparerGroups { get; }

        public LogicalOp Operator { get; set; }

        public override string ToString()
        {
            string[] terms = FieldComparers.Select(x => x.ToString()).Concat(FieldComparerGroups.Select(x => x.ToString())).ToArray();
            return terms.Length == 1
                ? terms[0]
                : "(" + string.Join($" {Operator.GetDescription()} ", terms) + ")";
        }
    }

    /// <summary>
    /// A nested combination of FieldComparers and other FieldComparerGroups.
    /// Used by the SearchQueryBuilder.
    /// </summary>
    public class FieldComparerGroup<TItem> : SearchTerm<TItem>
        where TItem : class
    {
        private readonly List<SearchTerm<TItem>> _terms = new List<SearchTerm<TItem>>();

        internal FieldComparerGroup(FieldComparerGroup<TItem> parent, LogicalOp op)
        {
            Parent = parent;
            Operator = op;
        }

        public FieldComparerGroup<TItem> Parent { get; }

        public LogicalOp Operator { get; }

        public IEnumerable<SearchTerm<TItem>> Terms => _terms;

        public FieldComparerGroup<TItem> AddComparer<TValue>(Expression<Func<TItem, TValue>> field, CompareOp op, TValue value)
        {
            if ((Operator == LogicalOp.None) && (_terms.Count > 0))
            {
                throw new ArgumentException($"You may not add more than one comparer when the logical operator is {LogicalOp.None}.");
            }

            _terms.Add(new FieldComparer<TItem>(GetFieldName(field), op, GetCompareValueString(value)));
            return this;
        }

        private static string GetCompareValueString<TValue>(TValue value)
        {
            Type valueType = typeof(TValue);

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return null;
                }

                valueType = valueType.GetGenericArguments()[0];
            }

            if (valueType == typeof(DateTime))
            {
                return ((DateTime)(object)value).ToString(FormatStrings.DATE_AND_TIME_WITH_MILLISECONDS);
            }

            return value?.ToString();
        }

        public FieldComparerGroup<TItem> AddComparerGroup(LogicalOp op)
        {
            if ((Operator == LogicalOp.None) && (_terms.Count > 0))
            {
                throw new ArgumentException($"You may not add more than one comparer when the logical operator is {LogicalOp.None}.");
            }

            var searchTerms = new FieldComparerGroup<TItem>(this, op);
            _terms.Add(searchTerms);
            return searchTerms;
        }

        public override string ToString()
        {
            return _terms.Count == 1
                ? _terms[0].ToString()
                : "(" + string.Join($" {Operator.GetDescription()} ", _terms) + ")";
        }

        public SearchQueryBuilder<TItem> Builder
        {
            get
            {
                var parent = Parent ?? this;
                while (parent?.Parent != null)
                {
                    parent = parent.Parent;
                }

                SearchQueryBuilder<TItem> searchQueryBuilder = parent as SearchQueryBuilder<TItem>;
                if (searchQueryBuilder == null)
                {
                    throw new ArgumentException($"Missing {typeof(SearchQueryBuilder<TItem>)}");
                }

                return searchQueryBuilder;
            }
        }

        public SearchQuery ToSearchQuery()
        {
            return SearchQueryConverter.ToSearchQuery(Builder);
        }
    }
}