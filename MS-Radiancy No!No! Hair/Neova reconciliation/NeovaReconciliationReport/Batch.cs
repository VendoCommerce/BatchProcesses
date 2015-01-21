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

namespace NeovaReconciliationReport
{
    class Batch
    {
       private string targetPath = "C:/BatchFiles/Neova/";
       private string targetPathLog = "C:/BatchFiles/Neova/Log/Log.txt";
       

       
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

     
     

  
    
        void LoadOrder_US_Canada(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            DataTable Dt1 = getDataTableByDate("pr_sp_get_order_reconciliation_US", startDate, endDate, 1);            
            
            //if (Dt1.Rows.Count > 0)
            //{                
                Console.WriteLine("US_Canada Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_Neova_US_Canada_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath  + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate("pr_get_order_reconciliation_US", startDate, endDate, 2);
                // bool flag =  Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);                   
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("Neova : No Order found US_Canada");
            //    Console.WriteLine("No Order found US_Canada");
            //}
        }

        void LoadOrder_UK(DateTime startDate, DateTime endDate, string FileDate)
        {
            // DataTable Dt1 = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate,1);            

            DataTable Dt1 = getDataTableByDate("pr_sp_get_order_reconciliation_UK", startDate, endDate, 1);

            //if (Dt1.Rows.Count > 0)
            //{
                Console.WriteLine("UK Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_Neova_UK_" + FileDate + ".csv";
                string FUllPAthwithFileName = targetPath + excelFileName;

                //  DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUSCanada", startDate, endDate, 2);
                // DataTable dtsummaryBottom = getDataTableByDate("pr_get_order_reconciliation_UK", startDate, endDate, 2);
                // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                bool flag = CreateTXTFile(Dt1, FUllPAthwithFileName);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);
                }
            //}
            //else
            //{
            //    SendZeroRecordFoundEmail("Neova : No Order found UK");
            //    Console.WriteLine("No Order found UK");
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

            // startDate = Convert.ToDateTime("05/06/2012 00:00:00");
            // endDate = Convert.ToDateTime("05/06/2012 23:59:59");
            
            Console.WriteLine("startDate" + startDate);
            Console.WriteLine("endDate " + endDate); 
           
            Console.WriteLine("Start Neova Reconciliation Report");

            Batch BatchNeova = new Batch();
            BatchNeova.LogToFile("Start Neova Reconciliation Report");
            BatchNeova.CreateDataTable();

            try
            {
                BatchNeova.LogToFile("Start US Canada");
                BatchNeova.LoadOrder_US_Canada(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNeova.LogToFile("Start US Canada" + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            try
            {
                BatchNeova.LogToFile("Start UK Orders");
                BatchNeova.LoadOrder_UK(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                BatchNeova.LogToFile("Start UK " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            


            Console.WriteLine("End Neova Reconciliation Report");

         


            try
            {
                if (BatchNeova.NewTempDataTable.Rows.Count > 0)
                {
                    BatchNeova.LogToFile("Start Sending excel attachments in Email");
                    Console.WriteLine("Sending excel attachments in Email...");
                    SendFileasAttachment(BatchNeova.NewTempDataTable);
                    BatchNeova.LogToFile("End Sending excel attachments in Email");
                }
            }
            catch (Exception ex)
            {
                BatchNeova.LogToFile("Sending Email Error Catch :: " + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }

            BatchNeova.LogToFile("End Neova Reconciliation Report");                        
        }
    }
}
