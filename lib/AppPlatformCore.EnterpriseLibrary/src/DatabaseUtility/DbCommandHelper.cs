using System.Data;
using System.Data.Common;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// General helper methods for database command objects
    /// </summary>
    public class DbCommandHelper
    {
        /// <summary>
        /// Append "@" at parameter name, if it doesn't starts with it
        /// </summary>
        /// <param name="inputParamName">Name of the command parameter</param>
        /// <returns></returns>
        public static string PrepareParamName(string inputParamName, IDbCommand command)
        {
            if (GetCommandClientType(command) == Database.DbClientType.SqlServer)
            {
                return inputParamName.StartsWith("@") ? inputParamName : string.Concat("@", inputParamName);
            }
            else
            {
                return inputParamName;
            }
        }

        /// <summary>
        /// Append "@" at parameter name, if it doesn't starts with it
        /// </summary>
        /// <param name="inputParamName">Name of the command parameter</param>
        /// <returns></returns>
        public static string PrepareParamName(string inputParamName, Database.DbClientType dbClientType)
        {
            if (dbClientType == Database.DbClientType.SqlServer)
            {
                return inputParamName.StartsWith("@") ? inputParamName : string.Concat("@", inputParamName);
            }
            else
            {
                return inputParamName;
            }
        }

        /// <summary>
        /// Get value from the output parameter
        /// </summary>
        /// <typeparam name="T">Type of the value to retrieve</typeparam>
        /// <param name="command">DbCommand containing parameter</param>
        /// <param name="paramName">Name of the paramter</param>
        /// <returns>Parameter output value</returns>
        public static T GetParameterValue<T>(IDbCommand command, string paramName)
        {
            return (T)((IDbDataParameter)command.Parameters[PrepareParamName(paramName, command)]).Value;
        }

        /// <summary>
        /// Translates SQL command from type SPCommand to text command
        /// </summary>
        /// <param name="commandToTranslate"></param>
        /// <param name="sqlTextCommandTemplate"></param>
        public static void TranslateSpCommandToTextCommand(DbCommand commandToTranslate, string sqlTextCommandTemplate)
        {
            if (commandToTranslate.CommandType == CommandType.Text)
            {
                commandToTranslate.CommandText = string.Format(sqlTextCommandTemplate, commandToTranslate.CommandText);
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("exec ");
            sqlCommandBuilder.Append(commandToTranslate.CommandText);
            sqlCommandBuilder.Append(" ");
            foreach (DbParameter parameter in commandToTranslate.Parameters)
            {
                sqlCommandBuilder.Append(string.Concat(parameter.ParameterName, " = ", parameter.ParameterName));
                if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
                {
                    sqlCommandBuilder.Append(" OUTPUT ");
                }
                sqlCommandBuilder.Append(",");
            }

            string sqlTextQuery = sqlCommandBuilder.ToString().TrimEnd(',');

            string commandText = !string.IsNullOrEmpty(sqlTextCommandTemplate)
                         ? string.Format(sqlTextCommandTemplate, sqlTextQuery)
                         : sqlTextQuery;

            commandToTranslate.CommandText = commandText;
            commandToTranslate.CommandType = CommandType.Text;
        }

        public static Database.DbClientType GetCommandClientType(IDbCommand command)
        {
            if (command is System.Data.OracleClient.OracleCommand)
            {
                return Database.DbClientType.OracleServer;
            }
            else
            {
                return Database.DbClientType.SqlServer;
            }

        }
    }
}