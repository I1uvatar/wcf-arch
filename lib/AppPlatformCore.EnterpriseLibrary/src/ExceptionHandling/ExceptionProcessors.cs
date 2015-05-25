using System;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.ExceptionHandling
{
    public class ExceptionProcessors : Dictionary<Type, IExceptionProcessor>
    {
        public void ProcessException(Exception ex, bool processAsBaseType)
        {
            if (ex == null)
            {
                return;
            }

            var exceptionType = ex.GetType();

            if (!this.Process(ex, exceptionType) && processAsBaseType)
            {
                if (exceptionType.IsGenericType && !this.Process(ex, exceptionType.GetGenericTypeDefinition()))
                {
                    this.ProcessBaseType(exceptionType, ex);
                }
            }
        }

        private void ProcessBaseType(Type exceptionType, Exception ex)
        {
            Type exceptionBaseType = exceptionType.BaseType;

            while (exceptionType.BaseType != typeof(Exception) && !this.Process(ex, exceptionBaseType))
            {
                exceptionBaseType = exceptionBaseType.BaseType;
            }
        }

        private bool Process(Exception ex, Type typeToHandle)
        {
            if (this.ContainsKey(typeToHandle))
            {
                this[typeToHandle].ProcessException(ex);
                return true;
            }

            return false;
        }
    }
}
