using System;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Text;
using System.Net.Mail;
using System.Xml.Serialization;
using System.Net;

namespace Resurgence_CustomerRegistrationBatch
{
    class Batch
    {
        private string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];
        private string targetPathLog = System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"];
        private int TotalOrderSend = 0;
        private int TotalOrderRejected = 0;
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

        static void runsql(string s)
        {
            string connstr1 = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDev"];
            SqlConnection conn = new SqlConnection(connstr1);
            conn.Open();
            SqlCommand cm = new SqlCommand(s, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }

        static DataTable getDataTableByDate(string StoredProcedureName, DateTime startDate, DateTime endDate)
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

                oCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                oCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
                Batch b1 = new Batch();
                b1.LogToFile(strMessage);
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
                message.To.Add(ToEmail);
                message.CC.Add(ToEmailcc);
                message.From = new MailAddress("info@conversionsystems.com");
                message.Subject = "Batch Error Resurgence Customer Registration Batch";
                message.Body = ex.Message;
                message.IsBodyHtml = true;
                SmtpClient client;
                client = new SmtpClient();
                client.Send(message);
            }
            catch (System.Exception ex1)
            {
                Console.WriteLine("Exception Details  : " + ex1.ToString());
            }
        }

        private void SendEmailtoClient(DateTime dt, string txtFileName, string fileNameOnly) // , int submittedorders, int failedOrders)
        {
            try
            {
                // int TotalOrders = submittedorders + failedOrders;
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["sendemailto"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
                MailMessage message = new MailMessage();
                message.To.Add(ToEmail);
                message.CC.Add(ToEmailcc);
                message.From = new MailAddress("info@conversionsystems.com");
                if (File.Exists(txtFileName))
                {
                    Attachment attachment1 = new Attachment(txtFileName); //create the attachment
                    attachment1.Name = fileNameOnly;
                    message.Attachments.Add(attachment1);
                }

                message.Subject = "Conversion Systems Resurgence Customer Registration";
                StringBuilder sb = new StringBuilder();
                sb.Append("Please see attached Customer Registration Report for ").Append(dt.ToString("yyyy-MM-dd"));
                string st = sb.ToString();
                st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
                message.Body = st;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Send(message);
                LogToFile("Email SendEmail to Client Succesfully");
            }
            catch (System.Exception ex)
            {
                string error = ex.Message + " StackTrace :: " + ex.StackTrace;
                LogToFile("Error SendEmail to Client " + error);
                Console.WriteLine("Exception Details  : " + ex.ToString());
            }
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

        protected T FromXml<T>(String xml)
        {
            T returnedXmlClass = default(T);
            using (TextReader reader = new StringReader(xml))
            {
                try
                {
                    returnedXmlClass =
                        (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                }
                catch (InvalidOperationException)
                {
                    // String passed is not XML, simply return defaultXmlClass
                }
            }
            return returnedXmlClass;
        }

        //private static bool Excel_FromDataTable(DataTable dt, string excelFileName, bool IsData)
        //{
        //    // we have SKIP the 1st Column of the DataTable in this Function.
        //    bool excelCreated = false;
        //    try
        //    {
        //        Excel.Application excel = new Excel.Application();
        //        Excel.Workbook workbook = excel.Application.Workbooks.Add(true);

        //        int iCol = 0;
        //        foreach (DataColumn c in dt.Columns)
        //        {
        //            iCol++;
        //            if (iCol != 1)
        //            {
        //                excel.Cells[1, iCol - 1] = c.ColumnName;
        //            }
        //        }

        //        if (IsData)
        //        {
        //            int iRow = 0;
        //            foreach (DataRow r in dt.Rows)
        //            {
        //                iRow++;
        //                iCol = 0;
        //                foreach (DataColumn c in dt.Columns)
        //                {
        //                    iCol++;
        //                    if (iCol != 1)
        //                    {
        //                        excel.Cells[iRow + 1, iCol - 1] = r[c.ColumnName];
        //                    }
        //                }
        //            }
        //        }

        //        object missing = System.Reflection.Missing.Value;
        //        if (System.IO.File.Exists(excelFileName))
        //        {
        //            // Do I delete the File 
        //        }

        //        // If wanting to Save the workbook...                
        //        workbook.SaveAs(excelFileName, Excel.XlFileFormat.xlXMLSpreadsheet, missing, missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
        //        workbook.Close();
        //        //// If wanting excel to shutdown...                
        //        ((Excel.Application)excel).Quit();

        //        // Use the Com Object interop marshall to release the excel object
        //        Marshal.ReleaseComObject(workbook);
        //        Marshal.ReleaseComObject(excel);
        //        workbook = null;
        //        excel = null;
        //        excelCreated = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        excelCreated = false;
        //    }
        //    finally
        //    {
        //        // force a garbage collection 
        //        System.GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //    }
        //    return excelCreated;
        //}

        public bool CSV_FromDataTable(DataTable dt, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < iColCount - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            // Now write all the rows.
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

            return true;
        }

        private void AddDataToTable(string excelFileName, string fileNameOnly)
        {
            DataRow row;
            row = NewTempDataTable.NewRow();
            row["excelFileName"] = excelFileName;
            row["fileNameOnly"] = fileNameOnly;
            NewTempDataTable.Rows.Add(row);
        }

        private void uploadFile(string FTPAddress, string filePath, string username, string password)
        {
            try
            {
                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/" + Path.GetFileName(filePath));

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                //Load the file
                FileStream stream = File.OpenRead(filePath);
                byte[] buffer = new byte[stream.Length];

                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                //Upload file
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Close();
            }
            catch
            {
                //sendemail(filePath);
            }
        }
        void LoadLeads(DateTime startDate, DateTime endDate, string FileDate)
        {
            DataTable Dt1 = getDataTableByDate("pr_report_customerregistration", startDate, endDate);
            Console.WriteLine("Total Leads: " + Dt1.Rows.Count.ToString());
            string excelFileName = "leads" + FileDate + ".csv";
            string FUllPAthwithFileName = targetPath + excelFileName;             
            // bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, true);
            bool flag = CSV_FromDataTable(Dt1, FUllPAthwithFileName);
            //if (flag == true)
            //{
            //    // AddDataToTable(FUllPAthwithFileName, excelFileName);
            //    try
            //    {
            //        //if (NewTempDataTable.Rows.Count > 0)
            //        //{
            //        // SendEmailtoClient(startDate, FUllPAthwithFileName, excelFileName);
            //        // }
            //        // uploadFile(System.Configuration.ConfigurationSettings.AppSettings["FTPURL"], FUllPAthwithFileName, System.Configuration.ConfigurationSettings.AppSettings["FTPLogin"], System.Configuration.ConfigurationSettings.AppSettings["FTPPassword"]);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogToFile("Error Block - Sending Email to Client :: " + ex.Message);
            //        SendExceptionEmail(ex);
            //    }
            //}
        }


        static void Main(string[] args)
        {
            DateTime startDate;
            DateTime endDate;
            startDate = DateTime.Now.AddDays(-1);
            endDate = DateTime.Now.AddDays(-1);
            DateTime tDate = DateTime.Now.AddDays(-1);

            string date = string.Empty;
            date = tDate.ToString("MM/dd/yyyy");

            startDate = Convert.ToDateTime(date + " 00:00:00");
            endDate = Convert.ToDateTime(date + " 23:59:59");

            //startDate = Convert.ToDateTime("01/01/2014 00:00:00");
            //endDate = Convert.ToDateTime("12/08/2014 23:59:59");

            Console.WriteLine("startDate" + startDate);
            Console.WriteLine("endDate " + endDate);
            Console.WriteLine("Start Resurgence Customer Registration Batch");
            Batch Batch1 = new Batch();
            Batch1.LogToFile("Start Resurgence Customer Registration Batch");
            Batch1.CreateDataTable();
            try
            {
                Batch1.LogToFile("Start Loading Leads");
                Batch1.LoadLeads(startDate, endDate, startDate.ToString("MMddyyyy"));
            }
            catch (Exception ex)
            {
                Batch1.LogToFile("Error Loading Leads" + ex.Message + " StackTrace:: " + ex.StackTrace);
                SendExceptionEmail(ex);
            }
            Batch1.LogToFile("****************End Resurgence Customer Registration Batch**********");
            Console.WriteLine("EndResurgence Customer Registration Batch");
        }
    }
}
