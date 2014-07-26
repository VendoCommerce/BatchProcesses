using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using Com.ConversionSystems.Utility;
using Com.ConversionSystems.DataAccess;


namespace Com.ConversionSystems.DataAccess
{
    public class SQLServerDAL : SQLServerDAO
    {
        private const string _FILENAME = "SQLServerDAL.cs";

       
        //Basic Coordinator Functions ________________________________________

        public bool Get_NoNo_Web_Report_Table(DateTime start, DateTime end, out DataTable dt)
        {
            return GetOrders("Get_NoNo_Web_Report", start, end, out dt);
        }

        public bool GetOrders (string spName, DateTime start, DateTime end, out DataTable dt)        
        {            
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            LogData Err;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            dt = null;

            try
            {
                //intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = spName;
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                SqlParameter startParam = new SqlParameter("@StartDate", start);
                oCmd.Parameters.Add(startParam);

                SqlParameter endParam = new SqlParameter("@EndDate", end);
                oCmd.Parameters.Add(endParam);


                oCmd.Connection = this.Connection;
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);

                bReturn = Helper.HasData(dt);

            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                Err = new LogData();
                Err.LogToFile(strMessage);
                //Err = new LogData(Helper.AppName, _FILENAME, "GetCoordinator(...):bool", intPhase, strMessage, ex);

            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
    
    }
}