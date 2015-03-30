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




namespace RadiancyTrynonoBatch
{
    class PredectiveDermawandBatch
    {
        private static string encapFormat = "\"{0}\",";
        private static DataSet dsCanada;
        private static int totallines;
        public static int hlines;
        public static int orderlines;

        public static Database m_dbObject;

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


        private static string GetCreditCardName(object PaymentType)
        {
            if (PaymentType == DBNull.Value)
                return "VISA";
            switch (PaymentType.ToString().ToUpper())
            {
                case "VISA":
                    return "VISA";
                case "MASTERCARD":
                    return "MC";
                case "AMERICANEXPRESS":
                case "AMEX":
                    return "AMEX";
                case "DISCOVER":
                    return "DISC";
                default:
                    return "VISA";
            }
        }

        static string fixnull(object o)
        {
            string s1 = "";
            if (o != null) { s1 = "" + o.ToString(); }
            return s1;
        }

        static void runsql(string s)
        {

            string connstr1 = System.Configuration.ConfigurationSettings.AppSettings["connectionstring"];

            SqlConnection conn = new SqlConnection(connstr1);
            conn.Open();
            SqlCommand cm = new SqlCommand(s, conn);
            cm.ExecuteNonQuery();
            conn.Close();

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

        public static string Mid(string param, int startIndex, int length)
        {
            string result = param.Substring(startIndex, length);
            return result;
        }

        public static bool isnum(char c)
        {
            bool b = false;
            if ((((int)c) >= 48) && (((int)c) <= 57)) b = true;
            return b;
        }

        public static string onlynums(string s)
        {
            string s1 = s;

            int i;
            string s2 = "";
            for (i = 0; i < s1.Length; i++)
            {
                if (isnum(s1[i]) == true) { s2 += s1[i]; }
            }

            return s2;

        }


        public static int countnums(string s)
        {
            string s1 = s;

            int i;
            int j = 0;
            for (i = 0; i < s1.Length; i++)
            {
                if (isnum(s1[i]) == true) { j++; }
            }

            return j;

        }


        static string spacetab(string s)
        {
            string s1 = s;
            s1 = onlynums(s1);
            if ((countnums(s1) > 10) && s1[0] == '1') s1 = Right(s1, s1.Length - 1);

            while (s1.Length < 10)
            {
                s1 = ' ' + s1;
            }
            s1 = Left(s1, 10);
            return s1;

        }

        public static void runapp(string appname, string args1)
        {
            ProcessStartInfo pi = new ProcessStartInfo(appname);
            pi.Arguments = args1;

            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            Process p = Process.Start(pi);
            StreamReader sr = p.StandardOutput;

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine("Read line: {0}", line);
            }
            p.WaitForExit();

        }


        public static string replace3chars(string s, int i, string s2)
        {
            string s1 = "";
            for (int j = 0; j < s.Length; j++)
            {
                if ((j == i - 1) || (j == i) || (j == i + 1))
                {
                    if (j == i - 1) s1 += s2[0];
                    if (j == i) s1 += s2[1];
                    if (j == i + 1) s1 += s2[2];
                }
                else
                {
                    s1 += s[j];
                }
            }

            return s1;

        }

        public static bool iscanada(string s)
        {
            bool b;
            b = false;
            foreach (DataRow r in dsCanada.Tables[0].Rows)
            {
                if (s == r.ItemArray[0].ToString()) b = true;
            }
            return b;
        }


        public static string afterslash(string s)
        {
            string s1 = s;
            string s2 = "";
            bool b;

            b = false;

            int j;
            j = -1;


            for (int i = s.Length - 1; i >= 0; i--)
            {
                if ((s[i].ToString() == "\\") && (j == -1))
                {
                    j = i;
                }
            }

            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i].ToString() == "\\") && i == j)
                {
                    b = true;
                }
                else
                {
                    if (b == true) { s2 += s[i]; }
                }
            }

            return s2;
        }


        static string fixaccents(string s)
        {
            string s1 = s;

            s1 = s1.Replace("Ç", "C");
            s1 = s1.Replace("ü", "u");
            s1 = s1.Replace("é", "r");
            s1 = s1.Replace("â", "a");
            s1 = s1.Replace("ä", "a");
            s1 = s1.Replace("à", "a");
            s1 = s1.Replace("å", "a");
            s1 = s1.Replace("ç", "c");
            s1 = s1.Replace("ê", "e");
            s1 = s1.Replace("ë", "e");
            s1 = s1.Replace("è", "e");
            s1 = s1.Replace("ï", "i");
            s1 = s1.Replace("î", "i");
            s1 = s1.Replace("ì", "i");
            s1 = s1.Replace("Ä", "A");
            s1 = s1.Replace("Å", "A");
            s1 = s1.Replace("É", "E");
            s1 = s1.Replace("æ", "a");
            s1 = s1.Replace("Æ", "a");
            s1 = s1.Replace("ô", "o");
            s1 = s1.Replace("ö", "o");
            s1 = s1.Replace("ò", "o");
            s1 = s1.Replace("û", "u");
            s1 = s1.Replace("ù", "u");
            s1 = s1.Replace("ÿ", "y");
            s1 = s1.Replace("Ö", "O");
            s1 = s1.Replace("Ü", "U");
            s1 = s1.Replace("ø", "0");
            s1 = s1.Replace("£", "L");
            s1 = s1.Replace("Ø", "0");
            s1 = s1.Replace("×", "x");
            s1 = s1.Replace("ƒ", "f");
            s1 = s1.Replace("á", "a");
            s1 = s1.Replace("í", "i");
            s1 = s1.Replace("ó", "o");
            s1 = s1.Replace("ú", "u");
            s1 = s1.Replace("ñ", "n");
            s1 = s1.Replace("Ñ", "N");
            s1 = s1.Replace("ª", "a");
            s1 = s1.Replace("º", "o");
            s1 = s1.Replace("Á", "A");
            s1 = s1.Replace("Â", "A");
            s1 = s1.Replace("À", "A");
            s1 = s1.Replace("ð", "o");
            s1 = s1.Replace("Ð", "D");
            s1 = s1.Replace("Ê", "E");
            s1 = s1.Replace("Ë", "E");
            s1 = s1.Replace("È", "E");
            s1 = s1.Replace("ı", "i");
            s1 = s1.Replace("Í", "I");
            s1 = s1.Replace("Î", "I");
            s1 = s1.Replace("Ï", "I");
            s1 = s1.Replace("Ô", "O");
            s1 = s1.Replace("Ò", "O");
            s1 = s1.Replace("õ", "o");
            s1 = s1.Replace("Õ", "o");
            s1 = s1.Replace("Ú", "U");
            s1 = s1.Replace("Û", "U");
            s1 = s1.Replace("Ù", "U");
            s1 = s1.Replace("ý", "y");
            s1 = s1.Replace("Ý", "Y");


            return s1;
        }

        static string filterchars(string s)
        {
            string s1 = "";
            for (int i = 0; i < s.Length; i++)
            {
                if ((((int)s[i]) >= 32) && (((int)s[i]) <= 125)) { s1 += s[i].ToString(); } else { s1 += " "; }
            }

            return s1;
        }


        public static bool AddLineItem(string[] lineItem)
        {
            DbCommand dbCommand = m_dbObject.GetStoredProcCommand("up_EDIT_OrderItems");

            m_dbObject.AddInParameter(dbCommand, "@ORDERNUM", DbType.String, lineItem[0]);
            m_dbObject.AddInParameter(dbCommand, "@ITEM", DbType.String, lineItem[1]);
            m_dbObject.AddInParameter(dbCommand, "@DESC", DbType.String, lineItem[2]);
            m_dbObject.AddInParameter(dbCommand, "@quantity1", DbType.String, lineItem[3]);
            m_dbObject.AddInParameter(dbCommand, "@IN_UNLIST", DbType.String, lineItem[4]);
            m_dbObject.AddInParameter(dbCommand, "@DISCOUNT", DbType.String, lineItem[5]);
            m_dbObject.AddInParameter(dbCommand, "@TAXABLE", DbType.String, lineItem[6]);
            m_dbObject.AddInParameter(dbCommand, "@GIFTWRAP", DbType.String, lineItem[7]);
            m_dbObject.AddInParameter(dbCommand, "@GFTWRAPCHG", DbType.String, lineItem[8]);
            m_dbObject.AddInParameter(dbCommand, "@RESUPDATED", DbType.String, lineItem[9]);
            m_dbObject.AddInParameter(dbCommand, "@DLLRDISC", DbType.String, lineItem[10]);
            m_dbObject.AddInParameter(dbCommand, "@IT_TOTAL", DbType.String, lineItem[11]);
            m_dbObject.AddInParameter(dbCommand, "@GWCHARGE", DbType.String, lineItem[12]);
            m_dbObject.AddInParameter(dbCommand, "@LINE_NOTES", DbType.String, lineItem[13]);

            return ((int)m_dbObject.ExecuteScalar(dbCommand) == 1);
        }



        public static bool AddHeaderOrder(string[] lineItem)
        {
            DbCommand dbCommand = m_dbObject.GetStoredProcCommand("up_EDIT_Orders");

            m_dbObject.AddInParameter(dbCommand, "@CALLREASON", DbType.String, lineItem[0]);
            m_dbObject.AddInParameter(dbCommand, "@OMPCUSTNUM", DbType.String, lineItem[1]);
            m_dbObject.AddInParameter(dbCommand, "@ALTCUSTNUM", DbType.String, lineItem[2]);
            m_dbObject.AddInParameter(dbCommand, "@ORDERNUM", DbType.String, lineItem[3]);
            m_dbObject.AddInParameter(dbCommand, "@DATE", DbType.String, lineItem[4]);
            m_dbObject.AddInParameter(dbCommand, "@SOURCEKEY", DbType.String, lineItem[5]);
            m_dbObject.AddInParameter(dbCommand, "@FIRSTNAME", DbType.String, lineItem[6]);
            m_dbObject.AddInParameter(dbCommand, "@LASTNAME", DbType.String, lineItem[7]);
            m_dbObject.AddInParameter(dbCommand, "@TITLE", DbType.String, lineItem[8]);
            m_dbObject.AddInParameter(dbCommand, "@COMPANY", DbType.String, lineItem[9]);
            m_dbObject.AddInParameter(dbCommand, "@ADDR", DbType.String, lineItem[10]);
            m_dbObject.AddInParameter(dbCommand, "@ADDR2", DbType.String, lineItem[11]);
            m_dbObject.AddInParameter(dbCommand, "@CITY", DbType.String, lineItem[12]);
            m_dbObject.AddInParameter(dbCommand, "@STATE", DbType.String, lineItem[13]);
            m_dbObject.AddInParameter(dbCommand, "@ZIPCODE", DbType.String, lineItem[14]);
            m_dbObject.AddInParameter(dbCommand, "@COUNTRY", DbType.String, lineItem[15]);
            m_dbObject.AddInParameter(dbCommand, "@PHONE", DbType.String, lineItem[16]);
            m_dbObject.AddInParameter(dbCommand, "@EMAIL", DbType.String, lineItem[17]);
            m_dbObject.AddInParameter(dbCommand, "@FIRST_ALT", DbType.String, lineItem[18]);
            m_dbObject.AddInParameter(dbCommand, "@LAST_ALT", DbType.String, lineItem[19]);
            m_dbObject.AddInParameter(dbCommand, "@ADDR_ALT", DbType.String, lineItem[20]);
            m_dbObject.AddInParameter(dbCommand, "@ADDR2_ALT", DbType.String, lineItem[21]);
            m_dbObject.AddInParameter(dbCommand, "@CITY_ALT", DbType.String, lineItem[22]);
            m_dbObject.AddInParameter(dbCommand, "@STATE_ALT", DbType.String, lineItem[23]);
            m_dbObject.AddInParameter(dbCommand, "@ZIP_ALT", DbType.String, lineItem[24]);
            m_dbObject.AddInParameter(dbCommand, "@CNTRY_ALT", DbType.String, lineItem[25]);
            m_dbObject.AddInParameter(dbCommand, "@PHONE_ALT", DbType.String, lineItem[26]);
            m_dbObject.AddInParameter(dbCommand, "@SHIPVIA", DbType.String, lineItem[27]);
            m_dbObject.AddInParameter(dbCommand, "@SHIPVIADSC", DbType.String, lineItem[28]);
            m_dbObject.AddInParameter(dbCommand, "@OVERNIGHT", DbType.String, lineItem[29]);
            m_dbObject.AddInParameter(dbCommand, "@SUBTOTAL", DbType.String, lineItem[30]);
            m_dbObject.AddInParameter(dbCommand, "@ODR_DISC", DbType.String, lineItem[31]);
            m_dbObject.AddInParameter(dbCommand, "@GIFT_MSG", DbType.String, lineItem[32]);
            m_dbObject.AddInParameter(dbCommand, "@MOP", DbType.String, lineItem[33]);
            m_dbObject.AddInParameter(dbCommand, "@CARDTYPE", DbType.String, lineItem[34]);
            m_dbObject.AddInParameter(dbCommand, "@CARDNUM", DbType.String, lineItem[35]);
            m_dbObject.AddInParameter(dbCommand, "@EXP", DbType.String, lineItem[36]);
            m_dbObject.AddInParameter(dbCommand, "@APPROVAL", DbType.String, lineItem[37]);
            m_dbObject.AddInParameter(dbCommand, "@MOP2", DbType.String, lineItem[38]);
            m_dbObject.AddInParameter(dbCommand, "@GCERTNUM", DbType.String, lineItem[39]);
            m_dbObject.AddInParameter(dbCommand, "@PAYTOTAL2", DbType.String, lineItem[40]);
            m_dbObject.AddInParameter(dbCommand, "@ODR_COMM", DbType.String, lineItem[41]);
            m_dbObject.AddInParameter(dbCommand, "@SALUTATION", DbType.String, lineItem[42]);
            m_dbObject.AddInParameter(dbCommand, "@EMAIL_ALT", DbType.String, lineItem[43]);
            m_dbObject.AddInParameter(dbCommand, "@NOSHPCOST", DbType.String, lineItem[44]);
            m_dbObject.AddInParameter(dbCommand, "@SETTLED", DbType.String, lineItem[45]);
            m_dbObject.AddInParameter(dbCommand, "@ICVSETTLED", DbType.String, lineItem[46]);
            m_dbObject.AddInParameter(dbCommand, "@TAXRATE", DbType.String, lineItem[47]);
            m_dbObject.AddInParameter(dbCommand, "@TAXSTATE", DbType.String, lineItem[48]);
            m_dbObject.AddInParameter(dbCommand, "@TAX", DbType.String, lineItem[49]);
            m_dbObject.AddInParameter(dbCommand, "@SHIPPING", DbType.String, lineItem[50]);
            m_dbObject.AddInParameter(dbCommand, "@ODR_TOTAL", DbType.String, lineItem[51]);
            m_dbObject.AddInParameter(dbCommand, "@SALES_ID", DbType.String, lineItem[52]);
            m_dbObject.AddInParameter(dbCommand, "@VALIDCC", DbType.String, lineItem[53]);
            m_dbObject.AddInParameter(dbCommand, "@GIFTMSG", DbType.String, lineItem[54]);
            m_dbObject.AddInParameter(dbCommand, "@NOTAXCALC", DbType.String, lineItem[55]);
            m_dbObject.AddInParameter(dbCommand, "@GIFTCERTNO", DbType.String, lineItem[56]);
            if (lineItem.Length > 57)
                m_dbObject.AddInParameter(dbCommand, "@CVV", DbType.String, lineItem[57]);
            else
                m_dbObject.AddInParameter(dbCommand, "@CVV", DbType.String, "123");
            return ((int)m_dbObject.ExecuteScalar(dbCommand) == 1);
        }


        public static bool UpdateOrder(object OrderNum, Database db)
        {
            Console.WriteLine("Updating Order");
            DbCommand dbCommand = db.GetStoredProcCommand("Orders_Update");
            db.AddInParameter(dbCommand, "@OrderNum", DbType.String, OrderNum.ToString());
            db.AddInParameter(dbCommand, "@Status", DbType.Boolean, true);

            return ((int)db.ExecuteScalar(dbCommand) == 1);
        }
        public static string GetShippingMethod(object item)
        {
            string returnItem = "09";

            if (((string)item).Trim() == "Ground")
            { returnItem = "09"; }
            else
            { returnItem = "19"; }

            return returnItem;
        }

        public static void SearchForNewFiles(string Path, bool IsProduction)
        {

            int idx = 0;
            DirectoryInfo di = new DirectoryInfo(Path);
            foreach (FileInfo fi in di.GetFiles("*.txt"))
            {
                idx = 0;
                using (StreamReader sr = new StreamReader(fi.FullName))
                {
                    string[] itemCollection = sr.ReadToEnd().Replace("\n", "|").Split('|');


                    foreach (string item in itemCollection)
                    {
                        string[] lineItem = item.Split(',');
                        if (idx > 0 && item.Length > 0)
                        {
                            switch (fi.Name.IndexOf("ITEMS"))
                            {
                                case -1: //line items
                                    Console.WriteLine("Writing Order Line");
                                    AddHeaderOrder(lineItem);
                                    break;

                                default:
                                    Console.WriteLine("Writing Detail Line");
                                    AddLineItem(lineItem);
                                    break;

                            }
                        }
                        idx++;
                    }

                }


            }
        }

        private static string GetPhoneNumber(object item)
        {
            string returnItem = item.ToString();
            returnItem = returnItem.Replace("-", string.Empty);
            returnItem = returnItem.Replace(" ", string.Empty);
            returnItem = returnItem.Replace("(", string.Empty);
            returnItem = returnItem.Replace(")", string.Empty);

            if (returnItem.Length == 10)
            {
                returnItem = string.Format("({0}) {1}-{2}", returnItem.Substring(0, 3),
                    returnItem.Substring(3, 3),
                    returnItem.Substring(6, 4));
            }
            return returnItem;
        }

        private static string GetCreditCardMethod(object item)
        {
            string returnItem = "V";
            if (item != DBNull.Value)
            {
                switch (((string)item).ToUpper())
                {
                    case "VISA":
                        returnItem = "V";
                        break;
                    case "MASTERCARD":
                        returnItem = "MC";
                        break;
                    case "AMERICANEXPRESS":
                    case "AMEX":
                        returnItem = "A";
                        break;
                    case "DISCOVER":
                        returnItem = "D";
                        break;

                }
            }
            return returnItem;
        }

        private static string GetCreditCardExpire(object item)
        {
            if (item != DBNull.Value)
            {
                DateTime expire = DateTime.MinValue;
                DateTime.TryParse(item.ToString(), out expire);
                return expire.ToString("MM/yy");
            }
            return string.Empty;
        }

        private static string FixDateForDBNull(object i, string format)
        {
            DateTime dte = DateTime.MinValue;
            DateTime.TryParse(i.ToString(), out dte);
            return dte.ToString(format);
        }

        private static string GetCountry(object item)
        {
            string returnItem = "001"; //usa
            switch (item.ToString())
            {
                case "United States":
                case "USA":
                case "US":
                case "U.S.A.":
                    break;
                case "Canada":
                case "CAN":
                case "034":
                    returnItem = "034";
                    break;
            }
            return returnItem;
        }

        static bool isdate(string s)
        {
            DateTime dat = new DateTime();
            bool b = false;
            try
            {
                dat = Convert.ToDateTime(s);
                b = true;
            }
            catch (Exception e)
            {
                b = false;
            }

            return b;
        }

        static string zeropad(string s, int i)
        {
            string s1 = s;
            while (s1.Length < i)
            {
                s1 = "0" + s1;
            }
            return s1;
        }

        static string fixdate(string s)
        {
            string s1 = s;
            DateTime dat;
            if (isdate(s))
            {
                dat = Convert.ToDateTime(s);
                s1 = zeropad(dat.Month.ToString(), 2) + "/" + zeropad(dat.Day.ToString(), 2) + "/" + Right(dat.Year.ToString(), 2);
            }
            return s1;

        }


        static DataSet getsql3(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["uploadqueue"];

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);

            conn.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
            adp.Fill(ds);

            conn.Close();

            return ds;

        }

        static void runsql3(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["uploadqueue"];

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);

            conn.Open();

            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();

            conn.Close();

        }


        static void runsql4(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstring2"];

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);

            conn.Open();

            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();

            conn.Close();

        }


        static string fixquotes(string s)
        {
            string s1 = s;
            s1 = s1.Replace("'", "''");
            return s1;
        }


        private static string ProductItem(Database m_dbObject, string OrderID)
        {
            IDataReader reader;
            int Counter = 0;
            string hline = "";
            string str = "select a.SkuCode,Quantity,a.FullPrice,o.ShippingCost as ShippingPrice,CASE WHEN a.SkuCode like '%CONTINUITY%' THEN 'Y' ELSE 'N' END as [ContinuityFlag],";
            str += " '1' as [PaymentPlanID],CASE WHEN ltrim(rtrim(a.FullPrice))=ltrim(rtrim(a.FullPrice)) THEN '1PAY' ELSE '3 PAY' END  as [PaymentPlanName] ";
            str += " from sku a inner join [OrderSku] b On a.SkuID=b.SkuID inner join [Order] o on o.OrderId = b.OrderId where b.OrderId=" + OrderID;
            DbCommand dbCommand = m_dbObject.GetSqlStringCommand(str);
            using (reader = m_dbObject.ExecuteReader(dbCommand))
            {
                while (reader.Read())
                {
                    Counter += 1;
                    hline += AddPipe(FixForDBNull(reader["SkuCode"]));
                    hline += AddPipe(FixForDBNull(reader["Quantity"]));
                    hline += AddPipe(FixForDBNull(reader["FullPrice"]));
                    hline += AddPipe(FixForDBNull(reader["ShippingPrice"]));
                    hline += AddPipe(FixForDBNull(reader["ContinuityFlag"]));
                    hline += AddPipe(FixForDBNull(reader["PaymentPlanID"]));
                    hline += AddPipe(FixForDBNull(reader["PaymentPlanName"]));
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
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                //sendemail(filePath);
            }
        }
        static void Main(string[] args)
        {
            totallines = 0;
            orderlines = 0;
            hlines = 0;

            string oldordernum = "";

            string CopyPath1 = System.Configuration.ConfigurationSettings.AppSettings["CopyPath"];

            string Mode1 = System.Configuration.ConfigurationSettings.AppSettings["Mode"];
            string ID1 = System.Configuration.ConfigurationSettings.AppSettings["ID"];
            string MediaCode1 = System.Configuration.ConfigurationSettings.AppSettings["MediaCode"];
            string Username1 = System.Configuration.ConfigurationSettings.AppSettings["Username"];
            string Password1 = System.Configuration.ConfigurationSettings.AppSettings["Password"];
            string CompanyNumber1 = System.Configuration.ConfigurationSettings.AppSettings["CompanyNumber"];
            string ProjectNumber1 = System.Configuration.ConfigurationSettings.AppSettings["ProjectNumber"];

            string key1 = System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];

            Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
            Mediachase.eCF.BusLayer.Common.Util.EncryptionManager em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();




            string connstr1 = System.Configuration.ConfigurationSettings.AppSettings["connectionstring"];


            string sendemailto1 = System.Configuration.ConfigurationSettings.AppSettings["sendemailto"];
            string bcc1 = System.Configuration.ConfigurationSettings.AppSettings["bcc"];
            string cc1 = System.Configuration.ConfigurationSettings.AppSettings["cc"];

            string site1 = System.Configuration.ConfigurationSettings.AppSettings["site1"];
            string user1 = System.Configuration.ConfigurationSettings.AppSettings["user1"];
            string pass1 = System.Configuration.ConfigurationSettings.AppSettings["pass1"];

            string ftpon1 = System.Configuration.ConfigurationSettings.AppSettings["ftpon"];

            string result1 = "";

            ArrayList obj = new ArrayList();


            m_dbObject = DatabaseFactory.CreateDatabase("Dermawand");


            DbCommand dbCommand = m_dbObject.GetStoredProcCommand("PredectiveOrderBatch");

            IDataReader reader;

            int linenum = 0;

            string hline = "";            
            string path2 = string.Format("{1}\\ictvDermawand_{0}.txt", DateTime.Today.ToString("MMddyyyy"), CopyPath1);
            string path2a = string.Format("ictvDermawand_{0}.txt", DateTime.Today.ToString("MMddyyyy"));

            using (StreamWriter sb = new StreamWriter(path2))
            {
                //get the header row
                using (reader = m_dbObject.ExecuteReader(dbCommand))
                {
                    while (reader.Read())
                    {
                        if (linenum == 0)
                        {
                            hline = "";
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
                            //hline += AddPipe("Customer Name, First");
                            //hline += AddPipe("Customer Name, Last");
                            //hline += AddPipe("Address 1");
                            //hline += AddPipe("Address 2");
                            hline += AddPipe("City");
                            hline += AddPipe("State");
                            hline += AddPipe("Zip/Postal Code");
                            hline += AddPipe("TLD");
                            hline += AddPipe("Area Code");
                            //hline += AddPipe("Phone Number");
                            //hline += AddPipe("Email address");
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
                            hline += AddPipe("Custom Field 10");
                            sb.WriteLine(hline);
                        }

                        hline = "";
                        // RESPONSE STRAT
                        hline += AddPipe("Live Web");                                                              //  "Origin Name" |
                        hline += AddPipe("123456790");                                                             // "Unique ID" |
                        hline += AddPipe("No MOP");                                                                // "Disposition" |
                        hline += AddPipe(FixDateForDBNull(reader["CreatedDate"], "MM/dd/yyyy"));                       // "Response Date" |
                        hline += AddPipe(FixDateForDBNull(reader["CreatedDate"], "hh:mm:ss"));                         //"Response Start Time" |
                        hline += AddPipe(FixDateForDBNull(reader["CreatedDate"], "hh:mm:ss"));                         //"Response End Time" |
                        hline += AddPipe("ET");          
                        //Media Agency name was removed from the file on 5/15 along with callerID and Email
                        //"Response Time Zone" |
                        //if (reader["url"] != null)
                        //{
                        //    if (FixForDBNull(reader["url"]).ToLower().Contains("uk") || FixForDBNull(reader["url"]).ToLower().Contains("ie"))                            
                        //        hline += AddPipe("Euro");                                                               //"Media Agency name" |                            
                        //    else
                        //        hline += AddPipe("Mercury");                                                               //"Media Agency name" |
                        //}
                        //else
                        //{
                        //    hline += AddPipe("Mercury");                                                               //"Media Agency name" |
                        //}
                        hline += AddPipe("Web");                                                                   // "Origin Type" |
                        hline += AddPipe("None");                                                                  // "Offer Code" |
                        hline += AddPipe("888.666.1212");                                                          // "Dialed TFN" |
                        hline += AddPipe("888.666.1212");                                                          // "Terminating number/unique ID" |
                        hline += AddPipe(FixForDBNull(reader["Version"]));                                         // "Affiliate ID" |
                        hline += AddPipe(FixForDBNull(reader["URL"]));                                                     // "URL" |
                        hline += AddPipe("00:00:00");                                                              // "Call Length" |
                        hline += AddPipe("1234");                                                                  // "Operator ID" |
                        hline += AddPipe("NNP2");                                                                  // "Response Input" |
                        hline += AddPipe("LFTV");            // "Media Type" |
                        hline += AddPipe("DIRECT");          // "Media ID (Medium)" |
                        hline += AddPipe("000.000.0000");    // "ANI" |
                        hline += AddPipe("00.00.00");        // "Hold time" |

                        // ORDER/CUSTOMER START
                        //hline += AddPipe(FixForDBNull(reader["FirstName"]));
                        //hline += AddPipe(FixForDBNull(reader["LastName"]));
                        //hline += AddPipe(FixForDBNull(reader["Address1"]));
                        //hline += AddPipe(FixForDBNull(reader["Address2"]));
                        hline += AddPipe(FixForDBNull(reader["City"]));
                        hline += AddPipe(FixForDBNull(reader["StateProvince"]));
                        hline += AddPipe(FixForDBNull(reader["ZipPostalCode"]));
                        hline += AddPipe(FixForDBNull(reader["CountryCode"]));
                        // Country Code
                        //try
                        //{
                        //    if (reader["PhoneNumber"] != Convert.DBNull)
                        //    hline += AddPipe(reader["PhoneNumber"].ToString().Substring(0, 3)); // Area Code
                        //}
                        //catch
                        //{
                            hline += AddPipe("");
                        //}
                        //hline += AddPipe(FixForDBNull(reader["PhoneNumber"]));
                        //hline += AddPipe(FixForDBNull(reader["Email"]));
                        hline += AddPipe(FixForDBNull(reader["OrderId"]));
                        hline += AddPipe(FixDateForDBNull(reader["CreatedDate"], "MM/dd/yyyy"));
                        hline += AddPipe(FixDateForDBNull(reader["CreatedDate"], "hh:mm:ss"));
                        hline += AddPipe("Y");                                           //"Rush/Priority flag"
                        hline += AddPipe("$00.00");                                      //"Rush/Priority $ Amt"
                        hline += AddPipe("1");                                           //"Payment Plan ID"
                        hline += AddPipe("3 Pay");                                       // "Payment Plan" 
                        hline += AddPipe("");                                            // "Sales ScriptID" |
                        hline += AddPipe("");                                            // "Payment Method" |
                        hline += AddPipe(FixForDBNull(reader["CardType"]));              // "Card Type" 

                        // ITEM  STRAT
                        hline += ProductItem(m_dbObject, reader["OrderId"].ToString());

                        // ORDER TOTAL
                        hline += AddPipe(FixCurrencyForDBNull(reader["SubTotal"], 2));
                        hline += AddPipe(FixCurrencyForDBNull(reader["ShippingCost"], 2));
                        hline += AddPipe(FixCurrencyForDBNull(reader["Tax"], 2));
                        hline += AddPipe(FixCurrencyForDBNull(reader["OrderTotal"], 2));
                        // CUSTOM FILDS-10 
                        for (int i = 1; i < 10; i++)
                        {
                            hline += AddPipe("");
                        }
                        sb.WriteLine(hline);

                        linenum++;
                        totallines = totallines + 1;
                    }
                }
            }

            Console.WriteLine(path2);
            uploadFile("ftp://www.ftp-directory.com/", path2, "ConvSys", "c0nV$ys");


            //string s1;
            //s1 = "-e -u ~Conversion Systems <info@conversionsystems.com>~ -r ~Brand Developers <alan.j.meier@gmail.com>~ --yes ~filepath~";
            //s1 = s1.Replace("~", ((char)(34)).ToString());
            //s1 = s1.Replace("filepath", path2);
            //Console.WriteLine("C:\\gnupg\\gpg.exe " + s1);

            //string appname = "C:\\gnupg\\gpg.exe";
            //string args1 = s1;

            //runapp(appname, args1);

            //string sta, stb;
            //sta = path2;
            //stb = path2;

            //sta = sta.Replace(".csv", ".gpg");
            //stb = stb.Replace(".csv", ".csv.pgp");

            //if (File.Exists(stb)) { File.Delete(stb); }
            //File.Move(sta, stb);



            //string stb1, stb1a, stb1b;

            //stb1 = stb.ToLower();
            //stb1a = CopyPath1.ToLower();
            //stb1b = CopyPath2.ToLower();

            //stb1 = stb1.Replace(stb1a, stb1b);




            //string st100;
            //st100 = "Set MySite = CreateObject([[[CuteFTPPro.TEConnection[[[)~MySite.Host = [[[host.trustcommerce.com[[[~MySite.Protocol = [[[SFTP[[[~MySite.Port = 22~MySite.Retries = 30~MySite.Delay = 30~MySite.MaxConnections = 4~MySite.TransferType = [[[AUTO[[[~MySite.DataChannel = [[[DEFAULT[[[~MySite.AutoRename = [[[OFF[[[~MySite.Login = [[[bd542[[[~MySite.Password = [[[Uth3Aita[[[~MySite.SocksInfo = [[[[[[~MySite.ProxyInfo = [[[[[[~MySite.Connect~MySite.Upload [[[" + path2 + "[[[, [[[/incoming/ConvSysOrders/" + path2a + "[[[~MySite.Download [[[/incoming/ConvSysOrders/" + afterslash(path2) + "[[[, [[[" + path2 + "2" + "[[[~MySite.Disconnect~MySite.Close~";
            //st100 = st100.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            //st100 = st100.Replace("[[[", ((char)(34)).ToString());


            //StreamWriter SW3a;


            //DataSet ds20 = getsql3("select count(*) from uploadqueue where filepath='" + fixquotes(path2) + "'");
            //if (Convert.ToInt32(ds20.Tables[0].Rows[0].ItemArray[0].ToString()) == 0)
            //{
            //    runsql3("insert into uploadqueue(filepath,datetime1,isuploaded,script1,prefix) values('" + fixquotes(path2) + "',getdate(),0,'" + fixquotes(st100) + "','ston')");
            //}



            //MailMessage message = new MailMessage();
            //message.From = new MailAddress("info@conversionsystems.com");


            //string[] tok = new string[200];
            //int numtok = 1;
            //for (int i1 = 0; i1 < 200; i1++)
            //{
            //    tok[i1] = "";
            //}
            //for (int i1 = 0; i1 < sendemailto1.Length; i1++)
            //{
            //    if (sendemailto1[i1].ToString() == ";")
            //    {
            //        numtok++;
            //    }
            //    else
            //    {
            //        tok[numtok] += sendemailto1[i1];
            //    }
            //}



            //for (int i1 = 1; i1 <= numtok; i1++)
            //{
            //    message.To.Add(new MailAddress(tok[i1]));
            //}



            //if (cc1 != "")
            //{
            //    numtok = 1;
            //    for (int i1 = 0; i1 < 200; i1++)
            //    {
            //        tok[i1] = "";
            //    }
            //    for (int i1 = 0; i1 < cc1.Length; i1++)
            //    {
            //        if (cc1[i1].ToString() == ";")
            //        {
            //            numtok++;
            //        }
            //        else
            //        {
            //            tok[numtok] += cc1[i1];
            //        }
            //    }
            //    for (int i1 = 1; i1 <= numtok; i1++)
            //    {
            //        message.CC.Add(new MailAddress(tok[i1]));
            //    }
            //}



            //if (bcc1 != "")
            //{
            //    numtok = 1;
            //    for (int i1 = 0; i1 < 200; i1++)
            //    {
            //        tok[i1] = "";
            //    }
            //    for (int i1 = 0; i1 < bcc1.Length; i1++)
            //    {
            //        if (bcc1[i1].ToString() == ";")
            //        {
            //            numtok++;
            //        }
            //        else
            //        {
            //            tok[numtok] += bcc1[i1];
            //        }
            //    }



            //    for (int i1 = 1; i1 <= numtok; i1++)
            //    {
            //        message.Bcc.Add(new MailAddress(tok[i1]));
            //    }
            //}


            //message.Subject = "StoneDine batch files added to the upload queue";
            //string st;
            //st = "Hello, ~~The following StoneDine batch file was added to the upload queue: <filename>~~This occured at <date and time of upload>~~There were <XX> orders in this batch file.~~Thank You,~~Conversion Systems";
            //st = st.Replace("<filename>", path2a);
            //st = st.Replace("<date and time of upload>", System.DateTime.Now.ToString());
            //st = st.Replace("<XX>", hlines.ToString());
            //st = st.Replace("<result>", result1);

            //st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            //message.Body = st;
            //SmtpClient client = new SmtpClient();
            //client.Send(message);


            //st = "Hello, ~~The following StoneDine batch files were uplaoded: <filename>~~This occured at <date and time of upload>~~There were <XX> orders in this batch file.~~Thank You,~~Conversion Systems";
            //st = st.Replace("<filename>", path2a);
            //st = st.Replace("<date and time of upload>", "{datetime}");
            //st = st.Replace("<XX>", hlines.ToString());
            //st = st.Replace("<result>", result1);

            //runsql3("update stonedineemail set msg1='" + fixquotes(st) + "',sendemailto1='" + fixquotes(sendemailto1) + "' where rownum=1");
        }
    }
}
