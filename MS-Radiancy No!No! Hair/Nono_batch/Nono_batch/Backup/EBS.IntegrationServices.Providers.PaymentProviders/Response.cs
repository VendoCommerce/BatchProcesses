using System;
using System.Collections.Generic;
using System.Text;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{
    public class Response
    {

        private string m_GatewayResponseRaw;
        private string m_TransactionID;
        private TransactionResponseType m_ResponseType;
        private TransactionCvvResponse m_CvvResponse;
        private TransactionAvsResponse m_AvsResponse;
        private string m_TimeStamp;
        private string m_ReasonText;
        private string m_AuthCode;
        private string m_MerchantDefined1 = "";
        private string m_MerchantDefined2 = "";
        public TransactionAvsResponse AvsResponse
        {
            get { return m_AvsResponse; }
            set { m_AvsResponse = value; }
        }

        public string GatewayResponseRaw
        {
            get { return m_GatewayResponseRaw; }
            set { m_GatewayResponseRaw = value; }
        }
        public string TimeStamp
        {
            get { return m_TimeStamp; }
            set { m_TimeStamp = value; }
        }
        public string TransactionID
        {
            get { return m_TransactionID; }
            set { m_TransactionID = value; }
        }
        public string AuthCode
        {
            get { return m_AuthCode; }
            set { m_AuthCode = value; }
        }
        public string MerchantDefined1
        {
            get { return m_MerchantDefined1; }
            set { m_MerchantDefined1 = value; }
        }
        public string MerchantDefined2
        {
            get { return m_MerchantDefined2; }
            set { m_MerchantDefined2 = value; }
        }

        public TransactionResponseType ResponseType
        {
            get { return m_ResponseType; }
            set { m_ResponseType = value; }
        }

        public TransactionCvvResponse CvvResponse
        {
            get { return m_CvvResponse; }
            set { m_CvvResponse = value; }
        }

        public string ReasonText
        {
            get { return m_ReasonText; }
            set { m_ReasonText = value; }
        }
                
    }

    public enum TransactionResponseType
    {
        Approved,
        Denied,
        Error
    }

    public enum TransactionAvsResponse {
        Match,
        NoMatch,
        NA,
        Error
    }

    public enum TransactionCvvResponse
    {
        Match,
        NoMatch,
        NA,
        Error
    }

}
