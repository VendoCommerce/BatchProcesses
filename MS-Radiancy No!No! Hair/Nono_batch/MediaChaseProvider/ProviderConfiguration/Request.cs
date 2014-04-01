using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ConversionSystems.Providers.MediaChase.OrderProcessing
{
    public class Request
    {

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

        private DataTable m_Orders;
        #endregion   

        #region Public Properties

        public string ID
        {
            set { m_ID = value; }
            get { return m_ID; }
        }
        public string Password
        {
            set { m_Password = value; }
            get { return m_Password; }
        }
        public string Username
        {
            set { m_Username = value; }
            get { return m_Username; }
        }
        public string ConnectionString
        {
            set { m_ConnectionString = value; }
            get { return m_ConnectionString; }
        }
        public string ProviderUrl
        {
            get { return m_ProviderUrl; }
            set { m_ProviderUrl = value; }
        }
        public DataTable Orders
        {
            get { return m_Orders; }
            set { m_Orders = value; }
        }
        public string CompanyNumber
        {
            set { m_CompanyNumber = value; }
            get { return m_CompanyNumber; }
        }
        public string ProjectNumber
        {
            set { m_ProjectNumber = value; }
            get { return m_ProjectNumber; }
        }
        public string SourceCode
        {
            set { m_SourceCode = value; }
            get { return m_SourceCode; }
        }
        public string TrackingCode
        {
            set { m_TrackingCode = value; }
            get { return m_TrackingCode; }
        }
        public string ShippingMethod
        {
            set { m_ShippingMethod = value; }
            get { return m_ShippingMethod; }

        }
        public string MediaCode
        {
            set { m_MediaCode = value; }
            get { return m_MediaCode; }

        }
       

        #endregion

    }
}
