using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using EBS.IntegrationServices.Providers.PaymentProviders;

namespace EBS.IntegrationServices.Providers.PaymentProviders.WireCardEFT
{
    public class WireCardEFT : GatewayProvider
    {

        #region Private Members

        private static string m_ProviderSection = "PaymentProviderWireCardETF";
        private static ProviderConfiguration m_ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(m_ProviderSection);
        public static Provider objProvider = null;
        private static GatewaySettings gatewaySettings = null;
        private static string errRequiredNode = "A required gateway node does not exist: {0}";

        #endregion

        #region Public Methods

        public WireCardEFT()
        {
            objProvider = m_ProviderConfiguration.Providers[m_ProviderConfiguration.DefaultProvider];
        }

        private static void ThrowRequiredNodeError(string nodeName)
        {
            throw new GatewayException(string.Format(errRequiredNode, nodeName));
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
                gatewaySettings.DelimData = objProvider.delimitedData.ToString();
                gatewaySettings.DelimChar = objProvider.delimitedCharacter;
                gatewaySettings.Version = objProvider.version;
                gatewaySettings.TestMode = objProvider.transactionTest.ToString();
                gatewaySettings.DeviceType = objProvider.deviceType;
                gatewaySettings.MarketType = objProvider.marketType;
                gatewaySettings.Password = objProvider.Password;

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
                    throw new GatewayException("TransactionKey/BusinessCaseSignature cannot be null");
                }
                if (string.IsNullOrEmpty(gatewaySettings.Password))
                {
                    throw new GatewayException("Password cannot be null");
                }

                if (string.IsNullOrEmpty(gatewaySettings.DelimData))
                {
                    gatewaySettings.DelimData = "TRUE";
                }

                if (string.IsNullOrEmpty(gatewaySettings.DelimChar))
                {
                    gatewaySettings.DelimData = "|";
                }

                if (string.IsNullOrEmpty(gatewaySettings.TestMode))
                {
                    gatewaySettings.DelimData = "FALSE";
                }


            }
            catch (Exception ex)
            {
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

            Response response = new Response();
            StreamWriter streamWriter = null;
            StreamReader streamReader = null;
            string strPost = string.Empty;

            if (gatewaySettings == null)
            {
                //Load settings if they have not been loaded
                WireCardEFT.GetSettings();
            }

            if (request.AdditionalInfo != null)
            {
                if (request.AdditionalInfo.ContainsKey("WireCardRequestType"))
                {
                    switch (request.AdditionalInfo["WireCardRequestType"].ToString().ToUpper())
                    {
                        case "DEBIT":
                            strPost = BuildRequestPostForDebit(request);
                            break;
                        case "POI":
                            strPost = BuildRequestPostForPOI(request);
                            break;
                        case "CREDITCHECK":
                            strPost = BuildRequestPostForCreditCheckScore(request);
                            break;
                        default:
                            strPost = BuildRequestPostForAuthorization(request);
                            break;
                    }
                }
            }
            else
            {
                strPost = BuildRequestPostForAuthorization(request);
            }
            //Initialize & populate HTTP request object
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(gatewaySettings.TransactionURL);

            //set header attributes
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.Credentials = new NetworkCredential(gatewaySettings.Login.Trim(), gatewaySettings.Password.Trim());
            objRequest.ContentType = "text/xml";

            try
            {

                //write post for request to url
                streamWriter = new StreamWriter(objRequest.GetRequestStream());
                streamWriter.Write(strPost);

            }
            catch (Exception ex)
            {
                //error while writing post
                throw new GatewayException("An exception occured while getting the request stream for the gateway", ex);
            }
            finally
            {
                streamWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            try
            {

                //read response from request
                streamReader = new StreamReader(objResponse.GetResponseStream());

                response = ParseResponse(streamReader.ReadToEnd(), request);

            }
            catch (Exception ex)
            {
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

        protected string BuildRequestPostForAuthorization(Request request)
        {

            string NameValueFormat = "{0}={1}";
            string s1 = "";




            //List<string> aryPostParams = new List<string>();


            ////Create parameters from objProvider settings
            //aryPostParams.Add(string.Format(NameValueFormat, "ePayAccountNum", objProvider.login));
            //aryPostParams.Add(string.Format(NameValueFormat, "password", objProvider.Password));
            //aryPostParams.Add(string.Format(NameValueFormat, "transactionCode", objProvider.transactionKey));


            //Create parameters from request

            //aryPostParams.Add(string.Format(NameValueFormat, "orderNum", request.InvoiceNumber));

            try
            {

                s1 += "<?xml version='1.0' encoding='UTF-8'?>";
                s1 += "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance'xsi:noNamespaceSchemaLocation='wirecard.xsd'>";
                s1 += "<W_REQUEST>";
                s1 += "<W_JOB>";
                s1 += "<JobID>job 2</JobID>";
                s1 += "<BusinessCaseSignature>" + gatewaySettings.TransactionKey + "</BusinessCaseSignature>";
                s1 += "<FNC_CC_TRANSACTION>";
                s1 += "<FunctionID>" + request.InvoiceNumber + "</FunctionID>";
                s1 += "<CC_TRANSACTION>";
                s1 += "<TransactionID>" + request.InvoiceNumber + "</TransactionID>";
                s1 += "<Amount>" + request.Amount * 100 + "</Amount>";
                s1 += "<Currency>" + request.CurrencyCode + "</Currency>";
                s1 += "<CountryCode>" + request.Country + "</CountryCode>";
                s1 += "<RECURRING_TRANSACTION>";
                s1 += "<Type>Single</Type>";
                s1 += "</RECURRING_TRANSACTION>";
                s1 += "<CREDIT_CARD_DATA>";
                s1 += "<CreditCardNumber>" + request.CardNumber + "</CreditCardNumber>";
                s1 += "<CVC2>" + request.CardCvv + "</CVC2>";
                s1 += "<ExpirationYear>" + request.ExpireDate.Substring(2, 4) + "</ExpirationYear>";
                s1 += "<ExpirationMonth>" + request.ExpireDate.Substring(0, 2) + "</ExpirationMonth>";
                s1 += "<CardHolderName>" + request.FirstName + " " + request.LastName + "</CardHolderName>";
                s1 += "</CREDIT_CARD_DATA>";
                s1 += "<CONTACT_DATA>";
                s1 += "<IPAddress>" + request.IPAddress + "</IPAddress>";
                s1 += "</CONTACT_DATA>";
                s1 += "<CORPTRUSTCENTER_DATA>";
                s1 += "<ADDRESS>";
                s1 += "<Address1>" + request.Address1 + "</Address1>";
                s1 += "<City>" + request.City + "</City>";
                s1 += "<ZipCode>" + request.ZipCode + "</ZipCode>";
                s1 += "<State>" + request.State + "</State>";
                s1 += "<Country>" + request.Country + "</Country>";
                s1 += "<Phone>" + request.Phone + "</Phone>";
                s1 += "<Email>" + request.Email + "</Email>";
                s1 += "</ADDRESS>";
                s1 += "</CORPTRUSTCENTER_DATA>";
                s1 += "</CC_TRANSACTION>";
                s1 += "</FNC_CC_TRANSACTION>";
                s1 += "</W_JOB>";
                s1 += "</W_REQUEST>";
                s1 += "</WIRECARD_BXML>";
            }
            catch (Exception ex)
            {

                throw new GatewayException(string.Format("An exception occured while creating the post parameters: {0}", ex.Message), ex);
            }

            //return string.Join("&", aryPostParams.ToArray());
            return s1;

        }
        protected string BuildRequestPostForDebit(Request request)
        {

            string NameValueFormat = "{0}={1}";
            string s1 = "";




            //List<string> aryPostParams = new List<string>();


            ////Create parameters from objProvider settings
            //aryPostParams.Add(string.Format(NameValueFormat, "ePayAccountNum", objProvider.login));
            //aryPostParams.Add(string.Format(NameValueFormat, "password", objProvider.Password));
            //aryPostParams.Add(string.Format(NameValueFormat, "transactionCode", objProvider.transactionKey));


            //Create parameters from request

            //aryPostParams.Add(string.Format(NameValueFormat, "orderNum", request.InvoiceNumber));

            try
            {

                s1 += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                s1 += "<WIRECARD_BXML xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"wirecard.xsd\"> ";
                s1 += "<W_REQUEST>";
                s1 += "<W_JOB>";
                s1 += "<BusinessCaseSignature>" + gatewaySettings.TransactionKey + "</BusinessCaseSignature>";
                s1 += "<JobID>" + request.InvoiceNumber + "</JobID>";
                s1 += "<FNC_FT_DEBIT>";
                s1 += "<FunctionID>" + request.InvoiceNumber + "</FunctionID>";
                s1 += "<FT_TRANSACTION>";
                s1 += "<TransactionID>" + request.InvoiceNumber + "</TransactionID>";
                s1 += "<PaymentGroupID></PaymentGroupID>";
                s1 += "<EXTERNAL_ACCOUNT>";
                s1 += "<FirstName>" + request.FirstName + "</FirstName>";
                s1 += "<LastName>" + request.LastName + "</LastName>";
                s1 += "<AccountNumber>" + request.CardNumber + "</AccountNumber>";
                s1 += "<AccountType>" + request.BankAcctType + "</AccountType>";
                s1 += "<BankCode>" + request.BankAbaCode + "</BankCode>";
                s1 += "<Country>" + request.Country + "</Country>";
                s1 += "<CheckNumber></CheckNumber>";
                s1 += "<COUNTRY_SPECIFIC>";
                s1 += "<IdentificationNumber></IdentificationNumber>";
                s1 += "</COUNTRY_SPECIFIC>";
                s1 += "</EXTERNAL_ACCOUNT>";
                s1 += "<INTERNAL_ACCOUNT>";
                s1 += "<AccountID></AccountID>";
                s1 += "</INTERNAL_ACCOUNT>";
                s1 += "<Amount minorunits=\"2\">" + request.Amount * 100 + "</Amount>";
                s1 += "<Currency>" + request.CurrencyCode + "</Currency>";
                s1 += "<Usage>" + request.InvoiceNumber + "</Usage>";
                s1 += "<CORPTRUSTCENTER_DATA>";
                s1 += "<ADDRESS>";
                s1 += "<Address1>" + request.Address1 + "</Address1>";
                s1 += "<Address2>" + request.Address2 + "</Address2>";
                s1 += "<City>" + request.City + "</City>";
                s1 += "<ZipCode>" + request.ZipCode + "</ZipCode>";
                s1 += "<State>" + request.State + "</State>";
                s1 += "<Country>" + request.Country + "</Country>";
                s1 += "<Phone>" + request.Phone + "</Phone>";
                s1 += "<Email>" + request.Email + "</Email>";
                s1 += "<IPAddress>" + request.IPAddress + "</IPAddress>";
                s1 += "</ADDRESS>";
                s1 += "<PERSONINFO>";
                s1 += "<DRIVERS_LICENSE>";
                s1 += "<LicenseNumber></LicenseNumber>";
                s1 += "<State></State>";
                s1 += "<Country></Country>";
                s1 += "</DRIVERS_LICENSE>";
                s1 += "<BirthDate></BirthDate>";
                s1 += "<TaxIdentificationNumber></TaxIdentificationNumber>";
                s1 += "</PERSONINFO>";
                s1 += "</CORPTRUSTCENTER_DATA>";
                s1 += "</FT_TRANSACTION>";
                s1 += "</FNC_FT_DEBIT>";
                s1 += "</W_JOB>";
                s1 += "</W_REQUEST>";
                s1 += "</WIRECARD_BXML>";
            }
            catch (Exception ex)
            {

                throw new GatewayException(string.Format("An exception occured while creating the post parameters: {0}", ex.Message), ex);
            }

            //return string.Join("&", aryPostParams.ToArray());
            return s1;

        }
        protected string BuildRequestPostForPOI(Request request)
        {

            string NameValueFormat = "{0}={1}";
            string s1 = "";




            //List<string> aryPostParams = new List<string>();


            ////Create parameters from objProvider settings
            //aryPostParams.Add(string.Format(NameValueFormat, "ePayAccountNum", objProvider.login));
            //aryPostParams.Add(string.Format(NameValueFormat, "password", objProvider.Password));
            //aryPostParams.Add(string.Format(NameValueFormat, "transactionCode", objProvider.transactionKey));


            //Create parameters from request

            //aryPostParams.Add(string.Format(NameValueFormat, "orderNum", request.InvoiceNumber));

            try
            {

                s1 += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                s1 += "<WIRECARD_BXML xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"wirecard.xsd\"> ";
                s1 += "<W_REQUEST>";
                s1 += "<W_JOB>";
                s1 += "<BusinessCaseSignature>" + gatewaySettings.TransactionKey + "</BusinessCaseSignature>";
                s1 += "<JobID>" + request.InvoiceNumber + "</JobID>";
                s1 += "<FNC_FT_AUTHORIZATION>";
                s1 += "<FunctionID>" + request.InvoiceNumber + "</FunctionID>";
                s1 += "<FT_TRANSACTION>";
                s1 += "<TransactionID>" + request.InvoiceNumber + "</TransactionID>";
                s1 += "<PaymentGroupID></PaymentGroupID>";
                s1 += "<EXTERNAL_ACCOUNT>";
                s1 += "<FirstName>" + request.FirstName + "</FirstName>";
                s1 += "<LastName>" + request.LastName + "</LastName>";
                s1 += "<AccountNumber>" + request.CardNumber + "</AccountNumber>";
                s1 += "<AccountType>" + request.BankAcctType + "</AccountType>";
                s1 += "<BankCode>" + request.BankAbaCode + "</BankCode>";
                s1 += "<Country>" + request.Country + "</Country>";
                s1 += "<CheckNumber></CheckNumber>";
                s1 += "<COUNTRY_SPECIFIC>";
                s1 += "<IdentificationNumber></IdentificationNumber>";
                s1 += "</COUNTRY_SPECIFIC>";
                s1 += "</EXTERNAL_ACCOUNT>";
                s1 += "<INTERNAL_ACCOUNT>";
                s1 += "<AccountID></AccountID>";
                s1 += "</INTERNAL_ACCOUNT>";
                s1 += "<Amount minorunits=\"2\">" + request.Amount * 100 + "</Amount>";
                s1 += "<Currency>" + request.CurrencyCode + "</Currency>";
                s1 += "<Usage>" + request.InvoiceNumber + "</Usage>";
                s1 += "<CORPTRUSTCENTER_DATA>";
                s1 += "<ADDRESS>";
                s1 += "<Address1>" + request.Address1 + "</Address1>";
                s1 += "<Address2>" + request.Address2 + "</Address2>";
                s1 += "<City>" + request.City + "</City>";
                s1 += "<ZipCode>" + request.ZipCode + "</ZipCode>";
                s1 += "<State>" + request.State + "</State>";
                s1 += "<Country>" + request.Country + "</Country>";
                s1 += "<Phone>" + request.Phone + "</Phone>";
                s1 += "<Email>" + request.Email + "</Email>";
                s1 += "<IPAddress>" + request.IPAddress + "</IPAddress>";
                s1 += "</ADDRESS>";
                s1 += "<PERSONINFO>";
                s1 += "<DRIVERS_LICENSE>";
                s1 += "<LicenseNumber></LicenseNumber>";
                s1 += "<State></State>";
                s1 += "<Country></Country>";
                s1 += "</DRIVERS_LICENSE>";
                s1 += "<BirthDate></BirthDate>";
                s1 += "<TaxIdentificationNumber></TaxIdentificationNumber>";
                s1 += "</PERSONINFO>";
                s1 += "</CORPTRUSTCENTER_DATA>";
                s1 += "</FT_TRANSACTION>";
                s1 += "</FNC_FT_AUTHORIZATION>";
                s1 += "</W_JOB>";
                s1 += "</W_REQUEST>";
                s1 += "</WIRECARD_BXML>";
            }
            catch (Exception ex)
            {

                throw new GatewayException(string.Format("An exception occured while creating the post parameters: {0}", ex.Message), ex);
            }

            //return string.Join("&", aryPostParams.ToArray());
            return s1;

        }
        protected string BuildRequestPostForCreditCheckScore(Request request)
        {

            string NameValueFormat = "{0}={1}";
            string s1 = "";

            try
            {

                s1 += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                s1 += "<risk-request mode=\"live\" solution=\"risk\" version=\"1.0\">";
                s1 += "<application>";
                s1 += "<entity-id>" + gatewaySettings.TransactionKey + "</entity-id>";
                s1 += "<request-id>" + request.InvoiceNumber + "</request-id>";
                s1 += "</application>";
                s1 += "<verify>";
                s1 += "<first-name>" + request.FirstName + "</first-name>";
                s1 += "<last-name>" + request.LastName + "</last-name>";
                s1 += "<gender-code></gender-code>";
                s1 += "<birth-date></birth-date>";
                s1 += "<customer-id>" + request.InvoiceNumber + "</customer-id>";
                s1 += "<current-address>";
                s1 += "<street-name>" + request.Address1 + "</street-name>";
                s1 += "<street-number>" + request.Address2 + "</street-number>";
                s1 += "<zip-code>" + request.ZipCode + "</zip-code>";
                s1 += "<city>" + request.City + "</city>";
                s1 += "<country>" + request.Country + "</country>";
                s1 += "</current-address>";
                s1 += "</verify>";
                s1 += "</risk-request>";

            }
            catch (Exception ex)
            {

                throw new GatewayException(string.Format("An exception occured while creating the post parameters: {0}", ex.Message), ex);
            }

            //return string.Join("&", aryPostParams.ToArray());
            return s1;

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

            Response response = new Response();
            response.GatewayResponseRaw = GatewayResponse;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GatewayResponse);
            string xPath = "";
            if (request.AdditionalInfo != null)
            {
                if (request.AdditionalInfo.ContainsKey("WireCardRequestType"))
                {
                    switch (request.AdditionalInfo["WireCardRequestType"].ToString().ToUpper())
                    {
                        case "DEBIT":
                            xPath = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_FT_DEBIT/FT_TRANSACTION/PROCESSING_STATUS";
                            break;
                        case "POI":
                            xPath = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_FT_AUTHORIZATION/FT_TRANSACTION/PROCESSING_STATUS";
                            break;
                        default:
                            xPath = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_TRANSACTION/CC_TRANSACTION/PROCESSING_STATUS";
                            break;
                    }
                }
            }
            else
            {
                xPath = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_TRANSACTION/CC_TRANSACTION/PROCESSING_STATUS";
            }
            XmlNode xResult = doc.SelectSingleNode(xPath);
            if (xResult != null)
            {
                switch (xResult["FunctionResult"].InnerText.ToUpper())
                {
                    case "ACK":
                        response.ResponseType = TransactionResponseType.Approved;
                        response.TransactionID = xResult["GuWID"].InnerText;
                        response.AuthCode = xResult["ReferenceID"].InnerText;
                        break;
                    case "NOK":
                        response.ResponseType = TransactionResponseType.Denied;
                        response.TransactionID = xResult["GuWID"].InnerText;
                        response.AuthCode = "";
                        break;
                    case "PENDING":
                        response.ResponseType = TransactionResponseType.Denied;
                        response.TransactionID = xResult["GuWID"].InnerText;
                        response.AuthCode = "";
                        break;
                    default:
                        response.ResponseType = TransactionResponseType.Error;
                        response.TransactionID = "";
                        response.AuthCode = "";
                        break;
                }
            }
            return response;

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
