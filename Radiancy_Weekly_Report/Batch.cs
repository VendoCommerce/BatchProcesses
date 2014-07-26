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
using Radiancy_Weekly_Report.HitsLinks;
using Com.ConversionSystems.DataAccess;

namespace Radiancy_Weekly_Report
{
    class ReportBatch
    {
        const string _report_Name = "Radiancy Weekly Report";
        //enum ReportPeriods : uint
        //{
        //    Daily = 1,
        //    Weekly = 2,
        //    Monthly = 3
        //}

        //static ReportTypes _reportType;

        enum ReportTypes : uint
        {
            Predictive = 1,
            Version = 2
        }
         const string report_filetype=".csv";
        private static string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];

        DAL dal = new DAL();

        private bool GenerateReport(DateTime startDate, DateTime endDate)
        {
            //try
            //{

            Reports reports = new Reports();
            DataTable reportTable;
            string fileNameTrailer =  startDate.ToString("M.dd") + " - " + endDate.ToString("M.dd.yyyy");
            string reportFileName;
            string reportName;

            ///// *********   NoNo Predictive Report  *********////////
            reportName = "NoNo Web Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            dal.SQLServer.Get_NoNo_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportTable);

            //reportTable= reports.Get_NoNo_Web_Report(startDate, endDate);
            if (reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath +reportFileName, true);
                SendFileasAttachment(targetPath +reportFileName, reportFileName, reportName);
            }
            return true;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error " + reportFileName + " " + ex.Message);
            //    Console.WriteLine("Error : " + ex.Message + " StackTrace : " + ex.StackTrace);
            //    //SendEmailToAdmin(ex.Message);
            //    LogToFile(ex.Message);
            //    LogToFile(ex.StackTrace);
            //}

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
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
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
                message.Subject = reportName + " Reporting";
                // message.Body = "Please see attached Euro Report.";
                message.Body = "Please see attached report for: " + reportName;
                message.IsBodyHtml = true;

                SmtpClient client;
                client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(message);
                sendemail = true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception Details  : " + ex.ToString());
                sendemail = false;
                Logging.LogToFile("Error " + ex.Message);
                Logging.LogToFile("Error " + ex.StackTrace);
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
                            sw.Write(dr[i]);
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

        static void Main(string[] args)
        {

            DateTime ReportDateFrom = DateTime.Today.AddDays(-8);
            DateTime ReportDateTo = DateTime.Today.AddDays(-1);
            //if (_reportType == ReportTypes.Version)
            //{
            //    ReportDateFrom = DateTime.Today.AddDays(-8);
            //    ReportDateTo = DateTime.Today.AddDays(-1);
            //}

            Console.WriteLine("Start " + _report_Name + " reports generation.");

            Radiancy_Weekly_Report.ReportBatch reportBatch = new Radiancy_Weekly_Report.ReportBatch();

            Logging.LogToFile(string.Format( "Start {0} @ {1}",_report_Name , DateTime.Now.ToString()));

            bool flag = reportBatch.GenerateReport(ReportDateFrom,ReportDateTo);

            if (flag == true)
            {
                Console.WriteLine(_report_Name + " created");
                Logging.LogToFile(_report_Name + " created: " + targetPath);
            }
            else
            {
                Logging.LogToFile("Excel Report Created False");
            }

            Console.WriteLine("End of " + _report_Name);
            Logging.LogToFile("End " + _report_Name + " @ " + DateTime.Now.ToString());


        }
    }
}
