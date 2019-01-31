using System.Collections.Generic;
using System.Linq;

namespace Eir.Common.Search
{
    /// <summary>
    /// A database search query.
    /// Serializable.
    /// </summary>
    public class SearchQuery : FieldComparerGroup
    {
        public SearchQuery()
        {
            OrderBy = new List<OrderBy>();
            GroupBy = new List<string>();
        }

        public int Skip { get; set; }

        public int Take { get; set; }

        public List<OrderBy> OrderBy { get; }

        public List<string> GroupBy { get; }

        public override string ToString()
        {
            var parts = new List<string>();
            if (Skip > 0)
            {
                parts.Add("Skip " + Skip);
            }

            if (Take > 0)
            {
                parts.Add("Take " + Take);
            }

            parts.Add(base.ToString());

            parts.AddRange(OrderBy.Select(x => x.ToString()));

            parts.AddRange(GroupBy.Select(x => $"Group by {x}"));

            return string.Join("; ", parts);
        }
    }
}