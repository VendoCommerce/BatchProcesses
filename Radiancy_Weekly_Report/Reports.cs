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
                HitLinkVisitor.Clear();
                //get hitslink data using hitslink service , and update the versionSummary data accordingly 
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();

                //get hitslink data for the url list using hitslink service
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("trykyro", "china2006", ReportsEnum.eCommerceActivitySummary, TimeFrameEnum.Daily, startDate.AddHours(3), endDate.AddHours(3), 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[3].Value);
                }

                DataTable returnTable1 = reportData.Clone();
                try
                {

                    foreach (DataRow dataRow in reportData.Rows)
                    {
                        string _url = dataRow["SID"].ToString().ToLower();

                        if (_url.Contains("im_"))
                        {
                            returnTable1.ImportRow(dataRow);
                            OrderPresentKey.Add(_url);

                        }
                    }
                }
                catch
                { }



                foreach (DataRow dataRow in returnTable1.Rows)
                {
                    string versionName = dataRow["sid"].ToString().ToLower();
                    decimal visitor = 0;
                    foreach (string key in HitLinkVisitor.Keys)
                    {
                        try
                        {
                            if ((key.Contains(versionName)))
                            {
                                visitor = Convert.ToDecimal(HitLinkVisitor[key].ToString().ToLower());
                            }
                        }
                        catch
                        {


                        }
                    }
                    visitor = Math.Abs(visitor);
                    if (visitor > 0)
                    {
                        dataRow["UniqueVisitors"] = visitor.ToString();
                        try
                        {
                            dataRow["Conversion"] = Math.Round((Convert.ToDecimal(dataRow["TotalOrders"]) * 100) / visitor, 2) + "%";
                        }
                        catch
                        {
                            dataRow["Conversion"] = "0%";
                        }
                    }
                    else
                    {
                        dataRow["Conversion"] = "100%";
                    }

                }

                foreach (string key in HitLinkVisitor.Keys)
                {
                    try
                    {
                        if (key.Contains("im_") && !OrderPresentKey.Contains(key))
                            returnTable1.Rows.Add(new object[] { key.ToUpper(), HitLinkVisitor[key].ToString(), "0", "0", "0%", "0" });
                    }
                    catch
                    {


                    }

                }

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

        public bool Get_NoNo_Web_Report_Report(DateTime startDate, DateTime endDate, out DataTable returnTable)
        {
            bool result = false;
            Hashtable HitLinkVisitor = new Hashtable();

            try
            {

                DataTable reportData;
                List<string> OrderPresentKey = new List<string>();
                DAL dal = new DAL();
                //get the versionSummary data from db , using OrderManager
                dal.SQLServer.Get_NoNo_Web_Report_Table(startDate, endDate, out reportData);
                HitLinkVisitor.Clear();
                //get hitslink data using hitslink service , and update the versionSummary data accordingly 
                ReportWSSoapClient hitslinkWS = new ReportWSSoapClient();

                //get hitslink data for the url list using hitslink service
                Data rptData = new ReportWSSoapClient().GetDataFromTimeframe("tgelman", "china2006", ReportsEnum.eCommerceActivitySummary, TimeFrameEnum.Daily, startDate.AddHours(3), endDate.AddHours(3), 100000000, 0, 0);
                for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                {
                    HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[3].Value);
                }

                DataTable returnTable1 = reportData.Clone();
                try
                {

                    foreach (DataRow dataRow in reportData.Rows)
                    {
                        string _url = dataRow["MID"].ToString().ToLower();

                        if (_url.Contains("im_"))
                        {
                            returnTable1.ImportRow(dataRow);
                            OrderPresentKey.Add(_url);

                        }
                    }
                }
                catch
                { }



                foreach (DataRow dataRow in returnTable1.Rows)
                {
                    string versionName = dataRow["mid"].ToString().ToLower();
                    decimal visitor = 0;
                    foreach (string key in HitLinkVisitor.Keys)
                    {
                        try
                        {
                            if ((key.Contains(versionName)))
                            {
                                visitor = Convert.ToDecimal(HitLinkVisitor[key].ToString().ToLower());
                            }
                        }
                        catch
                        {


                        }
                    }
                    visitor = Math.Abs(visitor);
                    if (visitor > 0)
                    {
                        dataRow["UniqueVisitors"] = visitor.ToString();
                        try
                        {
                            dataRow["Conversion"] = Math.Round((Convert.ToDecimal(dataRow["TotalOrders"]) * 100) / visitor, 2) + "%";
                        }
                        catch
                        {
                            dataRow["Conversion"] = "0%";
                        }
                    }
                    else
                    {
                        dataRow["Conversion"] = "100%";
                    }

                }

                foreach (string key in HitLinkVisitor.Keys)
                {
                    try
                    {
                        if (key.Contains("im_") && !OrderPresentKey.Contains(key))
                            returnTable1.Rows.Add(new object[] { key.ToUpper(), HitLinkVisitor[key].ToString(), "0", "0", "0%", "0" });
                    }
                    catch
                    {


                    }

                }



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
