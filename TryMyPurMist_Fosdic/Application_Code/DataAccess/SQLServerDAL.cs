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
        public bool GetOrdersForXMLBatch(DateTime CreationDate, out DataTable dt)
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
                oCmd.CommandText = "dbo.GetOrdersForFullFillMent";
                oCmd.CommandType = CommandType.StoredProcedure;

                // set the parameters
                //intPhase = 1;
                //oCmd.Parameters.Add(new SqlParameter("@ipCreationDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, CreationDate));

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
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---"+ex.Message;
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
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---"+ex.Message;
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
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---"+ex.Message;
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
        public bool GetSequenceNumber(out DataTable dt)
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
                oCmd.CommandText = "select * from [sequencenumber] order by SequenceNumber desc";
                oCmd.CommandType = CommandType.Text;

                
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
        public bool AddSequenceNumber(int sequencenumber)
        {
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            LogData Err;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

             

            try
            {
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "insert into [sequencenumber](sequencenumber) values ('" + sequencenumber + "')";
                oCmd.CommandType = CommandType.Text;


                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.ExecuteNonQuery();

                return true;

            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                Err = new LogData();
                Err.LogToFile(strMessage);
                return false;
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

        public bool UpdateOrderStatus(string OrderID )
        {
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            LogData Err;
            SqlCommand oCmd = null;
            
            try
            {
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = " update [Order] set [OrderStatusId]=11, [BatchtoFile] =1 where OrderID=" + OrderID;
                oCmd.CommandType = CommandType.Text;

                intPhase = 2;
                oCmd.Connection = this.Connection;
                oCmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                Err = new LogData();
                Err.LogToFile(strMessage);
                return false; 
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