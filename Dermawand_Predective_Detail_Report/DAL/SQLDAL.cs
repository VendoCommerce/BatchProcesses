using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DAL
{
    class SQLDAL
    {
        public static DataTable GetOrders(string spName, DateTime? start, DateTime? end)
        {
            string connstr = ConfigurationManager.AppSettings["connectionstring"];

            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            //LogData Err;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt =new DataTable();

            try
            {
                //intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = spName;
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                if (start.HasValue)
                {
                    SqlParameter startParam = new SqlParameter("@begin_dt", start);
                    oCmd.Parameters.Add(startParam);
                }
                if (end.HasValue)
                {
                    SqlParameter endParam = new SqlParameter("@end_dt", end);
                    oCmd.Parameters.Add(endParam);
                }

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);


            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                //Err = new LogData();
                //Err.LogToFile(strMessage);
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
            return dt;

        }

        public static DataTable GetSkus(string spName, int orderId)
        {
            string connstr = ConfigurationManager.AppSettings["connectionstring"];

            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            //LogData Err;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = new DataTable();

            try
            {
                //intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = spName;
                oCmd.CommandType = CommandType.StoredProcedure;

                SqlParameter startParam = new SqlParameter("@orderId", orderId);
                oCmd.Parameters.Add(startParam);

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);


            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                //Err = new LogData();
                //Err.LogToFile(strMessage);
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
            return dt;

        }


    }
}
