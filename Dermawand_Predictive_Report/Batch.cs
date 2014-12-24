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
using Dermawand_Predictive_Report.HitsLinks;
using Com.ConversionSystems.DataAccess;

namespace Dermawand_Predictive_Report
{
    class ReportBatch
    {
        const string _predictive_Report_Name = "Dermawand_Predictive_Report";
        //enum ReportPeriods : uint
        //{
        //    Daily = m,
        //    Weekly = 2,
        //    Monthly = 3
        //}

        static ReportTypes _reportType;

        enum ReportTypes : uint
        {
            Predictive_NoNo = 1,
            Predictive_Dermawand = 2,
            Version = 3
        }
         const string report_filetype=".csv";
        private string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];
        public Hashtable HitLinkVisitor = new Hashtable();

        protected bool GeneratePredictiveReport(DateTime startDate, DateTime endDate, string FUllPAthwithFileName)
        {
            bool result = false;
            try
            {
                DataTable reportData;
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.GetOrdersForPredictive(StartOfDay( startDate),EndOfDay( endDate),out reportData);

                //get hitslink data using hitslink service , and update the versionSummary data accordingly   trynono_url
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("dermawand", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
                }

                //Update Version List information
                foreach (DataRow dataRow in reportData.Rows)
                {
                    //decimal visitor = 0;
                    string versionName = dataRow["Version_Code"].ToString().ToLower();
                    if (HitLinkVisitor.ContainsKey(versionName))
                        dataRow["Visits"] = HitLinkVisitor[versionName].ToString().ToLower();
                    else
                        dataRow["Visits"] = "0";
                    //dataRow["Revenue"]
                    dataRow["Date"] = startDate.ToString("dd-MMM-yy");
                }
                try
                {
                        result = CreateCSVFile(reportData, FUllPAthwithFileName,true);
                }
                catch (Exception ex)
                {

                    string loginfo = "File creation status:" + result + Environment.NewLine + ex.Message.ToString();
                    LogToFile(loginfo);
                }
                return result;

            }
            catch (Exception ex)
            {

                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                LogToFile(loginfo);
                Console.WriteLine (ex.ToString());
                return result;
            }
            
        }

        public static DateTime EndOfDay(DateTime date)
        {
            return date.AddHours(21).AddMinutes(00).AddSeconds(00);
        }

        public static DateTime StartOfDay(DateTime date)
        {
            return date.AddDays(-1).AddHours(21).AddMinutes(00).AddSeconds(00);
        }

        private void LogToFile(string AdditionalInfo)
        {
            bool bResult = false;
            StreamWriter log;

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now);
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
                //client = new SmtpClient();
                //client.Send(oMsg);                
                client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                //client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.Send(oMsg);
                bResult = true;
            }
            catch (Exception ex)
            {
            }
            return bResult;
        }

        //public void SendEmailToAdmin(string ErrorMessage)
        //{
        //    try
        //    {
        //        StringBuilder _sbEmailMessageBody = new StringBuilder();
        //        _sbEmailMessageBody.Append("<html><body><table>");
        //        _sbEmailMessageBody.Append("<tr><td><b>" + Report_name +" Report Batch :</b></td></tr>");
        //        _sbEmailMessageBody.Append("<tr><td>This report was generated at " + DateTime.Now.ToString("MM/dd/yyyy-HH:mm") + "</td></tr>");
        //        _sbEmailMessageBody.Append("<tr><td>" + ErrorMessage + "</td></tr>");
        //        _sbEmailMessageBody.Append("<tr><td> Please do not reply to this email.</b></td></tr>");
        //        _sbEmailMessageBody.Append("</table></body></html>");
        //        string AdminEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
        //        string fromEmail = System.Configuration.ConfigurationSettings.AppSettings["fromEmail"];
        //        MailMessage _oMailMessage = new MailMessage(fromEmail, AdminEmail,  Report_name +" Report Generation Status", _sbEmailMessageBody.ToString());
        //        _oMailMessage.IsBodyHtml = true;
        //        _oMailMessage.Body = _sbEmailMessageBody.ToString();
        //        SendMail(_oMailMessage);
        //    }
        //    catch (Exception e)
        //    {
        //        // log.LogToFile("Error sending email---" + e.Message);
        //    }
        //}

        private bool SendFileasAttachment(string ReportFileName, string fileNameOnly,string reportName)
        {
            bool sendemail = false;
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["clientemail"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();

                message.To.Add(ToEmail);
                message.CC.Add(ToEmailcc);
                message.From = new MailAddress("info@conversionsystems.com");
                if (File.Exists(ReportFileName))
                {
                    Attachment attachment1 = new Attachment(ReportFileName); //create the attachment
                    attachment1.Name = fileNameOnly+report_filetype;
                    message.Attachments.Add(attachment1);
                }
                message.Subject = reportName.Replace("_"," ");
                // message.Body = "Please see attached Euro Report.";
                message.Body = "Please see attached report for: " + reportName.Replace("_", " ");
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                //client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(message);
                sendemail = true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception Details  : " + ex.ToString());
                sendemail = false;
                LogToFile("Error " + ex.Message);
                LogToFile("Error " + ex.StackTrace);
            }
            return sendemail;
        }

        private static bool CreateCSVFile(DataTable dt, string strFilePath,bool appendHeader)
        {
            bool CSVCreated = false;
            try
            {
                // Create the CSV file.
                StreamWriter sw = new StreamWriter(strFilePath, true);
                int iColCount = dt.Columns.Count;

                // First we will write the headers.
                //DataTable dt = m_dsProducts.Tables[0];
                if (appendHeader)
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        sw.Write(dt.Columns[i].ToString().Replace("_", " "));
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                //string value = "";
                //string fomattedValue = "";
                //int length = 0;

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
        
        public static void WriteFile(string content , string apath)
        {
            string path = @apath;

           
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(content);

                }
        }

        private void LoadReport(DateTime ReportDateFrom, DateTime ReportDateTo)
        {
            string reportFileName = string.Empty;
            try
            {
                //TODO: Remove for prod
                //ReportDateFrom = new DateTime(2014, 7, 14);
                //ReportDateTo = new DateTime(2014, 7, 20);

                reportFileName = _predictive_Report_Name + ReportDateTo.ToString("-yyyy-MM-dd");

                targetPath = targetPath.Replace("\\\\", "\\");
                string FUllPAthwithFileName = targetPath + reportFileName + report_filetype;

                if (GeneratePredictiveReport(ReportDateFrom, ReportDateTo, FUllPAthwithFileName))
                {
                    UploadToFtp(FUllPAthwithFileName);

                    Console.WriteLine(reportFileName + " created");
                    LogToFile(reportFileName + " created: " + FUllPAthwithFileName);
                }
                else
                {
                    LogToFile("Error Creating report");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + reportFileName + " " + ex.Message);
                Console.WriteLine("Error : " + ex.Message + " StackTrace : " + ex.StackTrace);
                //SendEmailToAdmin(ex.Message);
                LogToFile(ex.Message);
                LogToFile(ex.StackTrace);
            }
        }

        static void Main(string[] args)
        {
            //Test
            //DateTime ReportDateFrom = Convert.ToDateTime("2014/12/17");
            //DateTime ReportDateTo = Convert.ToDateTime("2014/12/17");
            //Prod
            DateTime ReportDateFrom = DateTime.Today.AddDays(-1);
            DateTime ReportDateTo = DateTime.Today.AddDays(-1);

            Dermawand_Predictive_Report.ReportBatch DailyReports = new Dermawand_Predictive_Report.ReportBatch();

            DailyReports.LogToFile("Start  Dermawand Predictive Report @ " + DateTime.Now.ToString());

            DailyReports.LoadReport(ReportDateFrom,ReportDateTo);

            Console.WriteLine("End of  Dermawand Predictive Report" );
            DailyReports.LogToFile("End  Dermawand Predictive Report @ " + DateTime.Now.ToString());


        }

        private static void UploadToFtp(string filePath)
        {
            try
            {
                string FTPAddress="ftp://www.ftp-directory.com/";
                string username="ConvSys"; 
                string password="c0nV$ys";
                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/Dermawand/" + Path.GetFileName(filePath));

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
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                //sendemail(filePath);
            }

        }
    }
}
