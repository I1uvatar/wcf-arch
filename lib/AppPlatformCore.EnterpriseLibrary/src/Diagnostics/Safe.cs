using System;
using System.Collections;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.Diagnostics
{
    /// <summary>
    /// Safe evaluator of long expressions
    /// </summary>
    public static class Safe
    {
        /// <summary>
        /// Get a value of an expression.
        /// If any part of the expression results in a null value, the NullReferenceExpression is swallowed
        /// and null value is returned as a result
        /// </summary>
        /// <typeparam name="T">must be a reference type</typeparam>
        /// <param name="expression">must not be null</param>
        /// <returns>Result of the <paramref name="expression"/> or null</returns>
        public static T Get<T>(Func<T> expression) where T : class
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            try
            {
                var value = expression();
                return value;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get a value of expression as an out paremeter.
        /// If any part of the expression results in a null value, the NullReferenceExpression is swallowed
        /// and null value is returned as a output parameter.
        /// </summary>
        /// <typeparam name="T">Must be a reference type.</typeparam>
        /// <param name="expression">Must not be null.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if value is not null, false otherwise.</returns>
        public static bool Get<T>(Func<T> expression, out T value) where T : class
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            value = null;
            
            try
            {
                value = expression();
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Get a value of an expression.
        /// If any part of the expression results in a null value, the NullReferenceExpression is swallowed
        /// and default value is returned as a result.
        /// </summary>
        /// <typeparam name="T">must be a value type</typeparam>
        /// <param name="expression">must not be null</param>
        /// <returns>Result of the <paramref name="expression"/> or default value of <typeparamref name="T"/></returns>
        public static T GetValue<T>(Func<T> expression) where T : struct
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            try
            {
                var value = expression();
                return value;
            }
            catch (NullReferenceException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get a value of an expression.
        /// If any part of the expression results in a null value, the NullReferenceExpression is swallowed
        /// and null value is returned as a result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">must not be null</param>
        /// <returns>Result of the <paramref name="expression"/> or null</returns>
        public static T? GetValue<T>(Func<T?> expression) where T : struct
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            try
            {
                var value = expression();
                return value;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks expression for nulls.
        /// </summary>
        /// <typeparam name="T">must be a reference type</typeparam>
        /// <param name="expression">must not be null</param>
        /// <returns>True if any part of the expression evaluates to null, false otherwise</returns>
        public static bool IsNull<T>(Func<T> expression) where T : class
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            try
            {
                var value = expression();
                return value == null;
            }
            catch (NullReferenceException)
            {
                return true;
            }
        }

        /// <summary>
        /// Checks expression for nulls.
        /// </summary>
        /// <typeparam name="T">must be a value type</typeparam>
        /// <param name="expression">must not be null</param>
        /// <returns>True if any part of the expression evaluates to null, false otherwise</returns>
        public static bool IsNullValue<T>(Func<T?> expression) where T : struct
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            try
            {
                var value = expression();
                return value == null;
            }
            catch (NullReferenceException)
            {
                return true;
            }
        }

        /// <summary>
        /// Safe execute the specified expression.
        /// </summary>
        /// <param name="expression">Expression to execute. This parameter must not be null.</param>
        /// <param name="exceptionHandler">Exception handler action.</param>
        /// <returns>Return <c>true</c> if execute complete without error. Otherwise return <c >false</c>.</returns>
        public static bool Execute(Action expression, Action<Exception> exceptionHandler)
        {
            Assert.IsNotNull(expression, "expression must not be <null>");
            try
            {
                expression();
                return true;
            }
            catch (Exception ex)
            {
                exceptionHandler(ex);
                return false;
            }
        }

        #region Basic methods checking only instance
        
        /// <summary>
        /// Instance is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(T instance) where T : class
        {
            return !IsNull(instance);
        }

        /// <summary>
        /// Nullable instance is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(T? instance) where T : struct
        {
            return !IsNull(instance);
        }

        /// <summary>
        /// Instance is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNull<T>(T instance) where T : class
        {
            return (instance == null);
        }

        /// <summary>
        /// Nullable instance is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNull<T>(T? instance) where T : struct
        {
            return (instance == null);
        }

        /// <summary>
        /// Returns true if collection is not null and contains items.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool HasItems(ICollection list)
        {
            return IsNotNull(list) && list.Count > 0; 
        }

        /// <summary>
        /// Returns true if collection is not null and contains items.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool HasItems(Func<ICollection> expression)
        {
            Assert.IsNotNull(expression, "expression must not be <null>");

            try
            {
                var value = expression();
                return HasItems(value);
            }
            catch (NullReferenceException)
            {
                return true;
            }
        }

        /// <summary>
        /// Returns if collection is not null and contains items.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IListHasItems<T>(IList<T> list)
        {
            return IsNotNull(list) && list.Count > 0;
        }

        /// <summary>
        /// Returns if collection is not null and contains items.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IListHasItems<T>(ICollection<T> list)
        {
            return IsNotNull(list) && list.Count > 0;
        }

        /// <summary>
        /// Return is value is null or DBNull. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDBNull(object value)
        {
            return (value == null || value == DBNull.Value);
        }

        /// <summary>
        /// Returns null or string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(object value)
        {
            return IsNull(value) ? null : value.ToString();
        }
        
        #endregion 
    }
}
