using System;
using System.Linq.Expressions;

namespace Eir.Common.Search
{
    /// <summary>
    /// A type-safe (abstract) search term; like the XXX in the search query "XXX or XXX or (XXX and XXX)".
    /// </summary>
    public abstract class SearchTerm<TItem>
        where TItem : class
    {
        protected static string GetFieldName<TValue>(Expression<Func<TItem, TValue>> field)
        {
            return ((MemberExpression)field.Body).Member.Name;
        }
    }
}