using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{
    public class Provider
    {
        #region -- Members --
        private string m_Name = string.Empty;
        private string m_ProviderType = string.Empty;
        private XmlAttributeCollection m_Attributes;
        private string m_Login = string.Empty;
        private string m_transactionUrl = string.Empty;
        private bool m_transactionTest = false;
        private string m_transactionKey = string.Empty;
        private string m_version = string.Empty;
        private string m_delimitedCharacter = "|";
        private bool m_relayReseponse = false;
        private bool m_delimitedData = true;
        private string m_marketType = "2";
        private string m_deviceType = string.Empty;
        private string m_Password = string.Empty;
        private string m_ServerIP = "127.0.0.1";
        private string m_MerchantId = "";
        private string m_TerminalId = "";
        private string m_IndustryType = "";
        private string m_Bin = "";
        private string m_partner = "";
        private string m_vendor = "";
        
        private PaymentRequestType m_requestType;

        #endregion

        #region -- Public Properties --

        public string Name
        {
            get { return m_Name; }
        }
        public string Password
        { 
            get { return m_Password; } 
        }
        public string ProviderType
        {
            get { return m_ProviderType; }
        }
        public bool delimitedData
        {
            get { return m_delimitedData; }
        }
        public bool relayResponse
        {
            get { return m_relayReseponse; }
        }
        public string delimitedCharacter
        {
            get { return m_delimitedCharacter; }
        }
        public string version
        {
            get { return m_version; }
        }
        public string transactionKey
        {
            get { return m_transactionKey; }
        }
        public bool transactionTest
        {
            get { return m_transactionTest; }
        }
        public string transactionUrl
        {
            get { return m_transactionUrl; }
        }
        public string login
        {
            get { return m_Login; }
        }
        public string marketType
        {
            get { return m_marketType; }
        }
        public PaymentRequestType requestType
        {
            get { return m_requestType; }
        }
        public string deviceType
        {
            get { return m_deviceType; }
        }
        public string ServerIP
        {
            get { return m_ServerIP; }
        }
        public string MerchantId
        {
            get { return m_MerchantId; }
        }
        public string TerminalId
        {
            get { return m_TerminalId; }
        }
        public string IndustryType
        {
            get { return m_IndustryType; }
        }
        public string Bin
        {
            get { return m_Bin; }
        }
        public XmlAttributeCollection Attributes
        {
            get { return m_Attributes; }
            set { m_Attributes = value; }
        }
        public string partner
        {
            get { return m_partner; }
        }
        public string vendor
        {
            get { return m_vendor; }
        }
        #endregion

        public Provider(XmlAttributeCollection attributes)
        {
            
            m_Attributes = attributes;

            m_Login = attributes["login"].Value;
            m_Name = attributes["name"].Value;
            if (attributes["Partner"] != null)
            {
                m_partner = attributes["Partner"].Value;
            }
            if (attributes["Vendor"] != null)
            {
                m_vendor = attributes["Vendor"].Value;
            }
            if (attributes["type"] != null)
            {
                m_ProviderType = attributes["type"].Value;            
            }
            if (attributes["Password"] != null)
            {
                m_Password = attributes["Password"].Value;
            }
            if (attributes["TerminalId"] != null)
            {
                m_TerminalId = attributes["TerminalId"].Value;
            }
            if (attributes["MerchantId"] != null)
            {
                m_MerchantId = attributes["MerchantId"].Value;
            }
            if (attributes["IndustryType"] != null)
            {
                m_IndustryType = attributes["IndustryType"].Value;
            }
            if (attributes["Bin"] != null)
            {
                m_Bin = attributes["Bin"].Value;
            }
            if (attributes["TransactionUrl"] != null)
            {
                m_transactionUrl = attributes["TransactionUrl"].Value;
            }
            if (attributes["TransactionKey"] != null)
            {
                m_transactionKey = attributes["TransactionKey"].Value;   
            }

            if (attributes["delimitedData"] != null)
                bool.TryParse(attributes["delimitedData"].Value, out m_delimitedData);

            if (attributes["relayReseponse"] != null)
                bool.TryParse(attributes["relayReseponse"].Value, out m_relayReseponse);

            if (attributes["transactionTest"] != null)
                bool.TryParse(attributes["transactionTest"].Value, out m_transactionTest);

            if (attributes["delimitedCharacter"] != null)
                m_delimitedCharacter = attributes["delimitedCharacter"].Value;

            if (attributes["version"] != null)
                m_version = attributes["version"].Value;

            if (attributes["deviceType"] != null)
                m_deviceType = attributes["deviceType"].Value;

            if (attributes["marketType"] != null)
                m_marketType = attributes["marketType"].Value;
      
        }

    }
}
