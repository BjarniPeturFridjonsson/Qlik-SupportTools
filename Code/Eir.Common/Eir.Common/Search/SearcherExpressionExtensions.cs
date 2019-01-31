using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Eir.Common.Extensions;

namespace Eir.Common.Search
{
    public static class SearcherExpressionExtensions
    {
        private static readonly MethodInfo _stringToUpperMethod = typeof(string).GetMethod("ToUpper", new Type[] { });
        private static readonly MethodInfo _staticStringEqualsMethod = typeof(string).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string), typeof(string), typeof(StringComparison) }, null);
        private static readonly MethodInfo _stringStartsWithWithoutStringComparisonMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo _stringContainsWithoutStringComparisonMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo _stringStartsWithWithStringComparisonMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _stringEndsWithWithStringComparisonMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _stringEndsWithWithoutStringComparisonMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        private static void FixExpressions(ref Expression firstExpression, ref Expression secondExpression)
        {
            if (firstExpression == null)
            {
                throw new ArgumentNullException(nameof(firstExpression));
            }

            if (secondExpression == null)
            {
                throw new ArgumentNullException(nameof(secondExpression));
            }

            var isFirstExpressionNullable = IsNullableExpression(firstExpression);
            var isSecondExpressionNullable = IsNullableExpression(secondExpression);

            if (isFirstExpressionNullable && !isSecondExpressionNullable)
            {
                secondExpression = FixExpressionType(secondExpression, firstExpression);
            }
            else if (!isFirstExpressionNullable && isSecondExpressionNullable)
            {
                firstExpression = FixExpressionType(firstExpression, secondExpression);
            }
        }

        public static Expression GreaterThan(Expression firstExpression, Expression secondExpression)
        {
            FixExpressions(ref firstExpression, ref secondExpression);
            return Expression.GreaterThan(firstExpression, secondExpression);
        }

        public static Expression GreaterThanOrEqual(Expression firstExpression, Expression secondExpression)
        {
            FixExpressions(ref firstExpression, ref secondExpression);
            return Expression.GreaterThanOrEqual(firstExpression, secondExpression);
        }

        public static Expression LessThan(Expression firstExpression, Expression secondExpression)
        {
            FixExpressions(ref firstExpression, ref secondExpression);
            return Expression.LessThan(firstExpression, secondExpression);
        }

        public static Expression LessThanOrEqual(Expression firstExpression, Expression secondExpression)
        {
            FixExpressions(ref firstExpression, ref secondExpression);
            return Expression.LessThanOrEqual(firstExpression, secondExpression);
        }

        public static Expression Equal(Expression firstExpression, Expression secondExpression)
        {
            FixExpressions(ref firstExpression, ref secondExpression);
            return Expression.Equal(firstExpression, secondExpression);
        }

        public static Expression NotEqual(Expression firstExpression, Expression secondExpression)
        {
            FixExpressions(ref firstExpression, ref secondExpression);
            return Expression.NotEqual(firstExpression, secondExpression);
        }

        private static Expression NullCheckedMethodCall(Expression memberExpression, Expression actualCompareExpression)
        {
            return Expression.Condition(
                Expression.Equal(memberExpression, Expression.Constant(null)),
                Expression.Constant(false),
                actualCompareExpression);
        }


        public static Expression Equals(Expression memberExpression, Expression searchFor, SearchIntent searchIntent)
        {
            if (searchIntent == SearchIntent.Undefined)
            {
                throw new ArgumentException($"{nameof(searchIntent)} cannot be Undefined", nameof(searchIntent));
            }

            if (searchFor.Type != typeof(string))
            {
                return Equal(memberExpression, searchFor);
            }

            return searchIntent == SearchIntent.Database
                ? GetDatabaseStringEquals(memberExpression, searchFor)
                : GetInMemoryStringEquals(memberExpression, searchFor);
        }

        private static Expression GetInMemoryStringEquals(Expression memberExpression, Expression searchFor)
        {
            return Expression.Call(_staticStringEqualsMethod, new[] { memberExpression, searchFor, Expression.Constant(StringComparison.OrdinalIgnoreCase) });
        }

        private static Expression GetDatabaseStringEquals(Expression memberExpression, Expression searchFor)
        {
            return Equal(Expression.Call(memberExpression, _stringToUpperMethod), Expression.Call(searchFor, _stringToUpperMethod));
        }

        public static Expression StartsWith(Expression memberExpression, Expression searchFor, SearchIntent searchIntent)
        {
            if (memberExpression.Type != searchFor.Type)
            {
                throw new ArgumentException($"{nameof(memberExpression)} and {nameof(searchFor)} must be expressions of same type. {nameof(memberExpression)} is of type {memberExpression.Type.ToPrettyString()} and {nameof(searchFor)} is of type {searchFor.Type.ToPrettyString()}.");
            }

            if (memberExpression.Type != typeof(string))
            {
                throw new ArgumentException($"{nameof(StartsWith)} can be used for strings only.");
            }

            return searchIntent == SearchIntent.Database
                ? GetDatabaseStartsWith(memberExpression, searchFor)
                : GetInMemoryStartsWith(memberExpression, searchFor);
        }

        private static Expression GetDatabaseStartsWith(Expression memberExpression, Expression searchFor)
        {
            return Expression.Call(
                memberExpression,
                _stringStartsWithWithoutStringComparisonMethod, searchFor);
        }

        private static Expression GetInMemoryStartsWith(Expression memberExpression, Expression searchFor)
        {
            return NullCheckedMethodCall(
                memberExpression,
                Expression.Call(
                    memberExpression,
                    _stringStartsWithWithStringComparisonMethod,
                    searchFor,
                    Expression.Constant(StringComparison.OrdinalIgnoreCase)));
        }


        public static Expression Contains(Expression memberExpression, Expression searchFor, SearchIntent searchIntent)
        {
            if (memberExpression.Type != searchFor.Type)
            {
                throw new ArgumentException($"{nameof(memberExpression)} and {nameof(searchFor)} must be expressions of same type. {nameof(memberExpression)} is of type {memberExpression.Type.ToPrettyString()} and {nameof(searchFor)} is of type {searchFor.Type.ToPrettyString()}.");
            }

            if (memberExpression.Type != typeof(string))
            {
                throw new ArgumentException($"{nameof(Contains)} can be used for strings only.");
            }

            return searchIntent == SearchIntent.Database
                ? GetDatabaseContains(memberExpression, searchFor)
                : GetInMemoryContains(memberExpression, searchFor);
        }

        private static Expression GetDatabaseContains(Expression memberExpression, Expression searchFor)
        {
            return Expression.Call(
                memberExpression,
                _stringContainsWithoutStringComparisonMethod, searchFor);
        }

        private static Expression GetInMemoryContains(Expression memberExpression, Expression searchFor)
        {
            return NullCheckedMethodCall(
                memberExpression,
                Expression.GreaterThan(
                    Expression.Call(
                        memberExpression,
                        typeof(string).GetMethod("IndexOf",
                            new[] { typeof(string), typeof(StringComparison) }), searchFor,
                        Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                    Expression.Constant(0)));
        }

        public static Expression EndsWith(Expression memberExpression, Expression searchFor, SearchIntent searchIntent)
        {
            if (memberExpression.Type != searchFor.Type)
            {
                throw new ArgumentException($"{nameof(memberExpression)} and {nameof(searchFor)} must be expressions of same type. {nameof(memberExpression)} is of type {memberExpression.Type.ToPrettyString()} and {nameof(searchFor)} is of type {searchFor.Type.ToPrettyString()}.");
            }

            if (memberExpression.Type != typeof(string))
            {
                throw new ArgumentException($"{nameof(Contains)} can be used for strings only.");
            }


            return searchIntent == SearchIntent.Database
                ? GetDatabaseEndsWith(memberExpression, searchFor)
                : GetInMemoryEndsWith(memberExpression, searchFor);
        }

        private static Expression GetDatabaseEndsWith(Expression memberExpression, Expression searchFor)
        {
            return Expression.Call(
                memberExpression,
                _stringEndsWithWithoutStringComparisonMethod, searchFor);
        }

        private static Expression GetInMemoryEndsWith(Expression memberExpression, Expression searchFor)
        {
            return NullCheckedMethodCall(
                memberExpression,
                Expression.Call(
                    memberExpression,
                    _stringEndsWithWithStringComparisonMethod,
                    searchFor,
                    Expression.Constant(StringComparison.OrdinalIgnoreCase)));
        }


        public static Expression Match(Expression memberExpression, object searchValue, SearchIntent searchIntent)
        {
            string matchText = searchValue?.ToString() ?? string.Empty;

            const char wildcardChar = '*';
            string wildcardString = wildcardChar.ToString();

            bool matchTextStartsWithWildCard = matchText.StartsWith(wildcardString);
            bool matchTextEndsWithWildCard = matchText.EndsWith(wildcardString);

            if (matchTextStartsWithWildCard && matchTextEndsWithWildCard)
            {
                return Contains(memberExpression, Expression.Constant(matchText.Trim(wildcardChar)), searchIntent);
            }

            if (matchTextStartsWithWildCard)
            {
                return EndsWith(memberExpression, Expression.Constant(matchText.TrimStart(wildcardChar)), searchIntent);
            }

            if (matchTextEndsWithWildCard)
            {
                return StartsWith(memberExpression, Expression.Constant(matchText.TrimEnd(wildcardChar)), searchIntent);
            }

            return Equals(memberExpression, Expression.Constant(matchText), searchIntent);
        }


        /****************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************/
        //
        /****************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************/


        private static bool IsNullableExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            //does not work because its expression....
            return (expression.Type == typeof(object)) ||
                   (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static Expression FixExpressionType(Expression notNullableExpression, Expression nullableExpression)
        {
            if (notNullableExpression == null)
            {
                throw new ArgumentNullException(nameof(notNullableExpression));
            }

            if (nullableExpression == null)
            {
                throw new ArgumentNullException(nameof(nullableExpression));
            }

            if (IsNullableExpression(notNullableExpression))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Not nullable expression is nullable: [{0}].",
                        notNullableExpression.Type.AssemblyQualifiedName ?? notNullableExpression.Type.FullName),
                    nameof(notNullableExpression));
            }

            if (!IsNullableExpression(nullableExpression))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Nullable expression is not nullable: [{0}].",
                        nullableExpression.Type.AssemblyQualifiedName ?? nullableExpression.Type.FullName),
                    nameof(nullableExpression));
            }

            if (!AreSameType(nullableExpression, notNullableExpression))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Supplied expressions are of different types. Not nullable expression is [{0}], nullable expression is [{1}].",
                        notNullableExpression.Type.AssemblyQualifiedName ?? notNullableExpression.Type.FullName,
                        nullableExpression.Type.AssemblyQualifiedName ?? nullableExpression.Type.FullName));
            }

            return Expression.Convert(notNullableExpression, nullableExpression.Type);
        }

        private static bool AreSameType(Expression nullableExpression, Expression notNullableExpression)
        {
            if (nullableExpression == null)
            {
                throw new ArgumentNullException(nameof(nullableExpression));
            }

            if (notNullableExpression == null)
            {
                throw new ArgumentNullException(nameof(notNullableExpression));
            }

            return Nullable.GetUnderlyingType(nullableExpression.Type) == notNullableExpression.Type;
        }
    }
}