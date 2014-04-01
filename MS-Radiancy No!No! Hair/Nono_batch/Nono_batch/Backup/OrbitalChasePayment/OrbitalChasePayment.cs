using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using EBS.IntegrationServices.Providers.PaymentProviders;
using EBS.IntegrationServices.Providers.PaymentProviders.OrbitalChasePayment.net.paymentech.wsvar;


namespace EBS.IntegrationServices.Providers.PaymentProviders.OrbitalChasePayment
{
    public class OrbitalChasePayment : GatewayProvider
    {
        
        #region Private Members
        private static string m_ProviderSection = "PaymentProvider";
        private static ProviderConfiguration m_ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(m_ProviderSection);
        public static Provider objProvider = null;
        private static GatewaySettings gatewaySettings = null;
        private static string errRequiredNode = "A required gateway node does not exist: {0}";

        #endregion

        #region Public Methods

        public OrbitalChasePayment()
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
                gatewaySettings.Login = objProvider.login;
                gatewaySettings.Version = objProvider.version;
                gatewaySettings.MerchantId = objProvider.MerchantId;
                gatewaySettings.TerminalId = objProvider.TerminalId;
                gatewaySettings.IndustryType = objProvider.IndustryType;
                gatewaySettings.Bin = objProvider.Bin;
                gatewaySettings.TransactionURL = objProvider.transactionUrl;
                gatewaySettings.TransactionKey = objProvider.Password;
                
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
        
        public override Response PerformRequest(Request request)
        {

            Response response = new Response();
            if (gatewaySettings == null)
            {
                //Load settings if they have not been loaded
                OrbitalChasePayment.GetSettings();
            }
            PaymentechGateway server = new PaymentechGateway();
            server.Url = objProvider.transactionUrl;
            NewOrderRequestElement authBean = new NewOrderRequestElement();
            //Default values coming from config file.
            authBean.merchantID = objProvider.MerchantId;
            authBean.terminalID = objProvider.TerminalId;
            authBean.industryType = objProvider.IndustryType;
            authBean.bin = objProvider.Bin;
            
            //Specific values per customer.
            authBean.orderID = request.InvoiceNumber;
            authBean.transType = request.RequestType.ToString();
            authBean.amount = (request.Amount * 100).ToString();
            authBean.ccAccountNum = request.CardNumber ;
            authBean.ccExp = request.ExpireDate;
            authBean.ccCardVerifyNum = request.CardCvv;
            authBean.avsName = request.FirstName + " " + request.LastName;
            authBean.avsAddress1 = request.Address1;
            authBean.avsCity = request.City;
            authBean.avsZip = request.ZipCode;
            authBean.avsCountryCode = request.Country;
            authBean.customerPhone = request.Phone;
            authBean.customerName = request.FirstName + " " + request.LastName;
            authBean.customerEmail = request.Email;
            
            
            try
            {
                NewOrderResponseElement responseBean =
                server.NewOrder(authBean);
                Console.WriteLine("ProcStatus: " +
                responseBean.procStatus);
                Console.WriteLine("ApprovalStatus: " +
                responseBean.approvalStatus);

                if (responseBean.respCode.Equals("00") && responseBean.approvalStatus.Equals("1"))
                {
                    response.ResponseType = TransactionResponseType.Approved;
                }
                else if(responseBean.approvalStatus.Equals("0"))
                {
                    response.ResponseType = TransactionResponseType.Denied;
                }
                else
                {
                    response.ResponseType = TransactionResponseType.Error;
                }
                response.ReasonText = responseBean.respCodeMessage;
                response.TransactionID = responseBean.txRefNum;
                response.AuthCode = responseBean.authorizationCode;
               response.AvsResponse = TransactionAvsResponse.Match;
               
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                Console.WriteLine(ex.Message);
                response.ResponseType = TransactionResponseType.Error;

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

            string NameValueFormat = "{0}={1}";

            List<string> aryPostParams = new List<string>();
                    

            //Create parameters from objProvider settings
            aryPostParams.Add(string.Format(NameValueFormat, "ePayAccountNum", objProvider.login));
            aryPostParams.Add(string.Format(NameValueFormat, "password", objProvider.Password));
            aryPostParams.Add(string.Format(NameValueFormat, "transactionCode", objProvider.transactionKey));


            //Create parameters from request

            aryPostParams.Add(string.Format(NameValueFormat, "orderNum", request.InvoiceNumber));

            try
            {

                if ((request.RequestType == PaymentRequestType.FC || request.RequestType == PaymentRequestType.R ) && request.TransactionID.Trim().Length == 0)
                {
                    throw new GatewayException("This request type requires a valid transaction id");
                }


               
                if (request.Amount > 0)
                {
                    aryPostParams.Add(string.Format("transactionAmount={0:.00}", request.Amount));
                }
                //load optional params
                aryPostParams.Add(string.Format(NameValueFormat, "CVV2", request.CardCvv));
                aryPostParams.Add(string.Format("expirationDate={0:####}", request.ExpireDate));
                aryPostParams.Add(string.Format(NameValueFormat, "cardAccountNum", request.CardNumber));
                aryPostParams.Add(string.Format(NameValueFormat, "InstallmentNum", "01"));
                aryPostParams.Add(string.Format(NameValueFormat, "InstallmentOf", "02"));

                if (request.CustomerID.Trim().Length > 0) aryPostParams.Add(string.Format(NameValueFormat, "CustomerNum", request.CustomerID));
                if (request.FirstName.Trim().Length > 0) aryPostParams.Add(string.Format(NameValueFormat, "CardHolderName", request.FirstName + "+" + request.LastName));
                if (request.Address1.Trim().Length > 0) aryPostParams.Add(string.Format(NameValueFormat, "CardHolderAddress", request.Address1 + (string.IsNullOrEmpty(request.Address2) == false ? "+" + request.Address2 : string.Empty)));
                if (request.City.Trim().Length > 0) aryPostParams.Add(string.Format(NameValueFormat, "CardHolderCity", request.City));
                if (request.State.Trim().Length > 0) aryPostParams.Add(string.Format(NameValueFormat, "CardHolderState", request.State));
                if (request.ZipCode.Trim().Length > 0) aryPostParams.Add(string.Format(NameValueFormat, "CardHolderZip", request.ZipCode));

                bool testMode = false;
                bool.TryParse(objProvider.transactionTest.ToString(), out testMode);
                aryPostParams.Add(string.Format(NameValueFormat, "testTransaction", (testMode == true ? "Y" : "N")));


            }
            catch (Exception ex)
            {

                throw new GatewayException(string.Format("An exception occured while creating the post parameters: {0}", ex.Message), ex);
            }

            return string.Join("&", aryPostParams.ToArray());

        }

        private Response ParseResponse(string GatewayResponse)
        {

            Response response = new Response();

            response.GatewayResponseRaw = GatewayResponse;

            string[] aryGatewayResponse = GatewayResponse.Split(Convert.ToChar(gatewaySettings.DelimChar));


            if (aryGatewayResponse.Length > 0)
            {
                switch (aryGatewayResponse[10])
                {
                    case "00":
                    case "000":
     
                        //Approved
                        response.ResponseType = TransactionResponseType.Approved;
                        break;
                    case "15":
                        //Declined
                        response.ResponseType = TransactionResponseType.Denied;
                        break;
                    default:
                        //Error
                        response.ResponseType = TransactionResponseType.Error;
                        break;
                }
               

                response.ReasonText = aryGatewayResponse[3];
                response.TransactionID = aryGatewayResponse[14];
                response.AuthCode = aryGatewayResponse[13];
                if (aryGatewayResponse[22].ToLower().Equals("u"))
                {
                    response.AvsResponse = TransactionAvsResponse.NoMatch;
                }
                else
                {
                    response.AvsResponse = TransactionAvsResponse.Match;
                }
                
               

            }
            else
            {
                response.ResponseType = TransactionResponseType.Error;
                response.ReasonText = "Unknown Error (" + aryGatewayResponse + ")";
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
            private string m_MerchantId = "";
            private string m_TerminalId = "";
            private string m_IndustryType = "";
            private string m_Bin = "";
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
            public string MerchantId
            {
                get { return m_MerchantId; }
                set { m_MerchantId = value; }
            }
            public string TerminalId
            {
                get { return m_TerminalId; }
                set { m_TerminalId = value; }
            }
            public string IndustryType
            {
                get { return m_IndustryType; }
                set { m_IndustryType = value; }
            }
            public string Bin
            {
                get { return m_Bin; }
                set { m_Bin = value; }
            }
            #endregion

        }

    }
}
