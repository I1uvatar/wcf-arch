namespace AppPlatform.Core.EnterpriseLibrary.Services.Helper
{
    public class NameHelper
    {
        public static void GetOperationAndContractName(string action, out string contractName, out string operationName)
        {
            contractName = null;
            operationName = null;

            if (string.IsNullOrEmpty(action))
            {
                return;
            }

            var elements = action.Split('/');

            if (elements.Length < 2)
            {
                return;
            }

            operationName = elements[elements.Length - 1];
            contractName = elements[elements.Length - 2];
        }
    }
}
