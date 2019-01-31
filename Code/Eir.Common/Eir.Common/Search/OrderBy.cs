using System.Data.SqlClient;

namespace Eir.Common.Search
{
    /// <summary>
    /// 'Order by'; giving the name of a class field.
    /// Serializable.
    /// </summary>
    public class OrderBy
    {
        public string FieldName { get; set; }

        public SortOrder SortOrder { get; set; }

        public override string ToString()
        {
            return $"Order by {FieldName} {(SortOrder == SortOrder.Descending ? "Desc" : "Asc")}";
        }
    }

    /// <summary>
    /// 'Order by'; giving the name of a class field.
    /// Used by the SearchQueryBuilder. (Immutable.)
    /// </summary>
    public class OrderBy<TItem>
        where TItem : class
    {
        internal OrderBy(string fieldName, SortOrder sortOrder)
        {
            FieldName = fieldName;
            SortOrder = sortOrder;
        }

        public string FieldName { get; }

        public SortOrder SortOrder { get; }

        public override string ToString()
        {
            return $"Order by {FieldName} {(SortOrder == SortOrder.Descending ? "Desc" : "Asc")}";
        }
    }
}