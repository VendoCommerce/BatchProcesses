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
         const string _predictive_Report_Name = "NoNo_Predictive_Report";
         const string _version_Report_Name = "NONO_Hair_ROI_RadioReport";
        //enum ReportPeriods : uint
        //{
        //    Daily = 1,
        //    Weekly = 2,
        //    Monthly = 3
        //}

        static ReportTypes _reportType;

        enum ReportTypes : uint
        {
            Predictive = 1,
            Version = 2
        }
         const string report_filetype=".csv";
        private string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];
        public Hashtable HitLinkVisitor = new Hashtable();

        private bool GenerateReport(DateTime startDate, DateTime endDate, string FUllPAthwithFileName,string reportFileName)
        {
            if (_reportType == ReportTypes.Predictive)
                return GeneratePredictiveReport(startDate, endDate, FUllPAthwithFileName);
            else
            {
                bool reportResult = GenerateVersionReport(startDate, endDate, FUllPAthwithFileName);
                if (reportResult)
                    SendFileasAttachment(FUllPAthwithFileName, reportFileName, _version_Report_Name);
                return reportResult;
            }

        }
        protected bool GenerateVersionReport(DateTime startDate, DateTime endDate, string FUllPAthwithFileName)
        {
            bool result = false;
            try
            {
                
                DateTime currentDay = startDate;
                while (currentDay <= endDate)
                {
                    HitLinkVisitor.Clear();
                    DataTable reportData;
                    DAL dal = new DAL();
                    //get the versionSummary data from db , using OrderManager
                    dal.SQLServer.GetOrdersForVersion(StartOfDay(currentDay), EndOfDay(currentDay), out reportData);


                    //get hitslink data using hitslink service , and update the versionSummary data accordingly   trynono_url
                    ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                    Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("tgelman", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, currentDay, currentDay, 100000000, 0, 0);
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
                            dataRow["Unique_Visits"] = HitLinkVisitor[versionName].ToString().ToLower();
                        else
                            dataRow["Unique_Visits"] = "0";
                        //dataRow["Revenue"]
                        dataRow["Date"] =currentDay.ToString("M-d-yyyy");
                    }
                    try
                    {
                        bool appendHeader = currentDay == startDate ? true : false;
                        result = CreateCSVFile(reportData, FUllPAthwithFileName,appendHeader);
                    }
                    catch (Exception ex)
                    {

                        string loginfo = "File creation status:" + result + Environment.NewLine + ex.Message.ToString();
                        LogToFile(loginfo);
                    }
                    currentDay = currentDay.AddDays(1);
                }

                return result;
            }
            catch (Exception ex)
            {

                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                return result;
            }
        }

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

                if (_reportType == ReportTypes.Predictive)
                    reportFileName = ReportDateTo.ToString("yyyyMMdd") + "_" + _predictive_Report_Name;
                else
                    reportFileName = _version_Report_Name + "_" +ReportDateTo.ToString("MMddyyyy") ;

                targetPath = targetPath.Replace("\\\\", "\\");
                string FUllPAthwithFileName = targetPath + reportFileName + report_filetype;
                bool flag = GenerateReport(ReportDateFrom, ReportDateTo, FUllPAthwithFileName,reportFileName);

                if (flag == true)
                {
                    Console.WriteLine(reportFileName + " created");
                    LogToFile(reportFileName + " created: " + FUllPAthwithFileName);
                }
                else
                {
                    LogToFile("Excel Report Created False");
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
            //set report preferences
            //ReportTypes report_type = ReportTypes.Daily;

            SetReportType(args);

            DateTime ReportDateFrom = DateTime.Today.AddDays(-1);
            DateTime ReportDateTo = DateTime.Today;
            if (_reportType == ReportTypes.Version)
            {
                ReportDateFrom = DateTime.Today.AddDays(-8);
                ReportDateTo = DateTime.Today.AddDays(-1);
            }

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

            string reportName = GetReportName();
            Console.WriteLine("Start " + reportName + " reports generation.");

            TryNONO_Predictive_Report.ReportBatch DailyReports = new TryNONO_Predictive_Report.ReportBatch();

            DailyReports.LogToFile("Start  " + reportName + " @ " + DateTime.Now.ToString());

            DailyReports.LoadReport(ReportDateFrom,ReportDateTo);

            Console.WriteLine("End of " + reportName);
            DailyReports.LogToFile("End " + reportName + " @ " + DateTime.Now.ToString());


        }

        static string GetReportName()
        {
            if (_reportType == ReportTypes.Predictive)
                return _predictive_Report_Name;
            return _version_Report_Name;

        }

        static void SetReportType(string[] args)
        {
            foreach (string argument in args)
            {
                if (argument.ToLower() == "predictive")
                {
                    _reportType = ReportTypes.Predictive;
                    return;
                }
            }
            _reportType = ReportTypes.Version;
        }
    }
}
