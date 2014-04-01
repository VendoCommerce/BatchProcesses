using System;
using System.Collections.Generic;
using System.Text;

namespace ConversionSystems.Providers.MediaChase.OrderProcessing
{
    public class Response : Object
    {
        #region Private Members
    
        private string m_Description;
        private string m_ExternalNumber;
        private DateTime m_ResponseDate;

        private int x, y;
        private TransactionResponse m_TransactionResponse;
        #endregion

        #region -- Public Overrides --

        public override int GetHashCode()
        {
            return x ^ y;
        }
        #endregion

        #region Public Properties


        public TransactionResponse ResponseType
        {
            get { return m_TransactionResponse; }
            set { m_TransactionResponse = value; }
        }

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        public DateTime ResponseDate
        {
            get { return m_ResponseDate; }
            set { m_ResponseDate = value; }
        }
        public string ExternalNumber
        {
            get { return m_ExternalNumber; }
            set { m_ExternalNumber = value; }
        }
        #endregion
    }



    public enum TransactionResponse 
    {
        Success,
        Failure,
        Unknown,
        CommunicationError,
        InvalidRequest,
        MissingRequiredFields
    }

}
      