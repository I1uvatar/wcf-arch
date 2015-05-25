using System;

namespace AppPlatform.Core.EnterpriseLibrary.ExceptionHandling
{
    public interface IExceptionProcessor
    {
        void ProcessException(Exception ex);
    }
}