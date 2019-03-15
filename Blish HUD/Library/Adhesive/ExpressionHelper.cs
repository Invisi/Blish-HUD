using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Blish_HUD.Library.Adhesive {
    internal static class ExpressionHelper {

        internal static object EvaluateExpression(Expression expression) {
            if (expression.NodeType == ExpressionType.Constant) {
                return ((ConstantExpression)expression).Value;
            }

            var lambda = Expression.Lambda(expression, Enumerable.Empty<ParameterExpression>());
            return lambda.Compile().DynamicInvoke();
        }

        public static Action<TSource> MakeAssignmentAction<TSource>(MethodInfo @set, object instance) {
            var f = (Action<TSource>)Delegate.CreateDelegate(typeof(Action<TSource>), instance, @set);
            return t => f(t);
        }

    }
}
