using System;
using System.Collections.Generic;
using System.Text;

namespace Com.ConversionSystems.GoldCanyon
{
    class Order : Com.ConversionSystems.UI.BasePage
    {
        private string _strSourceOrder = "CSR";
        private string _strSourceCode = "";
        private string _str800Number = "";
        private string _strFiller = "";
        private string _strOrderId = "";
        private string _strOrderDate = "";        
        private string _strShipToLastName = "";
        private string _strShipToFirstName = "";
        private string _strShipToPhone = "";
        private string _strShipToAddress1 = "";
        private string _strShipToAddress2 = "";
        private string _strShipToAddress3 = "";
        private string _strShipToCity = "";
        private string _strShipToState = "";
        private string _strShipToPostalCode = "";        
        private string _strItem = "";
        private string _strQuantity = "";
        private string _strCreditCardNumber = "";
        private string _strCreditCardExpiration = "";
        private string _strBillingAddress1 = "";
        private string _strBillingCity = "";
        private string _strBillingState = "";
        private string _strBillingPostalCode = "";
        private string _strCVV = "";
        private string _strFirstName = "";
        private string _strLastName = "";
        private string _strAddress1 = "";
        private string _strAddress2 = "";
        private string _strAddress3 = "";
        private string _strCity = "";
        private string _strState= "";
        private string _strPostalCode = "";
        private string _strCountry = "";
        private string _strEmail = "";
        private string _strPhone = "";
        private string _strBillingCountry = "";
        private string _strCreditCardType = "";
        private string _strCallBackFlag = "N";
        private string _strShippingMethod = "s";
        private string _strFreightAmount = "";
        private string _strShipToCountry = "";


        public string SourceOrder
        {
            get { return _strSourceOrder; }
            set { _strSourceOrder = value; }
        }
        public string SourceCode
        {
            get { return _strSourceCode; }
            set { _strSourceCode = value; }
        }
        public string Number800
        {
            get { return _str800Number; }
            set { _str800Number = value; }
        }
        public string CallBackFlag
        {
            get { return _strCallBackFlag; }
            set { _strCallBackFlag = value; }
        }
        public string ShippingMethod
        {
            get { return _strShippingMethod; }
            set { _strShippingMethod = value; }
        }
        public string FreightAmount
        {
            get { return _strFreightAmount; }
            set { _strFreightAmount = value; }
        }
        public string Filler
        {
            get { return _strFiller; }
            set { _strFiller = value; }
        }
        public string OrderId
        {
            get { return _strOrderId; }
            set { _strOrderId = value; }
        }
        public string OrderDate
        {
            get { return _strOrderDate; }
            set { _strOrderDate = value; }
        }
        public string ShipToLastName
        {
            get { return _strShipToLastName; }
            set { _strShipToLastName = value; }
        }
        public string ShipToFirstName
        {
            get { return _strShipToFirstName; }
            set { _strShipToFirstName = value; }
        }
        public string ShipToPhone
        {
            get { return _strShipToPhone; }
            set { _strShipToPhone = value; }
        }
        public string ShipToAddress1
        {
            get { return _strShipToAddress1; }
            set { _strShipToAddress1 = value; }
        }
        public string ShipToAddress2
        {
            get { return _strShipToAddress2; }
            set { _strShipToAddress2 = value; }
        }
        public string ShipToAddress3
        {
            get { return _strShipToAddress3; }
            set { _strShipToAddress3 = value; }
        }
        public string ShipToCity
        {
            get { return _strShipToCity; }
            set { _strShipToCity = value; }
        }
        public string ShipToState
        {
            get { return _strShipToState; }
            set { _strShipToState = value; }
        }
        public string ShipToPostalCode
        {
            get { return _strShipToPostalCode; }
            set { _strShipToPostalCode = value; }
        }
        public string ShipToCountry
        {
            get { return _strShipToCountry; }
            set { _strShipToCountry = value; }
        }
        public string Item
        {
            get { return _strItem; }
            set { _strItem = value; }
        }
        public string Quantity
        {
            get { return _strQuantity; }
            set { _strQuantity = value; }
        }
        public string CreditCardNumber
        {
            get { return _strCreditCardNumber; }
            set { _strCreditCardNumber = value; }
        }
        public string CreditCardType
        {
            get { return _strCreditCardType; }
            set { _strCreditCardType = value; }
        }
        public string CreditCardExpiration
        {
            get { return _strCreditCardExpiration; }
            set { _strCreditCardExpiration = value; }
        }
        public string BillingAddress1
        {
            get { return _strBillingAddress1; }
            set { _strBillingAddress1 = value; }
        }
        public string BillingCity
        {
            get { return _strBillingCity; }
            set { _strBillingCity = value; }
        }
        public string BillingState
        {
            get { return _strBillingState; }
            set { _strBillingState = value; }
        }
        public string BillingPostalCode
        {
            get { return _strBillingPostalCode; }
            set { _strBillingPostalCode = value; }
        }
        public string BillingCountry
        {
            get { return _strBillingCountry; }
            set { _strBillingCountry = value; }
        }
        public string CVV
        {
            get { return _strCVV; }
            set { _strCVV = value; }
        }
        public string FirstName
        {
            get { return _strFirstName; }
            set { _strFirstName = value; }
        }
        public string LastName
        {
            get { return _strLastName; }
            set { _strLastName = value; }
        }
        public string Address1
        {
            get { return _strAddress1; }
            set { _strAddress1 = value; }
        }
        public string Address2
        {
            get { return _strAddress2; }
            set { _strAddress2 = value; }
        }
        public string Address3
        {
            get { return _strAddress3; }
            set { _strAddress3 = value; }
        }
        public string City
        {
            get { return _strCity; }
            set { _strCity = value; }
        }
        public string State
        {
            get { return _strState; }
            set { _strState = value; }
        }
        public string PostalCode
        {
            get { return _strPostalCode; }
            set { _strPostalCode = value; }
        }
        public string Email
        {
            get { return _strEmail; }
            set { _strEmail = value; }
        }
        public string Phone
        {
            get { return _strPhone; }
            set { _strPhone = value; }
        }
        public string Country
        {
            get { return _strCountry; }
            set { _strCountry = value; }
        }

        public void clear()
        {
             _strSourceOrder = "CSA";
         _strSourceCode = "";
         _str800Number = "";
         _strFiller = "";
         _strOrderId = "";
         _strOrderDate = "";        
         _strShipToLastName = "";
         _strShipToFirstName = "";
         _strShipToPhone = "";
         _strShipToAddress1 = "";
         _strShipToAddress2 = "";
         _strShipToAddress3 = "";
         _strShipToCity = "";
         _strShipToState = "";
         _strShipToPostalCode = "";        
         _strItem = "";
         _strQuantity = "";
         _strCreditCardNumber = "";
         _strCreditCardExpiration = "";
         _strBillingAddress1 = "";
         _strBillingCity = "";
         _strBillingState = "";
         _strBillingPostalCode = "";
         _strCVV = "";
         _strFirstName = "";
         _strLastName = "";
         _strAddress1 = "";
         _strAddress2 = "";
         _strAddress3 = "";
         _strCity = "";
         _strState= "";
         _strPostalCode = "";
         _strCountry = "";
         _strEmail = "";
         _strPhone = "";
         _strBillingCountry = "";
         _strCreditCardType = "";
         _strCallBackFlag = "";
         _strShippingMethod = "s";
         _strFreightAmount = "";
         _strShipToCountry = "";

        }
    }
}
