using CSBusiness.PostSale;
using CSCore.DataHelper;
using CSPaymentProvider;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace VOIDBatch.Application_Code.DataAccess
{
    class ClientDAL
    {

        private static List<PaymentProviderSetting> GetAllProvidersFromDB(bool activeList)
        {
            List<PaymentProviderSetting> providerList = new List<PaymentProviderSetting>();
            using (SqlDataReader reader = GetAllProviders(activeList))
            {
                while (reader.Read())
                {
                    PaymentProviderType type = PaymentProviderType.EPayAccount;
                    if (System.Enum.TryParse<PaymentProviderType>(reader["ProviderName"].ToString(), out type))
                    {
                        PaymentProviderSetting item = new PaymentProviderSetting();
                        item.ProviderID = (int)reader["ProviderId"];
                        item.ProviderType = type;
                        item.Title = type.ToString();
                        item.ProviderXML = reader["ProviderXml"].ToString();
                        item.Active = Convert.ToBoolean(reader["Active"]);
                        item.IsDefault = Convert.ToBoolean(reader["IsDefault"]);
                        providerList.Add(item);
                    }

                }
            }
            return providerList;
        }

        /// <summary>
        /// Get the default payment provider
        /// </summary>
        /// <returns></returns>
        public static IPaymentProvider GetDefaultProvider()
        {
            List<PaymentProviderSetting> providerList = GetAllProvidersFromDB(true);
            for (int i = 0; i < providerList.Count; i++)
            {
                PaymentProviderSetting providerSetting = providerList[i];
                if (providerSetting.Active)
                {
                    if (providerSetting.IsDefault)
                    {
                        return PaymentProviderFactory.CreateProvider(providerSetting.ProviderType, providerSetting.ProviderXML);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get all providers from client database.
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        private static SqlDataReader GetAllProviders(bool active)
        {
            //////    /////// TODO: connectionString should be made from the active client database, using SiteID and Databasename
            string connectionString = ConfigHelper.GetDBConnection();
            //////    ////////////////////////////////////////////////////////
            String ProcName = "pr_get_all_provider";
            SqlParameter[] Parameters = new SqlParameter[1];
            Parameters[0] = new SqlParameter("@active", active);
            return BaseSqlHelper.ExecuteReader(connectionString, ProcName, Parameters);

        }
    }
}
