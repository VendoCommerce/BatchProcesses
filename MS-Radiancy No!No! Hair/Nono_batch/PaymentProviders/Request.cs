using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{
    public class Request
    {

        #region Private Members

        private string m_RequestID = string.Empty;
        private PaymentRequestType m_RequestType = PaymentRequestType.A;
        private PaymentMethodType m_MethodType;
        private CreditCardType m_CardType;
        private double m_Amount;
        private string m_CardNumber = string.Empty;
        private string m_ExpireDate = string.Empty;
        private string m_FirstName = string.Empty;
        private string m_LastName = string.Empty;
        private string m_Address1 = string.Empty;
        private string m_Address2 = string.Empty;
        private string m_City = string.Empty;
        private string m_State = string.Empty;
        private string m_ZipCode = string.Empty;
        private string m_CardCvv = string.Empty;
        private string m_Country = string.Empty;
        private string m_Phone = string.Empty;
        private string m_Fax = string.Empty;
        private string m_CustomerID = string.Empty;
        private string m_IPAddress = string.Empty;
        private string m_CustomerTaxID = string.Empty;
        private string m_Email = string.Empty;
        private string m_InvoiceNumber = string.Empty;
        private string m_TransactionDescription = string.Empty;
        private string m_ShipToFirstName = string.Empty;
        private string m_ShipToLastName = string.Empty;
        private string m_ShipToCompany = string.Empty;
        private string m_ShipToAddress = string.Empty;
        private string m_ShipToCity = string.Empty;
        private string m_ShipToState = string.Empty;
        private string m_ShipToZipCode = string.Empty;
        private string m_ShipToCountry = string.Empty;
        private string m_CurrencyCode = string.Empty;
        private string m_BankAbaCode = string.Empty;
        private string m_BankAcctNumber = string.Empty;
        private string m_BankAcctType = string.Empty;
        private string m_BankName = string.Empty;
        private string m_BankAcctName = string.Empty;
        private string m_ECheckType = string.Empty;
        private string m_TransactionID = string.Empty;
        private string m_AuthCode = string.Empty;
        private string m_PoNumber = string.Empty;
        private string m_Tax = string.Empty;
        private string m_TaxExempt = string.Empty;
        private string m_Freight = string.Empty;
        private string m_Duty = string.Empty;
        private string m_CompanyName = string.Empty;
        private Hashtable m_AdditionalInfo = new Hashtable();
        private string m_Method = string.Empty;
        private Hashtable m_RequestData = new Hashtable();
        private string m_ReturnUrl = string.Empty;
        private string m_CancelUrl = string.Empty;
        private string m_PaymentAction = string.Empty;
        private string m_Token = string.Empty;
	
        #endregion   

        #region Public Properties

        public CreditCardType CardType
        {
            get { return m_CardType; }
            set { m_CardType = value; }
        }

        public string CardCvv {
            get {return m_CardCvv;}
            set {m_CardCvv = value;}
        }

        public string ZipCode {
            get {return m_ZipCode;}
            set {m_ZipCode = value;}
        }

        public string State {
            get {return m_State;}
            set {m_State = value;}
        }

        public string City {
            get {return m_City;}
            set {m_City = value;}
        }

        public string Address2 {
            get {return m_Address2;}
            set {m_Address2 = value;}
        }

        public string CompanyName
        {
            get { return m_CompanyName; }
            set { m_CompanyName = value; }
        }

        public string Duty
        {
            get { return m_Duty; }
            set { m_Duty = value; }
        }

        public string Freight
        {
            get { return m_Freight; }
            set { m_Freight = value; }
        }

        public string TaxExempt
        {
            get { return m_TaxExempt; }
            set { m_TaxExempt = value; }
        }
        
        public string Tax
        {
            get { return m_Tax; }
            set { m_Tax = value; }
        }

        public string PoNumber
        {
            get { return m_PoNumber; }
            set { m_PoNumber = value; }
        }

        public string AuthCode
        {
            get { return m_AuthCode; }
            set { m_AuthCode = value; }
        }
        
        public string TransactionID
        {
            get { return m_TransactionID; }
            set { m_TransactionID = value; }
        }

        public string ECheckType
        {
            get { return m_ECheckType; }
            set { m_ECheckType = value; }
        }

        public string BankAcctName
        {
            get { return m_BankAcctName; }
            set { m_BankAcctName = value; }
        }

        public string BankName
        {
            get { return m_BankName; }
            set { m_BankName = value; }
        }

        public string BankAcctType
        {
            get { return m_BankAcctType; }
            set { m_BankAcctType = value; }
        }

        public string BankAcctNumber
        {
            get { return m_BankAcctNumber; }
            set { m_BankAcctNumber = value; }
        }

        public string BankAbaCode
        {
            get { return m_BankAbaCode; }
            set { m_BankAbaCode = value; }
        }

        public string CurrencyCode
        {
            get { return m_CurrencyCode; }
            set { m_CurrencyCode = value; }
        }


        public string ShipToCountry
        {
            get { return m_ShipToCountry; }
            set { m_ShipToCountry = value; }
        }

        public string ShipToZipCode
        {
            get { return m_ShipToZipCode; }
            set { m_ShipToZipCode = value; }
        }

        public string ShipToState
        {
            get { return m_ShipToState; }
            set { m_ShipToState = value; }
        }

        public string ShipToCity
        {
            get { return m_ShipToCity; }
            set { m_ShipToCity = value; }
        }

        public string ShipToAddress
        {
            get { return m_ShipToAddress; }
            set { m_ShipToAddress = value; }
        }

        public string ShipToCompany
        {
            get { return m_ShipToCompany; }
            set { m_ShipToCompany = value; }
        }

        public string ShipToLastName
        {
            get { return m_ShipToLastName; }
            set { m_ShipToLastName = value; }
        }

        public string ShipToFirstName
        {
            get { return m_ShipToFirstName; }
            set { m_ShipToFirstName = value; }
        }


        public string TransactionDescription
        {
            get { return m_TransactionDescription; }
            set { m_TransactionDescription = value; }
        }

        public string InvoiceNumber
        {
            get { return m_InvoiceNumber; }
            set { m_InvoiceNumber = value; }
        }


        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }


        public string CustomerTaxID
        {
            get { return m_CustomerTaxID; }
            set { m_CustomerTaxID = value; }
        }


        public string IPAddress
        {
            get { return m_IPAddress; }
            set { m_IPAddress = value; }
        }


        public string CustomerID
        {
            get { return m_CustomerID; }
            set { m_CustomerID = value; }
        }


        public string Fax
        {
            get { return m_Fax; }
            set { m_Fax = value; }
        }

        public string Phone
        {
            get { return m_Phone; }
            set { m_Phone = value; }
        }

        public string Country
        {
            get { return m_Country; }
            set { m_Country = value; }
        }

        public string LastName
        {
            get { return m_LastName; }
            set { m_LastName = value; }
        }
	
        public string FirstName
        {
            get { return m_FirstName; }
            set { m_FirstName = value; }
        }

        public string Address1
        {
            get { return m_Address1; }
            set { m_Address1 = value; }
        }

        public string RequestID
        {
            get { return m_RequestID; }
            set { m_RequestID = value; }
        }

        public PaymentRequestType RequestType
        {
            get { return m_RequestType; }
            set { m_RequestType = value; }
        }

        public PaymentMethodType MethodType
        {
            get { return m_MethodType; }
            set { m_MethodType = value; }
        }

        public double Amount
        {
            get { return m_Amount; }
            set { m_Amount = value; }
        }

        public string CardNumber
        {
            get { return m_CardNumber; }
            set { m_CardNumber = value; }
        }

        public string ExpireDate
        {
            get { return m_ExpireDate; }
            set { m_ExpireDate = value; }
        }

        public Hashtable AdditionalInfo
        {
            get { return m_AdditionalInfo; }
            set { m_AdditionalInfo = value; }
        }

        public string Method
        {
            get { return m_Method; }
            set { m_Method = value; }
        }

        public Hashtable RequestData
        {
            get { return m_RequestData; }
            set { m_RequestData = value; }
        }
        
        public string ReturnUrl
        {
            get { return m_ReturnUrl; }
            set { m_ReturnUrl = value; }
        }
        public string CancelUrl
        {
            get { return m_CancelUrl; }
            set { m_CancelUrl = value; }
        }
        public string PaymentAction
        {
            get { return m_PaymentAction; }
            set { m_PaymentAction = value; }
        }
        public string Token
        {
            get { return m_Token; }
            set { m_Token = value; }
        }
        
        #endregion

    }

    public enum CreditCardType
    {
        Visa,
        Mastercard,
        AmericanExpress,
        Discover,
        AW
    }

    public enum PaymentMethodType
    {
        CreditCard,
        Check
    }

    public enum PaymentRequestType
    {
        //A Authorization request, AC Authorization and Mark for Capture, FC Force-Capture request, R Refund request
        A,
        AC,
        FC,
        R
    }

}
