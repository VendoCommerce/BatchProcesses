using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace NoNoHairReconciliationReport
{
    class Batch
    {
       private string targetPath = "C:/BatchFiles/TryNoNO/Hair/";
       private string targetPathLog = "C:/BatchFiles/TryNoNO/Log/Log.txt";
       

        // For 226 server setup
       // private string targetPath = "D:/BatchProcesses/NoNoHairandSkin_ReconciliationReport/Hair/";       
       // private string targetPathLog = "D:/BatchProcesses/NoNoHairandSkin_ReconciliationReport/Log/Log.txt";

        private DataTable _NewTempDataTable = null;
        public DataTable NewTempDataTable
        {
            get { return _NewTempDataTable; }
            set { _NewTempDataTable = value; }
        }

        private void CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            DataColumn myDataColumn;
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "excelFileName";
            myDataTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "fileNameOnly";
            myDataTable.Columns.Add(myDataColumn);
            NewTempDataTable = myDataTable;
        }

        // create table rows dds
        private void AddDataToTable(string excelFileName, string fileNameOnly)
        {
            DataRow row;
            row = NewTempDataTable.NewRow();
            row["excelFileName"] = excelFileName;
            row["fileNameOnly"] = fileNameOnly;
            NewTempDataTable.Rows.Add(row);
        }   
  
        static DataSet getsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDev"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
            adp.Fill(ds);
            conn.Close();
            return ds;
        }

        static DataTable getDataTableByDate(string StoredProcedureName, DateTime startDate, DateTime endDate , int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDev"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate ;                
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                Batch Batch2 = new Batch();
                Batch2.LogToFile(strMessage);
                SendExceptionEmail(ex);
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


        private DataTable getDataTableByDate_MYNONO(string StoredProcedureName, DateTime startDate, DateTime endDate, int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringMyNoNo"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                SendExceptionEmail(ex);
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



        private DataTable getDataTableByDate_NoNoHairTV(string StoredProcedureName, DateTime startDate, DateTime endDate, int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstring_NoNoHairTV"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                SendExceptionEmail(ex);
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

        private DataTable getDataTableByDate_TryKyro(string StoredProcedureName, DateTime startDate, DateTime endDate, int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstring_TryKyro"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                SendExceptionEmail(ex);
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

        private DataTable getDataTableByDate_Kyrobak_UK(string StoredProcedureName, DateTime startDate, DateTime endDate, int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstring_Kyrobak.Co.Uk"];
            bool bReturn = false;
            string strMessage;
            SqlCommand oCmd = null;
            SqlDataAdapter da;
            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                SendExceptionEmail(ex);
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

        private DataTable getDataTableByDate_MYNONO_LTK(string StoredProcedureName, DateTime startDate, DateTime endDate, int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstring_LTK_MyNoNo"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                SendExceptionEmail(ex);
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
       
        static void runsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDev"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }

        private static void SendZeroRecordFoundEmail(string bodymessage)
        {
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["sendemailtoError"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(ToEmail));
                message.CC.Add(new MailAddress(ToEmailcc));
                message.From = new MailAddress("info@conversionsystems.com");

                message.Subject = "Batch Error No No - Daily Reconciliation Reports Batch";
                message.Body = bodymessage;
                message.IsBodyHtml = true;

                SmtpClient client;
                //client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client = new SmtpClient();
                client.Send(message);

            }
            catch (System.Exception ex1)
            {
                Console.WriteLine("Exception Details  : " + ex1.ToString());
            }
        }

        private static void SendExceptionEmail(Exception ex)
        {
            string excelFileName = "";
            string fileNameOnly = "";
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["sendemailtoError"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(ToEmail));
                message.CC.Add(new MailAddress(ToEmailcc));
                message.From = new MailAddress("info@conversionsystems.com");                              

                message.Subject = "Batch Error No No - Daily Reconciliation Reports Batch";
                message.Body =  ex.Message;
                message.IsBodyHtml = true;

                SmtpClient client;
                //client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client = new SmtpClient();
                client.Send(message);

            }
            catch (System.Exception ex1)
            {
                Console.WriteLine("Exception Details  : " + ex1.ToString());
            }
        }


        // private static void SendFileasAttachment(string excelFileName, string fileNameOnly, string countrycode, DataTable fileAttachments)
        private static void SendFileasAttachment(DataTable fileAttachments)
        {
            string excelFileName="";
            string fileNameOnly="";
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["sendemailto"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(ToEmail));
                message.CC.Add(new MailAddress(ToEmailcc));
                message.From = new MailAddress("info@conversionsystems.com");
                
                foreach (DataRow dRow in fileAttachments.Rows) // Loop over the rows.
                {
                    excelFileName = dRow["excelFileName"].ToString();
                    fileNameOnly = dRow["fileNameOnly"].ToString();
                    if (File.Exists(excelFileName))
                    {
                        Attachment attachment1 = new Attachment(excelFileName); //create the attachment
                        attachment1.Name = fileNameOnly;
                        message.Attachments.Add(attachment1);
                    }
                }

                message.Subject = "No No - Daily Reconciliation Reports";
                message.Body = "Please see attached Reconciliation Reports.";
                message.IsBodyHtml = true;

                SmtpClient client;
                // client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                // client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;                
                client = new SmtpClient();
                client.Send(message);

                Batch b2 = new Batch();
                b2.LogToFile("Email attachments send sucessfully. Total Attachments = " + message.Attachments.Count);

            }
            catch (System.Exception ex)
            {
                string error = ex.Message +" StackTrace:: "+ex.StackTrace;
                Batch b1 = new Batch();
                b1.LogToFile("Error Sending excel attachments in Email" + error);
                Console.WriteLine("Exception Details  : " + ex.ToString());
            }
        }

        private static string GetQuotedValue(string str)
        {
            return string.Format("\"{0}\"", str);
        }
        private static bool CreateTXTFile(DataTable dt, string strFilePath)
        {
            bool TXTCreated = false;
            try
            {
                // Create the TXT file.
                StreamWriter sw = new StreamWriter(strFilePath, false);
                int iColCount = dt.Columns.Count;

                // First we will write the headers.
                //DataTable dt = m_dsProducts.Tables[0];
                for (int i = 0; i < iColCount; i++)
                {
                    sw.Write(dt.Columns[i]);
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                TXTCreated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TXTCreated = false;
            }
            return TXTCreated;
        }

        private static bool Excel_FromDataTable(DataTable dt, string excelFileName, DataTable dtsummaryBottom)
        {
            bool excelCreated = false;
            try
            {
                Excel.Application excel = new Excel.Application();
                Excel.Workbook workbook = excel.Application.Workbooks.Add(true);

                int iCol = 0;
                foreach (DataColumn c in dt.Columns)
                {
                    iCol++;
                    excel.Cells[1, iCol] = c.ColumnName;
                }
                int iRow = 0;
                foreach (DataRow r in dt.Rows)
                {
                    iRow++;
                    iCol = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        iCol++;
                        excel.Cells[iRow + 1, iCol] = r[c.ColumnName];
                    }
                }

                
                if (dtsummaryBottom.Rows.Count > 0)
                {
                    DataTable NewTempDataTable = new DataTable();

                    DataColumn myDataColumn;

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = "col1";
                    NewTempDataTable.Columns.Add(myDataColumn);

                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = "col2";
                    NewTempDataTable.Columns.Add(myDataColumn);

                    DataRow emptyrow;
                    emptyrow = NewTempDataTable.NewRow();
                    emptyrow[0] = "";
                    emptyrow[1] = "";
                    NewTempDataTable.Rows.Add(emptyrow);

                    emptyrow = NewTempDataTable.NewRow();
                    emptyrow[0] = "";
                    emptyrow[1] = "";
                    NewTempDataTable.Rows.Add(emptyrow);

                    emptyrow = NewTempDataTable.NewRow();
                    emptyrow[0] = "";
                    emptyrow[1] = "";
                    NewTempDataTable.Rows.Add(emptyrow);

                    emptyrow = NewTempDataTable.NewRow();
                    emptyrow[0] = "";
                    emptyrow[1] = "";
                    NewTempDataTable.Rows.Add(emptyrow);

                    foreach (DataRow r2 in NewTempDataTable.Rows)
                    {
                        iRow++;
                        iCol = 0;
                        foreach (DataColumn c in NewTempDataTable.Columns)
                        {
                            iCol++;
                            excel.Cells[iRow + 1, iCol] = r2[c.ColumnName];
                        }
                    }

                    // Summary in excel at Bottom
                    foreach (DataRow r1 in dtsummaryBottom.Rows)
                    {
                        iRow++;
                        iCol = 0;
                        foreach (DataColumn c in dtsummaryBottom.Columns)
                        {
                            iCol++;
                            if (iCol == 2)
                            {
                                iCol--;
                                excel.Cells[iRow + 1, iCol] = r1[c.ColumnName];
                            }
                        }
                        //  excel.Cells[iRow + 1, iCol] = r1[1];           
                    }
                }

                object missing = System.Reflection.Missing.Value;

                if (System.IO.File.Exists(excelFileName))
                {
                    // Do I delete the File 
                }

                // If wanting to Save the workbook...                
                workbook.SaveAs(excelFileName, Excel.XlFileFormat.xlXMLSpreadsheet, missing, missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
                workbook.Close();
                //// If wanting excel to shutdown...                
                ((Excel.Application)excel).Quit();

                // Use the Com Object interop marshall to release the excel object
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excel);
                workbook = null;
                excel = null;
                excelCreated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                excelCreated = false;
            }
            finally
            {
                // force a garbage collection 
                System.GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return excelCreated;
        }

        void LoadOrder_Germany(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUK", startDate, endDate,1);
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationByBizIDGermany", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate("pr_OrderReconciliationByBizIDGermany", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("Germany Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_Germany_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;
                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUK", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationByBizIDGermany", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found for Germany");
            //    Console.WriteLine("No Order found Germany");
            //}
        }

        void LoadOrder_Spain(DateTime startDate, DateTime endDate, string FileDate)
        {
            DataTable Dt1 = getDataTableByDate("pr_OrderReconciliationByBizIDSpain", startDate, endDate, 1);
            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("Spain Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_Spain_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationByBizIDSpain", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    Console.WriteLine("Sending Email...");
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{                
            //    SendZeroRecordFoundEmail("No No Hair : No Order found for Spain");
            //    Console.WriteLine("No Order found Spain");
            //}
        }

        void LoadOrder_France(DateTime startDate, DateTime endDate, string FileDate)
        {
            DataTable Dt1 = getDataTableByDate("pr_OrderReconciliationByBizIDFrance", startDate, endDate, 1);
            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("France Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_France_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationByBizIDFrance", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName); 
                if (flag == true)
                {
                    Console.WriteLine("Sending Email...");
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found for France");
            //    Console.WriteLine("No Order found France");
            //}
        }

        void LoadOrder_IreLand(DateTime startDate, DateTime endDate, string FileDate)
        {            
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationIreland", startDate, endDate, 1);
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationByBizIDIreland", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate("pr_OrderReconciliationByBizIDIreland", startDate, endDate, 1);
            
            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("IreLand Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_Ireland_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationIreland", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationByBizIDIreland", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    Console.WriteLine("Sending Email...");
                    AddDataToTable(FUllPAthwithFileName, excelFileName);                    
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found for IreLand");
            //    Console.WriteLine("No Order found IreLand");
            //}
        }

        void LoadOrder_UK(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUK", startDate, endDate,1);
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationByBizIDUK", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate("pr_OrderReconciliationByBizIDUK", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{                
                Console.WriteLine("UK Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_UK_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath  + excelFileName;
                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUK", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationByBizIDUK", startDate, endDate, 2);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName); // , dtsummaryBottom);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);                   
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found for UK");
            //    Console.WriteLine("No Order found UK");
            //}
        }
        
        void LoadOrder_US_Canada(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            // DataTable Dt1 = getDataTableByDate("OrderReconciliationByBizIDUSCanada", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate("pr_OrderReconciliationByBizIDUSCanada", startDate, endDate, 1); 
            //Update for GOM8800 sku
            // Dt1.Columns["EM8800"].ColumnName = "GOM8800";
            //
            //if (Dt1.Rows.Count > 0)
            //{                
                Console.WriteLine("US_Canada Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_US_Canada_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath  + excelFileName;
                //DataTable dtsummaryBottom = new DataTable();
                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationByBizIDUSCanada", startDate, endDate, 2);
                //Update for GOM8800 sku
                //foreach (DataRow r in dtsummaryBottom.Rows)
                //{
                //    if (r["key"].ToString() == "EM8800")
                //        r["Value"] = r["Value"].ToString().Replace("EM8800", "GOM8800");
                //}
                //
                // bool flag =  Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);                   
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found US_Canada");
            //    Console.WriteLine("No Order found US_Canada");
            //}
        }

        void LoadOrder_MyNONO_US(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            // DataTable Dt1 = getDataTableByDate_MYNONO("pr_get_order_reconciliation", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate_MYNONO("pr_sp_get_order_reconciliation", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("My-NO-NO Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_My-NO-NO_US_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                //DataTable dtsummaryBottom = getDataTableByDate_MYNONO("pr_get_order_reconciliation", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found My-NO-NO.com");
            //    Console.WriteLine("No Order found My-NO-NO.com");
            //}
        }



        void LoadOrder_NoNoHairTV(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate_NoNoHairTV("pr_get_order_reconciliation", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate_NoNoHairTV("pr_sp_get_order_reconciliation", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("NoNoHairTV.com Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_NoNoHairTV_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                //DataTable dtsummaryBottom = getDataTableByDate_NoNoHairTV("pr_get_order_reconciliation", startDate, endDate, 2);
                //bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("NoNoHairTV : No Order found NoNoHairTV");
            //    Console.WriteLine("No Order found NoNoHairTV");
            //}
        }

        void LoadOrder_TryKyro(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            // DataTable Dt1 = getDataTableByDate_TryKyro("pr_get_order_reconciliation", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate_TryKyro("pr_sp_get_order_reconciliation", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("TryKyro Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_TryKyro_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate_TryKyro("pr_get_order_reconciliation", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found TryKyro");
            //    Console.WriteLine("No Order found TryKyro");
            //}
        }

        void LoadOrder_Kyrobak_UK(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            // DataTable Dt1 = getDataTableByDate_Kyrobak_UK("pr_get_order_reconciliation", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate_Kyrobak_UK("pr_sp_get_order_reconciliation", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("KyroBack_UK Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_Kyrobak_UK_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                //DataTable dtsummaryBottom = getDataTableByDate_Kyrobak_UK("pr_get_order_reconciliation", startDate, endDate, 2);
                //bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found TryKyro");
            //    Console.WriteLine("No Order found TryKyro");
            //}
        }

        void LoadOrder_MyNONO_LTK(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            // DataTable Dt1 = getDataTableByDate_MYNONO_LTK("pr_get_order_reconciliation_LTK", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate_MYNONO_LTK("pr_sp_get_order_reconciliation_LTK", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("LTK-My-NO-NO Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_LTK-My-NO-NO_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                //DataTable dtsummaryBottom = getDataTableByDate_MYNONO_LTK("pr_get_order_reconciliation_LTK", startDate, endDate, 2);
                //bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found LTK.My-NO-NO.com");
            //    Console.WriteLine("No Order found LTK.My-NO-NO.com");
            //}
        }
        void LoadOrder_MyNONO_UK(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            // DataTable Dt1 = getDataTableByDate_MYNONO("pr_get_order_reconciliation_UK", startDate, endDate, 1);
            DataTable Dt1 = getDataTableByDate_MYNONO("pr_sp_get_order_reconciliation_UK", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("My-NO-NO Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_HTTPBizID_My-NO-NO_UK" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                //DataTable dtsummaryBottom = getDataTableByDate_MYNONO("pr_get_order_reconciliation_UK", startDate, endDate, 2);
                //bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("No No Hair : No Order found My-NO-NO.CO.UK");
            //    Console.WriteLine("No Order found My-NO-NO.CO.UK");
            //}
        }

        private void LogToFile(string AdditionalInfo)
        {
            bool bResult = false;
            StreamWriter log;

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now);
            //if (Error != null)
            //{
            //    sb.Append("Exception error:" + Error.ToString() + "-");
            //}
            sb.Append("-" + AdditionalInfo + "-");
            try
            {
                if (!File.Exists(targetPathLog))
                {
                    log = new StreamWriter(targetPathLog);
                }
                else
                {
                    log = File.AppendText(targetPathLog);
                }
                log.WriteLine(sb.ToString());
                log.Close();
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            //  return bResult;
        }

        static void Main(string[] args)
        {
            DateTime startDate;
            DateTime endDate;
            startDate =  DateTime.Now.AddDays(-1);
            endDate = DateTime.Now.AddDays(-1);
            DateTime tDate = DateTime.Now.AddDays(-1);
            string date = string.Empty;           
            date = tDate.ToString("MM/dd/yyyy");

            

            startDate = Convert.ToDateTime(date + " 00:00:00");
            endDate = Convert.ToDateTime(date + " 23:59:59");

            //TODO: comment for prod
            //startDate = Convert.ToDateTime("2014/08/01 00:00:00");
            //endDate = Convert.ToDateTime("2014/08/07 23:59:59");

            Console.WriteLine("startDate" + startDate);
            Console.WriteLine("endDate " + endDate); 
           
            Console.WriteLine("Start NoNoHair Reconciliation Report");

            Batch BatchNoNoHair1 = new Batch();
            BatchNoNoHair1.LogToFile("Start NoNoHair Reconciliation Report");
            BatchNoNoHair1.CreateDataTable();

            try
            {
                BatchNoNoHair1.LogToFile("Start US Canada");
                BatchNoNoHair1.LoadOrder_US_Canada(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start US Canada" + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start UK Orders");
                BatchNoNoHair1.LoadOrder_UK(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start UK " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start IreLand Orders");
                BatchNoNoHair1.LoadOrder_IreLand(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start Ireland " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start Spain Orders");
                BatchNoNoHair1.LoadOrder_Spain(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start Spain " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start France Orders");
                BatchNoNoHair1.LoadOrder_France(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start France " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start Germany Orders");
                BatchNoNoHair1.LoadOrder_Germany(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start Germany " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start MY-NO-NO.Com Orders");
                BatchNoNoHair1.LoadOrder_MyNONO_US(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start MY-NO-NO.Com " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }
            try
            {
                BatchNoNoHair1.LogToFile("Start MY-NO-NO.Co.UK Orders");
                BatchNoNoHair1.LoadOrder_MyNONO_UK(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start MY-NO-NO.Co.UK " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }
            try
            {
                BatchNoNoHair1.LogToFile("Start LTK.MY-NO-NO.Com Orders");
                BatchNoNoHair1.LoadOrder_MyNONO_LTK (startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start LTK.MY-NO-NO.Com " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start TryKyro.Com Orders");
                BatchNoNoHair1.LoadOrder_TryKyro(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start TryKyro.Com " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start KyroBack_UK Orders");
                BatchNoNoHair1.LoadOrder_Kyrobak_UK(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start KyroBack_UK " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNoNoHair1.LogToFile("Start NoNoHairTV.Com Orders");
                BatchNoNoHair1.LoadOrder_NoNoHairTV(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Start NoNoHairTV.Com " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            Console.WriteLine("End NoNoHair Reconciliation Report");

            /* 
            Console.WriteLine("Start NoNoSkin Reconciliation Report");
            BatchNoNoSkin BatchNoNoSkin1 = new BatchNoNoSkin();
            BatchNoNoSkin1.CreateDataTable();
            
            try {
                BatchNoNoSkin1.LoadOrder_US(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                SendExceptionEmail(ex);
            }
            
            try{
                BatchNoNoSkin1.LoadOrder_UK(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                SendExceptionEmail(ex);
            }         

            DataTable temp1 = BatchNoNoSkin1.NewTempDataTable;
            string excelFileName; 
            string fileNameOnly;
            if (temp1.Rows.Count > 0)
            {
                foreach (DataRow dRow in temp1.Rows) // Loop over the rows.
                {
                    excelFileName = dRow["excelFileName"].ToString();
                    fileNameOnly = dRow["fileNameOnly"].ToString();
                    BatchNoNoHair1.AddDataToTable(excelFileName, fileNameOnly);
                }
            }
            Console.WriteLine("End NoNoSkin Reconciliation Report");

            */


            try
            {
                 if (BatchNoNoHair1.NewTempDataTable.Rows.Count > 0)
                {
                    BatchNoNoHair1.LogToFile("Start Sending excel attachments in Email");
                    Console.WriteLine("Sending excel attachments in Email...");
                    SendFileasAttachment(BatchNoNoHair1.NewTempDataTable);
                    BatchNoNoHair1.LogToFile("End Sending excel attachments in Email");
                }
            }
            catch (Exception ex)
            {
                BatchNoNoHair1.LogToFile("Sending Email Error Catch :: " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            BatchNoNoHair1.LogToFile("End NoNoHair Reconciliation Report");                        
        }
    }
}
