using System.Data;

namespace HermesSoftLab.EnterpriseLibrary.DatabaseUtility
{
    public class SqlCommandHelper
    {
        public static string PrepareParamName(string inputParamName)
        {
            return inputParamName.StartsWith("@") ? inputParamName : string.Concat("@", inputParamName);
        }

        public static T GetOutputValue<T>(IDbCommand command, string paramName)
        {
            return (T)((IDbDataParameter)command.Parameters[PrepareParamName(paramName)]).Value;
        }
    }
}