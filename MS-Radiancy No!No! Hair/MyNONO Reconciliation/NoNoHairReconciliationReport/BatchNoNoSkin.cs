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
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace NoNoHairReconciliationReport
{
    public class BatchNoNoSkin
    {
        //  private string targetPath = "C:/BatchFiles/TryNoNO/Skin/";
        private string targetPath = "D:/BatchProcesses/NoNoHairandSkin_ReconciliationReport/Skin/";

        private DataTable _NewTempDataTable = null;
        public DataTable NewTempDataTable
        {
            get { return _NewTempDataTable; }
            set { _NewTempDataTable = value; }
        }

        public void CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            DataColumn myDataColumn;
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "excelFileName";
            myDataTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "fileNameOnly";
            myDataTable.Columns.Add(myDataColumn);
            NewTempDataTable = myDataTable;
        }

        // create table rows dds
        private void AddDataToTable(string excelFileName, string fileNameOnly)
        {
            DataRow row;
            row = NewTempDataTable.NewRow();
            row["excelFileName"] = excelFileName;
            row["fileNameOnly"] = fileNameOnly; 
            NewTempDataTable.Rows.Add(row);
        }   
  
        private DataSet getsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDevSkin"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
            adp.Fill(ds);
            conn.Close();
            return ds;
        }

        private DataTable getDataTableByDate(string StoredProcedureName, DateTime startDate, DateTime endDate, int excelsection)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDevSkin"];

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
                oCmd.Parameters.Add("@excelsection", SqlDbType.Int).Value = excelsection;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();
                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
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
     
        private void runsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringDevSkin"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }
              
        private static bool Excel_FromDataTable(DataTable dt, string excelFileName, DataTable dtsummaryBottom)
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


                DataTable NewTempDataTable = new DataTable();

                DataColumn myDataColumn;

                myDataColumn = new DataColumn();
                myDataColumn.DataType = Type.GetType("System.String");
                myDataColumn.ColumnName = "col1";
                NewTempDataTable.Columns.Add(myDataColumn);

                myDataColumn = new DataColumn();
                myDataColumn.DataType = Type.GetType("System.String");
                myDataColumn.ColumnName = "col2";
                NewTempDataTable.Columns.Add(myDataColumn);

                DataRow emptyrow;
                emptyrow = NewTempDataTable.NewRow();
                emptyrow[0] = "";
                emptyrow[1] = "";
                NewTempDataTable.Rows.Add(emptyrow);

                emptyrow = NewTempDataTable.NewRow();
                emptyrow[0] = "";
                emptyrow[1] = "";
                NewTempDataTable.Rows.Add(emptyrow);

                emptyrow = NewTempDataTable.NewRow();
                emptyrow[0] = "";
                emptyrow[1] = "";
                NewTempDataTable.Rows.Add(emptyrow);

                emptyrow = NewTempDataTable.NewRow();
                emptyrow[0] = "";
                emptyrow[1] = "";
                NewTempDataTable.Rows.Add(emptyrow);

                foreach (DataRow r2 in NewTempDataTable.Rows)
                {
                    iRow++;
                    iCol = 0;
                    foreach (DataColumn c in NewTempDataTable.Columns)
                    {
                        iCol++;
                        excel.Cells[iRow + 1, iCol] = r2[c.ColumnName];
                    }                              
                }

                // Summary in excel at Bottom
                foreach (DataRow r1 in dtsummaryBottom.Rows)
                {
                    iRow++;
                    iCol = 0;
                    foreach (DataColumn c in dtsummaryBottom.Columns)
                    {
                        iCol++;
                        if (iCol == 2)
                        {
                            iCol--;
                            excel.Cells[iRow + 1, iCol] = r1[c.ColumnName];
                        }
                    }
                    //  excel.Cells[iRow + 1, iCol] = r1[1];           
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

        public void LoadOrder_UK(DateTime startDate, DateTime endDate, string FileDate)
        {
            DataTable Dt1 = getDataTableByDate("OrderReconciliationUK", startDate, endDate, 1);
           
            if (Dt1.Rows.Count > 0)
            {
                Console.WriteLine("UK Order: " + Dt1.Rows.Count.ToString());
                string excelFileName = "ReconciliationReport_Skin_UK_" + FileDate + ".xls";
                string FUllPAthwithFileName = targetPath + excelFileName;
                DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUK", startDate, endDate, 2);
                bool flag =  Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);                    
                }
            }
            else
            {
                Console.WriteLine("No Order found UK");
            }
        }

        public void LoadOrder_US(DateTime startDate, DateTime endDate, string FileDate)
        {
            Console.WriteLine("Start Loading Order US");            
            DataTable Dt1 = getDataTableByDate("OrderReconciliationUS", startDate, endDate, 1);
            
            if(Dt1.Rows.Count>0 )
            {
                Console.WriteLine("US Order: " + Dt1.Rows.Count.ToString());                
                string excelFileName = "ReconciliationReport_Skin_US_" + FileDate + ".xls";
                string FUllPAthwithFileName = targetPath + excelFileName;
                DataTable dtsummaryBottom = getDataTableByDate("OrderReconciliationUS", startDate, endDate, 2);
                bool flag = Excel_FromDataTable(Dt1, FUllPAthwithFileName, dtsummaryBottom);
                if (flag == true)
                {
                    AddDataToTable(FUllPAthwithFileName, excelFileName);                   
                }
            }
            else
            {
                Console.WriteLine("No Order found US");
            }
        }

        private static void SendExceptionEmail(Exception ex)
        {
            string excelFileName = "";
            string fileNameOnly = "";
            try
            {
                string ToEmail = System.Configuration.ConfigurationSettings.AppSettings["sendemailtoError"];
                string ToEmailcc = System.Configuration.ConfigurationSettings.AppSettings["sendemailtocc"];
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(ToEmail));
                message.CC.Add(new MailAddress(ToEmailcc));
                message.From = new MailAddress("info@conversionsystems.com");

                message.Subject = "Batch Error No No SKIN - Daily Reconciliation Reports Batch";
                message.Body = ex.Message;
                message.IsBodyHtml = true;

                SmtpClient client;
                client = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"]);
                client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(message);

            }
            catch (System.Exception ex1)
            {
                Console.WriteLine("Exception Details  : " + ex1.ToString());
            }
        }

    }
}
