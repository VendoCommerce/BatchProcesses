﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using Com.ConversionSystems.DataAccess;
using Com.ConversionSystems.Utility;
using Com.ConversionSystems.GoldCanyon;
using ConversionSystems.Providers;
using ConversionSystems.Providers.MediaChase.OrderProcessing;





namespace Com.ConversionSystems.GoldCanyon
{
    public class Batch : Com.ConversionSystems.UI.BasePage
    {
        private DataTable _dtOrders = null;
        private DataTable _dtRejectedOrders = null;
        private ArrayList AllOrders = new ArrayList();
        private DataTable _dtCustomerBillingAddress = null;
        private DataTable _dtCustomerShippingAddress = null;
        private DataTable _dtOrderSKU = null;
        private DataTable _dtSalesTaxRate = null;
        private int _intOrderID = 0;
        private int _intBillingAddress = 0;
        private int _intShippingAddress = 0;
        int _intRecords = 0;
        private Hashtable UpSell = new Hashtable();
        private string _srtPath = "C:\\batchstaging\\BatchProcesses\\ContourAbsBatchFiles";
        private string filenameCustomer = "";
        private string filenameOrder = "";
        private string filenameDetail = "";
        private string ActualfilenameCustomer = "";
        public static string file1;
        public static string file2;
        public static string file3;
        public static ArrayList PaymentPlanSKU = null;
        public static int count1;
        public static int count2;
        public static int count3;
        public string _TransactionId = "";
        public string _AuthCode = "";
        private string ActualfilenameOrder = "";
        private string ActualfilenameDetail = ""; 
        LogData log = new LogData();        
        private DataTable Orders
        {
            get
            {
                   //DAL.SQLServer.GetOrdersForXMLBatch(DateTime.ParseExact (DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd",CultureInfo.InvariantCulture), out _dtOrders);
                    DAL.SQLServer.GetOrdersForXMLBatch(out _dtOrders);
                    //DAL.SQLServer.GetOrdersForXMLBatch(DateTime.Now.AddDays(-1), out _dtOrders);
                    //DAL.SQLServer.GetOrdersForXMLBatch(new DateTime(2009, 06, 29), out _dtOrders);
                
                return _dtOrders;
           } 
        }        
        private DataTable RejectedOrders
        {
            get
            {
                if (_dtRejectedOrders == null)
                {
                    //DAL.SQLServer.GetOrdersForXMLBatch(DateTime.ParseExact (DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd",CultureInfo.InvariantCulture), out _dtOrders);
                    DAL.SQLServer.GetRejectedOrdersForXMLBatch(out _dtRejectedOrders);
                    //DAL.SQLServer.GetOrdersForXMLBatch(DateTime.Now.AddDays(-1), out _dtOrders);
                    //DAL.SQLServer.GetOrdersForXMLBatch(new DateTime(2009, 06, 29), out _dtOrders);
                }
                return _dtRejectedOrders;
            }
        }
        private DataTable CustomerBillingAddress
        {
            get
            {
                if (_dtCustomerBillingAddress == null)
                {
                    DAL.SQLServer.GetCustomerAddress(_intBillingAddress, out _dtCustomerBillingAddress);
                }
                return _dtCustomerBillingAddress;
            }
        }
        private DataTable CustomerShippingAddress
        {
            get
            {
                if (_dtCustomerShippingAddress == null)
                {
                    DAL.SQLServer.GetCustomerAddress(_intShippingAddress, out _dtCustomerShippingAddress);
                }
                return _dtCustomerShippingAddress;
            }
        }
        private DataTable OrderSKU
        {
            get
            {
                if (_dtOrderSKU == null)
                {
                    DAL.SQLServer.GetOrderSKU(_intOrderID, out _dtOrderSKU);
                }
                return _dtOrderSKU;
            }
        }
        public void DoEncryption()
        {
            //Check this before going forward completely
            string EncryptionFileKey = "C:\\enc\\encryptionkey.asc";
            PGPEncryption _oEncrypt = new PGPEncryption();
            //_oEncrypt.Encrypt(filename,EncryptionFileKey,filename);
        }
        
        public static string fixstring(object s1)
        {
            string s = "";
            if (s1 != null) { s = s1.ToString(); }

            s = s.Replace("&", "&amp;");
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");
            s = s.Replace(((char)(34)).ToString(), "&quot;");
            s = s.Replace("'", "&apos;");
            return s;

        }
        public static string fixQuot(object s1)
        {
            string s = "";
            if (s1 != null) { s = s1.ToString(); }
            s = s.Replace("'", "&apos;");
            return s;

        }

        private DataSet getsql(string s)
        {
            SqlConnection conn = new SqlConnection(Helper.SQLServerDAO["mode"].ToString());
            conn.Open();
            SqlDataAdapter adp = new SqlDataAdapter(s, conn);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            conn.Close();
            return ds;
        }
        private void runsql(string s)
        {
            SqlConnection conn = new SqlConnection(Helper.SQLServerDAO["mode"].ToString());
            conn.Open();
            SqlCommand cm = new SqlCommand(s, conn);
            cm.CommandType = CommandType.Text;
            cm.ExecuteNonQuery();
            conn.Close();
        }
        public string getomxml(string verifyflag, string orderid1)
        {
            _intOrderID = Convert.ToInt32(orderid1);
            string s1 = "";
            s1 += "<?xml version=[[[1.0[[[?>~";
            s1 += "<UDOARequest version=[[[2.00[[[>~";
            s1 += "	<UDIParameter>~";            
            s1 += "		<Parameter key=[[[UDIAuthToken[[[>6fac904d01af3048310bda400be25912ebe37bcecb47025b104ee40b9780a4236358cef42d2806c103e7d0451c0a1b10809be3260eb195c6e43a00a7504ddd0b1aa061d150305ce0663a82e30f64804ff20b9fa0bb172fd58640d6915f840a6f604b820befe0e71b77d98b072c10b45b0e8f1049810b9ea074eb3b0cf201766</Parameter>~";            
            s1 += "		<Parameter key=[[[Keycode[[[>@keyCode@</Parameter>~";
            s1 += "		<Parameter key=[[[VerifyFlag[[[>" + verifyflag + "</Parameter>~";
            s1 += "		<Parameter key=[[[QueueFlag[[[>True</Parameter>~";
            s1 += "	</UDIParameter>~";
            s1 += "	<Header>~";
            s1 += "		<OrderID>@orderid@</OrderID>~";
            s1 += "		<OrderDate>@orderdate@</OrderDate>~";
            s1 += "		<OriginType>3</OriginType>~";
            s1 += "		<StoreCode>Conversion Systems</StoreCode>~";
            s1 += "		<SiteID>@siteid@</SiteID>~";
            s1 += "		<CustomerIP></CustomerIP>~"; //ASK KEVIN AB THIS
            s1 += "	</Header>~";
            s1 += "	<Customer>~";
            s1 += "		<Address type=[[[BillTo[[[>~";
            s1 += "			<TitleCode>0</TitleCode>~";
            s1 += "			<Company/>~";
            s1 += "			<Firstname>@billfirstname@</Firstname>~";
            s1 += "			<Lastname>@billlastname@</Lastname>~";
            s1 += "			<Address1>@billaddress@</Address1>~";
            s1 += "			<Address2>@billaddress2@</Address2>~";
            s1 += "			<City>@billcity@</City>~";
            s1 += "			<State>@billstate@</State>~";
            s1 += "			<ZIP>@billzip@</ZIP>~";
            s1 += "			<TLD>@billcountry@</TLD>~";
            s1 += "			<PhoneNumber>@1@</PhoneNumber>~";
            s1 += "			<Email>@2@</Email>~";
            s1 += "		</Address>~";
            s1 += "		 <FlagData>~";
            s1 += "			<Flag name=[[[DoNotMail[[[>False</Flag>~";
            s1 += "			<Flag name=[[[DoNotCall[[[>False</Flag>~";
            s1 += "			<Flag name=[[[DoNotEmail[[[>False</Flag>~";
            s1 += "		 </FlagData>~";
            s1 += "	</Customer>~";
            s1 += "	<ShippingInformation>~";
            s1 += "		<Address type=[[[ShipTo[[[>~";
            s1 += "			<TitleCode>0</TitleCode>~";
            s1 += "			<Company/>~";
            s1 += "			<Firstname>@shipfirstname@</Firstname>~";
            s1 += "			<Lastname>@shiplastname@</Lastname>~";
            s1 += "			<Address1>@shipaddress@</Address1>~";
            s1 += "			<Address2>@shipaddress2@</Address2>~";
            s1 += "			<City>@shipcity@</City>~";
            s1 += "			<State>@shipstate@</State>~";
            s1 += "			<ZIP>@shipzip@</ZIP>~";
            s1 += "			<TLD>@shipcountry@</TLD>~";
            s1 += "			<PhoneNumber>@1@</PhoneNumber>~";
            s1 += "			<Email>@2@</Email>~";
            s1 += "		</Address>~";
            s1 += "		<MethodCode>@methodcode@</MethodCode>~";
            s1 += "		<ShippingAmount>@shippingamount@</ShippingAmount>~";
            s1 += "		";
            s1 += "	</ShippingInformation>~";
            s1 += "@@PaymentInfo@@";    

            string pay1 = "";
            PaymentPlanSKU = new ArrayList();
            foreach (DataRow r in OrderSKU.Rows)
            {
                PaymentPlanSKU.Add(r["SkuId"].ToString());
            }           
            
            s1 += "	<OrderDetail>@orderdetailstuff@</OrderDetail>~";

            s1 += "";
            s1 += "	<CustomFields>~";
            s1 += "	    <Report>~";
            s1 += "		    <Field fieldName=[[[OriginalOrderDate[[[>@OrderDateFormat@</Field>~";
            s1 += "		    <Field fieldName=[[[URL[[[>www.Neova.com</Field>~";                        
            s1 += "		    <Field fieldName=[[[CustomerIP[[[>@CustomerIP@</Field>~";
            s1 += "	    </Report>~";
            s1 += "	</CustomFields>~";
            s1 += "</UDOARequest>~";
            s1 = s1.Replace("[[[", ((char)(34)).ToString());
            s1 = s1.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            return s1;
        }
        public void sendxml1(string verifyflag, string ordernumber)
        {
            bool IsGiftCardOrder = false;
            _intOrderID = Convert.ToInt32(ordernumber);
            string key1 = System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];
            string response1 = "";
            string DeclineToEmail = "";
            string FirstName = "";
            string LastName = "";
            _dtOrderSKU = null;
            _dtCustomerBillingAddress = null;
            _dtCustomerShippingAddress = null;
            Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
            Mediachase.eCF.BusLayer.Common.Util.EncryptionManager em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();

            string orderid1 = ordernumber;

            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());

            string s1a = getomxml(verifyflag, orderid1);

            decimal total1z;
            decimal shipping1z;

            //<UnitPrice>@4</UnitPrice>
            
            string stb = "<LineItem lineNumber=[[[@1[[[>~<PaymentPlanID>@paymentplan@</PaymentPlanID><ItemCode>@2</ItemCode>~<Quantity>@3</Quantity>~@4~@6~@7</LineItem>";
            stb = stb.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            stb = stb.Replace("[[[", ((char)(34)).ToString());

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                string s1 = s1a;
                string cc1 = "";
                string cc2 = "";




                cc1 = r["creditcardnumber"].ToString();
                cc2 = em.Decrypt(cc1);

                string tempPaymentTag = "";
                if (r["GiftCard"] != null && !r["GiftCard"].ToString().ToLower().Equals("true"))
                {
                    tempPaymentTag += "	<Payment type=[[[1[[[>~";
                    tempPaymentTag += "		<CardNumber>@cardnumber@</CardNumber>~";
                    tempPaymentTag += "		<CardVerification>@cvv@</CardVerification>~";
                    tempPaymentTag += "		<CardExpDateMonth>@cardexpmonth@</CardExpDateMonth>~";
                    tempPaymentTag += "		<CardExpDateYear>@cardexpyear@</CardExpDateYear>~";
                    tempPaymentTag += "		<RealTimeCreditCardProcessing>False</RealTimeCreditCardProcessing>~";
                    tempPaymentTag += "		<CardStatus>11</CardStatus>~";
                    tempPaymentTag += "		<CardAuthCode>@CardAuthCode@</CardAuthCode>~";
                    tempPaymentTag += "		<CardTransactionID></CardTransactionID>~";
                    tempPaymentTag += "	</Payment>~";
                }
                else if (r["GiftCard"] != null && r["GiftCard"].ToString().ToLower().Equals("true"))
                {
                    if (cc2.Equals("1111222233334444"))
                    {
                        tempPaymentTag += "	<Payment type=[[[11[[[>~";
                        tempPaymentTag += "		<PaidAmount>@PaidAmount@</PaidAmount>~";
                        tempPaymentTag += "		<GiftCertificateCode>@GiftCertificateCode@</GiftCertificateCode>~";
                        tempPaymentTag += "	</Payment>~";
                    }
                    else
                    {
                        tempPaymentTag += "	<Payment type=[[[1[[[>~";
                        tempPaymentTag += "		<CardNumber>@cardnumber@</CardNumber>~";
                        tempPaymentTag += "		<CardVerification>@cvv@</CardVerification>~";
                        tempPaymentTag += "		<CardExpDateMonth>@cardexpmonth@</CardExpDateMonth>~";
                        tempPaymentTag += "		<CardExpDateYear>@cardexpyear@</CardExpDateYear>~";
                        tempPaymentTag += "		<RealTimeCreditCardProcessing>False</RealTimeCreditCardProcessing>~";
                        tempPaymentTag += "		<CardStatus>11</CardStatus>~";
                        tempPaymentTag += "		<CardAuthCode>@CardAuthCode@</CardAuthCode>~";
                        tempPaymentTag += "		<CardTransactionID></CardTransactionID>~";
                        tempPaymentTag += "		<AdditionalPayment>~";
                        tempPaymentTag += "	<Payment type=[[[11[[[>~";
                        tempPaymentTag += "		<PaidAmount>@PaidAmount@</PaidAmount>~";
                        tempPaymentTag += "		<GiftCertificateCode>@GiftCertificateCode@</GiftCertificateCode>~";
                        tempPaymentTag += "	</Payment>~";
                        tempPaymentTag += "		</AdditionalPayment>~";
                        tempPaymentTag += "	</Payment>~";
                    }
                }
                else
                {
                    tempPaymentTag += "	<Payment type=[[[1[[[>~";
                    tempPaymentTag += "		<CardNumber>@cardnumber@</CardNumber>~";
                    tempPaymentTag += "		<CardVerification>@cvv@</CardVerification>~";
                    tempPaymentTag += "		<CardExpDateMonth>@cardexpmonth@</CardExpDateMonth>~";
                    tempPaymentTag += "		<CardExpDateYear>@cardexpyear@</CardExpDateYear>~";
                    tempPaymentTag += "		<RealTimeCreditCardProcessing>False</RealTimeCreditCardProcessing>~";
                    tempPaymentTag += "		<CardStatus>11</CardStatus>~";
                    tempPaymentTag += "		<CardAuthCode>@CardAuthCode@</CardAuthCode>~";
                    tempPaymentTag += "		<CardTransactionID></CardTransactionID>~";
                    tempPaymentTag += "	</Payment>~";
                }
                tempPaymentTag = tempPaymentTag.Replace("[[[", ((char)(34)).ToString());
                tempPaymentTag = tempPaymentTag.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());

                s1 = s1.Replace("@@PaymentInfo@@", tempPaymentTag);


                if (r["DiscountCode"] != null && r["DiscountCode"].ToString().Length > 1)
                {
                    s1 = s1.Replace("@PaidAmount@", fixstring(r["DiscountAmount"]));
                    s1 = s1.Replace("@GiftCertificateCode@", fixstring(r["DiscountCode"]));
                }   



                
                FirstName = r["billfirstname"].ToString();
                LastName = r["billlastname"].ToString();
                s1 = s1.Replace("@billfirstname@", fixstring(r["billfirstname"]));

                s1 = s1.Replace("@billlastname@", fixstring(r["billlastname"]));
                s1 = s1.Replace("@billaddress@", fixstring(r["billaddress"]));
                s1 = s1.Replace("@billaddress2@", fixstring(r["billaddress2"]));

                s1 = s1.Replace("@billcity@", fixstring(r["billcity"]));
                s1 = s1.Replace("@billstate@", fixstring(r["billstate"]));
                s1 = s1.Replace("@billzip@", fixstring(r["billzip"]));
                DataSet dsCountry = getsql("select * from country where countryid = " + r["billcountry"]);
                s1 = s1.Replace("@billcountry@", dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim());

                DateTime d = new DateTime();
                d = Convert.ToDateTime(fixstring(r["orderdate"]));

                s1 = s1.Replace("@orderdate@", d.ToString("yyyy-MM-dd HH:MM:ss"));
                s1 = s1.Replace("@OrderDateFormat@", d.ToString("yyyy-MM-dd"));
                s1 = s1.Replace("@AffiliateID@", fixstring(r["affiliateid"]));
                s1 = s1.Replace("@CardAuthCode@", _AuthCode);// Will enter something in future.


                //s1 = s1.Replace("@keycode@", fixstring(r["keycode"]));
                s1 = s1.Replace("@orderid@", "CS_N_" + fixstring(r["orderid"]));


                s1 = s1.Replace("@shipfirstname@", fixstring(r["shipfirstname"]));
                s1 = s1.Replace("@shiplastname@", fixstring(r["shiplastname"]));
                s1 = s1.Replace("@shipaddress@", fixstring(r["shipaddress"]));
                s1 = s1.Replace("@shipaddress2@", fixstring(r["shipaddress2"]));
                s1 = s1.Replace("@shipcity@", fixstring(r["shipcity"]));
                s1 = s1.Replace("@shipstate@", fixstring(r["shipstate"]));
                s1 = s1.Replace("@shipzip@", fixstring(r["shipzip"]));
                dsCountry = getsql("select * from country where countryid = " + r["shipcountry"]);
                s1 = s1.Replace("@shipcountry@", dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim());

                if (r["ipaddress"] != null && r["ipaddress"].ToString() != "")
                {
                    s1 = s1.Replace("@CustomerIP@", fixstring(em.Decrypt(r["ipaddress"].ToString())));
                }
                else
                {
                    s1 = s1.Replace("@CustomerIP@", fixstring(""));
                }

                s1 = s1.Replace("@cardnumber@", fixstring(cc2));
                s1 = s1.Replace("@cvv@", fixstring(r["creditcardcsc"]));
                s1 = s1.Replace("@cardexpmonth@", fixstring(r["creditcardexpiredmonth"]));
                s1 = s1.Replace("@cardexpyear@", fixstring(r["creditcardexpiredyear"]));
                s1 = s1.Replace("@1@", fixstring(r["phonenumber"]));
                s1 = s1.Replace("@2@", fixstring(r["email"]));
                DeclineToEmail = fixstring(r["email"]);
                total1z = Convert.ToDecimal(r["totalamount"].ToString());
                shipping1z = Convert.ToDecimal(r["shippingamount"].ToString());
                decimal cust1 = Convert.ToDecimal("0");
                string stc = "";
                string stc1 = "";
                int cnt;
                cnt = 0;
                decimal price1;
                decimal price2;
                int qty1;
                int qty2;
                decimal price2b = 0;
                string coupcode;
                bool extra;
                string rep1 = "no";
                string method1 = fixstring(r["methodcode"]);
                if (r["version"].ToString().ToLower().Contains("/holiday"))
                {
                    s1 = s1.Replace("@keyCode@", "NEOVA50");
                }

                if (r["version"].ToString().ToLower().Contains("/silcsheer_rm1"))
                {
                    s1 = s1.Replace("@keyCode@", "CSYS-NVRMRKT");
                }
                bool EmailRemarket = false;

                foreach (DataRow r1 in OrderSKU.Rows)
                {
                    if (r1["paymentplanid"].ToString().ToUpper().Equals("CSYS-NVRMRKT"))
                    {
                        EmailRemarket = true;
                    }
                }

                if (EmailRemarket)
                {
                    s1 = s1.Replace("@keyCode@", "CSYS-NVRMRKT");
                }

                // Added on 11/12/2013 Promo Code: MAGIC10
                bool MAGIC10CodeApplied = false;
                bool TAKE20CodeApplied = false;
                if (r["DiscountCode"] != null && r["DiscountCode"].ToString().Length > 1 && r["DiscountCode"].ToString().ToUpper().Equals("MAGIC10"))
                {
                    s1 = s1.Replace("@keyCode@", "CSYS-MAGIC10");
                    MAGIC10CodeApplied = true;
                }
                else if (r["DiscountCode"] != null && r["DiscountCode"].ToString().Length > 1 && r["DiscountCode"].ToString().ToUpper().Equals("TAKE20"))
                {
                    s1 = s1.Replace("@keyCode@", "CSYS-TAKE20");
                    TAKE20CodeApplied = true;
                }
                
                IsGiftCardOrder = false;
                foreach (DataRow r1 in OrderSKU.Rows)
                {
                    if (r1["GiftCardCode"] != null && (r1["GiftCardCode"].ToString().Length > 1))
                    {
                        IsGiftCardOrder = true;
                    }
                    
                    if (r1["skuid"].ToString().Contains("571")) // GiftCard Dont add it to Main Order XMl. We have seperate XML for that.
                    {
                        Console.WriteLine("Create Separate GiftCard Order : " + IsGiftCardOrder.ToString());
                    }
                    else
                    {
                        cnt++;
                        stc1 = stc;
                        stc += stb;
                        stc = stc.Replace("@1", cnt.ToString());
                        stc = stc.Replace("@2", fixstring(r1["skucode"].ToString()).Replace(" ", ""));
                        qty1 = Convert.ToInt32(r1["OrderSkuQuantity"].ToString());
                        qty2 = Convert.ToInt32(r1["OrderSkuQuantity"].ToString());
                    
                        price1 = Convert.ToDecimal(r1["FullAmount"].ToString());
                        if (r1["paymentplanid"] != DBNull.Value)
                        {
                            if (r1["paymentplanid"].ToString().Length > 0)
                            {
                                stc = stc.Replace("@paymentplan@", r1["paymentplanid"].ToString());
                            }
                            else
                            {
                                if (r["GiftCard"] != null && r["GiftCard"].ToString().ToLower().Equals("true"))
                                {
                                    stc = stc.Replace("@paymentplan@", "");
                                }
                                else
                                {
                                    stc = stc.Replace("@paymentplan@", "0");
                                }
                            }
                        }
                        else
                        {
                            if (r["GiftCard"] != null && r["GiftCard"].ToString().ToLower().Equals("true"))
                            {
                                stc = stc.Replace("@paymentplan@", "");
                            }
                            else
                            {
                                stc = stc.Replace("@paymentplan@", "0");
                            }
                        }
                        if (r1["StandingOrderId"] != DBNull.Value)
                        {
                            if (r1["StandingOrderId"].ToString().Length > 0)
                            {
                                stc = stc.Replace("@7", "<StandingOrder configurationID=[[[@StandingOrder@[[[></StandingOrder>");
                                stc = stc.Replace("@StandingOrder@", r1["StandingOrderId"].ToString());
                            }
                            else
                            {
                                stc = stc.Replace("@7", "");
                            }
                        }
                        else
                        {
                            stc = stc.Replace("@7", "");
                        }


                        price2b = price1;
                        coupcode = "";
                        extra = false;
                        if (r1["skuid"].ToString().Contains("334"))
                        {
                            qty2 = 2;
                            price1 = 14.95M;
                        }

                        //Console.WriteLine("total1:" + total1.ToString());
                        stc = stc.Replace("@3", qty2.ToString());

                        if (r1["skuid"].ToString() == "564" && MAGIC10CodeApplied == true)
                        {
                            decimal priceDiscounted = price1 - 10;
                            stc = stc.Replace("@4", "<UnitPrice>" + fixstring(priceDiscounted.ToString()) + "</UnitPrice>");
                        }
                        else
                        {
                            if (r1["skuid"].ToString() != "529")
                            {
                                stc = stc.Replace("@4", "<UnitPrice>" + fixstring(price1.ToString()) + "</UnitPrice>");
                            }
                            else
                            {
                                stc = stc.Replace("@4", "");
                            }
                        }

                        stc = stc.Replace("@5", "");                    
                        stc = stc.Replace("@6", "");
                        stc = stc.Replace("[[[", ((char)(34)).ToString());
                        s1 = s1.Replace("@keyCode@", r1["keyCode"].ToString());
                    }
                }
                
                if (r["GiftCard"] != null && !r["GiftCard"].ToString().ToLower().Equals("true"))
                {
                    if (MAGIC10CodeApplied == false && TAKE20CodeApplied==false && r["DiscountCode"] != null && r["DiscountCode"].ToString().Length > 1)
                    {
                        cnt++;
                        stc1 = stc;
                        stc += stb;
                        stc = stc.Replace("@1", cnt.ToString());
                        stc = stc.Replace("@2", fixstring(r["DiscountCode"].ToString()).Replace(" ", ""));
                        qty1 = 1;
                        qty2 = 1;

                        price1 = Convert.ToDecimal(r["DiscountAmount"].ToString());
                        stc = stc.Replace("@paymentplan@", "");
                        stc = stc.Replace("@7", "");
                        price2b = price1;
                        coupcode = "";
                        extra = false;
                        stc = stc.Replace("@3", qty2.ToString());
                        stc = stc.Replace("@4", "<UnitPrice>-" + fixstring(price1.ToString()) + "</UnitPrice>");
                        stc = stc.Replace("@5", "");
                        stc = stc.Replace("@6", "");
                        stc = stc.Replace("[[[", ((char)(34)).ToString());
                    }
                }
                string cardstatus = "";
                cardstatus = "0";
                string st1 = "";
                int cnt39 = 0;
                method1 = "4";

                if (r["version"].ToString().ToLower().Contains("/silcsheer_rm1"))
                {
                    method1 = "0";
                }


                if (EmailRemarket)
                {
                    method1 = "0";
                }
                
                total1z = Convert.ToDecimal(r["totalamount"].ToString());
                shipping1z = Convert.ToDecimal(r["shippingamount"].ToString());
                if (shipping1z > 1)
                {
                    method1 = "1";
                }

                

                s1 = s1.Replace("@methodcode@", method1);
                s1 = s1.Replace("@siteid@", "");
                s1 = s1.Replace("@total@", fixstring(total1z.ToString()));
                s1 = s1.Replace("@shippingamount@", fixstring(shipping1z.ToString()));
                s1 = s1.Replace("@rep@", rep1);
                stc = "<OrderDetail>" + stc + "</OrderDetail>";
                s1 = s1.Replace("<OrderDetail>@orderdetailstuff@</OrderDetail>", stc);
                string s2 = "";
                string url = Helper.AppSettings["OMXUrl"].ToString();
                s2 = HttpPost(url, s1);
                //<TotalMerchandise>99.98</TotalMerchandise>
               
                //Decimal tax3 = Convert.ToDecimal(fixempty(getfromto(s2, "<Tax>", "</Tax>")))/Convert.ToDecimal(Session["numpay"].ToString());
                response1 = s2;
                decimal salesTax = 0;
                if (response1 != "")
                {
                    
                        string CheckSuccess = "";
                        try
                        {
                            CheckSuccess = getfromto(response1, "<Success>", "</Success>");
                            if (CheckSuccess.Equals("1"))
                            {
                                if (verifyflag.ToString().Equals("True"))
                                {
                                    salesTax = Convert.ToDecimal(fixempty(getfromto(response1, "<Tax>", "</Tax>")));
                                    runsql("update [order] set orderstatusid = 2, Request='" + ClearAccents(fixQuot(s1)) + "', Response = '" + ClearAccents(fixQuot(response1)) + "', tax=" + salesTax.ToString("N2") + "where orderid= " + orderid1);
                                    runsql("update [ordershipment] set tax=" + salesTax.ToString("N2") + "where orderid= " + orderid1);
                                }
                                else
                                {
                                    runsql("update [order] set orderstatusid = 2, Request1='" + ClearAccents(fixQuot(s1)) + "', Response1 = '" + ClearAccents(fixQuot(response1)) + "' where orderid= " + orderid1);

                                    //Check for GC order.
                                    if (IsGiftCardOrder)
                                        new GiftCardOrders().UploadGifTCardToOrderMotion(orderid1);
                                }
                            }
                            else
                            {
                                runsql("update [order] set orderstatusid = 2, Request='" + ClearAccents(fixQuot(s1)) + "', Response = '" + ClearAccents(fixQuot(response1)) + "' where orderid= " + orderid1);
                                sendEmailToAdmin(orderid1);
                            }
                        }
                        catch (Exception E)
                        {
                            
                            Console.WriteLine("Error While sending message---" + E.Message);
                        }

                }
                response1 = "";
                s1 = "";
            }

        }

        public bool DoAuthorization(string orderid1)
        {
            bool auth = true;
            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());
            EBS.IntegrationServices.Providers.PaymentProviders.Request _request =
                       new EBS.IntegrationServices.Providers.PaymentProviders.Request();
            string cc = em.Decrypt(ds.Tables[0].Rows[0]["creditcardnumber"].ToString());
            _request.CardNumber = cc;
            _request.ExpireDate = (fixstring(ds.Tables[0].Rows[0]["creditcardexpiredyear"]) + "" + fixstring(ds.Tables[0].Rows[0]["creditcardexpiredmonth"]).PadLeft(2, '0'));
            _request.RequestType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentRequestType.A;
            _request.Amount = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0]["totalamount"].ToString()), 2);
            if (_request.Amount==0)
                return true;
            _request.FirstName = ClearAccents(ds.Tables[0].Rows[0]["billfirstname"].ToString());
            _request.LastName = ClearAccents(ds.Tables[0].Rows[0]["billlastname"].ToString());
            _request.Address1 = ClearAccents(ds.Tables[0].Rows[0]["billaddress"].ToString());
            _request.State = ClearAccents(ds.Tables[0].Rows[0]["billstate"].ToString());
            _request.City = ClearAccents(ds.Tables[0].Rows[0]["billcity"].ToString());
            DataSet dsCountry = getsql("select * from country where countryid = " + ds.Tables[0].Rows[0]["billcountry"]);
            _request.Country = fixstring(dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim());
            _request.InvoiceNumber = orderid1;
            _request.ZipCode = ClearAccents(ds.Tables[0].Rows[0]["billzip"].ToString()); ;
            _request.MethodType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentMethodType.CreditCard;
            _request.CardCvv = ClearAccents(ds.Tables[0].Rows[0]["creditcardcsc"].ToString());
            _request.CustomerID = orderid1;
            _request.Email = ClearAccents(ds.Tables[0].Rows[0]["email"].ToString()); ;
            EBS.IntegrationServices.Providers.PaymentProviders.Response _response =
            EBS.IntegrationServices.Providers.PaymentProviders.GatewayProvider.Instance("PaymentProvider").PerformRequest(_request);

            if (_response != null && _response.ResponseType == EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                runsql("update [order] set authorizationcode = '" + _AuthCode + "', confirmationcode='" + _TransactionId + "'  where orderid=" + orderid1);
                return true;
            }
            else
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                runsql("update [order] set authorizationcode = '" + _AuthCode + "', confirmationcode='" + _TransactionId + "'  where orderid=" + orderid1);
                return false;

            }

            return auth;

        }
        public string getfromto(string s22, string sa22, string sb22)
        {
            string s;
            string sa;
            string sb;

            s = s22.ToLower();
            sa = sa22.ToLower();
            sb = sb22.ToLower();


            string s1 = s;
            s1 = s1.Replace(sa, ((char)(200)).ToString());
            s1 = s1.Replace(sb, ((char)(201)).ToString());

            bool b = false;
            bool c = false;
            string s2 = "";
            for (int i = 0; i < s1.Length; i++)
            {
                if (c == false)
                {
                    if (s1[i].ToString() == ((char)(200)).ToString()) { b = true; c = true; }
                }
                if (s1[i].ToString() == ((char)(201)).ToString()) { b = false; }

                if (b == true)
                {
                    if ((s1[i].ToString() != ((char)(200)).ToString()) && (s1[i].ToString() != ((char)(201)).ToString()))
                    {
                        s2 += s1[i];
                    }
                }
            }
            return s2;

        }
        void UploadOrdersToOrderMotion()
        {
            try
            {
                foreach (DataRow _drOrder in Orders.Rows)
                {
                    _AuthCode = "";
                    try
                    {
                        if (_drOrder["OrderId"].ToString() != null)
                        {
                            sendxml1("True", _drOrder["OrderId"].ToString());
                            if (_drOrder["Authorizationcode"] != null)
                            {
                                if (_drOrder["Authorizationcode"].ToString().Length > 2)
                                {
                                    _AuthCode = _drOrder["Authorizationcode"].ToString();
                                    sendxml1("False", _drOrder["OrderId"].ToString());
                                }
                                else
                                {
                                    if (DoAuthorization(_drOrder["OrderId"].ToString()))
                                    {
                                        sendxml1("False", _drOrder["OrderId"].ToString());
                                    }
                                    else
                                    {
                                        //update the order tabe for coloum authStaus and orderstatusid = 7.
                                        runsql("update [order] set orderstatusid = 7, AuthStatus = 'False' where orderid= " + _drOrder["OrderId"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                if (DoAuthorization(_drOrder["OrderId"].ToString()))
                                {
                                    sendxml1("False", _drOrder["OrderId"].ToString());
                                }
                                else
                                {
                                    //update the order tabe for coloum authStaus and orderstatusid = 7.
                                    runsql("update [order] set orderstatusid = 7, AuthStatus = 'False' where orderid= " + _drOrder["OrderId"].ToString());
                                }
                            }


                        }

                        _dtOrderSKU = null;
                    }
                    catch (Exception e) {
                        sendEmailToAdmin(_drOrder["OrderId"].ToString());
                    
                    }
                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while uploading orders -- " + e.Message);
            }
        }
        void RejectedOrdersCheck()
        {
            string FirstName = "";
            string LastName = "";
            try
            {
                foreach (DataRow _drRejectedOrder in RejectedOrders.Rows)
                {
                    if (_drRejectedOrder["OrderId"].ToString() != null)
                    {
                        DataSet ds = null;
                        ds = getsql("select * from [order] where orderid = " + _drRejectedOrder["OrderId"].ToString());
                        string email = ds.Tables[0].Rows[0]["Email"].ToString();
                        DataSet dsAddress = null;
                        dsAddress = getsql("select * from [Address] where Addressid = " + _drRejectedOrder["billingaddressid"].ToString());
                        FirstName = dsAddress.Tables[0].Rows[0]["FirstName"].ToString();
                        LastName = dsAddress.Tables[0].Rows[0]["LastName"].ToString();

                        try
                        {
                            sendDeclineEmail(email, FirstName, LastName, _drRejectedOrder["BankAccountName"].ToString());
                        }
                        catch 
                        {
                            Console.WriteLine("Error while sending rejected orders email.");                        
                        }
                        runsql("update [order] set orderstatusid = 5 where orderid= " + _drRejectedOrder["OrderId"].ToString()); //changing the orderstatus from 7 to 5 which indicates they are rejected.                        

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while checking rejected orders -- " + e.Message);
            }

        }
        string fixempty(string s)
        {
            string s1 = s;
            if (s1 == "") { s1 = "0"; }
            return s1;
        }        
        string filtercc(string s)
        {
            string s1 = s;
            string sa = "";
            sa = getfromto(s, "<CardNumber>", "</CardNumber>");
            s1 = s1.Replace(sa, "----");
            return s1;
        }
        string fixquotes(string s)
        {
            string s1 = s;
            s1 = s1.Replace("'", "''");
            return s1;
        }
        static string HttpPost(string uri, string parameters)
        {
            // parameters: name1=value1&name2=value2	
            WebRequest webRequest = WebRequest.Create(uri);
            //string ProxyString = 
            //   System.Configuration.ConfigurationManager.AppSettings
            //   [GetConfigKey("proxy")];
            //webRequest.Proxy = new WebProxy (ProxyString, true);
            //Commenting out above required change to App.Config
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            Stream os = null;
            try
            { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException ex)
            {
                Console.WriteLine("HttpPost: request error");
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            try
            { // get the response
                WebResponse webResponse = webRequest.GetResponse();
                if (webResponse == null)
                { return null; }
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                Console.WriteLine("HttpPost: Response error");
            }
            return null;
        }
        public static void sendEmailToAdmin(string OrderId)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(Helper.AppSettings["AdminEmail"].ToString()));
                message.From = new MailAddress("info@Neova.com");                

                message.Subject = "Neova.com error processing OrderId = " + OrderId;
                sb.Append("Please Check this OrderId on Neova.com.<br /><br />");
                sb.Append("--------------------------------------------------------<br />");
                sb.Append("<br /><br />");
                sb.Append("Neova.com<br />");

                string st;
                st = sb.ToString();

                st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
                message.Body = st;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                Helper.SendMail(message);
            }
            catch { }
        }
        public static void sendDeclineEmail(string DeclineToEmail, string FirstName, string LastName, string version)
        {
            StringBuilder sb = new StringBuilder();
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(DeclineToEmail));
            message.From = new MailAddress("info@Neova.com");
            string sendemailto1 = DeclineToEmail;

                message.Subject = "Neova.com - Unable to Process Your Order";
                sb.Append("Dear").Append(" ").Append(FirstName).Append(" ").Append(LastName).Append(",<br /><br />");
                sb.Append("Thank you for placing an order with Neova.com.<br /><br />");
                sb.Append("Unfortunately, we were not able to authorize your credit card and submit your order for processing.<br /><br />");
                sb.Append("Please visit our Website (http://www.Neova.com) and place an order with a new card.<br /><br />");
                sb.Append("Thank you and have a great day!<br />");
                sb.Append("--------------------------------------------------------<br />");
                sb.Append("<br /><br />");
                sb.Append("Neova.com<br />");
                
           

            string st;
            st = sb.ToString();

            st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            message.Body = st;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            Helper.SendMail(message);            
        }
        public static string ClearAccents(string text)
        {
            //url = Regex.Replace(url, @"\s+", "-");
            string stFormD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString());
        }
        static string key1;
        static Mediachase.eCF.BusLayer.Common.Util.EncryptionManager em;
        public static string fixpath(string s)
        {
            string s1 = s;
            if (s1.IndexOf(".cus") != -1) { s1 = s1.ToLower().Replace("muradbatchfiles", "muradbatchfiles1"); }
            if (s1.IndexOf(".det") != -1) { s1 = s1.ToLower().Replace("muradbatchfiles", "muradbatchfiles2"); }
            if (s1.IndexOf(".ord") != -1) { s1 = s1.ToLower().Replace("muradbatchfiles", "muradbatchfiles3"); }
            return s1;
        }
        public static void encryptfile(string s)
        {

            string s21 = s;
            s21 = s21.Replace(".cus", "_cus.cus");
            s21 = s21.Replace(".det", "_det.det");
            s21 = s21.Replace(".ord", "_ord.ord");

            string s2 = s;
            s2 = fixpath(s2);

            if (File.Exists(s2))
            {
                File.Delete(s2);
            }
            File.Move(s, s2);


            string s1;
            s1 = "-e -u ~Conversion Systems <info@conversionsystems.com>~ -r ~Christopher Soto <csoto@murad.com>~ --yes ~filepath~";
            s1 = s1.Replace("~", ((char)(34)).ToString());
            s1 = s1.Replace("filepath", s2);
            Console.WriteLine("C:\\gnupg\\gpg.exe " + s1);

            string appname = "C:\\gnupg\\gpg.exe";
            string args1 = s1;

            runapp(appname, args1);

            if (File.Exists(s21))
            {
                File.Delete(s21);
            }
            File.Move(s2, s21);

            string st2z = s2;
            string st21z = s21;
            st2z = st2z.Replace(".cus", ".gpg");
            st2z = st2z.Replace(".det", ".gpg");
            st2z = st2z.Replace(".ord", ".gpg");

            st21z = st21z.Replace(".cus", ".gpg");
            st21z = st21z.Replace(".det", ".gpg");
            st21z = st21z.Replace(".ord", ".gpg");

            if (File.Exists(st21z))
            {
                File.Delete(st21z);
            }
            File.Move(st2z, st21z);
            


            string sta, stb;




            sta = st21z;
            stb = st21z;
            stb = stb.Replace(".gpg", ".pgp");

            

            if (File.Exists(stb))
            {
                File.Delete(stb);
            }
            File.Move(sta, stb);


            string stb1 = stb.ToLower();
            stb1 = stb1.Replace("batchstaging", "ftp");


            stb1 = stb1.Replace("_cus", ".cus");
            stb1 = stb1.Replace("_det", ".det");
            stb1 = stb1.Replace("_ord", ".ord");

            if (File.Exists(stb1))
            {
                File.Delete(stb1);
            }
            File.Move(stb, stb1);



            string s3 = s21;
            s3 = s3.Replace("_cus.cus",".cus");
            s3 = s3.Replace("_det.det",".det");
            s3 = s3.Replace("_ord.ord",".ord");
            if (File.Exists(s3))
            {
                File.Delete(s3);
            }
            File.Move(s21, s3);

        }
        public static string afterslash(string s)
        {
            string s1 = s;
            string s2 = "";
            bool b;

            b = false;

            int j;
            j = -1;


            for (int i = s.Length - 1; i >= 0; i--)
            {
                if ((s[i].ToString() == "\\") && (j == -1))
                {
                    j = i;
                }
            }

            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i].ToString() == "\\") && i == j)
                {
                    b = true;
                }
                else
                {
                    if (b == true) { s2 += s[i]; }
                }
            }

            return s2;
        }
        public static void runapp(string appname, string args1)
        {
            ProcessStartInfo pi = new ProcessStartInfo(appname);
            pi.Arguments = args1;

            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            Process p = Process.Start(pi);
            StreamReader sr = p.StandardOutput;

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine("Read line: {0}", line);
            }
            p.WaitForExit();

        }
        public static int LineCount(string source)
        {
            if (source != null)
            {
                string text = source;
                int numOfLines = 0;
                
                    FileStream FS = new FileStream(source, FileMode.Open,
                       FileAccess.Read, FileShare.Read);
                    StreamReader SR = new StreamReader(FS);
                    while (text != null)
                    {
                        text = SR.ReadLine();
                        if (text != null)
                        {
                            ++numOfLines;
                        }
                    }
                    SR.Close();
                    FS.Close();
                    return (numOfLines);
                
   
            }
            else
            {
                // Handle a null source here
                return (0);
            }
        }
        public static void Main(string[] args)
        {
            key1= System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];
            Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
            em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();

            Batch StartBatch = new Batch();
            Console.WriteLine("Neova.com Batch - Started");
            Console.WriteLine("Please Wait - ");
            StartBatch.UploadOrdersToOrderMotion();
            Console.WriteLine("End - Neova.com Batch - Started");
            Console.WriteLine("Checking Rejected Orders");
            StartBatch.RejectedOrdersCheck();

            UK StartUKBatch = new UK();
            Console.WriteLine("Nono UK Batch - Started");
            Console.WriteLine("Please Wait - ");
            StartUKBatch.UploadOrdersToOrderMotion();
            Console.WriteLine("End - Nono UK Batch - Started");

            Console.WriteLine("Task Completed - ");            

            
        }
    }
  
}
