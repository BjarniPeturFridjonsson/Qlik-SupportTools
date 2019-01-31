using System;
using System.Linq;

namespace Eir.Common.Search
{
    /// <summary>
    /// Helper for converting a type-safe SearchQueryBuilder to the serializable type SearchQuery.
    /// </summary>
    internal static class SearchQueryConverter
    {
        public static SearchQuery ToSearchQuery<TItem>(SearchQueryBuilder<TItem> searchQueryBuilder)
            where TItem : class
        {
            var searchQuery = new SearchQuery
            {
                Operator = searchQueryBuilder.Operator,
                Skip = searchQueryBuilder.SkipCount,
                Take = searchQueryBuilder.TakeCount,
            };

            AppendFieldComparers(searchQueryBuilder, searchQuery);

            foreach (OrderBy<TItem> orderBy in searchQueryBuilder.OrderBy)
            {
                searchQuery.OrderBy.Add(new OrderBy { FieldName = orderBy.FieldName, SortOrder = orderBy.SortOrder });
            }

            searchQuery.GroupBy.AddRange(searchQueryBuilder.GroupBy);

            return searchQuery;
        }

        private static void AppendFieldComparers<TItem>(FieldComparerGroup<TItem> sourceFieldComparerGroup, FieldComparerGroup destinationFieldComparerGroup)
            where TItem : class
        {
            if ((sourceFieldComparerGroup.Terms.Count() > 1) && (sourceFieldComparerGroup.Operator == LogicalOp.None))
            {
                throw new ArgumentException($"The logical operator may not be {nameof(LogicalOp.None)} when more than one term is specified!");
            }

            foreach (SearchTerm<TItem> sourceSearchTerm in sourceFieldComparerGroup.Terms)
            {
                FieldComparer<TItem> fieldComparer = sourceSearchTerm as FieldComparer<TItem>;
                if (fieldComparer != null)
                {
                    destinationFieldComparerGroup.FieldComparers.Add(new FieldComparer
                    {
                        FieldName = fieldComparer.FieldName,
                        Operator = fieldComparer.Operator,
                        CompareValue = fieldComparer.CompareValue
                    });
                }
                else
                {
                    FieldComparerGroup<TItem> sourceSubFieldComparerGroup = sourceSearchTerm as FieldComparerGroup<TItem>;
                    if (sourceSubFieldComparerGroup != null)
                    {
                        var destinationSubFieldComparerGroup = new FieldComparerGroup
                        {
                            Operator = sourceSubFieldComparerGroup.Operator
                        };

                        AppendFieldComparers(sourceSubFieldComparerGroup, destinationSubFieldComparerGroup);

                        if (destinationSubFieldComparerGroup.FieldComparers.Count + destinationSubFieldComparerGroup.FieldComparerGroups.Count > 0)
                        {
                            destinationFieldComparerGroup.FieldComparerGroups.Add(destinationSubFieldComparerGroup);
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown search term: {sourceSearchTerm?.GetType()}");
                    }
                }
            }
        }
    }
}