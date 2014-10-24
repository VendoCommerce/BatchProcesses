using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Com.ConversionSystems.DataAccess;
using Radiancy_Weekly_Report.HitsLinks;
using System.Collections;

namespace Radiancy_Weekly_Report
{
    class Reports
    {
        public bool Get_NoNo_Skin_Web_Report(DateTime startDate, DateTime endDate, out DataTable returnTable)
        {
            bool result = false;
            Hashtable HitLinkVisitor = new Hashtable();

            try
            {
                //DateTime currentDay = startDate;
                HitLinkVisitor.Clear();
                DataTable reportData;
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.Get_NoNo_Skin_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportData);

                //get hitslink data using hitslink service , and update the versionSummary data accordingly 
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("toddgelman", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
                }
                //Update Version List information
                foreach (DataRow dataRow in reportData.Rows)
                {
                    //decimal visitor = 0;
                    string versionName = dataRow["Version"].ToString().ToLower();
                    if (HitLinkVisitor.ContainsKey(versionName))
                    {
                        decimal visitor = Convert.ToDecimal(HitLinkVisitor[versionName].ToString().ToLower());
                        visitor = Math.Abs(visitor);
                        if (visitor > 0)
                        {
                            dataRow["Unique_Visitors"] = visitor.ToString();
                            dataRow["Conversion_Perc"] = Math.Round((Convert.ToDecimal(dataRow["Total_Orders"]) * 100) / visitor, 1).ToString() + '%';
                            dataRow["Revenue_Per_Visitor"] = (Convert.ToDecimal(dataRow["Total_Revenue"]) / visitor).ToString("N");
                        }
                        //dataRow["Date"] =endDate.ToString("M-d-yyyy");
                    }
                }
                returnTable = reportData;
                return true;
            }
            catch (Exception ex)
            {
                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                Logging.LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                returnTable = null;
                return false;
            }
        }

        public bool Get_Neova_Insert_Report(DateTime startDate, DateTime endDate, out DataTable returnTable)
        {
            bool result = false;
            Hashtable HitLinkVisitor = new Hashtable();

            try
            {
                //DateTime currentDay = startDate;
                HitLinkVisitor.Clear();
                DataTable reportData;
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.Get_Neova_Insert_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportData);

                //get hitslink data using hitslink service , and update the versionSummary data accordingly 
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("neova", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
                }
                //get hitslink data for the url list using hitslink service
                rptData = new ReportWSSoapClient().GetDataFromTimeframe("neova2", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    string versionName = rptData.Rows[i].Columns[0].Value.ToLower();
                    if (!HitLinkVisitor.ContainsKey(versionName))
                        HitLinkVisitor.Add(versionName, rptData.Rows[i].Columns[9].Value);
                }
                //Update Version List information
                foreach (DataRow dataRow in reportData.Rows)
                {
                    //decimal visitor = 0;
                    string versionName = dataRow["Insert_Traffic"].ToString().ToLower();
                    string url = dataRow["URL"].ToString().ToLower();
                    decimal visitor = 0;
                    if (HitLinkVisitor.ContainsKey(versionName))
                        visitor = Convert.ToDecimal(HitLinkVisitor[versionName].ToString().ToLower());
                    if (HitLinkVisitor.ContainsKey(url))
                        visitor = Convert.ToDecimal(HitLinkVisitor[url].ToString().ToLower());
                    visitor = Math.Abs(visitor);
                    if (visitor > 0)
                    {
                        dataRow["Unique_Visitors"] = visitor.ToString();
                        dataRow["Conversion_Perc"] = Math.Round((Convert.ToDecimal(dataRow["Total_Orders"]) * 100) / visitor, 1).ToString() + '%';
                        dataRow["Revenue_Per_Visitor"] = (Convert.ToDecimal(dataRow["Total_Revenue"]) / visitor).ToString("N");
                    }
                    //dataRow["Date"] =endDate.ToString("M-d-yyyy");
                }
                returnTable = reportData;
                return true;
            }
            catch (Exception ex)
            {
                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                Logging.LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                returnTable = null;
                return false;
            }
        }

        public bool Get_MBI_Neova_Report(DateTime startDate, DateTime endDate, out DataTable returnTable)
        {
            bool result = false;
            Hashtable HitLinkVisitor = new Hashtable();

            try
            {
                //DateTime currentDay = startDate;
                HitLinkVisitor.Clear();
                DataTable reportData;
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.Get_MBI_Neova_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportData);

                //get hitslink data using hitslink service , and update the versionSummary data accordingly 
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("neova", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
                }
                //get hitslink data for the url list using hitslink service
                rptData = new ReportWSSoapClient().GetDataFromTimeframe("neova2", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    string versionName = rptData.Rows[i].Columns[0].Value.ToLower();
                    if (!HitLinkVisitor.ContainsKey(versionName))
                        HitLinkVisitor.Add(versionName, rptData.Rows[i].Columns[9].Value);
                }
                //Update Version List information
                foreach (DataRow dataRow in reportData.Rows)
                {
                    //decimal visitor = 0;
                    string versionName = dataRow["Insert_Traffic"].ToString().ToLower();
                    string url = dataRow["URL"].ToString().ToLower();
                    decimal visitor = 0;
                    if (HitLinkVisitor.ContainsKey(versionName))
                        visitor = Convert.ToDecimal(HitLinkVisitor[versionName].ToString().ToLower());
                    if (HitLinkVisitor.ContainsKey(url))
                        visitor = Convert.ToDecimal(HitLinkVisitor[url].ToString().ToLower());
                    visitor = Math.Abs(visitor);
                    if (visitor > 0)
                    {
                        dataRow["Unique_Visitors"] = visitor.ToString();
                        dataRow["Conversion_Perc"] = Math.Round((Convert.ToDecimal(dataRow["Total_Orders"]) * 100) / visitor, 1).ToString() + '%';
                        dataRow["Revenue_Per_Visitor"] = (Convert.ToDecimal(dataRow["Total_Revenue"]) / visitor).ToString("N");
                    }
                    //dataRow["Date"] =endDate.ToString("M-d-yyyy");
                }
                returnTable = reportData;
                return true;
            }
            catch (Exception ex)
            {
                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                Logging.LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                returnTable = null;
                return false;
            }
        }
     
        public bool Get_Kyrobak_Web_Report(DateTime startDate, DateTime endDate, out DataTable returnTable)
        {
            bool result = false;
            Hashtable HitLinkVisitor = new Hashtable();

            try
            {
                //DateTime currentDay = startDate;
                HitLinkVisitor.Clear();
                DataTable reportData;
                List<string> OrderPresentKey = new List<string>();
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.Get_Kyrobak_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportData);

                //get hitslink data using hitslink service , and update the versionSummary data accordingly 
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("trykyro", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate.AddHours(3), endDate.AddHours(-21), 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
                }
                //get hitslink data for the url list using hitslink service
                rptData = new ReportWSSoapClient().GetDataFromTimeframe("trykyro2", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate.AddHours(3), endDate.AddHours(-21), 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    string versionName = rptData.Rows[i].Columns[0].Value.ToLower();
                    if (!HitLinkVisitor.ContainsKey(versionName))
                        HitLinkVisitor.Add(versionName, rptData.Rows[i].Columns[9].Value);
                }
                //Update Version List information
                foreach (DataRow dataRow in reportData.Rows)
                {
                    //decimal visitor = 0;
                    string versionName = dataRow["Version"].ToString().ToLower();
                    string url = dataRow["Sites"].ToString().ToLower();
                    string versionUrl = url + "-" + versionName;
                    decimal visitor = 0;
                    if (HitLinkVisitor.ContainsKey(versionUrl))
                    //    visitor = Convert.ToDecimal(HitLinkVisitor[versionName].ToString().ToLower());
                    //if (HitLinkVisitor.ContainsKey(url))
                    {
                        visitor = Convert.ToDecimal(HitLinkVisitor[versionUrl].ToString().ToLower());
                        OrderPresentKey.Add(versionUrl);
                    }
                    visitor = Math.Abs(visitor);
                    if (visitor > 0)
                    {
                        dataRow["Unique_Visitors"] = visitor.ToString();
                        dataRow["Conversion_Perc"] = Math.Round((Convert.ToDecimal(dataRow["Total_Orders"]) * 100) / visitor, 1).ToString() + '%';
                        dataRow["Revenue_Per_Visitor"] = (Convert.ToDecimal(dataRow["Total_Revenue"]) / visitor).ToString("N");
                    }
                    //dataRow["Date"] =endDate.ToString("M-d-yyyy");
                }

                //for Items which are not part of order

                foreach (string key in HitLinkVisitor.Keys)
                {
                    if ((key.Contains("-im") || key.Contains("im_")) && !OrderPresentKey.Contains(key))
                        reportData.Rows.Add(new object[] { key.Substring(key.IndexOf("-")+1).ToUpper(), key.Substring(0, key.IndexOf("-")),
                    HitLinkVisitor[key].ToString().ToLower(), "0", "0%", "0", "0", "0" });
                }

                returnTable = reportData;
                return true;
            }
            catch (Exception ex)
            {
                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                Logging.LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                returnTable = null;
                return false;
            }
        }

        public bool Get_NoNo_Web_Report_Report(DateTime startDate, DateTime endDate, out DataTable returnTable)
        {
            bool result = false;
           

            try
            {
               
                DataTable reportData;
                List<string> OrderPresentKey = new List<string>();
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.Get_NoNo_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportData);



                DataTable returnTable1 = reportData.Clone();
                try
                {
                   
                    foreach (DataRow dataRow in reportData.Rows)
                    {
                        string _url =  dataRow["URL"].ToString().ToLower();
                        double _num;
                        //string _url_checkNumber = _url.Substring(_url.IndexOf(".com", System.StringComparison.Ordinal) - 3, 3);
                        //bool isNum = double.TryParse(_url_checkNumber, out _num);
                        Regex re = new Regex(@"(?:nono|nonopro)+\d+.com");
                        Match m = re.Match(_url);
                        if (m.Success)
                        {
                            returnTable1.ImportRow(dataRow);
                            
                        }
                    }
                }
                catch
                {}

                
                //for Items which are not part of order

                returnTable = returnTable1;
                return true;
            }
            catch (Exception ex)
            {
                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                Logging.LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                returnTable = null;
                return false;
            }
        }
    }
}
