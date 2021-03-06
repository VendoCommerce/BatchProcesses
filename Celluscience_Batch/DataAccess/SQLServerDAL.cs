﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using ConversionSystems.Celluscience.Utility;
using ConversionSystems.Celluscience.DataAccess;

namespace ConversionSystems.Celluscience.DataAccess
{
    public class SQLServerDAL : SQLServerDAO
    {
        public bool GetOrderNumbers(DateTime CreationDate, out DataTable dt)
        {
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            LogData Err = new LogData();
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            dt = null;

            try
            {
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "SELECT OrderId FROM [Order] where CONVERT(varchar(8), Created, 112) ='"  + CreationDate.ToString("yyyyMMdd")+"'";
                oCmd.CommandType = CommandType.Text;

                // set the parameters
                intPhase = 1;

                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.Prepare();

                intPhase = 3;
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);

            }
            catch (Exception ex)
            {
                strMessage = "Problem running sql query:  " + oCmd.CommandText + ". Error---" + ex.Message;
               
                Err.LogToFile(strMessage);
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

        public bool GetOrdersForTxtBatch(DateTime CreationDate, out DataTable dt)
        {
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
          //  LogData Err;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            dt = null;

            try
            {
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "dbo.GetOrders";
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                intPhase = 1;
                oCmd.Parameters.Add(new SqlParameter("@ipCreationDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, CreationDate));

                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.Prepare();

                intPhase = 3;
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);

            //    bReturn = Helper.HasData(dt);

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
            return bReturn;
        }

        public bool GetOrderSKU(int OrderID, out DataTable dt)
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
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "dbo.GetOrderSKU";
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                intPhase = 1;
                oCmd.Parameters.Add(new SqlParameter("@ipOrderID", SqlDbType.Int, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, OrderID));

                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.Prepare();

                intPhase = 3;
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

        public bool GetCustomerAddress(int AddressID, out DataTable dt)
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
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "dbo.GetCustomerAddress";
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                intPhase = 1;
                oCmd.Parameters.Add(new SqlParameter("@ipAddressID", SqlDbType.Int, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, AddressID));

                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.Prepare();

                intPhase = 3;
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

        public bool GetSalesTaxRate(string State, out DataTable dt)
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
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "dbo.GetSalesTaxRate";
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                intPhase = 1;
                oCmd.Parameters.Add(new SqlParameter("@ipState", SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, State));

                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.Prepare();

                intPhase = 3;
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
                //Err = new LogData(Helper.AppName, _FILENAME, "GetSalesTaxRate(...):bool", intPhase, strMessage, ex);

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
