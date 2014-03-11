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
using Com.ConversionSystems.Utility;
using DiscountNeovaUK.com.hitslink.www;
using Excel = Microsoft.Office.Interop.Excel;


namespace DiscountNeovaUK
{
    class Reports
    {
        LogData log = new LogData();
        private string targetPath = "C:/BatchFiles/Neova/";
        // private string targetPath = "D:/BatchProcesses/TryElastin/";

        static DataSet getsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringdev"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
            adp.Fill(ds);
            conn.Close();
            return ds;
        }

        static DataTable getDataTable(string StoredProcedureName, DateTime? StartDate, DateTime? EndDate)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringdev"];

            string strMessage;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, StartDate));
                oCmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, EndDate));

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
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

        static DataTable getDataTable(DateTime StartDate, DateTime EndDate)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringdev"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;
            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = "SELECT left(FirstName, CHARINDEX(' ', FirstName)) as firstname,substring(FirstName, CHARINDEX(' ', FirstName)+1, len(FirstName)-(CHARINDEX(' ', FirstName)-1)) as lastname,Email,Phone,ZipCode FROM [OrderInfo] where status is null and (Created >= '" + StartDate + "' ) and (Created < '" + EndDate + "')";
                oCmd.CommandType = CommandType.Text;                
                //oCmd.Parameters.Add(new SqlParameter("@ReportDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ReportDate));                
                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
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
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringdev"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }

        void CheckDataBase_Connection()
        {
            DataSet DS = getsql("Select * from SKU");
            DataTable DT = DS.Tables[0];
            if (DT.Rows.Count > 0)
            {
                Console.WriteLine(DT.Rows.Count.ToString());
            }
        }

        public int BindData(DateTime startDate, DateTime endDate)
        {
            int visitCount = 0;

            try
            {
                
                // VersionVisistorInfo.Clear();
                StringBuilder sb = new StringBuilder();
                ReportWS ws = new ReportWS();
                int i = 0;

                Data rptData = default(Data);
                //   Call the webservice and retrieve the report data 
                rptData = ws.GetDataFromTimeframe("tryelastin", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);

                decimal visitor = 0;
                for (i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    // VersionVisistorInfo.Add(rptData.Rows[i].Columns[0].Value.ToUpper(), rptData.Rows[i].Columns[13].Value);
                    //Response.Write(rptData.Rows[i].Columns[0].Value +"-"+rptData.Rows[i].Columns[13].Value);

                    if (rptData.Rows[i].Columns[0].Value.Equals("--- Overall ---"))
                    {
                        visitCount = Convert.ToInt32(rptData.Rows[i].Columns[13].Value);
                    }
                }
            }
            catch { }

            return visitCount;
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
               
        private void uploadFiletoFTP(string filePath)
        {
            Console.WriteLine("FTP File :: " + filePath );
            try
            {
                string FTPAddress = System.Configuration.ConfigurationSettings.AppSettings["FTPURL"];
                string username = System.Configuration.ConfigurationSettings.AppSettings["FTPLogin"];
                string password = System.Configuration.ConfigurationSettings.AppSettings["FTPPassword"];
                               
                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress);

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
            catch(Exception ex)
            {
                SendEmailToAdminFTPError(ex.Message);
            }
        }

        public void SendEmailToAdminFTPError(string ErrorMessage)
        {
            try
            {
                StringBuilder _sbEmailMessageBody = new StringBuilder();
                _sbEmailMessageBody.Append("<html><body><table>");
                _sbEmailMessageBody.Append("<tr><td><b>TryElastin.com - Uploading FTP Reports to Client Error :</b></td></tr>");
                _sbEmailMessageBody.Append("<tr><td>This report was generated at " + DateTime.Now.ToString("MM/dd/yyyy-HH:mm") + "</td></tr>");
                _sbEmailMessageBody.Append("<tr><td>" + ErrorMessage + "</td></tr>");
                _sbEmailMessageBody.Append("<tr><td> Please do not reply to this email.</b></td></tr>");
                _sbEmailMessageBody.Append("</table></body></html>");
                string AdminEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
                string fromEmail = System.Configuration.ConfigurationSettings.AppSettings["fromEmail"];
                MailMessage _oMailMessage = new MailMessage(fromEmail, AdminEmail, "TryElastin - Uploading FTP Reports to Client Error", _sbEmailMessageBody.ToString());
                _oMailMessage.IsBodyHtml = true;
                _oMailMessage.Body = _sbEmailMessageBody.ToString();
                SendMail(_oMailMessage);
            }
            catch (Exception e)
            {
                // log.LogToFile("Error sending email---" + e.Message);
            }
        }

        public void SendEmailToAdmin()
        {
            try
            {
                StringBuilder _sbEmailMessageBody = new StringBuilder();
                _sbEmailMessageBody.Append("<html><body><table>");
                _sbEmailMessageBody.Append("<tr><td><b>TryElastin.com - Daily Reports:</b></td></tr>");
                _sbEmailMessageBody.Append("<tr><td>This report was generated at " + DateTime.Now.ToString("MM/dd/yyyy-HH:mm") + "</td></tr>");                
                _sbEmailMessageBody.Append("<tr><td> Please do not reply to this email.</b></td></tr>");
                _sbEmailMessageBody.Append("</table></body></html>");
                string AdminEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
                string fromEmail = System.Configuration.ConfigurationSettings.AppSettings["fromEmail"];
                MailMessage _oMailMessage = new MailMessage(fromEmail, AdminEmail, "TryElastin - Daily Report", _sbEmailMessageBody.ToString());
                _oMailMessage.IsBodyHtml = true;
                _oMailMessage.Body = _sbEmailMessageBody.ToString();
                SendMail(_oMailMessage);
            }
            catch (Exception e)
            {
                // log.LogToFile("Error sending email---" + e.Message);
            }
        }

        public void SendEmailToAdmin(string ErrorMessage)
        {
            try
            {
                StringBuilder _sbEmailMessageBody = new StringBuilder();
                _sbEmailMessageBody.Append("<html><body><table>");
                _sbEmailMessageBody.Append("<tr><td><b>TryElastin.com - Daily Reports:</b></td></tr>");
                _sbEmailMessageBody.Append("<tr><td>This report was generated at " + DateTime.Now.ToString("MM/dd/yyyy-HH:mm") + "</td></tr>");
                _sbEmailMessageBody.Append("<tr><td>" + ErrorMessage + "</td></tr>");                
                _sbEmailMessageBody.Append("<tr><td> Please do not reply to this email.</b></td></tr>");
                _sbEmailMessageBody.Append("</table></body></html>");
                string AdminEmail  = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
                string fromEmail = System.Configuration.ConfigurationSettings.AppSettings["fromEmail"];
                MailMessage _oMailMessage = new MailMessage(fromEmail, AdminEmail, "TryElastin - Daily Report", _sbEmailMessageBody.ToString());
                _oMailMessage.IsBodyHtml = true;
                _oMailMessage.Body = _sbEmailMessageBody.ToString();                
               SendMail(_oMailMessage);
            }
            catch (Exception e)
            {
               // log.LogToFile("Error sending email---" + e.Message);
            }
        }

        public static bool SendMail(MailMessage oMsg)
        {
            SmtpClient client;
            bool bResult = false;           
            try
            {                
                client = new SmtpClient();                
                client.Send(oMsg);
                
                bResult = true;
            }
            catch (Exception ex)
            {                
            }
            return bResult;
        }

       

        private static bool CreateCSVFile(DataTable dt, string strFilePath)
        {       
            bool CSVCreated = false;
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(strFilePath, false);
                int iColCount = dt.Columns.Count;

                
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
                CSVCreated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CSVCreated = false;
            }
            return CSVCreated;
        }

        private static bool CreateCSVFile(string strFilePath)
        {
            bool CSVCreated = false;
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(strFilePath, false);
               


               
                            sw.Write("");
               
                   
               
                sw.Close();
                CSVCreated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CSVCreated = false;
            }
            return CSVCreated;
        }
        
        private static bool Excel_FromDataTable(DataTable dt, string excelFileName)
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
        
        void LoadDailySales(DateTime ReportDate)
        {
            try
            {
                int CurrentHour = DateTime.Now.Hour;
                DateTime? startdate = new DateTime();
                DateTime? enddate = new DateTime();
                if (DateTime.Now.DayOfWeek.ToString().ToUpper().Equals("MONDAY"))
                {
                    startdate = GetEastCoastStartDate(DateTime.Today.AddDays(-7));
                    enddate = GetEastCoastDate(DateTime.Today);
                }
                else
                {
                    startdate = GetEastCoastStartDate(DateTime.Today);
                    enddate = GetEastCoastDate(DateTime.Today);
                }
                

                Console.WriteLine("Generating  Daily Lead Reports");                
                string excelFileName = "NeovaUKDiscounts" + ReportDate.ToString("yyyyMMddmmHH") + ".csv"; 
                string FUllPAthwithFileName = targetPath + excelFileName;

                DataTable DT = getDataTable("GetDiscountDetailsByDate_UK", startdate, enddate);
                if (DT.Rows.Count > 0)
                {
                    Console.WriteLine("Rows " + DT.Rows.Count.ToString());                    
                    bool flag = CreateCSVFile(DT, FUllPAthwithFileName);
                        // Excel_FromDataTable(DT, FUllPAthwithFileName);
                    if (flag == true)
                    {
                        Console.WriteLine("CSV Report for Daily Leads Created");
                    }
                }
                else
                {
                    bool flag = CreateCSVFile(FUllPAthwithFileName);
                    // Excel_FromDataTable(DT, FUllPAthwithFileName);
                    if (flag == true)
                    {
                        Console.WriteLine("CSV Report for Daily Leads Created");
                    }
                }

                SendEmailToClient(FUllPAthwithFileName);
                // SendEmailToAdmin();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error Daily Sales Reports " + ex.Message);
            }
            
        }

        public void SendEmailToClient(string path)
        {
            try
            {
                StringBuilder _sbEmailMessageBody = new StringBuilder();
                _sbEmailMessageBody.Append("<html><body><table>");
                _sbEmailMessageBody.Append("<tr><td><b>MyNeova.co.UK   - Discount Daily Report: </b></td></tr>");
                _sbEmailMessageBody.Append("<tr><td>This report was generated at " + DateTime.Now.ToString("MM/dd/yyyy-HH:mm") + "</td></tr>");
                _sbEmailMessageBody.Append("<tr><td> Please do not reply to this email.</b></td></tr>");
                _sbEmailMessageBody.Append("</table></body></html>");
                MailMessage _oMailMessage = new MailMessage(System.Configuration.ConfigurationSettings.AppSettings["FromEmail"], System.Configuration.ConfigurationSettings.AppSettings["ClientEmail"] , "MyNeova.co.UK   - Discount Daily Report", _sbEmailMessageBody.ToString());
                _oMailMessage.IsBodyHtml = true;
                _oMailMessage.Body = _sbEmailMessageBody.ToString();
                Attachment a = new Attachment(path);
                _oMailMessage.Attachments.Add(a);
                bool status = SendMail(_oMailMessage);
                if (status)
                {
                    log.LogToFile("Email sent successfully to :" + _oMailMessage.To.ToString() + " on :" + DateTime.Now.ToString());
                }
                else
                {
                    log.LogToFile("Email NOT sent to :" + _oMailMessage.To.ToString() + " on :" + DateTime.Now.ToString());
                }

            }
            catch (Exception e)
            {
                log.LogToFile("Error sending email---" + e.Message);
            }
        }

        public static DateTime GetLastHour(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.AddHours(-1).Hour, 0, 0);
        }

        public static DateTime GetCurrentHour(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
        }
        public static DateTime? GetEastCoastStartDate(DateTime? input)
        {
            DateTime? val = input;
            if (val.HasValue)
                val = val.Value.AddDays(-1).AddHours(21).AddMinutes(00).AddSeconds(00);

            return val;
        }

        public static DateTime? GetEastCoastDate(DateTime? input)
        {
            DateTime? val = input;
            if (val.HasValue)
                val = val.Value.AddHours(21).AddMinutes(00).AddSeconds(00);

            return val;
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Start Lead Reports XtracNow.com");
            Reports DailyReports = new Reports();

            DateTime ReportDate = DateTime.Now;
            DailyReports.LoadDailySales(ReportDate);


            Console.WriteLine("End Lead Reports XtracNow.com");
        }
    }
}
