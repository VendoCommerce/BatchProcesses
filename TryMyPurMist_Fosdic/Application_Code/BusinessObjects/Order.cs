using System;
using System.Collections.Generic;
using System.Text;

namespace Com.ConversionSystems.GoldCanyon
{
    class Order : Com.ConversionSystems.UI.BasePage
    {
        private string _strOrderNumber = "";
        private string _strAuthCode = "";
        private string _strAuthDate = "";
        private string _strSourceCode = "";
        private string _strOrderType = "";
        private string _strProductID_AdCode = "";
        private string _strInboundDialed = "";
        private string _strTransactionTime = "";
        private string _strTransactionDate = "";
        private string _strOperatorCode = "";
        private string _strDeliveryMethod = "";
        private string _strOrderBaseAmount = "";
        private string _strShipping = "";
        private string _StrTax = "";
        private string _strDiscout = "";
        private string _strOrderTotal = "";
        private string _strMicrNumber = "";
        private string _strCheckNumber = "";
        private string _strBankName = "";
        private string _strBankCity = "";
        private string _strBankAccountType = "";
        private string _strFiller = "";
        private string _strCreditCardNumber = "";
        private string _strCreditCardExpiration = "";
        private string _strNumberOfPayments = "";
        private string _strBillToCompany = "";
        private string _strUniqueId = ""; 
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
        private string _strPhoneExt = "";
        private string _strItemNumber = "";
        private string _strItemQuantity = "";
        private string _strItemPrice = "";
        private string _strSpecialHandlingCharge = "";
        private string _strShipSeperately = "";
        private string _strApplyTo1stInstallment = "";
        private string _strMediaCode = "";
        private string _transactionid = "";
                
        public string BillToCompany
        {
            get { return _strBillToCompany; }
            set { _strBillToCompany = value; }
        }
        public string UniqueId
        {
            get { return _strUniqueId; }
            set { _strUniqueId = value; }
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
        public string PhoneExt
        {
            get { return _strPhoneExt; }
            set { _strPhoneExt = value; }
        }
        public string OrderNumber
        {
            get { return _strOrderNumber; }
            set { _strOrderNumber = value; }
        }
        public string AuthCode 
        {
            get { return _strAuthCode; }
            set { _strAuthCode = value; }
        }
        public string AuthDate
        {
            get { return _strAuthDate; }
            set { _strAuthDate = value; }
        }
        public string SourceCode
        {
            get { return _strSourceCode; }
            set { _strSourceCode = value; }
        }
        public string OrderType
        {
            get { return _strOrderType; }
            set { _strOrderType = value; }
        }
        public string ProductID_AdCode
        {
            get { return _strProductID_AdCode; }
            set { _strProductID_AdCode = value; }
        }
        public string InboundDialed
        {
            get { return _strInboundDialed; }
            set { _strInboundDialed = value; }
        }
        public string TransactionTime 
        {
            get { return _strTransactionTime; }
            set { _strTransactionTime = value; }
        }
        public string TransactionDate
        {
            get { return _strTransactionDate; }
            set { _strTransactionDate = value; }
        }
        public string OperatorCode
        {
            get { return _strOperatorCode; }
            set { _strOperatorCode = value; }
        }
        public string DeliveryMethod
        {
            get { return _strDeliveryMethod; }
            set { _strDeliveryMethod = value; }
        }
        public string OrderBaseAmount
        {
            get { return _strOrderBaseAmount; }
            set { _strOrderBaseAmount = value; }
        }
        public string Shipping
        {
            get { return _strShipping; }
            set { _strShipping = value; }
        }
        public string Tax
        {
            get { return _StrTax; }
            set { _StrTax = value; }
        }
        public string Discout
        {
            get { return _strDiscout; }
            set { _strDiscout = value; }
        }
        public string OrderTotal
        {
            get { return _strOrderTotal; }
            set { _strOrderTotal = value; }
        }
        public string MicrNumber
        {
            get { return _strMicrNumber; }
            set { _strMicrNumber = value; }
        }
        public string CheckNumber
        {
            get { return _strCheckNumber; }
            set { _strCheckNumber = value; }
        }
        public string BankName 
        {
            get { return _strBankName; }
            set { _strBankName = value; }
        }
        public string BankCity 
        {
            get { return _strBankCity; }
            set { _strBankCity = value; }
        }
        public string BankAccountType 
        {
            get { return _strBankAccountType; }
            set { _strBankAccountType = value; }
        }
        public string Filler  
        {
            get { return _strFiller; }
            set { _strFiller = value; }
        }
        public string NumberOfPayments  
        {
            get { return _strNumberOfPayments; }
            set { _strNumberOfPayments = value; }
        }
        public string ItemNumber 
        {
            get { return _strItemNumber; }
            set { _strItemNumber = value; }
        }
        public string ItemQuantity 
        {
            get { return _strItemQuantity; }
            set { _strItemQuantity = value; }
        }
        public string ItemPrice 
        {
            get { return _strItemPrice; }
            set { _strItemPrice = value; }
        }
        public string SpecialHandlingCharge 
        {
            get { return _strSpecialHandlingCharge; }
            set { _strSpecialHandlingCharge = value; }
        }
        public string ShipSeperately
        {
            get { return _strShipSeperately; }
            set { _strShipSeperately = value; }
        }
        public string ApplyTo1stInstallment 
        {
            get { return _strApplyTo1stInstallment; }
            set { _strApplyTo1stInstallment = value; }
        }
        public string ShipToFirstName
        {
            get { return _strShipToFirstName; }
            set { _strShipToFirstName = value; }
        }
        public string ShipToLastName
        {
            get { return _strShipToLastName; }
            set { _strShipToLastName = value; }
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
        public string MediaCode
        {
            get { return _strMediaCode; }
            set { _strMediaCode = value; }
        }

        public string TransactionId
        {
            get { return _transactionid; }
            set { _transactionid = value; }
        }

        

        public void clear()
        {
             _strOrderNumber = "";
         _strAuthCode = "";
         _strAuthDate = "";
         _strSourceCode = "";
         _strOrderType = "";
         _strProductID_AdCode = "";
         _strInboundDialed = "";
         _strTransactionTime = "";
         _strTransactionDate = "";
         _strOperatorCode = "";
         _strDeliveryMethod = "";
         _strOrderBaseAmount = "";
         _strShipping = "";
         _StrTax = "";
         _strDiscout = "";
         _strOrderTotal = "";
         _strMicrNumber = "";
         _strCheckNumber = "";
         _strBankName = "";
         _strBankCity = "";
         _strBankAccountType = "";
         _strFiller = "";
         _strCreditCardNumber = "";
         _strCreditCardExpiration = "";
         _strNumberOfPayments = "";
         _strBillToCompany = "";
         _strUniqueId = ""; 
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
         _strPhoneExt = "";
         _strItemNumber = "";
         _strItemQuantity = "";
         _strItemPrice = "";
         _strSpecialHandlingCharge = "";
         _strShipSeperately = "";
         _strApplyTo1stInstallment = "";
         _strMediaCode = "";
         _transactionid = "";

        }
    }
}
