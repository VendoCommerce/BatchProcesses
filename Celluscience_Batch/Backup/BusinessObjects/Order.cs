using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConversionSystems.Celluscience.BatchProcess
{
    class Order : ConversionSystems.Celluscience.UI.BaseClass
    {
        private string _strOrderId = "";
        private string _strOrderNumber = "";
        private string _strOrderDate = "";
        private string _strOrderTime = "";
        private string _strCustomerId = "";
        private string _strSkuId = "";

        #region Credit Card fields
        private string _strCreditCardNumber = "";
        private string _strCreditCardType = ""; 
        private string _strCreditCardExpiration = "";
        private string _strCVV = "";

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
        public string CVV
        {
            get { return _strCVV; }
            set { _strCVV = value; }
        }

        #endregion

        #region Shipping Address Fields & Properties
        private string _strShipToFirstName = "";
        private string _strShipToLastName = "";
        private string _strShipToCompany = "";
        private string _strShipToAddress1 = "";
        private string _strShipToAddress2 = "";
        private string _strShipToCity = "";
        private string _strShipToState = "";
        private string _strShipToZip = "";
        private string _strShipToCountry = "";
        private string _strShipToPhone = "";       

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
        public string ShipToCompany
        {
            get { return _strShipToCompany; }
            set { _strShipToCompany = value; }
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
        public string ShipToZip
        {
            get { return _strShipToZip; }
            set { _strShipToZip = value; }
        }
        public string ShipToCountry
        {
            get { return _strShipToCountry; }
            set { _strShipToCountry = value; }
        }
        public string ShipToPhone
        {
            get { return _strShipToPhone; }
            set { _strShipToPhone = value; }
        }
       
        #endregion

        #region Billing Address Fields & Properties
        private string _strBillingFirstName = "";
        private string _strBillingLastName = "";
        private string _strBillingCompany = "";
        private string _strBillingAddress1 = "";
        private string _strBillingAddress2 = "";
        private string _strBillingCity = "";
        private string _strBillingState = "";
        private string _strBillingZip = "";
        private string _strBillingCountry = "";
        private string _strBillingPhone = "";

        public string BillingFirstName
        {
            get { return _strBillingFirstName; }
            set { _strBillingFirstName = value; }
        }
        public string BillingLastName
        {
            get { return _strBillingLastName; }
            set { _strBillingLastName = value; }
        }
        public string BillingCompany
        {
            get { return _strBillingCompany; }
            set { _strBillingCompany = value; }
        }
        public string BillingAddress1
        {
            get { return _strBillingAddress1; }
            set { _strBillingAddress1 = value; }
        }
        public string BillingAddress2
        {
            get { return _strBillingAddress2; }
            set { _strBillingAddress2 = value; }
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
        public string BillingZip
        {
            get { return _strBillingZip; }
            set { _strBillingZip = value; }
        }
        public string BillingCountry
        {
            get { return _strBillingCountry; }
            set { _strBillingCountry = value; }
        }
        public string BillingPhone
        {
            get { return _strBillingPhone; }
            set { _strBillingPhone = value; }
        }
        #endregion

        private string _strMerchandiseTotal = ""; 
        private string _strShippingTotal = "";
        private string _strSalesTaxTotal = "";
        private string _strOrderTotal = "";

        private string _strItemCode = "";
        private string _strSalesTaxRate = "";  
        
        private string _strLineId = "";
        private string _strQuantity = "";
        private string _strPrice = "";
        
        private string _strOfferNumber = "";
        private string _strPrimarySKU = "";

        private string _strAccountName = "";
        private string _strAmount = "";
       
       
        private string _strTransactionId = "";

        private string _strEmail = "";
        private string _strShippingMethod = "";
        public string ShippingMethod
        {
            get { return _strShippingMethod; }
            set { _strShippingMethod = value; }
        }
        public string SkuId
        {
            get { return _strSkuId; }
            set { _strSkuId = value; }
        }

        public string OrderId
        {
            get { return _strOrderId; }
            set { _strOrderId = value; }
        }
        public string OrderNumber
        {
            get { return _strOrderNumber; }
            set { _strOrderNumber = value; }
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
       

        public string SalesTaxRate
        {
            get { return _strSalesTaxRate; }
            set { _strSalesTaxRate = value; }
        }
        public string MerchandiseTotal
        {
            get { return _strMerchandiseTotal; }
            set { _strMerchandiseTotal = value; }
        }
        public string ShippingTotal
        {
            get { return _strShippingTotal; }
            set { _strShippingTotal = value; }
        }
        public string SalesTaxTotal
        {
            get { return _strSalesTaxTotal; }
            set { _strSalesTaxTotal = value; }
        }
        public string OrderTotal
        {
            get { return _strOrderTotal; }
            set { _strOrderTotal = value; }
        }
        public string ItemCode
        {
            get { return _strItemCode; }
            set { _strItemCode = value; }
        }
        public string LineId
        {
            get { return _strLineId; }
            set { _strLineId = value; }
        }
        public string Quantity
        {
            get { return _strQuantity; }
            set { _strQuantity = value; }
        }
        public string Price
        {
            get { return _strPrice; }
            set { _strPrice = value; }
        }

        public string OfferNumber
        {
            get { return _strOfferNumber; }
            set { _strOfferNumber = value; }
        }
        public string PrimarySKU
        {
            get { return _strPrimarySKU; }
            set { _strPrimarySKU = value; }
        }
        
        public string AccountName
        {
            get { return _strAccountName; }
            set { _strAccountName = value; }
        }
        public string Amount
        {
            get { return _strAmount; }
            set { _strAmount = value; }
        }

      
        public string TransactionId
        {
            get { return _strTransactionId; }
            set { _strTransactionId = value; }
        }
        public string CustomerId
        {
            get { return _strCustomerId; }
            set { _strCustomerId = value; }
        }

        public string Email
        {
            get { return _strEmail; }
            set { _strEmail = value; }
        }       
    }
}
