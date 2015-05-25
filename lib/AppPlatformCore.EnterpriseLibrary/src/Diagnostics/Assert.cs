using System;
using System.Collections;

namespace AppPlatform.Core.EnterpriseLibrary.Diagnostics
{
    /// <summary>
    /// Assertions helper
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Throws NullReferenceException when <paramref name="test"/> is null.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        public static void IsNotNull(object test, string message)
        {
            if (test == null)
            {
                throw new NullReferenceException(message);
            }
        }

        /// <summary>
        /// Throws ArgumentNullException when <paramref name="test"/> is null.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="argumentName"></param>
        public static void ArgIsNotNull(object test, string argumentName)
        {
            if (test == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws NullReferenceException when <paramref name="test"/> is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="test"></param>
        /// <param name="message"></param>
        public static void HasValue<T>(T? test, string message) where T : struct
        {
            if (!test.HasValue)
            {
                throw new NullReferenceException(message);
            }
        }

        /// <summary>
        /// Throws NullReferenceException when <paramref name="list"/> is null or has no items.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void HasItems(ICollection list, string message)
        {
            if (list == null || list.Count == 0)
                throw new NullReferenceException(message);
        }
        
        /// <summary>
        /// Throws InvalidOperationException when <paramref name="test"/> is false.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        public static void IsTrue(bool test, string message)
        {
            if (!test)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throws InvalidOperationException when <paramref name="test"/> is true.        
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        public static void IsFalse(bool test, string message)
        {
            if (test)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throws InvalidOperationException when <paramref name="test"/> is empty string.
        /// Throws NullReferenceException when <paramref name="test"/> is null.        
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        public static void IsNotNullOrEmpty(string test, string message)
        {
            if (test == null)
            {
                throw new NullReferenceException(message);
            }
            if (test.Length == 0)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Used for checking the arguments of methods.
        /// Throws ArgumentNullException when <paramref name="argument"/> is null.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="message"></param>
        public static void ArgumentIsNotNull(object argument, string message)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(String.IsNullOrEmpty(message) ? "Argument shouldn't be null!" : message );
            }
        }

        /// <summary>
        /// Used for checking the string arguments of methods.
        /// Throws InvalidOperationException when <paramref name="argument"/> is empty string.
        /// Throws NullReferenceException when <paramref name="argument"/> is null.        
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="message"></param>
        public static void ArgumentIsNotNullOrEmpty(string argument, string message)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(String.IsNullOrEmpty(message) ? "Argument shouldn't be null!" : message);
            }
            if (argument.Length == 0)
            {
                throw new ArgumentException(String.IsNullOrEmpty(message) ? "Argument shouldn't be empty!" : message);
            }
        }
    }   
}
