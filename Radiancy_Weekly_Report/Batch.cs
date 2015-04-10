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
using System.Configuration;

namespace Radiancy_Weekly_Report
{
    class ReportBatch
    {
        const string _report_Name = "Radiancy Weekly Report";
        
        enum ReportTypes : uint
        {
            Predictive = 1,
            Version = 2
        }
        const string report_filetype = ".csv";
        private static string targetPath = System.Configuration.ConfigurationSettings.AppSettings["targetPath"];

        DAL dal = new DAL();

        private bool GenerateReport(DateTime startDate, DateTime endDate)
        {
            
            Reports reports = new Reports();
            DataTable reportTable;
            string fileNameTrailer = startDate.ToString("M.dd") + " - " + endDate.AddDays(-1).ToString("M.dd.yyyy");
            string reportFileName;
            string reportName;
            bool reportSuccess = false;
            ///// *********   NoNo Web Report  *********////////
            reportName = "NoNo Web Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            reportSuccess = reports.Get_NoNo_Web_Report_Report(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportTable);
           // reportSuccess = dal.SQLServer.Get_NoNo_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportTable);

            if (reportSuccess && reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath + reportFileName, true);
                SendFileasAttachment(targetPath + reportFileName, reportFileName, reportName);
            }

            ///// *********   NoNo Skin Web Report  *********////////
            reportName = "NoNo Skin Web Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            reportSuccess = reports.Get_NoNo_Skin_Web_Report(startDate, endDate, out reportTable);

            if (reportSuccess && reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath + reportFileName, true);
                SendFileasAttachment(targetPath + reportFileName, reportFileName, reportName);
            }

            //TODO: should get proper visitor list 
            ///// *********  Neova Insert Report  *********////////
            reportName = "Neova Insert Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            reportSuccess = reports.Get_Neova_Insert_Report(startDate, endDate, out reportTable);

            if (reportSuccess && reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath + reportFileName, true);
                SendFileasAttachment(targetPath + reportFileName, reportFileName, reportName);
            }

            //TODO: should get proper visitor list 
            ///// *********  Neova Insert Report  *********////////
            reportName = "MBI Neova Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            reportSuccess = reports.Get_MBI_Neova_Report(startDate, endDate, out reportTable);

            if (reportSuccess && reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath + reportFileName, true);
                SendFileasAttachment(targetPath + reportFileName, reportFileName, reportName);
            }

            //TODO: Should get proper versions based version report
            ///// *********  MBI WebReport *********////////
            reportName = "MBI Web Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            reportSuccess = dal.SQLServer.Get_MBI_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportTable);

            if (reportSuccess && reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath + reportFileName, true);
                SendFileasAttachment(targetPath + reportFileName, reportFileName, reportName);
            }

            //TODO: should get proper visitor list 
            ///// *********  Kyrobak Web Report   *********////////
            reportName = "Kyrobak Web Report";
            reportFileName = reportName + " " + fileNameTrailer + report_filetype;
            reportSuccess = reports.Get_Kyrobak_Web_Report(startDate, endDate, out reportTable);

            if (reportSuccess && reportTable != null)
            {
                CreateCSVFile(reportTable, targetPath + reportFileName, true);
                SendFileasAttachment(targetPath + reportFileName, reportFileName, reportName);
            }
            return true;
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

        private bool SendFileasAttachment(string ReportFileName, string fileNameOnly, string reportName)
        {
            bool sendemail = false;
            try
            {
                string ToEmail = ConfigurationSettings.AppSettings[reportName + "_List"];
                string ToEmailcc = ConfigurationSettings.AppSettings["sendemailtocc"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();

                message.To.Add(ToEmail);
                message.CC.Add(ToEmailcc);
                message.From = new MailAddress(ConfigurationSettings.AppSettings["fromEmail"]);
                if (File.Exists(ReportFileName))
                {
                    Attachment attachment1 = new Attachment(ReportFileName); //create the attachment
                    attachment1.Name = fileNameOnly + report_filetype;
                    message.Attachments.Add(attachment1);
                }
                message.Subject = ConfigurationSettings.AppSettings[reportName + "_Subject"];
                message.Body = "Please see attached reports for the previous week.<br>Thank you, <br>Conversion Systems";
                message.IsBodyHtml = true;

                sendemail = SendMail(message);
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

        private static bool CreateCSVFile(DataTable dt, string strFilePath, bool appendHeader)
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

        public static void WriteFile(string content, string apath)
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

            DateTime ReportDateFrom = DateTime.Today.AddDays(-7);//.AddHours(-3);
            DateTime ReportDateTo = DateTime.Today;//.AddHours(-3);

            //TODO: Comment for prod
            ReportDateFrom = DateTime.Parse("3/30/2015");
            ReportDateTo = DateTime.Parse("4/6/2015");

            Console.WriteLine("Start " + _report_Name + " reports generation.");

            Radiancy_Weekly_Report.ReportBatch reportBatch = new Radiancy_Weekly_Report.ReportBatch();

            Logging.LogToFile(string.Format("Start {0} @ {1}", _report_Name, DateTime.Now.ToString()));

            bool flag = reportBatch.GenerateReport(ReportDateFrom, ReportDateTo);

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
