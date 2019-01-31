using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eir.Common.Common;

namespace Eir.Common.Search
{
    public static class SearchExpressionBuilder
    {
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";

        public static Expression<Func<T, bool>> CreateSearchExpression<T>(SearchQuery searchQuery, SearchIntent searchIntent)
            where T : class
        {
            ParameterExpression param = Expression.Parameter(typeof(T));

            Expression search = ExpressionFromFieldComparerGroup<T>(searchQuery, param, searchIntent);

            return Expression.Lambda<Func<T, bool>>(search, param);
        }

        private static PropertyInfo GetPropertyWithValidation<T>(string fieldName)
        {
            var result = typeof(T).GetProperty(fieldName);

            if (result == null)
            {
                throw new PropertyNotFoundException($"Property '{fieldName}' was not found on type '{typeof(T).FullName}'");
            }

            return result;
        }

        private static Expression ExpressionFromFieldComparerGroup<T>(FieldComparerGroup fieldComparerGroup, ParameterExpression param, SearchIntent searchIntent)
            where T : class
        {
            Expression search = null;

            foreach (FieldComparer fieldComparer in fieldComparerGroup.FieldComparers)
            {
                // The expression tree we are building
                PropertyInfo propertyInfo = GetPropertyWithValidation<T>(fieldComparer.FieldName);

                if (propertyInfo == null)
                {
                    throw new Exception($"The column '{fieldComparer.FieldName}' does not exist in my dataclass");
                }

                object convertedSearchValue = GetConvertedSearchValue(propertyInfo, fieldComparer);

                Expression searchWithType;

                // ReSharper disable once PossibleNullReferenceException
                if (propertyInfo.PropertyType == typeof(Guid) && Guid.Empty.Equals((Guid)convertedSearchValue))
                {
                    searchWithType = Expression.Field(null, typeof(Guid), "Empty");
                    convertedSearchValue = Guid.Empty;
                }
                else
                {
                    searchWithType = Expression.Constant(convertedSearchValue);
                }

                MemberExpression memberExpression = Expression.PropertyOrField(param, fieldComparer.FieldName);

                Expression body;
                switch (fieldComparer.Operator)
                {
                    case CompareOp.Equals:
                        body = SearcherExpressionExtensions.Equals(memberExpression, searchWithType, searchIntent);
                        break;
                    case CompareOp.NotEquals:
                        body = SearcherExpressionExtensions.NotEqual(memberExpression, searchWithType);
                        break;
                    case CompareOp.GreaterThanOrEqual:
                        body = SearcherExpressionExtensions.GreaterThanOrEqual(memberExpression, searchWithType);
                        break;
                    case CompareOp.LessThanOrEqual:
                        body = SearcherExpressionExtensions.LessThanOrEqual(memberExpression, searchWithType);
                        break;
                    case CompareOp.LessThan:
                        body = SearcherExpressionExtensions.LessThan(memberExpression, searchWithType);
                        break;
                    case CompareOp.GreaterThan:
                        body = SearcherExpressionExtensions.GreaterThan(memberExpression, searchWithType);
                        break;
                    //case CompareOp.EqualsCaseSensitive:
                    //    body = SearcherExpressionExtensions.Equal(memberExpression, searchWithType);
                    //    break;
                    case CompareOp.StartsWith:
                        body = SearcherExpressionExtensions.StartsWith(memberExpression, searchWithType, searchIntent);
                        break;
                    //case CompareOp.StartsWithCaseSensitive:
                    //    body = SearcherExpressionExtensions.StartsWith(memberExpression, searchWithType);
                    //    break;
                    case CompareOp.Contains:
                        body = SearcherExpressionExtensions.Contains(memberExpression, searchWithType, searchIntent);
                        break;
                    //case CompareOp.ContainsCaseSensitive:
                    //    body = SearcherExpressionExtensions.Contains(memberExpression, searchWithType);
                    //    break;
                    case CompareOp.EndsWith:
                        body = SearcherExpressionExtensions.EndsWith(memberExpression, searchWithType, searchIntent);
                        break;
                    //case CompareOp.EndsWithOrdinalIgnoreCase:
                    //    body = SearcherExpressionExtensions.EndsWithOrdinalIgnoreCase(memberExpression, searchWithType);
                    //    break;
                    case CompareOp.Match:
                        body = SearcherExpressionExtensions.Match(memberExpression, convertedSearchValue, searchIntent);
                        break;
                    default:
                        throw new Exception($"Search term {fieldComparer.Operator} not supported");
                }

                if (search == null)
                {
                    search = body;
                }
                else
                {
                    search = fieldComparerGroup.Operator == LogicalOp.And
                        ? Expression.AndAlso(search, body)
                        : Expression.OrElse(search, body);
                }
            }

            foreach (FieldComparerGroup subFieldComparerGroup in fieldComparerGroup.FieldComparerGroups)
            {
                var subExpression = ExpressionFromFieldComparerGroup<T>(subFieldComparerGroup, param, searchIntent);
                if (search == null)
                {
                    search = subExpression;
                }
                else
                {
                    search = fieldComparerGroup.Operator == LogicalOp.And
                        ? Expression.AndAlso(search, subExpression)
                        : Expression.OrElse(search, subExpression);
                }
            }

            return search ?? Expression.Constant(true);
        }

        private static object GetConvertedSearchValue(PropertyInfo propertyInfo, FieldComparer fieldComparer)
        {
            Type propertyType = propertyInfo.PropertyType;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (fieldComparer.CompareValue == null)
                {
                    return null;
                }

                propertyType = propertyType.GetGenericArguments()[0];
            }

            if (propertyType == typeof(DateTime))
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(
                    fieldComparer.CompareValue,
                    new[] { DATE_TIME_FORMAT, FormatStrings.DATE_AND_TIME_WITH_MILLISECONDS, FormatStrings.UTC_DATE_AND_TIME_WITH_MILLISECONDS },
                    null,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out dateTime))
                {
                    return dateTime;
                }

                throw new FormatException($"The string '{fieldComparer.CompareValue}' cannot be parsed to DateTime. Expected format is '{DATE_TIME_FORMAT}'.");
            }

            TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
            return converter.ConvertFrom(fieldComparer.CompareValue);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, SearchQuery searchQuery, SearchIntent searchIntent)
            where T : class
        {
            if (searchQuery.GroupBy.Count > 0)
            {
                throw new NotSupportedException($"{nameof(searchQuery.GroupBy)} is not supported for filtering {nameof(SearchQuery)}.");
            }

            Func<T, bool> predicate = CreateSearchExpression<T>(searchQuery, searchIntent).Compile();

            items = items
                .Where(predicate)
                .OrderBy(searchQuery.OrderBy);

            if (searchQuery.Skip > 0)
            {
                items = items.Skip(searchQuery.Skip);
            }

            if (searchQuery.Take > 0)
            {
                items = items.Take(searchQuery.Take);
            }

            return items;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IEnumerable<OrderBy> orderBys)
        {
            IOrderedEnumerable<T> orderedItems = null;

            Type itemType = typeof(T);

            foreach (OrderBy orderBy in orderBys)
            {
                ParameterExpression lambdaParameterExpression = Expression.Parameter(itemType, "x");
                PropertyInfo propertyInfo = GetPropertyWithValidation<T>(orderBy.FieldName);
                Type propertyType = propertyInfo.PropertyType;
                bool isDescending = orderBy.SortOrder == SortOrder.Descending;

                ConstantExpression comparerExpression;
                if (propertyType == typeof(string))
                {
                    comparerExpression = Expression.Constant(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    object defaultComparer = typeof(Comparer<>)
                        .MakeGenericType(propertyType)
                        .GetProperties()
                        .Single(x => x.Name == "Default")
                        .GetValue(null);

                    comparerExpression = Expression.Constant(defaultComparer);
                }

                if (orderedItems == null)
                {
                    ParameterExpression itemsParamExpression = Expression.Parameter(typeof(IEnumerable<T>), "theItems");

                    MethodInfo sortMethodinfo = isDescending ? _orderByDescendingMethod : _orderByMethod;
                    sortMethodinfo = sortMethodinfo.MakeGenericMethod(itemType, propertyType);

                    LambdaExpression keySelectorExpression = Expression.Lambda(
                        typeof(Func<,>).MakeGenericType(itemType, propertyType),
                        Expression.MakeMemberAccess(lambdaParameterExpression, propertyInfo),
                        lambdaParameterExpression);

                    MethodCallExpression sortMethodCallExpression = Expression.Call(
                        sortMethodinfo,
                        itemsParamExpression,
                        keySelectorExpression,
                        comparerExpression);

                    var sortMethodExpression = Expression.Lambda<Func<IEnumerable<T>, IOrderedEnumerable<T>>>(
                        sortMethodCallExpression,
                        itemsParamExpression);

                    orderedItems = sortMethodExpression.Compile()(source);
                }
                else
                {
                    ParameterExpression orderedItemsParamExpression = Expression.Parameter(typeof(IOrderedEnumerable<T>), "theOrderedItems");

                    MethodInfo sortMethodinfo = isDescending ? _thenByDescendingMethod : _thenByMethod;
                    sortMethodinfo = sortMethodinfo.MakeGenericMethod(itemType, propertyType);

                    LambdaExpression keySelectorExpression = Expression.Lambda(
                        typeof(Func<,>).MakeGenericType(itemType, propertyType),
                        Expression.MakeMemberAccess(lambdaParameterExpression, propertyInfo),
                        lambdaParameterExpression);

                    MethodCallExpression sortMethodCallExpression = Expression.Call(
                        sortMethodinfo,
                        orderedItemsParamExpression,
                        keySelectorExpression,
                        comparerExpression);

                    var sortMethodExpression = Expression.Lambda<Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>>>(
                        sortMethodCallExpression,
                        orderedItemsParamExpression);

                    orderedItems = sortMethodExpression.Compile()(orderedItems);
                }
            }

            return orderedItems ?? source;
        }

        private static readonly MethodInfo _orderByMethod = GetGenericMethodDefinition(() => default(IEnumerable<object>).OrderBy(default(Func<object, object>), Comparer<object>.Default));

        private static readonly MethodInfo _orderByDescendingMethod = GetGenericMethodDefinition(() => default(IEnumerable<object>).OrderByDescending(default(Func<object, object>), Comparer<object>.Default));

        private static readonly MethodInfo _thenByMethod = GetGenericMethodDefinition(() => default(IOrderedEnumerable<object>).ThenBy(default(Func<object, object>), Comparer<object>.Default));

        private static readonly MethodInfo _thenByDescendingMethod = GetGenericMethodDefinition(() => default(IOrderedEnumerable<object>).ThenByDescending(default(Func<object, object>), Comparer<object>.Default));

        private static MethodInfo GetGenericMethodDefinition<T>(Expression<Func<T>> method)
        {
            MethodCallExpression methodCallExpression = (MethodCallExpression)method.Body;
            return methodCallExpression.Method.GetGenericMethodDefinition();
        }
    }
}