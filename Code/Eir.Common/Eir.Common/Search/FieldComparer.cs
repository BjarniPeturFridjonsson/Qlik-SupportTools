using Eir.Common.Extensions;

namespace Eir.Common.Search
{
    /// <summary>
    /// Wrapper for comparing a class field with a constant value.
    /// Serializable.
    /// </summary>
    public class FieldComparer
    {
        public string FieldName { get; set; }

        public CompareOp Operator { get; set; }

        public string CompareValue { get; set; }

        public override string ToString()
        {
            return $"({FieldName} {Operator.GetDescription()} \"{CompareValue}\")";
        }
    }

    /// <summary>
    /// Wrapper for comparing a class field with a constant value.
    /// Used by the SearchQueryBuilder.
    /// </summary>
    public class FieldComparer<TItem> : SearchTerm<TItem>
        where TItem : class
    {
        internal FieldComparer(string fieldName, CompareOp op, string compareValue)
        {
            FieldName = fieldName;
            Operator = op;
            CompareValue = compareValue;
        }

        public string FieldName { get; }

        public CompareOp Operator { get; }

        public string CompareValue { get; }

        public override string ToString()
        {
            return $"({FieldName} {Operator.GetDescription()} \"{CompareValue}\")";
        }
    }
}