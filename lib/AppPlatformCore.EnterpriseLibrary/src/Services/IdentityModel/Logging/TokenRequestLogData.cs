using System;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Types;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel.Logging
{
    [Serializable]
    public class TokenRequestLogData
    {
        public string Endpoint { get; set; }
        public bool ReissueToken { get; set; }
        public DateTime? TokenValidityExpiration { get; set; }
        public DateTime TokenRequestTime { get; set; }
        public List<Pair<string>> CustomRequestParameters { get; set; }
        public bool HasFailed { get; set; }
    }
}