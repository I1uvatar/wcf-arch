using System;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    public class DbStoredProcedureNamesMap
    {

        private static Dictionary<string, string> columnNamesMap;

        /// <summary>
        /// <par
        /// </summary>
        static DbStoredProcedureNamesMap()
        {
            columnNamesMap = new Dictionary<string, string>();
            // custom added mapping
            //from UserManager
            columnNamesMap.Add("UserInsert", "UserInsert");
            columnNamesMap.Add("UserUpdate", "UserUpdate");
            columnNamesMap.Add("UserSelect", "UserSelect");
            columnNamesMap.Add("UserDelete", "UserDelete");
            columnNamesMap.Add("UserSelectByProfessionalID", "UserSelectByProfessionalID");
            columnNamesMap.Add("UserSelectByWindowsIdentity", "UserSelectByWindowsIdentity");
            columnNamesMap.Add("UsersSelectByRoleCode", "UsersSelectByRoleCode");
            columnNamesMap.Add("UserSelectAll", "UserSelectAll");
            columnNamesMap.Add("UserSelectByUserTranID", "UserSelectByUserTranID");

            //from HealthCareProfessionalManager
            columnNamesMap.Add("HealthCareProfessionalUpdate", "HealthCareProfessionalUpdate");
            columnNamesMap.Add("HealthCareProfessionalInsert", "HealthCareProfessionalInsert");
            columnNamesMap.Add("HealthCareProfessionalDelete", "HealthCareProfessionalDelete");
            columnNamesMap.Add("HealthCareProfessionalSelect", "HealthCareProfessionalSelect");
            columnNamesMap.Add("HealthCareProfessionalByIVZCode", "HealthCareProfessionalByIVZCode");
            columnNamesMap.Add("HealthCareProfessionalByIdentityUserName", "HealthCareProfByIdUserName");
            columnNamesMap.Add("HealthCareProfessionalByWorkplaceTranIDAndRole", "HealthCareProfessionalByWorkplaceTranIDAndRole");
            columnNamesMap.Add("HealthCareProfessionalSelectByUserTranID", "HealthCareProfessionalSelectByUserTranID");
            columnNamesMap.Add("HealthCareProfessionalSelectAll", "HealthCareProfessionalSelectAll");
            columnNamesMap.Add("HealthCareProfessionalByIVZServiceAndRole", "HealthCareProfessionalByIVZServiceAndRole");
            columnNamesMap.Add("HealthCareProfessionalByIVZService", "HealthCareProfessionalByIVZService");
            columnNamesMap.Add("HealthCareProfessionalByRole", "HealthCareProfessionalByRole");
            columnNamesMap.Add("HealthCareProfessionalSelectAllActiveAndFutureActive", "HealthCareProfessionalSelectAllActiveAndFutureActive");
            columnNamesMap.Add("HealthCareProfessionalSelectByTranID", "HealthCareProfessionalSelectByTranID");

            //from HealthCareIvzServiceManager
            columnNamesMap.Add("HealthCareIvzServicesByProfessionalTranID", "HealthCareIvzSvcsByProfTranID");
            columnNamesMap.Add("AllHealthCareIvzServices", "AllHealthCareIvzServices");
            columnNamesMap.Add("HealthCareIvzServicesSelectWithSameSecurityGroupByIvzServiceTranID", "HealthCareIvzServicesSelectWithSameSecurityGroupByIvzServiceTranID");

            // from HealthCareSpecializationManager
            columnNamesMap.Add("HealthCareProfessionalsSpecialitiesByProfessionalIVZCode", "HCareProfSpecByProIVZCode");
            

            // from HealthCareWorkplaceManager
            columnNamesMap.Add("HealthCareWorkplaceSelect", "HealthCareWorkplaceSelect");
            columnNamesMap.Add("HealthCareWorkplaceSelectAll", "HealthCareWorkplaceSelectAll");
            columnNamesMap.Add("HealthCareWorkplaceSelectByTranID", "HealthCareWorkplaceSelectByTranID");
            columnNamesMap.Add("HealthCareWorkplacelByProviderIDAndProfessionalID", "HealthCareWorkplacelByProviderIDAndProfessionalID");
            columnNamesMap.Add("HealthCareWorkplacelByProfessionalID", "HealthCareWorkplacelByProfessionalID");
            columnNamesMap.Add("HealthCareWorkPlacesGetByProfessionalIVZCode", "HealthCareWorkPlacesGetByProfessionalIVZCode");
            columnNamesMap.Add("HealthCareWorkPlacesAllowedToByProfessionalTranID", "HCareWorkPlAllwdToByProfTranID");
            columnNamesMap.Add("HealthCareWorkPlacesDefaultByProfessionalTranID", "HealthCareWorkPlacesDefaultByProfessionalTranID");
            columnNamesMap.Add("HealthCareWorkplaceByCode", "HealthCareWorkplaceByCode");
            columnNamesMap.Add("HealthCareWorkplacesByIVZServiceAndProviderID", "HealthCareWorkplacesByIVZServiceAndProviderID");
            columnNamesMap.Add("WorkPlaceManager.SelectLogging", "WorkPlaceManager.SelectLogging");
            columnNamesMap.Add("HealthCareWorkplaceInsert", "HealthCareWorkplaceInsert");
            columnNamesMap.Add("HealthCareWorkplaceUpdate", "HealthCareWorkplaceUpdate");
            columnNamesMap.Add("HealthCareWorkplaceDelete", "HealthCareWorkplaceDelete");


            // generated from list


        }

        /// <summary>
        /// Returns mapped (short) name if DBType is Oracle, else returns input parameter name value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbClientType"></param>
        /// <returns></returns>
        public static String GetDbName(string name, Database.DbClientType dbClientType)
        {
            if (dbClientType == Database.DbClientType.OracleServer)
            {
                try
                {
                    return columnNamesMap[name];
                }
                catch (KeyNotFoundException ex)
                {
                    // if no value then return back name
                    return name;
                }
            }
            else
            {
                return name;
            }
        }
    }
}
