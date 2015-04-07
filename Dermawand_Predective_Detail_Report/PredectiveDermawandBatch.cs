using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using ConversionSystems.Providers;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Data;
using System.Xml.Serialization;
using System.Xml;

using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Net.Mail;

using ConversionSystems.Providers.MediaChase.OrderProcessing;
using RadiancyTrynonoBatch.Hitslink;


namespace CS
{
    class PredectiveDermawandBatch
    {
        static long uniqueRowId = Convert.ToInt64(Convert.ToString(DateTime.Now.ToString("yyyyMMddhhmmssfff")));
        private static string Filler(int spaces)
        {
            string item = "";
            return item.PadRight(spaces, ' ');
        }
        private static string FixForDBNull(object i)
        {
            if (i == DBNull.Value)
                return string.Empty;
            else
                return (string)i.ToString();
        }
        private static string FixDoubleForDBNull(object i, int length, int places)
        {
            double o = 0;
            double.TryParse(i.ToString(), out o);
            return o.ToString("N" + places).Replace(".", string.Empty).PadLeft(length, '0');
        }
        private static string FixCurrencyForDBNull(object i, int length)
        {
            double o = 0;
            double.TryParse(i.ToString(), out o);
            return o.ToString("N2").PadLeft(length, '0');
        }


        private static string FixDoubleForDBNull(object i, int length)
        {
            return FixDoubleForDBNull(i, length, 2);
        }
        private static string FixIntForDBNull(object i, int length)
        {
            int o = 0;
            int.TryParse(i.ToString(), out o);
            return o.ToString().PadRight(length, ' ');
        }
        private static string FixDateForDBNull(object i, int length)
        {
            DateTime dte = DateTime.MinValue;
            DateTime.TryParse(i.ToString(), out dte);
            return dte.ToString("MMddyy").PadRight(length, ' ');
        }
        private static string FixForDBNull(object i, int length)
        {

            if (i == DBNull.Value)
                return "".PadRight(length, ' ');
            else
                return ((string)i.ToString()).PadRight(length, ' ');
        }

        static string fixnull(object o)
        {
            string s1 = "";
            if (o != null) { s1 = "" + o.ToString(); }
            return s1;
        }

        public static string Left(string param, int length)
        {
            string result = "";
            if (param.Length > length)
            {
                result = param.Substring(0, length);
            }
            else
            {
                result = param;
            }
            return result;
        }
        public static string Right(string param, int length)
        {
            string result = param.Substring(param.Length - length, length);
            return result;
        }

        private static string FixDateForDBNull(object i, string format)
        {
            DateTime dte = DateTime.MinValue;
            DateTime.TryParse(i.ToString(), out dte);
            return dte.ToString(format);
        }

        static string fixquotes(string s)
        {
            string s1 = s;
            s1 = s1.Replace("'", "''");
            return s1;
        }


        private static string ProductItem(int OrderID)
        {
            //return "";
            int Counter = 0;
            string hline = "";
            if (OrderID>0)
            {
                DataTable dt = DAL.SQLDAL.GetSkus("GetPredectiveSkuDetail", OrderID);
                foreach (DataRow row in dt.Rows)
                {
                    Counter += 1;
                    hline += AddPipe(FixForDBNull(row["SkuCode"]));
                    hline += AddPipe(FixForDBNull(row["Quantity"]));
                    hline += AddPipe(FixForDBNull(row["FullPrice"]));
                    hline += AddPipe(FixForDBNull(row["ShippingPrice"]));
                    hline += AddPipe(FixForDBNull(row["ContinuityFlag"]));
                    hline += AddPipe(FixForDBNull(row["PaymentPlanID"]));
                    hline += AddPipe(FixForDBNull(row["PaymentPlanName"]));
                }
            }
            for (int i = Counter + 1; i <= 10; i++)
            {
                hline += AddPipe("");
                hline += AddPipe("");
                hline += AddPipe("");
                hline += AddPipe("");
                hline += AddPipe("");
                hline += AddPipe("");
                hline += AddPipe("");
            }
            return hline;
        }

        private static string AddPipe(string pStr)
        {
            pStr = pStr.Replace("|", "");
            return pStr + "|";
        }


        public static void uploadFile(string FTPAddress, string filePath, string username, string password)
        {
            try
            {
                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/DermaWand/" + Path.GetFileName(filePath));

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                //Load the file
                FileStream stream = File.OpenRead(filePath);
                byte[] buffer = new byte[stream.Length];

                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                //Upload file
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                //sendemail(filePath);
            }
        }
        static void Main(string[] args)
        {
            string CopyPath1 = System.Configuration.ConfigurationSettings.AppSettings["CopyPath"];

            ArrayList obj = new ArrayList();

            string path2 = string.Format("{1}\\ictvDermawand_{0}.txt", DateTime.Today.ToString("MMddyyyy"), CopyPath1);
            string path2a = string.Format("ictvDermawand_{0}.txt", DateTime.Today.ToString("MMddyyyy"));
            //m_dbObject = DatabaseFactory.CreateDatabase("Dermawand");

            DateTime startDate = DateTime.Today.AddDays(-3);
            DateTime endDate = DateTime.Today.AddDays(-1);
            using (StreamWriter sb = new StreamWriter(path2))
            {
                //write the header row
                WriteHeader(sb);
                //write data
                DataTable dt = DAL.SQLDAL.GetOrders("PredectiveOrderBatch", startDate,endDate);
                foreach (DataRow row in dt.Rows)
                {
                    WriteRow(row, sb);
                }
                //Write HitsLink Data
                foreach (DateTime day in EachDay(startDate, endDate))
                    WriteHitsLinkData(day, day, sb);
            }

            Console.WriteLine(path2);
            uploadFile("ftp://www.ftp-directory.com/", path2, "ConvSys", "c0nV$ys");

        }

        private static void WriteRow(DataRow row, StreamWriter sb)
        {
            string hline = "";
            // RESPONSE STRAT
            hline += AddPipe("Live Web");                                                              //  "Origin Name" |
            hline += AddPipe((uniqueRowId++).ToString());                                                             // "Unique ID" |
            hline += AddPipe("No MOP");                                                                // "Disposition" |
            hline += AddPipe(FixDateForDBNull(row["CreatedDate"], "MM/dd/yyyy"));                       // "Response Date" |
            hline += AddPipe(FixDateForDBNull(row["CreatedDate"], "HH:mm:ss"));                         //"Response Start Time" |
            hline += AddPipe(FixDateForDBNull(row["CreatedDate"], "HH:mm:ss"));                         //"Response End Time" |
            hline += AddPipe("ET");
            //Media Agency name was removed from the file on 5/15 along with callerID and Email
            hline += AddPipe("Web");                                                                   // "Origin Type" |
            hline += AddPipe("None");                                                                  // "Offer Code" |
            hline += AddPipe("888.666.1212");                                                          // "Dialed TFN" |
            hline += AddPipe("888.666.1212");                                                          // "Terminating number/unique ID" |
            hline += AddPipe(FixForDBNull(row["Version"]));                                         // "Affiliate ID" |
            hline += AddPipe(FixForDBNull(row["URL"]));                                                     // "URL" |
            hline += AddPipe("00:00:00");                                                              // "Call Length" |
            hline += AddPipe("1234");                                                                  // "Operator ID" |
            hline += AddPipe("NNP2");                                                                  // "Response Input" |
            hline += AddPipe("LFTV");            // "Media Type" |
            hline += AddPipe("DIRECT");          // "Media ID (Medium)" |
            hline += AddPipe("000.000.0000");    // "ANI" |
            hline += AddPipe("00.00.00");        // "Hold time" |

            // ORDER/CUSTOMER START
            hline += AddPipe(FixForDBNull(row["City"]));
            hline += AddPipe(FixForDBNull(row["StateProvince"]));
            hline += AddPipe(FixForDBNull(row["ZipPostalCode"]));
            hline += AddPipe(FixForDBNull(row["CountryCode"]));
            // Country Code
            hline += AddPipe("");
            hline += AddPipe(FixForDBNull(row["OrderId"]));
            hline += AddPipe(FixDateForDBNull(row["CreatedDate"], "MM/dd/yyyy"));
            hline += AddPipe(FixDateForDBNull(row["CreatedDate"], "hh:mm:ss"));
            hline += AddPipe("Y");                                           //"Rush/Priority flag"
            hline += AddPipe("$00.00");                                      //"Rush/Priority $ Amt"
            hline += AddPipe("1");                                           //"Payment Plan ID"
            hline += AddPipe("3 Pay");                                       // "Payment Plan" 
            hline += AddPipe("");                                            // "Sales ScriptID" |
            hline += AddPipe("");                                            // "Payment Method" |
            hline += AddPipe(FixForDBNull(row["CardType"]));              // "Card Type" 

            // ITEM  STRAT
            hline += ProductItem(Convert.ToInt32( row["OrderId"]));

            // ORDER TOTAL
            hline += AddPipe(FixCurrencyForDBNull(row["SubTotal"], 2));
            hline += AddPipe(FixCurrencyForDBNull(row["ShippingCost"], 2));
            hline += AddPipe(FixCurrencyForDBNull(row["Tax"], 2));
            hline += AddPipe(FixCurrencyForDBNull(row["OrderTotal"], 2));
            // CUSTOM FILDS-10 
            for (int i = 1; i < 10; i++)
            {
                hline += AddPipe("");
            }
            sb.WriteLine(hline);

        }

        private static void WriteRow(StreamWriter sb, string version, DateTime date)
        {
            string hline = "";
            // RESPONSE STRAT
            hline += AddPipe("Live Web");                                                              //  "Origin Name" |
            hline += AddPipe((uniqueRowId++).ToString());                                                             // "Unique ID" |
            hline += AddPipe("Visit");                                                                // "Disposition" |
            hline += AddPipe(FixDateForDBNull(date, "MM/dd/yyyy"));                       // "Response Date" |
            hline += AddPipe("00:00:00");                         //"Response Start Time" |
            hline += AddPipe("00:00:00");                         //"Response End Time" |
            hline += AddPipe("ET");
            //Media Agency name was removed from the file on 5/15 along with callerID and Email
            hline += AddPipe("Web");                                                                   // "Origin Type" |
            hline += AddPipe("None");                                                                  // "Offer Code" |
            hline += AddPipe("888.666.1212");                                                          // "Dialed TFN" |
            hline += AddPipe("888.666.1212");                                                          // "Terminating number/unique ID" |
            hline += AddPipe(version);                                                             // "Affiliate ID" |
            hline += AddPipe("");                                                     // "URL" |
            hline += AddPipe("00:00:00");                                                              // "Call Length" |
            hline += AddPipe("1234");                                                                  // "Operator ID" |
            hline += AddPipe("NNP2");                                                                  // "Response Input" |
            hline += AddPipe("LFTV");            // "Media Type" |
            hline += AddPipe("DIRECT");          // "Media ID (Medium)" |
            hline += AddPipe("000.000.0000");    // "ANI" |
            hline += AddPipe("00.00.00");        // "Hold time" |

            // ORDER/CUSTOMER START
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");
            // Country Code
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");                                           //"Rush/Priority flag"
            hline += AddPipe("");                                      //"Rush/Priority $ Amt"
            hline += AddPipe("");                                           //"Payment Plan ID"
            hline += AddPipe("");                                       // "Payment Plan" 
            hline += AddPipe("");                                            // "Sales ScriptID" |
            hline += AddPipe("");                                            // "Payment Method" |
            hline += AddPipe("");              // "Card Type" 

            // ITEM  STRAT
            hline += ProductItem(0);

            // ORDER TOTAL
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");
            hline += AddPipe("");
            // CUSTOM FILDS-10 
            for (int i = 1; i < 10; i++)
            {
                hline += AddPipe("");
            }
            sb.WriteLine(hline);

        }

        private static void WriteHeader(StreamWriter sb)
        {
            string hline = "";
            // RESPONSE STRAT
            hline += AddPipe("Origin Name");
            hline += AddPipe("Unique ID");
            hline += AddPipe("Disposition");
            hline += AddPipe("Response Date");
            hline += AddPipe("Response Start Time");
            hline += AddPipe("Response End Time");
            hline += AddPipe("Response Time Zone");
            //hline += AddPipe("Media Agency name");
            hline += AddPipe("Origin Type");
            hline += AddPipe("Offer Code");
            hline += AddPipe("Dialed TFN");
            hline += AddPipe("Terminating number/unique ID");
            hline += AddPipe("Affiliate ID");
            hline += AddPipe("URL");
            hline += AddPipe("Call Length");
            hline += AddPipe("Operator ID");
            hline += AddPipe("Response Input");
            hline += AddPipe("Media Type");
            hline += AddPipe("Media ID (Medium)");
            hline += AddPipe("ANI");
            hline += AddPipe("Hold time");
            // ORDER/CUSTOMER START
            hline += AddPipe("City");
            hline += AddPipe("State");
            hline += AddPipe("Zip/Postal Code");
            hline += AddPipe("TLD");
            hline += AddPipe("Area Code");
            hline += AddPipe("Order ID");
            hline += AddPipe("Order Date");
            hline += AddPipe("Order Time");
            hline += AddPipe("Rush/Priority flag");
            hline += AddPipe("Rush/Priority $ Amt");
            hline += AddPipe("Payment Plan ID");
            hline += AddPipe("Payment Plan");
            hline += AddPipe("Sales ScriptID");
            hline += AddPipe("Payment Method");
            hline += AddPipe("Card Type");
            // ITEM 1 STRAT
            for (int i = 0; i < 10; i++)
            {
                hline += AddPipe("Item/Product");
                hline += AddPipe("Quantity");
                hline += AddPipe("Unit Price");
                hline += AddPipe("SH Price");
                hline += AddPipe("Continuity Flag");
                hline += AddPipe("Payment Plan ID");
                hline += AddPipe("Payment Plan Name");
            }
            // ORDER TOTAL
            hline += AddPipe("Unit Price Total");
            hline += AddPipe("SH Total");
            hline += AddPipe("Tax Total");
            hline += AddPipe("Order Total");
            // CUSTOM FILDS
            hline += AddPipe("Custom Field 1");
            hline += AddPipe("Custom Field 2");
            hline += AddPipe("Custom Field 3");
            hline += AddPipe("Custom Field 4");
            hline += AddPipe("Custom Field 5");
            hline += AddPipe("Custom Field 6");
            hline += AddPipe("Custom Field 7");
            hline += AddPipe("Custom Field 8");
            hline += AddPipe("Custom Field 9");
            hline += "Custom Field 10";
            sb.WriteLine(hline);
        }

        private static void WriteHitsLinkData(DateTime startDate, DateTime endDate, StreamWriter sb)
        {
            Hashtable HitLinkVisitor = new Hashtable();
            Data rptData = new ReportWS().GetDataFromTimeframe("dermawand", "china2006", ReportsEnum.MultiVariate, TimeFrameEnum.Daily, startDate, endDate, 100000000, 0, 0);
            for (int i = 0; i <= rptData.Rows.GetUpperBound(0); i++)
            {
                HitLinkVisitor.Add(rptData.Rows[i].Columns[0].Value.ToLower(), rptData.Rows[i].Columns[9].Value);
            }

            DataTable OrderInfo1 = DAL.SQLDAL.GetOrders("pr_report_order_version_batch",null,null);
            //Update Version List information
            foreach (DataRow row in OrderInfo1.Rows)
            {
                int visitor = 0;
                //if (row["Title"].ToString().ToLower().Equals(VersionName.ToLower())) // 04/01 Orders and Visitiors from B2 version only Pranav
                //{
                if (row["Title"].ToString().ToLower().Equals(row["ShortName"].ToString().ToLower()))
                {
                    if (HitLinkVisitor.ContainsKey(row["Title"].ToString().ToLower()))
                    {
                        visitor += Convert.ToInt32(HitLinkVisitor[row["Title"].ToString().ToLower()].ToString());
                        visitor = Math.Abs(visitor);
                        for (int i = 0; i < visitor; i++)
                        {
                            WriteRow(sb, row["Title"].ToString(), startDate);
                        }

                    }
                }
                //else
                //{
                //    //Added this to fix bug of orderhelper.getversionname()
                //    if (HitLinkVisitor.ContainsKey(row["Title"].ToString().ToLower()))
                //    {
                //        visitor += Convert.ToInt32(HitLinkVisitor[row["Title"].ToString().ToLower()].ToString());
                //    }
                //    if (HitLinkVisitor.ContainsKey(row["ShortName"].ToString().ToLower()))
                //    {
                //        visitor += Convert.ToInt32(HitLinkVisitor[row["ShortName"].ToString().ToLower()].ToString());
                //    }
                //    visitor = Math.Abs(visitor);
                //}
                //item.UniqueVisitors = visitor;
                //}                 
            }
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static DateTime EndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

    }
}
