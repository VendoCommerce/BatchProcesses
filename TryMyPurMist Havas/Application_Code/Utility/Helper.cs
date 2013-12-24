using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using Com.ConversionSystems.Utility;

namespace Com.ConversionSystems.Utility
{
    public class Helper
    {


        private const string _FILENAME = "Helper.cs";

        public Helper() { }

        public static NameValueCollection AppSettings
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("Helper"); }
        }
        public static NameValueCollection AppServers
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("AppServers"); }
        }
        public static NameValueCollection Translations
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("Translations"); }
        }
        public static NameValueCollection CacheKeys
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("CacheKeys"); }
        }
        public static NameValueCollection LookupGroups
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("LookupGroups"); }
        }
        public static NameValueCollection Lookups
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("Lookups"); }
        }
        public static NameValueCollection ApplicationRoleTypes
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("ApplicationRoleTypes"); }
        }
        public static NameValueCollection ApplicationRoles
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("ApplicationRoles"); }
        }
        public static NameValueCollection ErrorLogAPI
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("ErrorLogAPI"); }
        }
        public static NameValueCollection WebServerLocation
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("WebServerLocation"); }
        }
        public static NameValueCollection SectionContentCategories
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("SectionContentCategories"); }
        }
        public static NameValueCollection SectionContentTypes
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("SectionContentTypes"); }
        }
        public static NameValueCollection AdminEmailsForNewRoleRequest
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("AdminEmailsForNewRoleRequest"); }
        }
        public static NameValueCollection CCPaymentType
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("CCPaymentType"); }
        }
        public static NameValueCollection ItemCodes
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("ItemCodes"); }
        }
        public static NameValueCollection Country
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("Country"); }
        }
        public static NameValueCollection SQLServerDAO
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("SQLServerDAO"); }
        }
        public static NameValueCollection DataServicesXMLBatchConfig
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection("DataServicesXMLBatchConfig"); }
        }
        public static string WebServerLocationOnServer
        {
            get
            {
                return WebServerLocation[ServerMode];
            }
        }


        public static string ServerMode
        {
            get
            {
                /*string strMode = "Dev";
                string strReturn = "";
                string strServerName = "";
                string strServerIP = "";
                strServerName = Helper.ServerName;
                strServerIP = Helper.ServerIP;
                // first let's try the name...
                strMode = AppServers[strServerName];
                if (strMode != null)
                {
                    strMode = Decode(strMode, "Dev", strMode, Decode(
                        strMode, "Tst", strMode, Decode(
                            strMode, "Prd", strMode, "Dev")));
                }
                else
                {
                    // if the name isn't found then let's try the IP...
                    strMode = AppServers[strServerIP];
                    if (strMode != null)
                    {
                        strMode = Decode(strMode, "Dev", strMode, Decode(
                            strMode, "Tst", strMode, Decode(
                                strMode, "Prd", strMode, "Dev")));
                    }
                    else { strMode = "Dev"; }
                }
                strReturn = strMode;
                return strReturn;*/
                return "mode";

            }
        }
        public static string Translate(string Input)
        {
            string strTranslation = "";
            string strReturn = "";
            strTranslation = Translations[Input];
            if (strTranslation == null) { strTranslation = ""; }
            strReturn = strTranslation;
            return strReturn;
        }
        public static string GetCacheKeyValueForKey(string Input)
        {
            string strValue = "";
            string strReturn = "";
            strValue = CacheKeys[Input];
            if (strValue == null) { strValue = ""; }
            strReturn = strValue;
            return strReturn;
        }
        public static string GetLookupGroupValueForKey(string Input)
        {
            string strValue = "";
            string strReturn = "";
            strValue = LookupGroups[Input];
            if (strValue == null) { strValue = ""; }
            strReturn = strValue;
            return strReturn;
        }
        public static string GetLookupValueForKey(string Input)
        {
            string strValue = "";
            string strReturn = "";
            strValue = Lookups[Input];
            if (strValue == null) { strValue = ""; }
            strReturn = strValue;
            return strReturn;
        }
        public static string GetApplicationRoleTypeValueForKey(string Input)
        {
            string strValue = "";
            string strReturn = "";
            strValue = ApplicationRoleTypes[Input];
            if (strValue == null) { strValue = ""; }
            strReturn = strValue;
            return strReturn;
        }
        public static string GetApplicationRoleValueForKey(string Input)
        {
            string strValue = "";
            string strReturn = "";
            strValue = ApplicationRoles[Input];
            if (strValue == null) { strValue = ""; }
            strReturn = strValue;
            return strReturn;
        }



        public static char Delimiter
        {
            get
            {
                return 'þ';
            }
        }

        public static bool SendMail(MailMessage oMsg)
        {
            SmtpClient client;
            bool bResult = false;
            int intFlag = -1;
            try
            {
                intFlag = -2;
                oMsg.BodyEncoding = System.Text.Encoding.UTF8;

                if (Helper.ServerMode.ToLower().Equals("dev") || Helper.ServerMode.ToLower().Equals("tst"))
                {
                    intFlag = -3;
                    oMsg.Body = "To: " + oMsg.To.ToString() + "\n\n\n" + oMsg.Body;
                    oMsg.CC.Clear();
                    oMsg.Bcc.Clear();
                }

                intFlag = -4;
                                
                client = new SmtpClient(Helper.SmtpServer);
                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(oMsg);
                bResult = true;
            }
            catch (Exception ex)
            {
                string strErrMsg = string.Format("[{0}][{1}][Flag:{2}]:{3}", Helper.AppName, "Helper.sendMail(MailMessage)", intFlag, ex.Message);
                //Helper.ErrorLogAPI.LogToEvent(strErrMsg, EventLogEntryType.Error);
                 LogData log = new LogData();
                log.LogToFile("Error sending email---" + strErrMsg);
            }
            return bResult;
        }




        public static bool IsValidEmail(string email)
        {

            email = email.Replace("_", "");
            email = email.Replace("-", "");

            string pattern = @"^[-a-zA-Z0-9][-.a-zA-Z0-9]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\.(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$";

            Regex check = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            bool valid = false;
            if (string.IsNullOrEmpty(email)) { valid = false; }
            else { valid = check.IsMatch(email); }
            return valid;
        }
        public static bool HasData(DataSet ds)
        {
            bool bResult = false;
            if (ds != null)
                if (ds.Tables != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows != null)
                        {
                            bResult = (ds.Tables[0].Rows.Count > 0);
                        }
                    }

                }
            return bResult;
        }
        public static bool HasData(DataTable dt)
        {
            bool bResult = false;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    bResult = true;
                }
            }
            return bResult;
        }


        public static string MachineName
        {
            get
            {
                return System.Environment.MachineName;
            }
        }
        public static string DNSName
        {
            get
            {
                return System.Net.Dns.GetHostEntry("LocalHost").HostName;

            }
        }


        public static string AppAcronym
        {
            get
            {
                return Helper.AppSettings["AppAcronym"];
            }
        }
        public static string AppName
        {
            get
            {
                return Helper.AppSettings["AppName"];
            }
        }
        public static string AppVersion
        {
            get
            {
                return Helper.AppSettings["AppVersion"];
            }
        }
        public static string AppReleaseType
        {
            get
            {
                return Helper.AppSettings["AppReleaseType"];
            }
        }
        public static string AppReleaseDate
        {
            get
            {
                return Helper.AppSettings["ReleaseDate"];
            }
        }
        public static string CatalystOrganizationName
        {
            get
            {
                return Helper.AppSettings["CatalystOrganizationName"];
            }
        }
        public static string Producer
        {
            get
            {
                return Helper.AppSettings["Producer"];
            }
        }
        public static string ReleaseDate
        {
            get
            {
                return Helper.AppSettings["ReleaseDate"];
            }
        }
        public static int CacheTimer
        {
            get
            {
                return Helper.ToInt32(Helper.AppSettings["CacheTimer"]);
            }
        }
        public static string AdminEmail
        {
            get
            {
                return Helper.AppSettings["AdminEmail"];
            }
        }
        public static string MonitorEmail
        {
            get
            {
                return Helper.AppSettings["MonitorEmail"];
            }
        }
        public static string SmtpServer
        {
            get
            {
                return Helper.AppSettings["SmtpServer"];
            }
        }
        public static int PageSize
        {
            get
            {
                int intSize = 0;
                try
                {
                    intSize = Helper.ToInt32(Helper.AppSettings["PageSize"]);
                }
                catch
                {
                    intSize = 10;
                }
                return intSize;
            }
        }
        private static int KeySize
        {
            get
            {
                return Helper.ToInt32(Helper.AppSettings["KeySize"]);
            }
        }
        private static string KeyPath
        {
            get
            {
                string strPath = Helper.AppSettings["KeyPath"];
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                return strPath;
            }
        }
        private static string KeyFile
        {
            get
            {
                return Helper.AppSettings["KeyFile"];
            }
        }
        public static string ProfilePage
        {
            get
            {
                return Helper.AppSettings["ProfilePage"];
            }
        }


        public static string[] Tokenizer(string strInput)
        {
            string strValue = "";
            if (strInput != null)
            {
                strValue = strInput.Trim();
                strValue = strValue.Replace(",", ";");
                strValue = strValue.Replace(" ", ";");
            }
            return strValue.Split(';');
        }
        public static double ToDouble(object oValue)
        {
            double dblValue = 0.0;
            try { dblValue = System.Convert.ToDouble(oValue); }
            catch { dblValue = 0.0; }
            return dblValue;
        }
        public static bool IntToBoolean(int Input)
        {
            bool bReturn = false;
            if (Input > 0) { bReturn = true; }
            return bReturn;
        }
        public static int BooleanToInt(bool Input)
        {
            int intReturn = 0;
            if (Input) { intReturn = 1; }
            return intReturn;
        }
        public static string Nvl(string Input, string Default)
        {
            string strOutput = "";
            if (Input == null)
            {
                if (Default == null) { Default = ""; }
                strOutput = Default;
            }
            else { strOutput = Input; }
            return strOutput;
        }
        public static string Nvl2(string Input, string Default)
        {
            string strOutput = "";
            if (Input.Equals("%"))
            {
                if (Default == null) { Default = ""; }
                strOutput = Default;
            }
            else { strOutput = Input; }
            return strOutput;
        }
        public static string Decode(string Input, string Test, string Output, string Default)
        {
            string strReturn = "";
            if (Input.ToLower().Equals(Test.ToLower())) { strReturn = Output; }
            else { strReturn = Default; }
            return strReturn;
        }
        public static decimal ToDecimal(object oValue)
        {
            decimal decValue = 0.0m;
            try
            {
                decValue = System.Convert.ToDecimal(oValue);
            }
            catch
            {
                decValue = 0.0m;
            }
            return decValue;
        }
        public static int ToInt32(object oValue)
        {
            int intValue = 0;
            try { intValue = System.Convert.ToInt32(oValue); }
            catch { intValue = 0; }
            return intValue;
        }
        public static float ToFloat(object oValue)
        {
            float fValue = 0.0F;
            try { fValue = System.Convert.ToSingle(oValue); }
            catch { fValue = 0.0F; }
            return fValue;
        }
        public static long ToInt64(object oValue)
        {
            long lngValue = 0;
            try
            {
                lngValue = System.Convert.ToInt64(oValue);
            }
            catch
            {
                lngValue = 0;
            }
            return lngValue;
        }
        public static DateTime ToDateTime(object oValue)
        {
            DateTime dt = new DateTime();
            try { dt = System.Convert.ToDateTime(oValue); }
            catch { dt = new DateTime(); }
            return dt;
        }
        public static byte[] FileToByte(string strFile)
        {
            byte[] data = null;
            FileStream fs = null;
            BinaryReader br = null;

            if (File.Exists(strFile))
            {
                try
                {
                    fs = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                    br = new BinaryReader(fs);
                    data = br.ReadBytes((int)fs.Length);
                }
                finally
                {
                    br.Close();
                    fs.Close();
                    br = null;
                    fs = null;
                }
            }
            return data;
        }
        public static Stream StringToStream(string strValue)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] byteValue = ByteConverter.GetBytes(strValue);
            return (Stream)new MemoryStream(byteValue);
        }
        public static string StreamToString(Stream stream)
        {
            string strValue = "";
            stream.Position = 0;
            using (StreamReader sr = new StreamReader(stream))
            {
                strValue = sr.ReadToEnd();
                sr.Close();
            }
            return strValue;
        }
        public static byte[] StringToByte(string strValue)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            return ByteConverter.GetBytes(strValue);
        }
        public static string ByteToString(byte[] byteValue)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            return ByteConverter.GetString(byteValue);
        }
        public static byte[] Base64ToByte(string strValue)
        {
            return Convert.FromBase64String(strValue);
        }
        public static string ByteToBase64(byte[] byteValue)
        {
            return Convert.ToBase64String(byteValue);
        }
        public static string ConvertDataSetToXml(DataSet ds)
        {
            string strXml = "";
            //if (ds != null)
            //  strXml = ds.GetXml();
            StringWriter swXml = new StringWriter();
            ds.WriteXml(swXml, XmlWriteMode.WriteSchema);
            strXml = swXml.ToString();
            swXml.Close();
            return strXml;
        }
        public static DataSet ConvertXmlToDataSet(string strXml)
        {
            DataSet ds = null;
            StringReader reader = null;
            try
            {
                reader = new StringReader(strXml);
                ds = new DataSet();
                ds.ReadXml(reader);
            }
            finally
            {
                reader.Close();
            }
            return ds;
        }

        public static string ConvertDateToReadableDate(DateTime dt)
        {
            string strReturn = "";
            try
            {
                if (dt == null || dt == DateTime.MinValue)
                {
                    strReturn = "Date not specified.";
                }
                else
                {
                    strReturn = dt.ToLongDateString();
                }
            }
            catch
            {
                strReturn = "Date not specified.";
            }
            return strReturn;
        }
        public static string ConvertBooleanToYesOrNo(bool b)
        {
            string strReturn = "No";
            if (b) { strReturn = "Yes"; }
            return strReturn;
        }
        public static bool IsBlank(string s)
        {
            bool bReturn = true;
            if (s != null && s.Length > 0) { bReturn = false; }
            return bReturn;
        }

        public static string TrimCSV(string CSV, bool Spacer)
        {
            string sReturn = "";
            StringBuilder sb = new StringBuilder("");
            string sDelimiter = "";
            string sSpacer = "";
            string[] sValues;
            try
            {
                if (CSV.Length > 0)
                {
                    if (Spacer) { sSpacer = " "; }
                    sValues = CSV.Split(new char[] { ',' });
                    if (sValues.Length > 0)
                    {
                        for (int i = 0; i < sValues.Length; i++)
                        {
                            sb.Append(sDelimiter + sValues[i].Trim());
                            sDelimiter = "," + sSpacer;
                        }
                    }
                }
            }
            catch { }
            sReturn = sb.ToString();
            return sReturn;
        }
        public static string ConvertStringArrayToSQLIntArray(string[] sArray, bool NoSpaces)
        {
            string sReturn = "";
            StringBuilder sb = new StringBuilder("");
            string sDelimiter = "";
            string sSpacer = " ";
            long lngValue;

            if (NoSpaces) { sSpacer = ""; }
            try
            {
                if (sArray.Length > 0)
                {
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        lngValue = ToInt64(sArray[i].Trim());

                        if (lngValue != 0)
                        {
                            sb.Append(sDelimiter + lngValue.ToString());
                            sDelimiter = "," + sSpacer;
                        }
                    }
                }
            }
            catch { }
            sReturn = sb.ToString();
            return sReturn;
        }
        public static string ConvertStringArrayToSQLStringArray(string[] sArray, bool NoQuotes, bool NoSpaces)
        {
            string sReturn = "";
            StringBuilder sb = new StringBuilder("");
            string sDelimiter = "";
            string sSpacer = " ";
            string sQuote = "'";
            string sValue = "";

            if (NoQuotes) { sQuote = ""; }
            if (NoSpaces) { sSpacer = ""; }
            try
            {
                if (sArray.Length > 0)
                {
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        sValue = sArray[i].Trim();
                        sValue = sValue.Replace(";", "");
                        sValue = sValue.Replace("'", "'" + sQuote);
                        if (sValue.Length > 0)
                        {
                            sb.Append(sDelimiter + sQuote + sValue + sQuote);
                            sDelimiter = "," + sSpacer;
                        }
                    }
                }
            }
            catch { }
            sReturn = sb.ToString();
            return sReturn;
        }
        public static string BooleanToString(bool Input)
        {
            string strReturn = "N";
            if (Input) { strReturn = "Y"; }
            return strReturn;
        }

    }
}