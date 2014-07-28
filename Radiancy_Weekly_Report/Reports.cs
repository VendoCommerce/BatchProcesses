using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
    }
}
