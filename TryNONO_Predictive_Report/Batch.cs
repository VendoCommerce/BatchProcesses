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
using TryNONO_Predictive_Report.HitsLinks;
using Com.ConversionSystems.DataAccess;

namespace TryNONO_Predictive_Report
{
    class ReportBatch
    {
        private  List<string> versions = new List<string> ();
        static string Report_name = "NoNo_Predictive_Report";
        //enum ReportTypes : uint
        //{
        //    Daily = 1,
        //    weekly = 2,
        //    Monthly = 3
        //}
        static string report_filetype;
        private string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];
        public Hashtable HitLinkVisitor = new Hashtable();

        protected bool GenerateReport(DateTime startDate, DateTime endDate, string FUllPAthwithFileName)
        {
            bool result = false;
            try
            {
                DataTable reportData;
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.GetOrdersForDailyReport(StartOfDay( startDate),EndOfDay( endDate),out reportData);

                //get hitslink data using hitslink service , and update the versionSummary data accordingly   trynono_url
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("tgelman", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
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
                        result = CreateCSVFile(reportData, FUllPAthwithFileName);
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
                client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
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

        //private static bool SendFileasAttachment(string ReportFileName, string fileNameOnly)
        //{
        //    bool sendemail = false;
        //    try
        //    {
        //        string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["clientemail"];
        //        string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
        //        StringBuilder sb = new StringBuilder();
        //        MailMessage message = new MailMessage();

        //        message.To.Add(ToEmail);
        //        message.CC.Add(ToEmailcc);
        //        message.From = new MailAddress("info@conversionsystems.com");
        //        if (File.Exists(ReportFileName))
        //        {
        //            Attachment attachment1 = new Attachment(ReportFileName); //create the attachment
        //            attachment1.Name = ReportFileName;
        //            message.Attachments.Add(attachment1);
        //        }
        //        message.Subject = Report_name + " Reporting";
        //        // message.Body = "Please see attached Euro Report.";
        //        message.Body = "Please see attached daily report for: "+ Report_name ;
        //        message.IsBodyHtml = true;

        //        SmtpClient client;
        //        client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
        //        client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
        //        client.Send(message);
        //        sendemail = true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("Exception Details  : " + ex.ToString());
        //        sendemail = false;
        //        TryNONO_Predictive_Report.eTrraficReport Report1 = new TryNONO_Predictive_Report.eTrraficReport();
        //        Report1.LogToFile("Error " + ex.Message);
        //        Report1.LogToFile("Error " + ex.StackTrace);
        //    }
        //    return sendemail;
        //}


        //private static bool CreateHTMLFile(DataTable dt, string strFilePath)
        //{
        //    bool HTMLCreated = false;
        //    try
        //    {

        //        string tab = "\t";

        //        StringBuilder sb = new StringBuilder();

        //        sb.AppendLine("<html>");
        //        sb.AppendLine("<style type=\"text/css\">");
        //        sb.AppendLine("    table {background: #fff; text-align: left; border:1px solid #e5e5e5;font:8pt Calibri}");
        //        sb.AppendLine("    td, th {padding: 3px 3px;background:#F6F6F6 }");
        //        sb.AppendLine("    h1 {font: bold 10pt Calibri; margin-top: 12pt;}");
        //        sb.AppendLine("</style>");
        //        sb.AppendLine(tab + "<body>");
        //        sb.AppendLine(tab + tab + "<table>");


        //        // headers.
        //        sb.Append(tab + tab + tab + "<tr>");

        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            sb.AppendFormat("<td><h1>{0}</h1></td>", dc.ColumnName);
        //        }

        //        sb.AppendLine("</tr>");

        //        // data rows
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            sb.Append(tab + tab + tab + "<tr>");

        //            foreach (DataColumn dc in dt.Columns)
        //            {
        //                string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
        //                sb.AppendFormat("<td>{0}</td>", cellValue);
        //            }

        //            sb.AppendLine("</tr>");
        //        }

        //        sb.AppendLine(tab + tab + "</table>");
        //        sb.AppendLine(tab + "</body>");
        //        sb.AppendLine("</html>");
        //        WriteFile(sb.ToString (), strFilePath );
               
        //        HTMLCreated = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("HTML file not created: " + ex.Message);
        //        HTMLCreated = false;
        //    }
        //    return HTMLCreated;
        //}


        private static bool CreateCSVFile(DataTable dt, string strFilePath)
        {
            bool CSVCreated = false;
            try
            {
                // Create the CSV file.
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

        private void LoadReport(DateTime ReportDate)
        {
            try
            {
                //TODO: Remove for prod
                DateTime StartDate = StartOfDay(ReportDate);
                DateTime Enddate = EndOfDay(ReportDate);
                //DateTime StartDate = new DateTime(2014,3,31);
                //DateTime Enddate = new DateTime(2014, 7, 20);

                // File name format = PSWmmdd (ex. PSW0115.txt)                
                string reportFileName = Enddate.ToString("yyyyMMdd") + "_" + Report_name  ;
          
                targetPath = targetPath.Replace("\\\\", "\\");
                string FUllPAthwithFileName = targetPath + reportFileName+ report_filetype ;
                bool flag = GenerateReport(StartDate, Enddate, FUllPAthwithFileName);
       
                if (flag == true)
                {
                    Console.WriteLine(Report_name+" created");
                    LogToFile(Report_name+" created: " + FUllPAthwithFileName);
                }
                else
                {
                    LogToFile("Excel Report Created False");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + Report_name +" " + ex.Message);
                Console.WriteLine("Error : " + ex.Message + " StackTrace : " + ex.StackTrace);
                //SendEmailToAdmin(ex.Message);
                LogToFile(ex.Message);
                LogToFile(ex.StackTrace);
            }
        }

        static void Main(string[] args)
        {
            //set report preferences
            //ReportTypes report_type = ReportTypes.Daily;
            report_filetype = ".csv";
            
            //---------
            
            DateTime ReportDate=DateTime.Now.AddDays(-1);


            //if (report_type == ReportTypes.Daily)
            //{
                 //ReportDate = DateTime.Now.AddDays(-1);
            //}
            //else if (report_type == ReportTypes.weekly)
            //{
            //     ReportDate = DateTime.Now.AddDays(-6);
            //}
            //else if (report_type == ReportTypes.Monthly )
            //{
            //    int no_Days= DateTime.DaysInMonth ( DateTime.Now.Year , DateTime.Now.Month );
            //     ReportDate = DateTime.Now.AddDays(- no_Days);
            //}

            Console.WriteLine("Start " + Report_name + " reports generation.");

            TryNONO_Predictive_Report.ReportBatch DailyReports = new TryNONO_Predictive_Report.ReportBatch();
            
            DailyReports.LogToFile("Start  " + Report_name + " @ " + DateTime.Now.ToString());

            DailyReports.LoadReport(ReportDate);
            
            Console.WriteLine("End of " + Report_name );
            DailyReports.LogToFile("End " + Report_name + " @ "  + DateTime.Now.ToString());


        }
    }
}
