using System;

namespace AppPlatform.Core.EnterpriseLibrary.Object
{
    public class Activator
    {
        /// <summary>
        /// Returns <typeparamref name="T"/>T instance. 
        /// May return null is type T and typeName don't refer to the same type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(string typeName) where T : class
        {
            Type aType = Type.GetType(typeName, true);
            var anInstance = System.Activator.CreateInstance(aType);
            return anInstance as T;
        }
    }
}
