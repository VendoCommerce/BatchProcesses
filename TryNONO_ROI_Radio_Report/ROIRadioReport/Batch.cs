﻿using System;
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
using ROIRadioReport.com.hitslink.www;

namespace ROIRadioReport
{
    class Batch
    {
        private string targetPath = "C:/BatchFiles/TRYNONO_ROIRadio/";
        private string targetPathLog = "C:/BatchFiles/TRYNONO_ROIRadio/Log/Log.txt";

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
            myDataColumn.ColumnName = "VersionCode";
            myDataTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.DateTime");
            myDataColumn.ColumnName = "Date";
            myDataTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Unique Visits";
            myDataTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Orders";
            myDataTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "Revenue";
            myDataTable.Columns.Add(myDataColumn);
            // System.String  System.Int32  System.DateTime  System.double;
            NewTempDataTable = myDataTable;
        }

        // Add table rows
        private void AddDataToTable(string VersionCode, DateTime Date, int UniqueVisits, int Orders, Double Revenue)
        {
            DataRow row;
            row = NewTempDataTable.NewRow();
            row["VersionCode"] = VersionCode.ToUpper();
            row["Date"] = Date;
            row["Unique Visits"]=UniqueVisits;
            row["Orders"] = Orders;
            row["Revenue"] = Revenue;            
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

        private static void SendExceptionEmail(Exception ex)
        {   
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["sendemailtoError"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtoccError"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(ToEmail));
                message.CC.Add(new MailAddress(ToEmailcc));
                message.From = new MailAddress("info@conversionsystems.com");

                message.Subject = "Batch Error TryNoNo - ROI Radio Reports Batch";
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

        private static bool SendFileasAttachment(string txtFileName, string fileNameOnly)
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
                // message.CC.Add(new MailAddress(ToEmailcc));
                message.From = new MailAddress("info@conversionsystems.com");

                if (File.Exists(txtFileName))
                {
                    Attachment attachment1 = new Attachment(txtFileName); //create the attachment
                    attachment1.Name = fileNameOnly;
                    message.Attachments.Add(attachment1);
                }

                message.Subject = "No No Hair - Weekly ROI Radio Campaign Results ";
                message.Body = "Please see attached for last week's results for the ROI Radio Campaign.";
                message.IsBodyHtml = true;

                SmtpClient client;
                client = new SmtpClient();                
                client.Send(message);
                sendemail = true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception Details  : " + ex.ToString());
                sendemail = false;
                SendExceptionEmail(ex);
            }
            return sendemail;
        }

        private Hashtable BindData(DateTime ReportDate, DateTime ReportEndDate) // , DateTime endDate)
        {            
            Hashtable hashtable = new Hashtable();
            try
            {
                DateTime endDate = ReportDate; // ReportEndDate;  // new DateTime(ReportDate.Year, ReportDate.Month, ReportDate.Day, 21, 0, 0);
                DateTime startDate = ReportDate;  //  new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 21, 0, 0);

                Console.WriteLine(startDate);
                Console.WriteLine(endDate);

                StringBuilder sb = new StringBuilder();
                ReportWS ws = new ReportWS();
                // int i = 0;
                Data rptData = default(Data);
                //   Call the webservice and retrieve the report data                 
                rptData = ws.GetDataFromTimeframe("tgelman", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);

                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    hashtable.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
                    // Console.WriteLine("Unique Clicks " + rptData.Rows[i].Columns[0].Value + "-" + rptData.Rows[i].Columns[9].Value);
                }                
            }
            catch { }
            return hashtable;            
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

        private bool Excel_FromDataTable(DataTable dt, string excelFileName)
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
        
        private void Load_ROIRadio_Report(DateTime ReportDate, DateTime ReportEndDate, Hashtable HitLinkVisitor)
        {
            string VersionCode = "";
            int UniqueVisits=0;
            int Orders=0;
            Double Revenue;
            DataTable DT = getDataTableByDate("pr_report_order_ROI_Radio", ReportDate.AddHours(-3), ReportEndDate.AddHours(-3));
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow r in DT.Rows)
                {                    
                    Revenue=0;
                    Orders=0;
                    VersionCode = r["Title"].ToString().ToLower();
                    Revenue = Convert.ToDouble(r["TotalRevenue"].ToString());
                    Orders = Convert.ToInt32(r["TotalOrders"].ToString());
                    UniqueVisits = 0;
                    if (HitLinkVisitor.ContainsKey(VersionCode))
                    {
                        UniqueVisits = Convert.ToInt32(HitLinkVisitor[VersionCode].ToString());
                    }
                    AddDataToTable(VersionCode, ReportDate, UniqueVisits, Orders, Revenue);
                }
            }
            else
            {

            }
        }

        static void Main(string[] args)
        {
            Batch batch1 = new Batch();
                                    
            DateTime tDate = DateTime.Now;
            string date = string.Empty;
            date = tDate.ToString("MM/dd/yyyy");
            DateTime startDate = Convert.ToDateTime(date + " 00:00:00");
            // DateTime endDate = Convert.ToDateTime(date + " 23:59:59");
                        
            DateTime ReportDate = startDate.AddDays(-7);
            DateTime ReportEndDate = ReportDate.AddDays(1);
            // batch1.CheckDataBase_Connection();
            batch1.LogToFile("Start ROIRadio Report "+DateTime.Now.ToString());
            batch1.CreateDataTable();

            Hashtable hsHitsLinkData = new Hashtable();
            for(int cnt=7 ; cnt>=1; cnt--)            
            {
                ReportDate = startDate.AddDays(-cnt);
                ReportEndDate = ReportDate.AddDays(1);
                Console.WriteLine("Start Date " + ReportDate.ToString() + " & EndDate" + ReportEndDate.ToString());
                hsHitsLinkData.Clear();
                hsHitsLinkData =  batch1.BindData(ReportDate, ReportEndDate);
                batch1.Load_ROIRadio_Report(ReportDate, ReportEndDate, hsHitsLinkData);
            }

            string excelFileName = "NONOHairROIRadioReport_" + DateTime.Now.ToString("MMddyyyy") + ".xls";
            batch1.targetPath = batch1.targetPath.Replace("\\\\", "\\");
            string FUllPAthwithFileName = batch1.targetPath + excelFileName;
            Console.WriteLine("Rows " + batch1.NewTempDataTable.Rows.Count.ToString());
            batch1.NewTempDataTable.DefaultView.Sort = "VersionCode asc, Date asc";
            bool flag = batch1.Excel_FromDataTable(batch1.NewTempDataTable, FUllPAthwithFileName);

            if (flag == true)
            {
                batch1.LogToFile("Excel Report Created " + FUllPAthwithFileName);
                bool emailSent = SendFileasAttachment(FUllPAthwithFileName, excelFileName);
                if (emailSent == false)
                {
                    batch1.LogToFile("Error Sending Files as Attachment ROI Radio Report");
                }
                else
                {
                    batch1.LogToFile("ROI Radio Report created and emailed successfully");
                }
            }
            batch1.LogToFile("End ROIRadio Report "+DateTime.Now.ToString());                        
        }
    
    }
}
