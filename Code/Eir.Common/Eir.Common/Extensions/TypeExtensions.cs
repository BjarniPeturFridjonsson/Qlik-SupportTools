using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Eir.Common.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Extracts the <see cref="MethodInfo"/> for the call made in the <paramref name="action"/> expression,
        /// constructing a generic <see cref="MethodInfo"/> object based on the type <paramref name="type"/>, and
        /// returns that object.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to create a generic <see cref="MethodInfo"/>.</param>
        /// <param name="action">The expression from which to extract the <see cref="MethodInfo"/> to use as
        /// generic template. If it is not a generic method, the operation will fail.</param>
        public static MethodInfo GetConstructedMethodInfo(this Type type, Expression<Action> action)
        {
            if (action.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException("Parameter action is not a Call expression", "action");
            }

            MethodInfo calledMethod = ((MethodCallExpression)action.Body).Method;
            if (!calledMethod.IsGenericMethod)
            {
                throw new ArgumentException("Expected a call to a generic method in the call expression.", "action");
            }

			return calledMethod.GetGenericMethodDefinition().MakeGenericMethod(type);
		}

        public static string ToPrettyString(this Type type, bool shortNames = false)
        {
            Func<Type, string> nameFunc = shortNames
                ? new Func<Type, string> (t => t.Name)
                : new Func < Type, string> (t => t.FullName);

            if (type.IsConstructedGenericType)
            {
                var tickPos = nameFunc(type).IndexOf("`");
                return $"{nameFunc(type).Substring(0, tickPos)}<{string.Join(", ", type.GenericTypeArguments.Select(gta => gta.ToPrettyString(shortNames)))}>";
            }

            return nameFunc(type);
        }
    }
}