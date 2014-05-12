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
using Com.ConversionSystems.DataAccess;
using Com.ConversionSystems.Utility;
using System.Threading;
using System.Configuration ;
using CSWeb.HitLinks;
using Tamir.SharpSsh;

////using Tamir.SharpSsh;

namespace StonedineWarrantyDB_MyDataTree
{
    class Batch  
    {
        LogData log = new LogData();
        string hitsLinkUserName = Helper.AppSettings["hitsLinkUserName"];
        string hitsLinkPassword = Helper.AppSettings["hitsLinkPassword"];
        
        static DataSet getsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
            adp.Fill(ds);
            conn.Close();
            return ds;
        }

        static DataTable getDataTable(string StoredProcedureName,int selection)
        {
             string connstr="";
             switch (selection)
             {
                 case 1:
                     connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringStoneDineProd"];
                     break;
                 case 2:
                     connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringStoneDineShopProd"];
                     break;
             }

            string strMessage;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
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

        static DataTable getDataTable(string StoredProcedureName, DateTime StartDate, DateTime EndDate, int selection)
        {

            string connstr = "";
            switch (selection)
            {
                case 1:
                    connstr = System.Configuration.ConfigurationSettings.AppSettings["CS_TryMyPurMistProd"];
                    break;
                case 2:
                    connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringStoneDineShopProd"];
                    break;
            }

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName; // 
                oCmd.CommandType = CommandType.StoredProcedure;

                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@startdate", SqlDbType.DateTime);
                para[0].Value = StartDate;
                para[1] = new SqlParameter("@enddate", SqlDbType.DateTime);
                para[1].Value = EndDate;
                oCmd.Parameters.AddRange(para);                
                
                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
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

        static DataTable AddToDataTable(string StoredProcedureName, string StartDate, string EndDate,DataTable orgDT)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];

            bool bReturn = false;
            string strMessage;

            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName; // 
                oCmd.CommandType = CommandType.StoredProcedure;

                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@startdate", SqlDbType.VarChar);
                para[0].Value = StartDate;
                para[1] = new SqlParameter("@enddate", SqlDbType.VarChar);
                para[1].Value = EndDate;
                oCmd.Parameters.AddRange(para);

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
                foreach (DataRow dr in orgDT.Rows)
                {
                    dt.Rows.Add(dr.ItemArray);
                }
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
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
        
        static void runsql_stoneDine(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringStoneDineProd"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }
        static void runsql_stoneDineShop(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringStoneDineShopProd"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }

        public static string ClearAccents(string text)
        {
            //url = Regex.Replace(url, @"\s+", "-");
            string stFormD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString());
        }
        public static string fixstring(object s1)
        {
            string s = "";
            if (s1 != null) { s = s1.ToString(); }

            s = s.Replace("&", "&amp;");
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");
            s = s.Replace(((char)(34)).ToString(), "&quot;");
            s = s.Replace("'", "&apos;");
            return s;

        }
        public static string fixQuot(object s1)
        {
            string s = "";
            if (s1 != null) { s = s1.ToString(); }
            s = s.Replace("'", "&apos;");
            return s;

        }
    
        public static void sendEmailToAdmin(string ErrorMSg, string subject)
        {
            StringBuilder sb = new StringBuilder();
            MailMessage message = new MailMessage();
            //  message.To.Add(new MailAddress(Helper.AppSettings["AdminEmail"]));
            message.To.Add(Helper.AppSettings["AdminEmail"]);
            message.From = new MailAddress("info@StoneDine.com");

            message.Subject = subject;             
            sb.Append("Here is the response from MyDataTree API for Warranty Database. Please address this Warranty import:");
            sb.Append(ErrorMSg);

            string st;
            st = sb.ToString();

            st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            message.Body = st;
            message.IsBodyHtml = false;
            SmtpClient client = new SmtpClient();
            Helper.SendMail(message);
        }

        public string CheckSQLInjection(string input)
        {
            input = input.Replace("'", "''"); //' espace the single quote
            input = input.Replace("\"\"", ""); // "
            input = input.Replace(")", ""); // ) 
            input = input.Replace("(", ""); // (
            input = input.Replace(";", ""); // ;
            input = input.Replace("-", ""); // -
            input = input.Replace("|", ""); // |
            return input;
        }

        public static DateTime GetEndDate(DateTime input)
        {
            DateTime val = input;
            val = val.AddHours(23).AddMinutes(59).AddSeconds(59);

            return val;
        }
        public Hashtable HitLinkVisitor = new Hashtable();
        string  Load_HavasData(DateTime ImportDate)
        {

            try
            {
                log.LogToFile("***************************************************************************");
                log.LogToFile("Start Importing data to Havas: Date " + DateTime.Now.ToString());
                
                //DateTime.TryParse("6/28/2013",out startDate); //one time only for all orders back to 6/28/13
                DateTime startDate = GetEastCoastStartDate(ImportDate.AddDays(-7));
                DateTime endDate = GetEastCoastDate(ImportDate.AddDays(-1));
                //TODO: remove this line and untag top line
                //DateTime startDate = new DateTime(2014, 4, 28);
                //DateTime endDate = new DateTime(2014, 5, 5);
                //DateTime endDate = GetEastCoastDate(startDate.AddDays(+2));

                log.LogToFile("Loading TryMyPurMist to Havas Date: " + startDate.ToString());
                log.LogToFile("Loading TryMyPurMist to Havas Date: " + endDate.ToString());

                //String DatePartS = startDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                String DatePartE =endDate.ToString("MMdd", CultureInfo.InvariantCulture);
                string SavePath = Helper.AppSettings["FileDirectoryPath"].ToString();
                string Proc_name = Helper.AppSettings["Proc_name"].ToString();
                string fullPathFileName = SavePath + "\\VAWAPO" + DatePartE + ".txt";
                StreamWriter sw = new StreamWriter(fullPathFileName,false);
                //write_file_header(sw);

                ////////Data rptData = new ReportWSSoapClient().GetDataFromTimeframe(hitsLinkUserName, hitsLinkPassword, ReportsEnum.eCommerceLatestClicks, TimeFrameEnum.Daily, startDate, endDate, 1000000, 0, 0);
                ////////for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
                ////////{
                ////////    if (!HitLinkVisitor.ContainsKey(rptData.Rows[i].Columns[0].Value.ToLower()))
                ////////        HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[7].Value);
                ////////}
                DataTable dt_TryMyPurmist = getDataTable(Proc_name, startDate, endDate, 1);
                
                if (dt_TryMyPurmist.Rows.Count > 0)
                {
                    Console.WriteLine("Record Found = " + dt_TryMyPurmist.Rows.Count.ToString());
                    log.LogToFile("Records Found on TryMyPurMist = " + dt_TryMyPurmist.Rows.Count.ToString()); 
                    foreach (DataRow dr in dt_TryMyPurmist.Rows)
                    {
                       // Console.WriteLine(DateTime.Now.ToLongTimeString());
                        add_Order_totxtFile(dr, sw);
                    }
                }
                else
                {
                    log.LogToFile("No Record Found for importing data to Havas from TryMyPurMist");
                    Console.WriteLine("No Record Found  on TryMyPurMist");
                }

                sw.Close();
                log.LogToFile("Finished Importing TryMyPurMist data to Havas");
                return fullPathFileName;
            }
            catch (Exception ex)
            {
                string error = "ERROR: Catch Block " + ex.Message + " StackTrace :: " + ex.StackTrace;
                log.LogToFile(error);

                sendEmailToAdmin(error, "Alert - TryMyPurmist.com - Error generating Havas report.");
            }
            return string.Empty;
        }
        
        public void add_Order_totxtFile(DataRow dr,StreamWriter  sw  )
        {
            try
            {
               
                StringBuilder sb = new StringBuilder("");
                sb.Clear();
                DateTime odate = Convert.ToDateTime(dr["odate"].ToString());
                //sb.Append(dr["orderid"].ToString() + "|");
                sb.Append( odate.ToString("MM/dd/yyyy hh:mm:ss") + "|");
                sb.Append(dr["TimeZone"].ToString() + "|");
                sb.Append(dr["act"].ToString() + "|");
                sb.Append(dr["qnt"].ToString() + "|");
                string ttl = "00";
                if (!dr["subtotal"].ToString().Equals(""))
                {
                    ttl = dr["subtotal"].ToString();
                    
                }
                
                sb.Append(ttl + "|");
                sb.Append(dr["zipcode"].ToString() + "|");
                sb.Append(dr["IpAddress"].ToString() + "|");

                if (!dr["acode"].ToString().Equals(""))
                {
                    sb.Append(dr["acode"].ToString().Substring(0, 3) + "|");
                }
                else
                {
                    sb.Append(" |");
                }
                sb.Append(" | |");

           

                //if (!dr["landingUrl"].ToString().Equals(""))
                //{
                //    sb.Append(dr["landingUrl"].ToString() + "|");
                //}
                //else
                //{
                //    sb.Append("http://www.mypurmist.com:81 |");
                //}
                
                
                
                //sb.Append(dr["Tag"].ToString());// + "|");
                //sb.Append(dr["ProductList"].ToString() );
               
                sw.WriteLine(sb.ToString());
                sb.Clear();
                
            }
            catch (Exception ex)
            { 
                string error =  "Catch Block " + ex.Message + " StackTrace :: " + ex.StackTrace;
                log.LogToFile(error);
                
            
            }

        }
        public void write_file_header (StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder("");
                sb.Append("First Name" + "\t");
                sb.Append("Last Name" + "\t");
                sb.Append("Email Address" + "\t");
                sb.Append("Address" + "\t");
                sb.Append("City" + "\t");
                sb.Append("State" + "\t");
                sb.Append("Country" + "\t");
                sb.Append("Phone" + "\t");
                sb.Append("Purchased From" + "\t");
                sb.Append("Warranty ID" + "\t");
                sb.Append("Date Purchased" + "\t");
                sb.Append("Date Received" + "\t");
                sb.Append("Gift Item" + "\t");
                sb.Append("Product Purchased_1" + "\t");
                sb.Append("Product Purchased_2" + "\t");
                sb.Append("Product Purchased_3" + "\t");
                sb.Append("Product Purchased_4" + "\t");
                sb.Append("Product Purchased_5" + "\t");
                sb.Append("Zip Code " + "\t");
                sw.WriteLine(sb.ToString());
        }

        public static DateTime GetEastCoastStartDate(DateTime input)
        {
            DateTime val = input;

            val = val.AddDays(-1).AddHours(21).AddMinutes(00).AddSeconds(00);

            return val;
        }

        public static DateTime GetEastCoastDate(DateTime input)
        {
            DateTime val = input;

            val = val.AddHours(21).AddMinutes(00).AddSeconds(00);

            return val;
        }
        public bool upload_FTP(string fullPathFileName)
        {
            // Get the objects used to communicate with the server.
            bool response;
            string FTP_Site = Helper.AppSettings["sFTPsite"].ToString();
            string FTP_username = Helper.AppSettings["FTPusername"].ToString();
            string FTP_pass = Helper.AppSettings["FTPpassword"].ToString();
            string FTP_folder = Helper.AppSettings["FTPsiteFolder"].ToString();
            string Chk_folder = Helper.AppSettings["ChkFolder"].ToString();
            try
            {
                // Use 
                Sftp sftpClient = new Sftp(FTP_Site, FTP_username, FTP_pass);

                sftpClient.Connect();
                string absoluteFileName = Path.GetFileName(fullPathFileName);
                sftpClient.Put(@fullPathFileName, FTP_folder + "/" + absoluteFileName);
                ArrayList post_response = sftpClient.GetFileList(FTP_folder);
                if (post_response.IndexOf(absoluteFileName) >= 0)
                {
                    string dest = Chk_folder + "\\" + "chkd_" + absoluteFileName;
                    string source = FTP_folder + "/" + absoluteFileName;
                    sftpClient.Get(source, @dest);
                    response = true;
                }
                else
                {
                    response = false;

                }
                return response;
            }


            catch (Exception ex)
            {

                string error = "FTP ERROR: Catch Block " + ex.Message + " StackTrace :: " + ex.StackTrace;
                log.LogToFile(error);

                sendEmailToAdmin(error, "Alert - TryMyPurMist.com - Error uploading data to Havas FTP.");

                return false;
            }

        }

        static void Main(string[] args)
        {            
            Console.WriteLine("Start Importing TryMyPurMist Data : " + DateTime.Now.ToString()  );
            Batch StartBatch = new Batch();
            DateTime ImportDate = DateTime.Today;
            string fullPathFilename = StartBatch.Load_HavasData(ImportDate);
            //Upload to ftp server if batch file was successfully craeted.
            if (fullPathFilename.Length > 0)
            {
                Console.WriteLine("Start Uploading To FTP Server : " + DateTime.Now.ToString());
                StartBatch.upload_FTP(fullPathFilename);
                Console.WriteLine("End Uploading To FTP Server : " + DateTime.Now.ToString());
            }
            Console.WriteLine("End Importing data to TryMyPurMist : " + DateTime.Now.ToString());
            Environment.Exit(0);
        }
    }
}
