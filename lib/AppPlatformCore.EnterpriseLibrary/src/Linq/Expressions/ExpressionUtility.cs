using System;
using System.Linq;
using System.Linq.Expressions;

namespace AppPlatform.Core.EnterpriseLibrary.Linq.Expressions
{
    /// <summary>
    /// Various helpers for work with expressions
    /// </summary>
    public static class ExpressionUtility
    {
        /// <summary>
        /// Gets the name of the last property in the <para>getter</para> expression.
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static string GetPropertyName<Source>(Expression<Func<Source, object>> getter)
        {
            // Look at page http://msdn.microsoft.com/en-us/library/system.linq.expressions.aspx for linq expression types
            if (getter.Body is MemberExpression)
            {
                return (getter.Body as MemberExpression).Member.Name;
            }

            if (getter.Body is UnaryExpression)
            {
                var aBody = (UnaryExpression)getter.Body;
                var anOperand = (MemberExpression)aBody.Operand;
                return anOperand.Member.Name;
            }

            throw new ArgumentException(String.Format("Expression type {0} not supported.", getter.Body.GetType()), "getter");
        }

        /// <summary>
        /// Gets a complete path to the property in the <para>getter</para> expression.
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static string GetPropertyPath<Source>(Expression<Func<Source, object>> getter)
        {
            // Look at page http://msdn.microsoft.com/en-us/library/system.linq.expressions.aspx for linq expression types
            if (getter.Body is MemberExpression)
            {
                string body = (getter.Body as MemberExpression).ToString();

                char[] path = body.SkipWhile(ch => !ch.Equals('.')).Skip(1).ToArray();
                return new string(path);
            }

            if (getter.Body is UnaryExpression)
            {
                var aBody = (UnaryExpression)getter.Body;
                var anOperand = aBody.Operand.ToString();

                char[] path = anOperand.SkipWhile(ch => !ch.Equals('.')).Skip(1).ToArray();
                return new string(path);                
            }

            throw new ArgumentException(String.Format("Expression type {0} not supported.", getter.Body.GetType()), "getter");
        }
    }
}
