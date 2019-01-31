using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Eir.Common.Search
{
    /// <summary>
    /// Type-safe builder for a database search query.
    /// </summary>
    public class SearchQueryBuilder<TItem> : FieldComparerGroup<TItem>
        where TItem : class
    {
        private readonly List<OrderBy<TItem>> _orderBy = new List<OrderBy<TItem>>();
        private readonly List<string> _groupBy = new List<string>();

        public SearchQueryBuilder(LogicalOp op = LogicalOp.None)
            : base(null, op)
        {
        }

        public IEnumerable<OrderBy<TItem>> OrderBy => _orderBy;

        public IEnumerable<string> GroupBy => _groupBy;

        public int SkipCount { get; private set; }

        public int TakeCount { get; private set; }

        public SearchQueryBuilder<TItem> AddOrderBy<TValue>(Expression<Func<TItem, TValue>> field, SortOrder sortOrder = SortOrder.Ascending)
        {
            _orderBy.Add(new OrderBy<TItem>(GetFieldName(field), sortOrder));
            return this;
        }

        public SearchQueryBuilder<TItem> AddGroupBy<TValue>(Expression<Func<TItem, TValue>> field)
        {
            _groupBy.Add(GetFieldName(field));
            return this;
        }

        public SearchQueryBuilder<TItem> Skip(int skip)
        {
            SkipCount = skip;
            return this;
        }

        public SearchQueryBuilder<TItem> Take(int take)
        {
            TakeCount = take;
            return this;
        }

        public override string ToString()
        {
            var parts = new List<string>();
            if (SkipCount > 0)
            {
                parts.Add("Skip " + SkipCount);
            }

            if (TakeCount > 0)
            {
                parts.Add("Take " + TakeCount);
            }

            parts.Add(base.ToString());

            parts.AddRange(OrderBy.Select(x => x.ToString()));

            parts.AddRange(GroupBy.Select(x => $"Group by {x}"));

            return string.Join("; ", parts);
        }
    }
}