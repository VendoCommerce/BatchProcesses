using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EBS.IntegrationServices.Providers.PaymentProviders;
using System.Web;
using CS.ErrorFramework.LogError;

namespace EBS.IntegrationServices.Providers.PaymentProviders.PayPalExpress
{
    public class Batch : GatewayProvider
    {

        #region Private Members

        private static string m_ProviderSection = "PaymentProviderPayPalExpress";
        private static ProviderConfiguration m_ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(m_ProviderSection);
        public static Provider objProvider = null;
        private static GatewaySettings gatewaySettings = null;
        private static string errRequiredNode = "A required gateway node does not exist: {0}";

        #endregion

        #region Public Methods

        public Batch()
        {
            objProvider = m_ProviderConfiguration.Providers[m_ProviderConfiguration.DefaultProvider];
        }

        private static void ThrowRequiredNodeError(string nodeName)
        {
            throw new GatewayException(string.Format(errRequiredNode, nodeName));
        }
        private static void LogMessage(string message)
        {
            if (System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFilePath"] != null)
            {
                using (StreamWriter w = File.AppendText(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFilePath"] + "\\Log_" + DateTime.Today.ToString("MMddyyyy")))
                {
                    LogError.LogErrorToFile(message, w);
                }
            }
        }
        private static void GetSettings()
        {

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xmlNode;

            try
            {
                //Load provider object from configuration
                objProvider = m_ProviderConfiguration.Providers[m_ProviderConfiguration.DefaultProvider];

                gatewaySettings = new GatewaySettings();

                //get transaction url attribute
                gatewaySettings.TransactionURL = objProvider.transactionUrl;
                gatewaySettings.Login = objProvider.login;
                gatewaySettings.TransactionKey = objProvider.transactionKey;
                gatewaySettings.Password = objProvider.Password;
                gatewaySettings.DelimData = objProvider.delimitedData.ToString();
                gatewaySettings.DelimChar = "&";
                gatewaySettings.Version = objProvider.version;
                gatewaySettings.TestMode = objProvider.transactionTest.ToString();
                gatewaySettings.DeviceType = objProvider.deviceType;
                gatewaySettings.MarketType = objProvider.marketType;

                if (string.IsNullOrEmpty(gatewaySettings.TransactionURL))
                {
                    throw new GatewayException("TransactionURL cannot be null");
                }

                if (string.IsNullOrEmpty(gatewaySettings.Login))
                {
                    throw new GatewayException("Login cannot be null");
                }

                if (string.IsNullOrEmpty(gatewaySettings.TransactionKey))
                {
                    throw new GatewayException("TransactionKey cannot be null");
                }

                if (string.IsNullOrEmpty(gatewaySettings.DelimData))
                {
                    gatewaySettings.DelimData = "TRUE";
                }

                if (string.IsNullOrEmpty(gatewaySettings.DelimChar))
                {
                    gatewaySettings.DelimChar = "&";
                }

                if (string.IsNullOrEmpty(gatewaySettings.TestMode))
                {
                    gatewaySettings.DelimData = "FALSE";
                }


            }
            catch (Exception ex)
            {
                LogMessage("Error while reading the gateway settings - " + ex.Message);
                throw new GatewayException("An error occured while reading the gateway settings", ex);
            }

        }
        string CheckUnwantedCharacters(string input)
        {
            input = input.Replace("'", "''"); //' espace the single quote
            input = input.Replace("\"\"", ""); // "
            input = input.Replace(")", ""); // ) 
            input = input.Replace("(", ""); // (
            input = input.Replace(";", ""); // ;
            input = input.Replace("-", ""); // -
            input = input.Replace("|", ""); // |
            input = input.Replace(".", ""); // .
            input = input.Replace("/", ""); // /
            input = input.Replace(",", ""); // ,
            input = input.Replace("#", ""); // #
            input = input.Replace("&", ""); // &
            return input;
        }
        public override Response PerformRequest(Request request)
        {
            LogMessage("Enter 'PerformRequest' - ");
            Response response = new Response();
            StreamWriter streamWriter = null;
            StreamReader streamReader = null;
            string strPost = string.Empty;

            if (gatewaySettings == null)
            {
                //Load settings if they have not been loaded
                Batch.GetSettings();
            }

            strPost = BuildRequestPost(request);
            LogMessage("Enter 'Posting Info' - ");
            //Initialize & populate HTTP request object
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(gatewaySettings.TransactionURL);

            //set header attributes
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {

                //write post for request to url
                streamWriter = new StreamWriter(objRequest.GetRequestStream());
                streamWriter.Write(strPost);

            }
            catch (Exception ex)
            {
                //error while writing post
                LogMessage("Error while posting - " + ex.Message);
                throw new GatewayException("An exception occured while getting the request stream for the gateway", ex);
            }
            finally
            {
                streamWriter.Close();
            }
            LogMessage("Exit 'Posting Info' - ");
            LogMessage("Enter 'GetResponse' - ");
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            try
            {

                //read response from request
                streamReader = new StreamReader(objResponse.GetResponseStream());

                response = ParseResponse(streamReader.ReadToEnd(),request);

            }
            catch (Exception ex)
            {
                LogMessage("Error while getting the response stream for the gateway - " + ex.Message);
                throw new GatewayException("An exception occured while getting the response stream for the gateway", ex);
            }
            finally
            {
                streamReader.Close();
            }

            return response;

        }
        public override Response PerformVoidRequest(Request request)
        {

            Response response = new Response();
            return response;

        }

        protected string BuildRequestPost(Request request)
        {
            LogMessage("Entered 'BuildRequestPost' - ");
            string str;
            string empty;
            string str1 = "{0}={1}";
            List<string> strs = new List<string>();
            strs.Add(string.Format(str1, "USER", objProvider.login));
            strs.Add(string.Format(str1, "PWD", objProvider.Password));
            strs.Add(string.Format(str1, "SIGNATURE", objProvider.transactionKey));            
            List<string> strs1 = strs;
            try
            {
                switch (request.Method.ToLower())
                {
                    case "setexpresscheckout":
                        {
                            LogMessage("Entered 'setexpresscheckout' - ");
                            strs.Add(string.Format(str1, "VERSION", objProvider.version));
                            strs.Add(string.Format(str1, "METHOD", request.Method));
                            strs.Add(string.Format(str1, "PAYMENTREQUEST_0_AMT", request.Amount.ToString("N2")));
                            strs.Add(string.Format(str1, "PAYMENTREQUEST_0_CURRENCYCODE", request.CurrencyCode));
                            strs.Add(string.Format(str1, "RETURNURL", request.ReturnUrl));
                            strs.Add(string.Format(str1, "CANCELURL", request.CancelUrl));
                            strs.Add(string.Format(str1, "PAYMENTREQUEST_0_PAYMENTACTION", request.PaymentAction));
                            
                            Hashtable additionalParams = new Hashtable();
                            if (request.RequestData != null && request.RequestData.Count > 0)
                            {
                                additionalParams = request.RequestData;
                            }
                            foreach (string s in additionalParams.Keys)
                            {
                                strs.Add(string.Format(str1, s.ToUpper(), additionalParams[s]));
                            }
                            LogMessage("Exit 'setexpresscheckout' - ");
                            break;
                        }
                    case "getexpresscheckoutdetails":
                        {
                            LogMessage("Enter 'getexpresscheckoutdetails' - ");
                            strs.Add(string.Format(str1, "VERSION", objProvider.version));
                            strs.Add(string.Format(str1, "METHOD", request.Method));
                            strs.Add(string.Format(str1, "TOKEN", request.Token));
                            LogMessage("Exit 'getexpresscheckoutdetails' - ");
                            break;
                        }
                    case "doexpresscheckoutpayment":
                        {
                            LogMessage("Enter 'doexpresscheckoutpayment' - ");
                            strs.Add(string.Format(str1, "VERSION", objProvider.version));
                            strs.Add(string.Format(str1, "METHOD", request.Method));
                            strs.Add(string.Format(str1, "TOKEN", request.Token));
                            strs.Add(string.Format(str1, "PAYMENTREQUEST_0_PAYMENTACTION", request.PaymentAction));
                            strs.Add(string.Format(str1, "PAYMENTREQUEST_0_AMT", request.Amount.ToString("N2")));
                            strs.Add(string.Format(str1, "PAYMENTREQUEST_0_CURRENCYCODE", request.CurrencyCode));
                            strs.Add(string.Format(str1, "PAYERID", request.InvoiceNumber));
                            Hashtable additionalParams = new Hashtable();
                            if (request.RequestData != null && request.RequestData.Count > 0)
                            {
                                additionalParams = request.RequestData;
                            }
                            foreach (string s in additionalParams.Keys)
                            {
                                strs.Add(string.Format(str1, s.ToUpper(), additionalParams[s]));
                            }
                            LogMessage("Exit 'doexpresscheckoutpayment' - ");
                            break;
                        }

                        
                    case "":
                        {
                            LogMessage("Enter 'Blank' - ");
                            break;
                        }
                }       
                bool flag = false;                                                
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                LogMessage("Error while building request - " + exception1.Message);
                throw new GatewayException(string.Format("An exception occured while creating the post parameters: {0}", exception.Message), exception);
            }
            string str6 = string.Join("&", strs1.ToArray());
            LogMessage("Exit 'BuildRequestPost' - ");
            return str6;
        }
        public static string fixtype(string s)
        {
            // VI VISA AX AMERICAN EXPRESS DI DISCOVER MC MASTER CARD 

            //AMEX
            //Discover
            //MasterCard
            //VISA

            string s1 = s.ToLower();
            if (s1 == Convert.ToString("VISA").ToLower()) { s1 = "VI"; }
            if (s1 == Convert.ToString("AMEX").ToLower()) { s1 = "AX"; }
            if (s1 == Convert.ToString("AmericanExpress").ToLower()) { s1 = "AX"; }
            if (s1 == Convert.ToString("Discover").ToLower()) { s1 = "DI"; }
            if (s1 == Convert.ToString("MasterCard").ToLower()) { s1 = "MC"; }

            return s1;
        }
       
        private Response ParseResponse(string GatewayResponse, Request request)
        {
            LogMessage("Enter 'ParseResponse' - " + GatewayResponse);
            Hashtable _htResponse = new Hashtable();
            Response gatewayResponse = new Response();
            gatewayResponse.GatewayResponseRaw = GatewayResponse;
            Response response1 = gatewayResponse;
            char[] chr = new char[1];
            chr[0] = Convert.ToChar(gatewaySettings.DelimChar);
            string[] strArrays = GatewayResponse.Split(chr);

            foreach (string s in strArrays)
            {
                _htResponse.Add(s.Split('=')[0], HttpUtility.UrlDecode(s.Split('=')[1]));
            }

            if (_htResponse.ContainsKey("ACK"))
            {
                if (_htResponse["ACK"].ToString().ToLower().Equals("success"))
                {
                    response1.ResponseType = TransactionResponseType.Approved;
                    response1.ResponseData = _htResponse;
                    response1.ReasonText = string.Concat("(", strArrays, ")");
                    if (_htResponse["PAYMENTINFO_0_TRANSACTIONID"] != null)
                    {
                        response1.TransactionID = _htResponse["PAYMENTINFO_0_TRANSACTIONID"].ToString();
                    }
                    else
                    {
                        response1.TransactionID = _htResponse["CORRELATIONID"].ToString();
                    }
                    response1.AuthCode = _htResponse["CORRELATIONID"].ToString();
                }
                else
                {
                    response1.ResponseType = TransactionResponseType.Denied;
                    response1.ResponseData = _htResponse;
                    response1.ReasonText = string.Concat("(", strArrays, ")");
                }
            }
            else
            {
                response1.ResponseType = TransactionResponseType.Error;
                response1.ResponseData = _htResponse;
                response1.ReasonText = string.Concat("(", strArrays, ")");                
            }
            return response1; ;

        }

        #endregion

        private class GatewaySettings
        {

            #region Private Members

            private string m_Login;
            private string m_TransactionKey;
            private string m_DelimData = "TRUE";
            private string m_DelimChar;
            private string m_EncapChar;
            private string m_Version;
            private string m_RelayResponse = "FALSE";
            private string m_TransactionURL;
            private string m_TestMode = "TRUE";
            private string m_DuplicateWindow;
            private string m_EmailCustomer;
            private string m_MerchantEmail;
            private string m_CurrencyCode;
            private string m_MarketType = "";
            private string m_DeviceType = "";
            private string m_Password = "";
            #endregion

            #region Public Properties
            public string CurrencyCode
            {
                get { return m_CurrencyCode; }
                set { m_CurrencyCode = value; }
            }


            public string MerchantEmail
            {
                get { return m_MerchantEmail; }
                set { m_MerchantEmail = value; }
            }

            public string EmailCustomer
            {
                get { return m_EmailCustomer; }
                set { m_EmailCustomer = value; }
            }


            public string DuplicateWindow
            {
                get { return m_DuplicateWindow; }
                set { m_DuplicateWindow = value; }
            }


            public string Login
            {
                get { return m_Login; }
                set { m_Login = value; }
            }

            public string TransactionKey
            {
                get { return m_TransactionKey; }
                set { m_TransactionKey = value; }
            }

            public string DelimData
            {
                get { return m_DelimData; }
                set { m_DelimData = value; }
            }

            public string DelimChar
            {
                get { return m_DelimChar; }
                set { m_DelimChar = value; }
            }

            public string EncapChar
            {
                get { return m_EncapChar; }
                set { m_EncapChar = value; }
            }

            public string Version
            {
                get { return m_Version; }
                set { m_Version = value; }
            }

            public string RelayResponse
            {
                get { return m_RelayResponse; }
            }

            public string TransactionURL
            {
                get { return m_TransactionURL; }
                set { m_TransactionURL = value; }
            }

            public string TestMode
            {
                get { return m_TestMode; }
                set { m_TestMode = value; }
            }
            public string MarketType
            {
                get { return m_MarketType; }
                set { m_MarketType = value; }
            }
            public string DeviceType
            {
                get { return m_DeviceType; }
                set { m_DeviceType = value; }
            }
            public string Password
            {
                get { return m_Password; }
                set { m_Password = value; }
            }
            
            #endregion

        }

    }
}
