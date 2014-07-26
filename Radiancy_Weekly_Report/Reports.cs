﻿using System;
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
        public DataTable Get_NoNo_Web_Report(DateTime startDate, DateTime endDate)
        {
            bool result = false;
            Hashtable HitLinkVisitor = new Hashtable();

            try
            {
                
                //DateTime currentDay = startDate;
                //while (currentDay <= endDate)
                //{
                    HitLinkVisitor.Clear();
                    DataTable reportData;
                    DAL dal = new DAL();
                    //get the versionSummary data from db , using OrderManager
                    dal.SQLServer.Get_NoNo_Web_Report_Table(Logging.StartOfDay(startDate), Logging.EndOfDay(endDate), out reportData);

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
                            dataRow["Unique_Visits"] = HitLinkVisitor[versionName].ToString().ToLower();
                        else
                            dataRow["Unique_Visits"] = "0";
                        //dataRow["Revenue"]
                        //dataRow["Date"] =endDate.ToString("M-d-yyyy");
                    }
                    //try
                    //{
                    //    bool appendHeader = currentDay == startDate ? true : false;
                    //    result = CreateCSVFile(reportData, FUllPAthwithFileName,appendHeader);
                    //}
                    //catch (Exception ex)
                    //{

                    //    string loginfo = "File creation status:" + result + Environment.NewLine + ex.Message.ToString();
                    //    LogToFile(loginfo);
                    //}
                    //currentDay = currentDay.AddDays(1);
                  return reportData;
              

            }
            catch (Exception ex)
            {
                string loginfo = "Report generation status:" + result + Environment.NewLine + ex.Message.ToString();
                Logging.LogToFile(loginfo);
                Console.WriteLine(ex.ToString());
                return null;
            }
        }



    }
}
