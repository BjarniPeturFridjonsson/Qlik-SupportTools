using System.Globalization;
using Ciloci.Flee;

namespace Gjallarhorn.Common
{
    public class RuleValidator
    {
        public bool ValidateValue(double value, string validationExpression)
        {
            var expressionContext = new ExpressionContext();
            expressionContext.Options.ParseCulture = CultureInfo.InvariantCulture;
            expressionContext.Variables.Clear();
            expressionContext.Variables.Add("value", value);

            var expression = expressionContext.CompileGeneric<bool>(validationExpression);

            return expression.Evaluate();
        }
    }
}
