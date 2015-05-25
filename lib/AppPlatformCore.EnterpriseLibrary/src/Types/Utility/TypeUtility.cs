using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppPlatform.Core.EnterpriseLibrary.Types.Utility
{
    public class TypeUtility
    {
        public static bool IsListOf<T>(object data)
        {
            if (data == null)
            {
                return false;
            }

            var type = data.GetType();

            var destType = typeof(T);

            if (type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];

                if ((destType.IsInterface && elementType.GetInterface(destType.FullName) != null)
                    || destType.IsAssignableFrom(elementType))
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool CheckIfListAndCreateBlank<T>(object data, out IList listOfOriginalType, out IList<T> listOfT)
            where T : class
        {
            listOfT = null;
            listOfOriginalType = null;

            if (data == null)
            {
                return false;
            }

            var type = data.GetType();

            var destType = typeof (T);
            
            if (type.GetGenericTypeDefinition() == typeof(List<>))
            {
                listOfOriginalType = Activator.CreateInstance(type) as IList;

                var elementType = type.GetGenericArguments()[0];

                if ((destType.IsInterface && elementType.GetInterface(destType.FullName) != null)
                    || destType.IsAssignableFrom(elementType))
                {
                    listOfT = (data as IEnumerable).OfType<T>().ToList();

                    return true;
                }

                return false;
            }
            return false;
        }
    }
}
