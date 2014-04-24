using System;
using System.Collections.Generic;
using System.Text;

namespace Com.ConversionSystems
{
    class OrderOld : Com.ConversionSystems.UI.BasePage
    {
        private string _strOrderId = "";
        private string _strOrderDate = "";
        private string _strOrderTime = "";        
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
        private string _strShipToCounty = "";
        private decimal _strShippingCost = 0;
        private decimal _strTax = 0;
        private decimal _strSubTotal = 0;
        private decimal _strTotal = 0;
        private string _strAuthCode = "";
        private string _strPermissionToEmail = "N";
        private string _strPermissionToCall = "N";
        private string _strAmountProcessedOnline = "";
        private string _strNoOfPaymentRequested = "";
        private string _strPaymentAuthorized = "N";
        private string _strPaymentSettled = "N";
        private string _strBillingTitle = "";
        private string _strShippingTitle = "";
        private string _strProductSku = "";
        private string _strProductSkuAmount = "";
        private string _strLandLine = "";
        private string _strShippingLandLine = "";
        private string _strCounty = "";
        private string _strMobilePhone = "";




        
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
        public string OrderTime
        {
            get { return _strOrderTime; }
            set { _strOrderTime = value; }
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
        public string ShipToCounty
        {
            get { return _strShipToCounty; }
            set { _strShipToCounty = value; }
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
        public decimal ShippingCost
        {
            get { return _strShippingCost; }
            set { _strShippingCost = value; }
        }
        public decimal Tax
        {
            get { return _strTax; }
            set { _strTax = value; }
        }
        public decimal SubTotal
        {
            get { return _strSubTotal; }
            set { _strSubTotal = value; }
        }
        public decimal Total
        {
            get { return _strTotal; }
            set { _strTotal = value; }
        }
        public string AuthCode
        {
            get { return _strAuthCode; }
            set { _strAuthCode = value; }
        }
        public string PermissionToEmail
        {
            get { return _strPermissionToEmail; }
            set { _strPermissionToEmail = value; }
        }
        public string PermissionToCall
        {
            get { return _strPermissionToCall; }
            set { _strPermissionToCall = value; }
        }
        public string AmountProcessedOnline
        {
            get { return _strAmountProcessedOnline; }
            set { _strAmountProcessedOnline = value; }
        }
        public string NoOfPaymentRequested
        {
            get { return _strNoOfPaymentRequested; }
            set { _strNoOfPaymentRequested = value; }
        }
        public string PaymentAuthorized
        {
            get { return _strPaymentAuthorized; }
            set { _strPaymentAuthorized = value; }
        }
        public string PaymentSettled
        {
            get { return _strPaymentSettled; }
            set { _strPaymentSettled = value; }
        }
        public string BillingTitle
        {
            get { return _strBillingTitle; }
            set { _strBillingTitle = value; }
        }
        public string ShippingTitle
        {
            get { return _strShippingTitle; }
            set { _strShippingTitle = value; }
        }
        public string ProductSku
        {
            get { return _strProductSku; }
            set { _strProductSku = value; }
        }
        public string ProductSkuAmount
        {
            get { return _strProductSkuAmount; }
            set { _strProductSkuAmount = value; }
        }
        public string LandLine
        {
            get { return _strLandLine; }
            set { _strLandLine = value; }
        }
        public string ShipToLandLine
        {
            get { return _strShippingLandLine; }
            set { _strShippingLandLine = value; }
        }
        public string County
        {
            get { return _strCounty; }
            set { _strCounty = value; }
        }
        public string MobilePhone
        {
            get { return _strMobilePhone; }
            set { _strMobilePhone = value; }
        }









        public void clear()
        {
             
        _strOrderId = "";
        _strOrderDate = "";
        _strOrderTime = "";        
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
        _strCallBackFlag = "N";
        _strShippingMethod = "s";
        _strFreightAmount = "";
        _strShipToCountry = "";
        _strShipToCounty = "";
        _strShippingCost = 0;
        _strTax = 0;
        _strSubTotal = 0;
        _strTotal = 0;
        _strAuthCode = "";
        _strPermissionToEmail = "N";
        _strPermissionToCall = "N";
        _strAmountProcessedOnline = "";
        _strNoOfPaymentRequested = "";
        _strPaymentAuthorized = "N";
        _strPaymentSettled = "N";
        _strBillingTitle = "";
        _strShippingTitle = "";
        _strProductSku = "";
        _strProductSkuAmount = "";
        _strLandLine = "";
        _strShippingLandLine = "";
        _strCounty = "";
        _strMobilePhone = "";


        }
    }
}
