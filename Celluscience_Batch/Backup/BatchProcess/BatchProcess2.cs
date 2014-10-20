using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using ConversionSystems.Celluscience.DataAccess;
using ConversionSystems.Celluscience.Utility;

namespace ConversionSystems.Celluscience.BatchProcess
{
    public class BatchProcess2 : ConversionSystems.Celluscience.UI.BaseClass
    {
        static string key1;
        static Mediachase.eCF.BusLayer.Common.Util.EncryptionManager em;
        #region Variables & Properties Section
        private DataTable _dtOrders = null;
        private DataTable _dtCustomerBillingAddress = null;
        private DataTable _dtCustomerShippingAddress = null;
        private DataTable _dtOrderSKU = null;
        private DataTable _dtSalesTaxRate = null;
        private int _intOrderID = 0;
        private int _intBillingAddress = 0;
        private int _intShippingAddress = 0;
        LogData log = new LogData();
        bool IsMultiPay = false;
        private DataTable OrderSKU
        {
            get
            {
                if (_dtOrderSKU == null)
                {
                    DAL.SQLServerDAL_Get.GetOrderSKU(_intOrderID, out _dtOrderSKU);
                }
                return _dtOrderSKU;
            }
        }

        private DataTable CustomerShippingAddress
        {
            get
            {
                if (_dtCustomerShippingAddress == null)
                {
                    DAL.SQLServerDAL_Get.GetCustomerAddress(_intShippingAddress, out _dtCustomerShippingAddress);
                }
                return _dtCustomerShippingAddress;
            }
        }

        private DataTable CustomerBillingAddress
        {
            get
            {
                if (_dtCustomerBillingAddress == null)
                {
                    DAL.SQLServerDAL_Get.GetCustomerAddress(_intBillingAddress, out _dtCustomerBillingAddress);
                }
                return _dtCustomerBillingAddress;
            }
        }

        private Hashtable ItemsCodes = new Hashtable();
        private string _srtPath = Helper.DataServicesTxtBatchConfig["FilePath"];       
        private string txtfilename = "";
        const string dq = "\"";
        const string dqcdq = "\",\"";
        private string Actualfilename = "";
        private string RESERVED = "";
        #endregion

        #region Batch Process

        public bool ProcessBatch(DateTime dtbp, string EmployeeNumber)
        {
            bool _breturn = false;
           // string DNIS = "5700";
            string DNIS = Helper.DataServicesTxtBatchConfig["Field#22_GET"];   

            //Console.WriteLine("\n Please Enter the 4 digit DNIS of the Telephone # dialed by customer.");
            //DNIS = Console.ReadLine();


            #region Create Files
            if (!Directory.Exists(_srtPath))
            {
                Directory.CreateDirectory(_srtPath);
            }
            Actualfilename = EmployeeNumber + DateTime.Now.ToString("MMddyyyy") + "_cellu.txt";
            txtfilename = _srtPath + "\\" + Actualfilename ;
            StreamWriter SWtxtfile = new StreamWriter(txtfilename,true);
            SWtxtfile.Flush();
            #endregion

            try
            {  
                DAL.SQLServerDAL_Get.GetOrdersForTxtBatch(dtbp, out _dtOrders);
                foreach (DataRow _drOrder in _dtOrders.Rows)
                {
                    #region MyRegion
                    Order ord = new Order();
                    _breturn = true;
                    _intOrderID = Helper.ToInt32(_drOrder["OrderId"].ToString());                  
                    #region Shipping Address Info

                    _intShippingAddress = Helper.ToInt32(_drOrder["CustomerShippingAddressId"].ToString());

                    foreach (DataRow _drShippingInfo in CustomerShippingAddress.Rows)
                    {
                        ord.ShipToLastName = _drShippingInfo["LastName"].ToString();
                        ord.ShipToFirstName = _drShippingInfo["FirstName"].ToString();
                        ord.ShipToAddress1 = _drShippingInfo["Address1"].ToString();
                        ord.ShipToAddress2 = _drShippingInfo["Address2"].ToString();
                        ord.ShipToCity = _drShippingInfo["City"].ToString();
                        ord.ShipToState = _drShippingInfo["StateProvince"].ToString();
                        ord.ShipToZip = _drShippingInfo["ZipPostalCode"].ToString();
                        ord.ShipToCountry = _drShippingInfo["CountryCode"].ToString();
                        ord.ShipToPhone = ValidatePhone(_drShippingInfo["PhoneNumber"].ToString());
                        _dtCustomerShippingAddress = null;
                        break;
                    }
                    #endregion
                    #region Billing Address Info

                    _intBillingAddress = Helper.ToInt32(_drOrder["BillingAddressId"].ToString());

                    foreach (DataRow _drBillingInfo in CustomerBillingAddress.Rows)
                    {
                        ord.BillingFirstName = _drBillingInfo["FirstName"].ToString();
                        ord.BillingLastName = _drBillingInfo["LastName"].ToString();
                        ord.BillingAddress1 = _drBillingInfo["Address1"].ToString();
                        ord.BillingAddress2 = _drBillingInfo["Address2"].ToString();
                        ord.BillingCity = _drBillingInfo["City"].ToString();
                        ord.BillingState = _drBillingInfo["StateProvince"].ToString();
                        ord.BillingZip = _drBillingInfo["ZipPostalCode"].ToString();
                        ord.BillingCountry = _drBillingInfo["CountryCode"].ToString();
                        ord.BillingPhone = ord.ShipToPhone; //Billing Phone number is always same as shipping phone number.
                        _dtCustomerBillingAddress = null;
                        break;
                    }
                    #endregion
                    ord.Email = _drOrder["Email"].ToString();
                    #region Credit Card Info
                    key1 = System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];
                    Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
                    em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();
                    ord.CreditCardNumber = em.Decrypt(_drOrder["CreditCardNumber"].ToString());
                    ord.CreditCardType = _drOrder["CreditCardName"].ToString().Substring(0, 1).ToUpper();
                    if (ord.CreditCardType == "M") ord.CreditCardType = "MC";                    
                    ord.CreditCardExpiration = Convert.ToDateTime(_drOrder["CreditCardExpired"]).ToString("MM/yy");
                    ord.CVV = _drOrder["CreditCardCSC"].ToString();
                    #endregion
                    #region Sales Tax

                    DAL.SQLServerDAL_Get.GetSalesTaxRate(ord.ShipToState, out _dtSalesTaxRate);
                    foreach (DataRow _drSalesTax in _dtSalesTaxRate.Rows)
                    {
                        ord.SalesTaxRate = _drSalesTax["taxrate"].ToString();
                        _dtSalesTaxRate = null;
                        break;
                    }
                    #endregion
                    #region Order Info
                    //Start - Taking care of passing multipay as one pay - as per clients need
                    IsMultiPay = false;
                    foreach (DataRow _drOrderSKU in OrderSKU.Rows)
                    {
                        if (_drOrderSKU["SKUID"].ToString().Equals("326") || _drOrderSKU["SKUID"].ToString().Equals("327") || _drOrderSKU["SKUID"].ToString().Equals("328") || _drOrderSKU["SKUID"].ToString().Equals("329"))
                        {
                            IsMultiPay = true;
                        }
                    }
                    if (IsMultiPay)
                    {
                        if (ord.ShipToCountry.Equals("US "))
                        {
                            if (ord.ShipToState.Equals("FL"))
                            {
                                //Recalculating Tax for FL only
                                ord.OrderNumber = _drOrder["OrderId"].ToString();
                                ord.OrderDate = Convert.ToDateTime(_drOrder["Created"]).ToString("MM/dd/yyyy");
                                ord.OrderTime = Convert.ToDateTime(_drOrder["Created"]).ToString("HH:mm:ss");

                                ord.MerchandiseTotal = Helper.ToDecimal("292.96").ToString();
                                ord.MerchandiseTotal = Math.Round((decimal.Parse(ord.MerchandiseTotal)), 2).ToString();

                                ord.ShippingTotal = Helper.ToDecimal("0").ToString();
                                ord.ShippingTotal = Math.Round(decimal.Parse(ord.ShippingTotal), 2).ToString();
                                decimal _taxRate = decimal.Parse(ord.SalesTaxRate);

                                ord.SalesTaxTotal = (_taxRate * (decimal.Parse(ord.ShippingTotal) + decimal.Parse(ord.MerchandiseTotal))).ToString();
                                ord.SalesTaxTotal = Math.Round(decimal.Parse(ord.SalesTaxTotal), 2).ToString();

                                ord.OrderTotal = (decimal.Parse(ord.MerchandiseTotal) + decimal.Parse(ord.ShippingTotal) + decimal.Parse(ord.SalesTaxTotal)).ToString();
                                ord.OrderTotal = Math.Round(decimal.Parse(ord.OrderTotal), 2).ToString();
                            }
                            else
                            {
                                ord.OrderNumber = _drOrder["OrderId"].ToString();
                                ord.OrderDate = Convert.ToDateTime(_drOrder["Created"]).ToString("MM/dd/yyyy");
                                ord.OrderTime = Convert.ToDateTime(_drOrder["Created"]).ToString("HH:mm:ss");

                                ord.MerchandiseTotal = Helper.ToDecimal("292.96").ToString();
                                ord.MerchandiseTotal = Math.Round((decimal.Parse(ord.MerchandiseTotal)), 2).ToString();

                                ord.ShippingTotal = Helper.ToDecimal("0").ToString();
                                ord.ShippingTotal = Math.Round(decimal.Parse(ord.ShippingTotal), 2).ToString();

                                ord.SalesTaxTotal = _drOrder["Tax"].ToString().Trim();
                                ord.SalesTaxTotal = Math.Round(decimal.Parse(ord.SalesTaxTotal), 2).ToString();

                                ord.OrderTotal = (decimal.Parse(ord.MerchandiseTotal) + decimal.Parse(ord.ShippingTotal) + decimal.Parse(ord.SalesTaxTotal)).ToString();
                                ord.OrderTotal = Math.Round(decimal.Parse(ord.OrderTotal), 2).ToString();
                            }
                        }
                        else
                        {
                            ord.OrderNumber = _drOrder["OrderId"].ToString();
                            ord.OrderDate = Convert.ToDateTime(_drOrder["Created"]).ToString("MM/dd/yyyy");
                            ord.OrderTime = Convert.ToDateTime(_drOrder["Created"]).ToString("HH:mm:ss");

                            ord.MerchandiseTotal = Helper.ToDecimal("292.96").ToString();
                            ord.MerchandiseTotal = Math.Round((decimal.Parse(ord.MerchandiseTotal)), 2).ToString();

                            ord.ShippingTotal = Helper.ToDecimal("10").ToString();
                            ord.ShippingTotal = Math.Round(decimal.Parse(ord.ShippingTotal), 2).ToString();

                            ord.SalesTaxTotal = _drOrder["Tax"].ToString().Trim();
                            ord.SalesTaxTotal = Math.Round(decimal.Parse(ord.SalesTaxTotal), 2).ToString();

                            ord.OrderTotal = (decimal.Parse(ord.MerchandiseTotal) + decimal.Parse(ord.ShippingTotal) + decimal.Parse(ord.SalesTaxTotal)).ToString();
                            ord.OrderTotal = Math.Round(decimal.Parse(ord.OrderTotal), 2).ToString();
                        }


                    }
                    else
                    {
                        //This means it is single pay.
                        ord.OrderNumber = _drOrder["OrderId"].ToString();
                        ord.OrderDate = Convert.ToDateTime(_drOrder["Created"]).ToString("MM/dd/yyyy");
                        ord.OrderTime = Convert.ToDateTime(_drOrder["Created"]).ToString("HH:mm:ss");

                        ord.MerchandiseTotal = _drOrder["SubTotal"].ToString();
                        ord.MerchandiseTotal = Math.Round(decimal.Parse(ord.MerchandiseTotal), 2).ToString();

                        ord.ShippingTotal = _drOrder["ShippingCost"].ToString();
                        ord.ShippingTotal = Math.Round(decimal.Parse(ord.ShippingTotal), 2).ToString();

                        ord.SalesTaxTotal = _drOrder["Tax"].ToString().Trim();
                        ord.SalesTaxTotal = Math.Round(decimal.Parse(ord.SalesTaxTotal), 2).ToString();

                        ord.OrderTotal = (decimal.Parse(ord.MerchandiseTotal) + decimal.Parse(ord.ShippingTotal) + decimal.Parse(ord.SalesTaxTotal)).ToString();
                        ord.OrderTotal = Math.Round(decimal.Parse(ord.OrderTotal), 2).ToString();
                    }
                    //End - Taking care of passing multipay as one pay - as per clients need
                    ord.ShippingMethod = _drOrder["ShippingMethod"].ToString();
                    if (ord.ShippingMethod.Equals("RegularShipping"))
                        ord.ShippingMethod = "REG";
                    if (ord.ShippingMethod.Equals("RushShipping"))
                        ord.ShippingMethod = "RSH";

                    #endregion

                    #endregion

                    #region OrderSku1
                    string PRODUCT = "";
                    string PRODUCT01 = "";
                    string PRODUCT02 = "";
                    string PRODUCT03 = "";
                    string PRODUCT04 = "";
                    string PRODUCT05 = "";
                    string QUANTITY = "";
                    string QUANTITY01 = "";
                    string QUANTITY02 = "";
                    string QUANTITY03 = "";
                    string QUANTITY04 = "";
                    string QUANTITY05 = "";
                    string PRICE = "";
                    string PRICE01 = "";
                    string PRICE02 = "";
                    string PRICE03 = "";
                    string PRICE04 = "";
                    string PRICE05 = "";
                    string DISCOUNT = "";
                    string DISCOUNT01 = "";
                    string DISCOUNT02 = "";
                    string DISCOUNT03 = "";
                    string DISCOUNT04 = "";
                    string DISCOUNT05 = "";
                    int SrNo = 1;                   
                    foreach (DataRow _drOrderSKU in OrderSKU.Rows)
                    {
                        PRODUCT = _drOrderSKU["SKUCode"].ToString();
                        QUANTITY = _drOrderSKU["OrderSkuQuantity"].ToString();
                        if (IsMultiPay)
                        {
                            PRICE = Helper.ToDecimal("292.96").ToString();
                            PRICE = Math.Round((decimal.Parse(PRICE)), 2).ToString();
                        }
                        else
                        {
                            PRICE = _drOrderSKU["OrderSkuPrice"].ToString();
                            PRICE = Math.Round(decimal.Parse(PRICE), 2).ToString();
                        }
                        DISCOUNT = _drOrderSKU["OrderSkuDiscount"].ToString();
                        DISCOUNT = Math.Round(decimal.Parse(DISCOUNT), 2).ToString();
                        switch (SrNo)
                        {
                            case 1:
                                PRODUCT01 = PRODUCT;
                                QUANTITY01 = QUANTITY;
                                PRICE01 = PRICE;
                                DISCOUNT01 = DISCOUNT;
                                break;
                            case 2:
                                PRODUCT02 = PRODUCT;
                                QUANTITY02 = QUANTITY;
                                PRICE02 = PRICE;
                                DISCOUNT02 = DISCOUNT;
                                break;
                            case 3:
                                PRODUCT03 = PRODUCT;
                                QUANTITY03 = QUANTITY;
                                PRICE03 = PRICE;
                                DISCOUNT03 = DISCOUNT;
                                break;
                            case 4:
                                PRODUCT04 = PRODUCT;
                                QUANTITY04 = QUANTITY;
                                PRICE04 = PRICE;
                                DISCOUNT04 = DISCOUNT;
                                break;
                            case 5:
                                PRODUCT05 = PRODUCT;
                                QUANTITY05 = QUANTITY;
                                PRICE05 = PRICE;
                                DISCOUNT05 = DISCOUNT;
                                break;                            
                        }  
                        SrNo += 1;

                    }
                    _dtOrderSKU = null;
                    #endregion
                    #region txt output
                    string BCOUNTRY = "001";
                    string SCOUNTRY = "001";
                    if (ord.BillingCountry == "CA") BCOUNTRY = "034";
                    if (ord.ShipToCountry == "CA") SCOUNTRY = "034";

                    string OrderRecord = dq + RESERVED + dqcdq                           //1	RESERVED
                                      + EmployeeNumber+"-"+ord.OrderNumber + dqcdq       //2	CUSTOMER_NUMBER
                                      + ord.BillingLastName + dqcdq                      //3	BILL_LAST_NAME
                                      + ord.BillingFirstName + dqcdq                     //4	BILL_FIRST_NAME
                                      + ord.BillingCompany + dqcdq                       //5  	BILL_COMPANY

                                      + ord.BillingAddress1 + dqcdq                       //6	    BILL_ADDRESS1
                                      + ord.BillingAddress2 + dqcdq                       //7	    BILL_ADDRESS2
                                      + ord.BillingCity + dqcdq                           //8	    BILL_CITY
                                      + ord.BillingState + dqcdq                          //9	    BILL_STATE
                                      + ord.BillingZip + dqcdq                            //10	BILL_ZIPCODE

                                      + RESERVED + dqcdq                                  //11	RESERVED                     
                                      + ord.BillingPhone + dqcdq                          //12	BILL_PHONE_NUMBER
                                      + RESERVED + dqcdq          //13	ACH_BANK_NAME
                                      + RESERVED + dqcdq          //14	NO_SOLICITING
                                      + RESERVED + dqcdq                                  //15	RESERVED

                                      + RESERVED + dqcdq         //16	PAYMENT_TYPE
                                      + RESERVED + dqcdq                                  //17	RESERVED
                                      + RESERVED + dqcdq                                  //18	RESERVED
                                      + ord.CreditCardType + dqcdq                        //19	CC_TYPE
                                      + ord.CreditCardNumber + dqcdq                      //20	CC_NUMBER

                                      + ord.CreditCardExpiration + dqcdq                  //21	EXP_DATE
                                      + DNIS + dqcdq                                      //22	DNIS
                                      + RESERVED + dqcdq                                  //23	KEY_CODE
                                      + EmployeeNumber + dqcdq                            //24	EMP_NUMBER
                                      + RESERVED + dqcdq                                  //25	RESERVED

                                      + RESERVED + dqcdq                                  //26	RESERVED
                                      + ord.ShippingMethod + dqcdq                        //27	SHIPPING_METHOD
                                      + RESERVED + dqcdq                                  //28	ORDER_ALREADY_FULLFILLED
                                      + RESERVED + dqcdq                                  //29	AMOUNT_ALREADY_PAID
                                      + "N" + dqcdq                                       //30	CONTINUED

                                      + ord.OrderDate+dqcdq                                //31	ORDER_DATE
                                      + EmployeeNumber + "-" + ord.OrderNumber + dqcdq    //32	ORDER_NUMBER
                                      + PRODUCT01 + dqcdq                                 //33	PRODUCT01
                                      + QUANTITY01 + dqcdq                                //34	QUANTITY01
                                      + PRODUCT02 + dqcdq                                         //35	PRODUCT02

                                      + QUANTITY02 + dqcdq                                //36	QUANTITY02
                                      + PRODUCT03 + dqcdq                                 //37	PRODUCT03
                                      + QUANTITY03 + dqcdq                                //38	QUANTITY03
                                      + PRODUCT04 + dqcdq                                 //39	PRODUCT04
                                      + QUANTITY04 + dqcdq                                //40	QUANTITY04

                                      + PRODUCT05 + dqcdq                                 //41	PRODUCT05
                                      + QUANTITY05 + dqcdq                                //42	QUANTITY05
                                      + ord.ShipToLastName + dqcdq                        //43	SHIP_TO_LAST_NAME
                                      + ord.ShipToFirstName + dqcdq                       //44	SHIP_TO_FIRST_NAME
                                      + ord.ShipToCompany+dqcdq                           //45	SHIP_TO_COMPANY

                                      + ord.ShipToAddress1 + dqcdq                        //46	SHIP_TO_ADDRESS1
                                      + ord.ShipToAddress2 + dqcdq                        //47	SHIP_TO_ADDRESS2
                                      + ord.ShipToCity + dqcdq                            //48	SHIP_TO_CITY
                                      + ord.ShipToState  + dqcdq                          //49	SHIP_TO_STATE
                                      + ord.ShipToZip + dqcdq                         //50	SHIP_TO_ZIP

                                      + RESERVED + dqcdq                                  //51	ORDER_HOLD_DATE
                                      + "CC" + dqcdq                                      //52	PAYMENT_METHOD
                                      + RESERVED + dqcdq                                  //53	ACH_ROUTING_NUMBER
                                      + RESERVED + dqcdq                                  //54	ACH_ACCOUNT_NUMBER
                                      + RESERVED + dqcdq                                  //55	RESERVED

                                      + "N" + dqcdq                                       //56	USE_PRICES
                                      + PRICE01 + dqcdq                                   //57	PRICE01
                                      + DISCOUNT01 + dqcdq                                //58	DISCOUNT01
                                      + PRICE02 + dqcdq                                   //59	PRICE02
                                      + DISCOUNT02 + dqcdq                                //60	DISCOUNT02

                                      + PRICE03 + dqcdq                                   //61	PRICE03
                                      + DISCOUNT03 + dqcdq                                //62	DISCOUNT03
                                      + PRICE04 + dqcdq                                   //63	PRICE04
                                      + DISCOUNT04 + dqcdq                                //64	DISCOUNT04
                                      + PRICE05 + dqcdq                                   //65	PRICE05

                                      + DISCOUNT05 + dqcdq                                //66	DISCOUNT05
                                      + "N" + dqcdq                                       //67	USE_SHIPPING
                                      + ord.ShippingTotal + dqcdq                         //68	SHIPPING
                                      + ord.Email + dqcdq                                 //69	EMAIL
                                      + RESERVED + dqcdq                                  //70	MERCHANT_TRANSACTION_ID

                                      + BCOUNTRY + dqcdq                                  //71	COUNTRY USA = 001
                                      + SCOUNTRY + dqcdq                                  //72	SCOUNTRY USA = 001
                                      + ord.BillingPhone + dqcdq                          //73	BILL_PHONE_2
                                      + ord.ShipToPhone + dqcdq                           //74	SHIP_TO_PHONE
                                      + RESERVED + dqcdq                                  //75	RESERVED

                                      + RESERVED + dqcdq                                  //76	RESERVED
                                      + RESERVED + dqcdq                                  //77	RESERVED
                                      + RESERVED + dqcdq                                  //78	RESERVED
                                      + RESERVED + dqcdq                                  //79	CUSTOM_1
                                      + RESERVED + dqcdq                                  //80	CUSTOM_2

                                      + RESERVED + dqcdq                                  //81	CUSTOM_3
                                      + RESERVED + dqcdq                                  //82	CUSTOM_4
                                      + RESERVED + dqcdq                                  //83	CUSTOM_5
                                      + RESERVED + dqcdq                                  //84	COUPON_CODE01
                                      + RESERVED + dqcdq                                  //85	COUPON_CODE02
                                     
                                      + RESERVED + dqcdq                                 //86	COUPON_CODE03
                                      + RESERVED + dqcdq                                 //87	COUPON_CODE04
                                      + RESERVED + dqcdq                                 //88	COUPON_CODE05
                                      + RESERVED + dqcdq                                 //89	RESERVED
                                      + RESERVED + dqcdq                                 //90	TRACKING_NUM
                                      + RESERVED + dqcdq
                                      + RESERVED + dqcdq                                 //91	SHIPPING_CARRIER
                                      + RESERVED + dqcdq                                 //92	SHIPPING_DATE
                                      + RESERVED + dqcdq                                 //93	CLIENT_TRANSACTION_ID
                                      + ord.CVV + dqcdq                                  //94	CVV_CODE
                                      + RESERVED + dqcdq                                 //95	VOICE_RECORDING_ID

                                      + RESERVED + dqcdq                                 //96	ACH_CHECK_NUMBER
                                      + ord.SalesTaxTotal + dqcdq                          //97	ORDER_STATE_SALES_TAX
                                      + "0" + dqcdq                                      //98	ORDER_FEDERAL_SALES_TAX
                                      + RESERVED + dqcdq                                 //99	CUSTOMER_COMMENTS
                                      + RESERVED + dqcdq                                 //100	ACH_IS_SAVINGS_ACCOUNT
                                      + RESERVED + dqcdq  
                                      + RESERVED + dqcdq                                //101	CUSTOMER_AGE
                                      + RESERVED + dqcdq                                //102	CUSTOMER_DOB
                                      + RESERVED + dqcdq                                //103	CUSTOMER_GENDER
                                      + RESERVED + dqcdq                                //104	CUSTOMER_SSN
                                      + RESERVED + dqcdq                                //105	PACKING_SLIP_COMMENTS

                                      + RESERVED + dqcdq                                //106	ORDER_COMMENTS
                                      + RESERVED + dqcdq                                //107	LOCATION_CODE
                                      + dq                                      ;
                             SWtxtfile.WriteLine( OrderRecord);

                    #endregion

                }
                SWtxtfile.Flush();
                SWtxtfile.Close();
                log.LogToFile("   " + txtfilename + " - File Successfully created ");
                Console.WriteLine("   " + txtfilename + " - File Successfully created ");

            }
            catch (Exception e)
            {
                log.LogToFile("Error creating file ---" + e.Message);
                _breturn = false;
                SWtxtfile.Flush();
                SWtxtfile.Close();
                return _breturn;

            }
            return _breturn;
        }
        public string ValidatePhone(string input)
        {
            string result = string.Empty;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    result += c;
                }
            }
            return result;
        }
        #endregion
      
    }
}
