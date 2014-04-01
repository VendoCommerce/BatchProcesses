using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace ConversionSystems.Providers.MediaChase.OrderProcessing
{
    public class Provider
    {

        private string m_Name = string.Empty;
        private string m_ProviderType = string.Empty;
        private string m_ProviderLicenseKey = string.Empty;
        private XmlAttributeCollection m_Attributes;

        #region Private Members
        private string m_ID;
        private string m_Password;
        private string m_Username;
        private string m_ProviderUrl;
        private string m_ConnectionString;

        private string m_CompanyNumber;
        private string m_ProjectNumber;
        private string m_SourceCode;
        private string m_TrackingCode;
        private string m_ShippingMethod;
        private string m_MediaCode;
        private string m_EnableAlerts;
        private string m_AlertNotificationEmail;
        private string m_CopyPath;
        private string m_Mode;
        private string m_SearchPath;
        #endregion

        #region Public Properties
        public string EnableAlerts
        {
            get { return (m_EnableAlerts ?? "false"); }
        }
        public string AlertNotificationEmail
        {
            get { return (m_AlertNotificationEmail ?? ""); }
        }
        public string MediaCode
        {
            get { return m_MediaCode; }

        }
        public string CopyPath
        {
            get { return m_CopyPath; }
        }
        public string Mode
        {
            get { return m_Mode; }
        }
        public string SearchPath
        {
            get { return m_SearchPath; }
        }
        public string CompanyNumber
        {

            get { return m_CompanyNumber; }
        }
        public string ProjectNumber
        {

            get { return m_ProjectNumber; }
        }
        public string SourceCode
        {

            get { return m_SourceCode; }
        }
        public string TrackingCode
        {

            get { return m_TrackingCode; }
        }
        public string ShippingMethod
        {
            get { return m_ShippingMethod; }

        }

        public string ID
        {

            get { return m_ID; }
        }
        public string Password
        {

            get { return m_Password; }
        }
        public string Username
        {

            get { return m_Username; }
        }
        public string ConnectionString
        {

            get { return m_ConnectionString; }
        }
        public string ProviderUrl
        {
            get { return m_ProviderUrl; }

        }


        #endregion

        public string Name
        {
            get { return m_Name; }
        }

        public string ProviderType
        {
            get { return m_ProviderType; }
        }

  
        public XmlAttributeCollection Attributes
        {
            get { return m_Attributes; }
            set { m_Attributes = value; }
        }

        public Provider(XmlAttributeCollection attributes)
        {
            
            m_Attributes = attributes;

            m_Name = attributes["name"].Value;
            m_ProviderType = attributes["type"].Value;
            m_ID = attributes["ID"].Value;
            m_Username = attributes["Username"].Value;
            m_Password = attributes["Password"].Value;
            m_ProviderUrl = attributes["ProviderUrl"].Value;
            m_ConnectionString = attributes["ConnectionString"].Value;
            m_ProjectNumber = attributes["ProjectNumber"].Value;
            m_CompanyNumber = attributes["CompanyNumber"].Value;
            m_SourceCode = attributes["SourceCode"].Value;
            m_ShippingMethod = attributes["ShippingMethod"].Value;
            m_CompanyNumber = attributes["CompanyNumber"].Value;
            m_MediaCode  = attributes["MediaCode"].Value;
            m_TrackingCode = attributes["TrackingCode"].Value;
            m_TrackingCode = attributes["TrackingCode"].Value;
            m_CopyPath = attributes["CopyPath"].Value;
            m_SearchPath = attributes["SearchPath"].Value;
            m_Mode = (attributes["Mode"] != null ? attributes["Mode"].Value : "TEST");
            m_AlertNotificationEmail = (attributes["AlertNotificationEmail"] != null ? attributes["AlertNotificationEmail"].Value : string.Empty);
            m_EnableAlerts = (attributes["EnableAlerts"] != null ? attributes["EnableAlerts"].Value : string.Empty);
        }

    }
}
