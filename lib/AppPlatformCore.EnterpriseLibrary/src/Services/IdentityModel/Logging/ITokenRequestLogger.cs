using System;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel.Logging
{
    public interface ITokenRequestLogger
    {
        void Log(TokenRequestLogData logData);
        bool ShallLogRequest();
        bool ShallLogFailedRequest();
        void LogCustomReqParamRead(TokenRequestParam requestParam, Exception ex);
        void LogCustomReqParamWrite(TokenRequestParam requestParam, Exception ex);
    }
}