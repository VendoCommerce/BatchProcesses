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
using Nono_IncrementalMedia_Report.com.hitslink.www;

namespace ChiefMediafile
{
    class Report 
    {
        private string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];

        
        public Hashtable HitLinkVisitor = new Hashtable();
        public int CategoryUniqueVistiors = 0;
        private DataTable _dtOrderInfo = null;
        private DataTable _dtOrderInfo1 = null;
        //DateTime enddate;
        //DateTime startdate;

        static DataSet getsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
            adp.Fill(ds);
            conn.Close();
            return ds;
        }

        static DataTable getDataTable(string StoredProcedureName)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];

            string strMessage;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

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

        static DataTable getDataTable(string StoredProcedureName, DateTime startDate, DateTime endDate, int versionID)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];

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
                oCmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, startDate));
                oCmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, endDate));
                oCmd.Parameters.Add(new SqlParameter("@Version", SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ""));
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

        static DataTable getDataTable(string StoredProcedureName, DateTime startDate, DateTime endDate, string Archivedata)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];

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
                oCmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, startDate));
                oCmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, endDate));
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
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }

        public static DateTime EndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
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
                if (!File.Exists(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"]))
                {
                    log = new StreamWriter(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"]);
                }
                else
                {
                    log = File.AppendText(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"]);
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

        public static bool SendMail(MailMessage oMsg)
        {
            SmtpClient client;
            bool bResult = false;
            try
            {
                client = new SmtpClient();                
                //client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(oMsg);
                bResult = true;
            }
            catch (Exception ex)
            {
            }
            return bResult;
        }

        public void SendEmailToAdmin(string ErrorMessage)
        {
            try
            {
                StringBuilder _sbEmailMessageBody = new StringBuilder();
                _sbEmailMessageBody.Append("<html><body><table>");
                _sbEmailMessageBody.Append("<tr><td><b>Trynono.Com - Incremental Media Report Batch :</b></td></tr>");
                _sbEmailMessageBody.Append("<tr><td>This report was generated at " + DateTime.Now.ToString("MM/dd/yyyy-HH:mm") + "</td></tr>");
                _sbEmailMessageBody.Append("<tr><td>" + ErrorMessage + "</td></tr>");
                _sbEmailMessageBody.Append("<tr><td> Please do not reply to this email.</b></td></tr>");
                _sbEmailMessageBody.Append("</table></body></html>");
                string AdminEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
                string fromEmail = System.Configuration.ConfigurationSettings.AppSettings["fromEmail"];
                MailMessage _oMailMessage = new MailMessage(fromEmail, AdminEmail, "Trynono.Com - Incremental Media Report Generation Status", _sbEmailMessageBody.ToString());
                _oMailMessage.IsBodyHtml = true;
                _oMailMessage.Body = _sbEmailMessageBody.ToString();
                SendMail(_oMailMessage);
            }
            catch (Exception e)
            {
                // log.LogToFile("Error sending email---" + e.Message);
            }
        }

        private static bool SendFileasAttachment(string txtFileName, string fileNameOnly, string date)
        {
            bool sendemail = false;
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];                
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();

                message.To.Add(ToEmail);
                                                               
                message.From = new MailAddress("info@conversionsystems.com");                                
                if (File.Exists(txtFileName))
                {
                    Attachment attachment1 = new Attachment(txtFileName); //create the attachment
                    attachment1.Name = fileNameOnly;
                    message.Attachments.Add(attachment1);
                }
                message.Subject = "no!no! – Weekly Incremental Media CPA Report - Week Ending" + date;
                // message.Body = "Please see attached Euro Report.";
                message.Body = "Please see attached daily report for trynono.com";
                message.IsBodyHtml = true;

                SmtpClient client;
                client = new SmtpClient();
                //client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(message);
                sendemail = true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception Details  : " + ex.ToString());
                sendemail = false;
                Report Report1 = new Report();
                Report1.LogToFile("Error " + ex.Message);
                Report1.LogToFile("Error " + ex.StackTrace);
            }
            return sendemail;
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
                string value = "";
                string fomattedValue = "";
                int length = 0;

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

        public static DateTime GetEastCoastStartDate(DateTime input)
        {
            DateTime val = input;
            return val.AddDays(-1).AddHours(21).AddMinutes(00).AddSeconds(00);

        }

        public static DateTime GetEastCoastDate(DateTime input)
        {
            DateTime val = input;
            return val.AddHours(21).AddMinutes(00).AddSeconds(00);

        }
        
        private void LoadReport(DateTime ReportDate, int versionID, string VersionName , string FileName_PostFix)
        {
            try
            {
                // stored procedure convert time to EST Time Zone.
                DateTime StartDate = StartOfDay(ReportDate.AddDays(-6));                
                DateTime Enddate = EndOfDay(ReportDate);

             
                                           
                string txtFileName = Enddate.ToString("yyyyMMdd") + "_trynono_orders"+FileName_PostFix + ".csv";
                string txtFileName1 = Enddate.ToString("yyyyMMdd") + "_trynono_Incrementalmedia"+FileName_PostFix + ".csv";
                targetPath = targetPath.Replace("\\\\", "\\");
                string FUllPAthwithFileName = targetPath + txtFileName;
                string FUllPAthwithFileName1 = targetPath + txtFileName1;

                
                             
               
                               
                string hline = "";
                char[] delimiterChars = { '/' };
                using (StreamWriter sb = new StreamWriter(FUllPAthwithFileName1))
                {
                    hline += "Date" + ",";
                    hline += "SUBID" + ",";
                    hline += "MID" + ",";
                    hline += "Visits" + ",";
                    hline += "Total Orders" + ",";
                    hline += "TotalRevenue" + ",";                    
                    sb.WriteLine(hline.Trim(','));

                    hline = "";
                    try
                    {
                        for (DateTime date = StartDate; date <= Enddate; date = date.AddDays(1.0))
                        {
                            int visitors = BindData(GetEastCoastStartDate(date), GetEastCoastDate(date), VersionName.ToLower());
                            DataTable DT = getDataTable("MDI_OrderLoadByDatesRange_Incremental", GetEastCoastStartDate(date), GetEastCoastDate(date), versionID);
                            // logic here

                         foreach (DictionaryEntry entry in HitLinkVisitor)
                        {
                            if (entry.Key.ToString().ToLower().Contains("cpa_im"))
                            {
                                bool OrderInfoPresent = false;
                                hline = "";
                                hline += GetEastCoastDate(date).ToShortDateString() + ",";
                                if (entry.Key.ToString().Contains("/"))
                                {
                                    string[] words = entry.Key.ToString().Split(delimiterChars);
                                    string subId = words[1];
                                    string mId = words[0];
                                    hline += subId + ",";
                                    hline += mId + ",";
                                }
                                else
                                {
                                    hline += "" + ",";
                                    hline += entry.Key.ToString() + ",";
                                }
                                //hline += URl + ",";
                                //if (HitLinkVisitor.ContainsKey(URl))
                                //{
                                hline += entry.Value + ",";
                                //}
                                //else
                                //{
                                //    hline += "0" + ",";
                                //}
                                foreach (DataRow row in DT.Rows)
                                {
                                    if (row["MID"].ToString().ToLower().Equals(entry.Key.ToString().ToLower()))
                                    {
                                        OrderInfoPresent = true;
                                        hline += row["TotalOrders"].ToString() + ",";
                                        hline += row["TotalRevenue"].ToString() + ",";
                                    }
                                }
                                if (!OrderInfoPresent)
                                {
                                    hline += "0" + ",";
                                    hline += "0" + ",";
                                }
                                hline = hline.Trim(',');
                                sb.WriteLine(hline);
                            }
                        }
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    
                }

                SendFileasAttachment(FUllPAthwithFileName1, txtFileName1, Enddate.ToShortDateString());
                //SendEmailToAdmin("File sent successfully");              

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Incremental  Media Report " + ex.Message);
                Console.WriteLine("Error : " + ex.Message + " StackTrace : " + ex.StackTrace);
                SendEmailToAdmin(ex.Message);
                LogToFile(ex.Message);
                LogToFile(ex.StackTrace);
            }
        }

        protected int BindData(DateTime startDate, DateTime endDate, string VersionName)
        {            
            HitLinkVisitor.Clear();
            Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("tgelman", "china2006", ReportsEnum.eCommerceActivitySummary, TimeFrameEnum.Daily, startDate, endDate, 1000, 0, 0);
            for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
            {
                if (rptData.Rows[i].Columns[0].Value.ToString().ToLower().Contains("cpa_im"))
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToString(), rptData.Rows[i].Columns[3].Value.ToString());
                }
            }            
            CategoryUniqueVistiors = 0;           

            return CategoryUniqueVistiors;           

        }

        static void Main(string[] args)
        {
            DateTime ReportDate = DateTime.Now.AddDays(-1);
            Console.WriteLine("Start Incremental  Media Reports trynono.com " + ReportDate.ToString());
            Report DailyReports = new Report();
            DailyReports.LogToFile("Start Incremental  Media Reports " + DateTime.Now.ToString());                                    
            // DailyReports.CheckDataBase_Connection();           
            string versionname = "";
            string FileName_PostFix = "";
            int VersionID = 0;           
             DailyReports.LoadReport(ReportDate, VersionID, versionname, FileName_PostFix); // Version ID are as per PRODUCTION VersionID                        
                 
            Console.WriteLine("End  Incremental  Media Reports trynono.com");
            DailyReports.LogToFile("End Mercury Media Reports " + DateTime.Now.ToString());   
        }
    }
}
