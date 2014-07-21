using System;
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
using System.Web;
using System.Collections.Specialized;





namespace Com.ConversionSystems.GoldCanyon
{
    public class Germany : Com.ConversionSystems.UI.BasePage
    {
        private DataTable _dtOrders = null;
        private DataTable _dtRejectedOrders = null;
        private DataTable _dtBadOrders = null;
        private ArrayList AllOrders = new ArrayList();
        private DataTable _dtCustomerBillingAddress = null;
        private DataTable _dtCustomerShippingAddress = null;
        private DataTable _dtOrderSKU = null;
        private DataTable _dtSalesTaxRate = null;
        private int _intOrderID = 0;
        private int _intBillingAddress = 0;
        private int _intShippingAddress = 0;
        public static Hashtable skuMapping = null;
        int _intRecords = 0;
        private Hashtable UpSell = new Hashtable();
        private string _srtPath = "C:\\batchstaging\\BatchProcesses\\NonoBatchFiles";
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
        private string KeyCode = "";
        LogData log = new LogData();
        private DataTable Orders
        {
            get
            {
                //DAL.SQLServerDALUK.GetOrdersForXMLBatch(DateTime.ParseExact (DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd",CultureInfo.InvariantCulture), out _dtOrders);
                DAL.SQLServerDALGermany.GetOrdersForXMLBatch(out _dtOrders);
                //DAL.SQLServerDALUK.GetOrdersForXMLBatch(DateTime.Now.AddDays(-1), out _dtOrders);
                //DAL.SQLServerDALUK.GetOrdersForXMLBatch(new DateTime(2009, 06, 29), out _dtOrders);

                return _dtOrders;
            }
        }
        private DataTable RejectedOrders
        {
            get
            {
                if (_dtRejectedOrders == null)
                {
                    //DAL.SQLServerDALUK.GetOrdersForXMLBatch(DateTime.ParseExact (DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd",CultureInfo.InvariantCulture), out _dtOrders);
                    DAL.SQLServerDALGermany.GetRejectedOrdersForXMLBatch(out _dtRejectedOrders);
                    //DAL.SQLServerDALUK.GetOrdersForXMLBatch(DateTime.Now.AddDays(-1), out _dtOrders);
                    //DAL.SQLServerDALUK.GetOrdersForXMLBatch(new DateTime(2009, 06, 29), out _dtOrders);
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
                    DAL.SQLServerDALGermany.GetCustomerAddress(_intBillingAddress, out _dtCustomerBillingAddress);
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
                    DAL.SQLServerDALGermany.GetCustomerAddress(_intShippingAddress, out _dtCustomerShippingAddress);
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
                    DAL.SQLServerDALGermany.GetOrderSKU(_intOrderID, out _dtOrderSKU);
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
            using (SqlConnection conn = new SqlConnection(Helper.SQLServerDAO["mode"].ToString()))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter(s, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                conn.Close();

                return ds;
            }
        }
        private void runsql(string s)
        {
            using (SqlConnection conn = new SqlConnection(Helper.SQLServerDAO["mode"].ToString()))
            {
                conn.Open();
                SqlCommand cm = new SqlCommand(s, conn);
                cm.CommandType = CommandType.Text;
                cm.ExecuteNonQuery();
                conn.Close();
            }
        }
        public string getomxml(string verifyflag, string orderid1, string creditCardName)
        {
            _intOrderID = Convert.ToInt32(orderid1);
            string s1 = "";
            s1 += "<?xml version=[[[1.0[[[?>~";
            s1 += "<UDOARequest version=[[[2.00[[[>~";
            s1 += "	<UDIParameter>~";
            s1 += "		<Parameter key=[[[RequestType[[[>UniversalDirectOrderAppending</Parameter>~";
            s1 += "		<Parameter key=[[[UDIAuthToken[[[>0a718dfa0681f046860b96a0af75f66c1ae72cf251920c6cc042870a367040fc5e84fe984f73b61c0afad04c370ba9d0a8e8415ff9e78cc1e62004183047540b72c02ca60d5d65a155aecce10fb2704f780bb080549382c6ebebacc4dbcc08c7d04ec30831807edad664249d16b0e2290578a0496f0a0d10f9cae65cc8197e9</Parameter>~";

            DataSet ds80 = getsql("select keycode from ordersku,OrderMotionSkuInfo where OrderMotionSkuInfo.usekeycode=1 and OrderMotionSkuInfo.sku=ordersku.skuid and orderid=" + orderid1);
            s1 += "		<Parameter key=[[[Keycode[[[>@keyCode@</Parameter>~";

            s1 += "		<Parameter key=[[[VerifyFlag[[[>" + verifyflag + "</Parameter>~";
            s1 += "		<Parameter key=[[[QueueFlag[[[>True</Parameter>~";

            s1 += "	</UDIParameter>~";
            s1 += "	<Header>~";
            s1 += "		<OrderDate>@orderdate@</OrderDate>~";
            s1 += "		<OriginType>3</OriginType>~";
            s1 += "		<OrderID>@orderid@</OrderID>~";
            s1 += "		<StoreCode>Conversion Systems</StoreCode>~";
            s1 += "		<CustomerIP>@CustomerIP@</CustomerIP>~"; //ASK KEVIN AB THIS
            s1 += "		<Username />~";
            s1 += "		<Reference />~";
            s1 += "	</Header>~";
            s1 += "	<Customer>~";
            s1 += "		<Address type=[[[BillTo[[[>~";
            s1 += "			<TitleCode>0</TitleCode>~";
            s1 += "			<Company/>~";
            s1 += "			<Firstname>@billfirstname@</Firstname>~";
            s1 += "			<MidInitial />~";
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
            s1 += "	</Customer>~";
            s1 += "	<ShippingInformation>~";
            s1 += "		<Address type=[[[ShipTo[[[>~";
            s1 += "			<TitleCode>0</TitleCode>~";
            s1 += "			<Firstname>@shipfirstname@</Firstname>~";
            s1 += "			<Lastname>@shiplastname@</Lastname>~";
            s1 += "			<Address1>@shipaddress@</Address1>~";
            s1 += "			<Address2>@shipaddress2@</Address2>~";
            s1 += "			<City>@shipcity@</City>~";
            s1 += "			<State>@shipstate@</State>~";
            s1 += "			<ZIP>@shipzip@</ZIP>~";
            s1 += "			<TLD>@shipcountry@</TLD>~";
            s1 += "		</Address>~";
            s1 += "		<MethodCode>@methodcode@</MethodCode>~";
            s1 += "		<MethodName></MethodName>~";
            s1 += "		<ShippingAmount>@shippingamount@</ShippingAmount>~";
            s1 += "		";
            s1 += "	</ShippingInformation>~";
            s1 += "@@paymentInfo@@";
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
            s1 += "		    <Field fieldName=[[[URL[[[>www.trynono.com</Field>~";
            s1 += "		    <Field fieldName=[[[AffiliateID[[[>@AffiliateID@</Field>~";
            s1 += "		    <Field fieldName=[[[CustomerIP[[[>@CustomerIP@</Field>~";
            s1 += "		    <Field fieldName=[[[BankAccountNumber[[[>@BankAccountNumber@</Field>~";
            s1 += "		    <Field fieldName=[[[BankCode[[[>@BankCode@</Field>~";
            s1 += "		    <Field fieldName=[[[GuWID[[[>@EFTTranasactionID@</Field>~";
            s1 += "		    <Field fieldName=[[[BillingAgreementID[[[>@PayPalBillingAgreementID@</Field>~";
            s1 += "	    </Report>~";
            s1 += "	</CustomFields>~";
            s1 += "</UDOARequest>~";

            // We will be switching the keycodes for any orders which had laser card. 
            if (creditCardName.ToLower().Equals("lasercard"))
            {

                if (PaymentPlanSKU.Contains("390") || PaymentPlanSKU.Contains("391") || PaymentPlanSKU.Contains("392") || PaymentPlanSKU.Contains("400") || PaymentPlanSKU.Contains("561") || PaymentPlanSKU.Contains("562"))
                {
                    s1 = s1.Replace("@keyCode@", "WEB-IE8800LFL");
                }
                else if (PaymentPlanSKU.Contains("413") || PaymentPlanSKU.Contains("414") || PaymentPlanSKU.Contains("415") || PaymentPlanSKU.Contains("416"))
                {
                    s1 = s1.Replace("@keyCode@", "CSYS-IELTRAK1L");
                }
                else if (PaymentPlanSKU.Contains("417") || PaymentPlanSKU.Contains("418") || PaymentPlanSKU.Contains("419") || PaymentPlanSKU.Contains("420"))
                {
                    s1 = s1.Replace("@keyCode@", "CSYS-IELTRAK2L");
                }
                else
                {
                    KeyCode = ds80.Tables[0].Rows[0].ItemArray[0].ToString();                    
                }
            }
            else
            {
                KeyCode = ds80.Tables[0].Rows[0].ItemArray[0].ToString();                
            }

            try
            {
                skuMapping = null;
                DataSet dsKeyCode = new DataSet();
                dsKeyCode = getsql("exec GetKeyCodeByVersion " + orderid1.ToString());
                if (dsKeyCode.Tables[0].Rows.Count > 0)
                {
                    if (dsKeyCode.Tables[0].Rows[0]["OMXKeyCodes"] != null && !dsKeyCode.Tables[0].Rows[0]["OMXKeyCodes"].ToString().Equals(""))
                        KeyCode = dsKeyCode.Tables[0].Rows[0]["OMXKeyCodes"].ToString();


                    if (dsKeyCode.Tables[0].Rows[0]["OMXItemsCodes"] != null && !dsKeyCode.Tables[0].Rows[0]["OMXItemsCodes"].ToString().Equals(""))
                    {
                        skuMapping = new Hashtable();
                        string xml = dsKeyCode.Tables[0].Rows[0]["OMXItemsCodes"].ToString();
                        XmlDocument x = new XmlDocument();
                        x.LoadXml(xml);
                        XmlNodeList list = x.SelectNodes("/root/add");
                        if (list != null)
                        {
                            foreach (XmlNode item in list)
                            {
                                if (!skuMapping.ContainsKey(item.Attributes["itemcode"].Value.ToLower()))
                                    skuMapping.Add(item.Attributes["itemcode"].Value.ToLower(), item.Attributes["replacewith"].Value);
                            }
                        }
                    }


                }

            }
            catch (Exception E) { sendEmailToAdmin("<br />" + E.Message + "RELATED TO DYNAMIC KEYCODES", orderid1); }

            s1 = s1.Replace("[[[", ((char)(34)).ToString());
            s1 = s1.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            return s1;

        }
        public void sendxml1(string verifyflag, string ordernumber)
        {
            _intOrderID = Convert.ToInt32(ordernumber);
            _dtOrderSKU = null;
            string key1 = System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];
            string response1 = "";
            string DeclineToEmail = "";
            string FirstName = "";
            string LastName = "";
            Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
            Mediachase.eCF.BusLayer.Common.Util.EncryptionManager em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();

            string orderid1 = ordernumber;

            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());

            KeyCode = "";
            string s1a = getomxml(verifyflag, orderid1, ds.Tables[0].Rows[0]["Creditcardname"].ToString());

            decimal total1z;
            decimal shipping1z;


            string stb = "<LineItem lineNumber=[[[@1[[[><ItemCode>@2</ItemCode>@PaymentPlanTag@~<Quantity>@3</Quantity>~<UnitPrice>@4</UnitPrice>~<Recurrence patternID=[[[@5[[[></Recurrence>~</LineItem>";
            stb = stb.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            stb = stb.Replace("[[[", ((char)(34)).ToString());




            foreach (DataRow r in ds.Tables[0].Rows)
            {
                string s1 = s1a;
                string cc1 = "";
                string cc2 = "";
                string strPayment = "";
                string strBillingAgreementId = "";


                if (r["CreditCardName"].ToString().ToUpper().Equals("EFT"))
                {
                    strPayment = "";
                    strPayment += "	<Payment type=[[[7[[[>~";           
                    strPayment += "		<BankAccountNumber>@BankAccountNumber@</BankAccountNumber>~";
                    strPayment += "		<BankRoutingNumber>@BankCode@</BankRoutingNumber>~";                    
                    strPayment += "	</Payment>~";

                    //Uncomment the above lines once we are live with wirecard EFT. For now EFT orders are sent as OPENINVOICE
                    //strPayment += "	<Payment type=[[[6[[[>~";
                    //strPayment += "	</Payment>~";

                }
                else if (r["CreditCardName"].ToString().ToUpper().Equals("PAYPAL"))
                {
                    strPayment = "";
                    strPayment += "	<Payment type=[[[13[[[>~";
                    strPayment += "		<PaidAmount>@AuthorizationAmount@</PaidAmount>~";
                    strPayment += "		<TransactionID>@TranasactionID@</TransactionID>~";
                    strPayment += "		<PayerID>@PayerID@</PayerID>~";
                    strPayment += "		<BuyerEmail>@BuyerEmail@</BuyerEmail>~";
                    strPayment += "		<PayPalTransactionType>1</PayPalTransactionType>~";
                    strPayment += "	</Payment>~";
                }
                else if (r["CreditCardName"].ToString().ToUpper().Equals("OPENINVOICE") || r["CreditCardName"].ToString().ToUpper().Equals("POI"))
                {
                    if (r["billcountry"].ToString().Equals("320"))
                    {
                        strPayment += "	<Payment type=[[[18[[[>~";
                        strPayment += "	</Payment>~";
                    }
                    else
                    {
                        strPayment += "	<Payment type=[[[6[[[>~";
                        strPayment += "	</Payment>~";
                    }
                    if (r["CusTempDataID"] != null)
                    {
                        if (r["CusTempDataID"].ToString().Length > 0)
                        {
                            DataSet dsKeyCode = getsql("SELECT CREDITCHECKRESPONSE FROM CUSTEMPDATA WHERE ID = " + r["CusTempDataID"].ToString());
                            if (dsKeyCode.Tables[0].Rows.Count > 0)
                            {
                                if (dsKeyCode.Tables[0].Rows[0]["CreditCheckResponse"] != null)
                                {
                                    if (dsKeyCode.Tables[0].Rows[0]["CreditCheckResponse"].ToString().ToLower().Contains("risk-response") || dsKeyCode.Tables[0].Rows[0]["CreditCheckResponse"].ToString().ToLower().Contains("riskresponse"))
                                        KeyCode = "WEB-GR8800CC";
                                }
                            }
                        }
                    }
                    //
                }
                else
                {

                    strPayment += "	<Payment type=[[[1[[[>~";
                    strPayment += "		<CardNumber>@cardnumber@</CardNumber>~";
                    strPayment += "		<CardVerification>@cvv@</CardVerification>~";
                    strPayment += "		<CardExpDateMonth>@cardexpmonth@</CardExpDateMonth>~";
                    strPayment += "		<CardExpDateYear>@cardexpyear@</CardExpDateYear>~";
                    strPayment += "		<RealTimeCreditCardProcessing>False</RealTimeCreditCardProcessing>~";
                    strPayment += "		<CardStatus>11</CardStatus>~";
                    strPayment += "		<CardAuthCode>@CardAuthCode@</CardAuthCode>~";
                    strPayment += "	</Payment>~";
                }
                strPayment = strPayment.Replace("[[[", ((char)(34)).ToString());
                strPayment = strPayment.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());

                s1 = s1.Replace("@@paymentInfo@@", strPayment.ToString());
                s1 = s1.Replace("@keyCode@", KeyCode);
                //updating above params:

                try
                {
                    NameValueCollection nvcQueryString = null;
                    nvcQueryString = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(r["PaypalResponse"].ToString()));
                    if (nvcQueryString["BILLINGAGREEMENTID"] != null)
                    {
                        strBillingAgreementId = nvcQueryString["BILLINGAGREEMENTID"].ToString();
                    }
                }
                catch { }

                s1 = s1.Replace("@AuthorizationAmount@", Math.Round(Convert.ToDecimal(fixstring(r["totalamount"])), 2).ToString());
                s1 = s1.Replace("@TranasactionID@", _TransactionId);
                s1 = s1.Replace("@PayerID@", _AuthCode);
                s1 = s1.Replace("@BuyerEmail@", fixstring(r["email"]));
                s1 = s1.Replace("@PayPalBillingAgreementID@", fixstring(strBillingAgreementId));
                s1 = s1.Replace("@BuyerEmail@", fixstring(r["email"]));


                FirstName = r["billfirstname"].ToString();
                LastName = r["billlastname"].ToString();
                s1 = s1.Replace("@billfirstname@", fixstring(r["billfirstname"]));

                s1 = s1.Replace("@billlastname@", fixstring(r["billlastname"]));
                s1 = s1.Replace("@billaddress@", fixstring(r["billaddress"]));
                s1 = s1.Replace("@billaddress2@", fixstring(r["billaddress2"]));

                s1 = s1.Replace("@billcity@", r["billcity"].ToString());

                s1 = s1.Replace("@billstate@", "");
                s1 = s1.Replace("@billzip@", r["billzip"].ToString());

                DataSet dsCountry = getsql("select * from country where countryid = " + r["billcountry"]);
                s1 = s1.Replace("@billcountry@", dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim());

                DateTime d = new DateTime();
                d = Convert.ToDateTime(fixstring(r["orderdate"]));

                s1 = s1.Replace("@orderdate@", d.ToString("yyyy-MM-dd HH:MM:ss"));
                s1 = s1.Replace("@OrderDateFormat@", d.ToString("yyyy-MM-dd"));
                s1 = s1.Replace("@AffiliateID@", fixstring(r["affiliateid"]));
                s1 = s1.Replace("@CardAuthCode@", _AuthCode);


                s1 = s1.Replace("@keycode@", fixstring(r["keycode"]));
                s1 = s1.Replace("@orderid@", "GER" + fixstring(r["orderid"]));


                s1 = s1.Replace("@shipfirstname@", fixstring(r["shipfirstname"]));
                s1 = s1.Replace("@shiplastname@", fixstring(r["shiplastname"]));
                s1 = s1.Replace("@shipaddress@", fixstring(r["shipaddress"]));
                s1 = s1.Replace("@shipaddress2@", fixstring(r["shipaddress2"]));
                s1 = s1.Replace("@shipcity@", fixstring(r["shipcity"]));

                s1 = s1.Replace("@shipstate@", "");
                s1 = s1.Replace("@shipzip@", fixstring(r["shipzip"]));
                dsCountry = getsql("select * from country where countryid = " + r["shipcountry"]);
                s1 = s1.Replace("@shipcountry@", dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim());

                cc1 = r["creditcardnumber"].ToString();
                cc2 = em.Decrypt(cc1);
                if (r["ipaddress"] != null && r["ipaddress"].ToString() != "")
                {
                    s1 = s1.Replace("@CustomerIP@", fixstring(em.Decrypt(r["ipaddress"].ToString())));
                }
                else
                {
                    s1 = s1.Replace("@CustomerIP@", fixstring(""));
                }
                s1 = s1.Replace("@cardnumber@", fixstring(cc2));


                if (r["CreditCardName"].ToString().ToUpper().Equals("EFT"))
                {
                    s1 = s1.Replace("@BankAccountNumber@", fixstring(cc2));
                    s1 = s1.Replace("@EFTTranasactionID@", _TransactionId);
                    if (r["SortCode"] != null)
                    {
                        s1 = s1.Replace("@BankCode@", r["SortCode"].ToString());
                    }
                    else
                        s1 = s1.Replace("@BankCode@", "");

                }
                else
                {
                    s1 = s1.Replace("@BankAccountNumber@", "");
                    s1 = s1.Replace("@EFTTranasactionID@", "");
                    s1 = s1.Replace("@BankCode@", "");

                }
                s1 = s1.Replace("@BankAccountNumber@", fixstring(cc2));

                s1 = s1.Replace("@cvv@", fixstring(r["creditcardcsc"]));
                s1 = s1.Replace("@cardexpmonth@", fixstring(r["creditcardexpiredmonth"]));
                s1 = s1.Replace("@cardexpyear@", fixstring(r["creditcardexpiredyear"]));
                s1 = s1.Replace("@1@", fixstring(r["phonenumber"]));
                s1 = s1.Replace("@2@", fixstring(r["email"]));
                if (r["ipaddress"] != null)
                {
                    s1 = s1.Replace("@CustomerIP@", fixstring(em.Decrypt(r["ipaddress"].ToString())));
                }
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

                string method1 = fixstring(r["methodcode"]);

                foreach (DataRow r1 in OrderSKU.Rows)
                {
                    if (r1["skuid"].ToString().Equals("336"))
                    {
                        continue;
                    }
                    cnt++;
                    stc1 = stc;
                    stc += stb;
                    stc = stc.Replace("@1", cnt.ToString());
                    if (r1["skuid"].ToString().Equals("577") ||
                        r1["skuid"].ToString().Equals("579") ||
                        r1["skuid"].ToString().Equals("581") ||
                        r1["skuid"].ToString().Equals("736") ||
                        r1["skuid"].ToString().Equals("738") ||
                        r1["skuid"].ToString().Equals("740") ||
                        r1["skuid"].ToString().Equals("742") ||
                        r1["skuid"].ToString().Equals("744") ||
                        r1["skuid"].ToString().Equals("746")

                        
                        )
                    {
                        stc = stc.Replace("@PaymentPlanTag@", "<PaymentPlanID>@0</PaymentPlanID>");
                        stc = stc.Replace("@0", "1");
                    }
                    else
                    {
                        stc = stc.Replace("@PaymentPlanTag@", "");
                    }
                    if (r1["skucode"].ToString().Equals("CONTINUITY"))
                    {
                        stc = stc.Replace("@2", "");
                    }
                    else
                    {
                        stc = stc.Replace("@2", fixstring(r1["skucode"].ToString()).Replace(" ", ""));
                    }
                    qty1 = Convert.ToInt32(r1["OrderSkuQuantity"].ToString());
                    qty2 = Convert.ToInt32(r1["OrderSkuQuantity"].ToString());
                    price1 = Convert.ToDecimal(r1["Fullprice"].ToString());
                    price2b = price1;
                    coupcode = "";
                    extra = false;

                    stc = stc.Replace("@3", qty2.ToString());
                    stc = stc.Replace("@4", fixstring(price1.ToString()));
                    stc = stc.Replace("@5", "");

                }


                if (PaymentPlanSKU.Contains("735") ||
                    PaymentPlanSKU.Contains("737") ||
                    PaymentPlanSKU.Contains("738") ||
                    PaymentPlanSKU.Contains("739") ||
                    PaymentPlanSKU.Contains("740") ||
                    PaymentPlanSKU.Contains("741") ||
                    PaymentPlanSKU.Contains("742") ||
                    PaymentPlanSKU.Contains("743") ||
                    PaymentPlanSKU.Contains("744") ||
                    PaymentPlanSKU.Contains("745") ||
                    PaymentPlanSKU.Contains("746") 
                    )
                {
                      cnt++;
                    stc += stb;
                    stc = stc.Replace("@1", cnt.ToString());
                    stc = stc.Replace("@2", fixstring("FREE3YRWAR".ToString()).Replace(" ", "").Replace("-", ""));
                    stc = stc.Replace("@3", "1".ToString());
                    stc = stc.Replace("@PaymentPlanTag@", "");                    
                    stc = stc.Replace("@4", fixstring("0".ToString()));
                    stc = stc.Replace("@5", "");                
                }
                string cardstatus = "";
                cardstatus = "0";
                string st1 = "";
                int cnt39 = 0;


                if (dsCountry.Tables[0].Rows[0]["Code"].ToString().ToLower().Equals("ch"))
                {
                    s1 = s1.Replace("@methodcode@", "7");
                }
                else
                {
                    s1 = s1.Replace("@methodcode@", "6");

                }
                total1z = Convert.ToDecimal(r["totalamount"].ToString());
                shipping1z = Convert.ToDecimal(r["shippingamount"].ToString());
                s1 = s1.Replace("@total@", fixstring(total1z.ToString()));
                s1 = s1.Replace("@shippingamount@", fixstring(shipping1z.ToString()));
                stc = "<OrderDetail>" + stc + "</OrderDetail>";
                s1 = s1.Replace("<OrderDetail>@orderdetailstuff@</OrderDetail>", stc);
                string s2 = "";
                string url = Helper.AppSettings["OMXUrl"].ToString();
                s2 = HttpPost(url, s1);

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
                                //we are not recording sales tax for UK/GER Site.
                                salesTax = Convert.ToDecimal(fixempty(getfromto(response1, "<Tax>", "</Tax>")));
                                runsql("update [order] set orderstatusid = 2, Request='" + ClearAccents(fixQuot(s1)) + "', Response = '" + ClearAccents(fixQuot(response1)) + "' where orderid= " + orderid1);
                                //runsql("update [ordershipment] set tax=" + salesTax.ToString("N2") + "where orderid= " + orderid1);
                            }
                            else
                            {
                                runsql("update [order] set orderstatusid = 2, Authorizationcode='" + _AuthCode + "',  Request1='" + ClearAccents(fixQuot(s1)) + "', Response1 = '" + ClearAccents(fixQuot(response1)) + "' where orderid= " + orderid1);
                            }
                        }
                        else
                        {
                            runsql("update [order] set orderstatusid = 2, Request='" + ClearAccents(fixQuot(s1)) + "', Response = '" + ClearAccents(fixQuot(response1)) + "' where orderid= " + orderid1);
                        }
                    }
                    catch (Exception E)
                    {

                        Console.WriteLine("Error While sending message---" + E.Message);
                    }

                }
                response1 = "";
                s1 = "";
                _dtOrderSKU = null;
            }

        }

        public bool DoAuthorization(string orderid1)
        {
            bool auth = false;
            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());
            if (ds.Tables[0].Rows[0]["CreditCardName"].ToString().ToUpper().Equals("PAYPAL") || ds.Tables[0].Rows[0]["CreditCardName"].ToString().ToUpper().Equals("OPENINVOICE"))
                return true;
            else if (ds.Tables[0].Rows[0]["CreditCardName"].ToString().ToUpper().Equals("POI"))
                return DoAuthorizationWithPOI(orderid1);
            else if (ds.Tables[0].Rows[0]["CreditCardName"].ToString().ToUpper().Equals("EFT"))
                return true;//return DoAuthorizationWithEFT(orderid1);
            EBS.IntegrationServices.Providers.PaymentProviders.Request _request =
                       new EBS.IntegrationServices.Providers.PaymentProviders.Request();
            string cc = em.Decrypt(ds.Tables[0].Rows[0]["creditcardnumber"].ToString());
            _request.CardNumber = cc;
            _request.ExpireDate = (fixstring(ds.Tables[0].Rows[0]["creditcardexpiredyear"]) + "" + fixstring(ds.Tables[0].Rows[0]["creditcardexpiredmonth"]).PadLeft(2, '0'));
            _request.RequestType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentRequestType.A;
            _request.Amount = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0]["totalamount"].ToString()), 2);
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
            EBS.IntegrationServices.Providers.PaymentProviders.GatewayProvider.Instance("PaymentProvider1").PerformRequest(_request);

            if (_response != null && _response.ResponseType == EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                auth = true;
            }
            else
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                auth = false;

            }
            runsql("update [order] set authorizationcode = '" + _AuthCode + "', confirmationcode='" + _TransactionId + "' where orderid=" + orderid1);
            return auth;

        }
        public bool DoAuthorizationWithEFT(string orderid1)
        {
            bool auth = false;
            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());
            EBS.IntegrationServices.Providers.PaymentProviders.Request _request =
                       new EBS.IntegrationServices.Providers.PaymentProviders.Request();
            string cc = em.Decrypt(ds.Tables[0].Rows[0]["creditcardnumber"].ToString());
            _request.CardNumber = cc;
            _request.CurrencyCode = "EUR";
            Hashtable additionalInfo = new Hashtable();
            additionalInfo.Add("WireCardRequestType", "DEBIT");
            _request.AdditionalInfo = additionalInfo;

            _request.BankAbaCode = ClearAccents(ds.Tables[0].Rows[0]["sortCode"].ToString());
            _request.ExpireDate = (fixstring(ds.Tables[0].Rows[0]["creditcardexpiredyear"]) + "" + fixstring(ds.Tables[0].Rows[0]["creditcardexpiredmonth"]).PadLeft(2, '0'));
            _request.RequestType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentRequestType.A;
            _request.Amount = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0]["totalamount"].ToString()), 2);
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
            EBS.IntegrationServices.Providers.PaymentProviders.GatewayProvider.Instance("PaymentProviderWireCardETF").PerformRequest(_request);

            if (_response != null && _response.ResponseType == EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                auth = true;
            }
            else
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                auth = false;

            }
            runsql("update [order] set authorizationcode = '" + _AuthCode + "', confirmationcode='" + _TransactionId + "' where orderid=" + orderid1);
            return auth;

        }
        public bool DoAuthorizationWithPOI(string orderid1)
        {
            bool auth = false;
            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());
            EBS.IntegrationServices.Providers.PaymentProviders.Request _request =
                       new EBS.IntegrationServices.Providers.PaymentProviders.Request();
            string cc = em.Decrypt(ds.Tables[0].Rows[0]["creditcardnumber"].ToString());
            //Account number and ABA code is always same for wirecard POI.
            _request.CardNumber = "54455";
            _request.BankAbaCode = "51230800";
            _request.CurrencyCode = "EUR";
            Hashtable additionalInfo = new Hashtable();
            additionalInfo.Add("WireCardRequestType", "POI");
            _request.AdditionalInfo = additionalInfo;


            _request.ExpireDate = (fixstring(ds.Tables[0].Rows[0]["creditcardexpiredyear"]) + "" + fixstring(ds.Tables[0].Rows[0]["creditcardexpiredmonth"]).PadLeft(2, '0'));
            _request.RequestType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentRequestType.A;
            _request.Amount = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0]["totalamount"].ToString()), 2);
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
            EBS.IntegrationServices.Providers.PaymentProviders.GatewayProvider.Instance("PaymentProviderWireCardETF").PerformRequest(_request);

            if (_response != null && _response.ResponseType == EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                auth= true;
            }
            else
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                auth= false;

            }
            runsql("update [order] set authorizationcode = '" + _AuthCode + "', confirmationcode='" + _TransactionId + "' where orderid=" + orderid1);
            return auth;

        }
        public bool CreditCheck(string orderid1)
        {
            bool auth = true;
            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());
            EBS.IntegrationServices.Providers.PaymentProviders.Request _request =
                       new EBS.IntegrationServices.Providers.PaymentProviders.Request();
            string cc = em.Decrypt(ds.Tables[0].Rows[0]["creditcardnumber"].ToString());
            _request.CardNumber = "54455";
            _request.CurrencyCode = "EUR";
            Hashtable additionalInfo = new Hashtable();
            additionalInfo.Add("WireCardRequestType", "CREDITCHECK");
            _request.AdditionalInfo = additionalInfo;

            _request.BankAbaCode = "51230800";
            _request.ExpireDate = (fixstring(ds.Tables[0].Rows[0]["creditcardexpiredyear"]) + "" + fixstring(ds.Tables[0].Rows[0]["creditcardexpiredmonth"]).PadLeft(2, '0'));
            _request.RequestType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentRequestType.A;
            _request.Amount = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0]["totalamount"].ToString()), 2);
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
            EBS.IntegrationServices.Providers.PaymentProviders.GatewayProvider.Instance("PaymentProviderWireCardETF").PerformRequest(_request);

            if (_response != null && _response.ResponseType == EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                return true;
            }
            else
            {
                _AuthCode = _response.AuthCode;
                _TransactionId = _response.TransactionID;
                return false;

            }
            //runsql("update [order] set authorizationcode = " + _AuthCode + ", confirmationcode=" + _TransactionId + " where orderid=" + orderid1);
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
        public void UploadOrdersToOrderMotion()
        {
            key1 = System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];
            Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
            em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();

            try
            {
                foreach (DataRow _drOrder in Orders.Rows)
                {
                    try
                    {
                        if (_drOrder["OrderId"].ToString() != null)
                        {
                            sendxml1("True", _drOrder["OrderId"].ToString());

                            if (_drOrder["creditcardname"].ToString().ToLower().Equals("lasercard"))
                            {
                                try
                                {
                                    _AuthCode = "";
                                    _TransactionId = "";
                                    sendxml1("False", _drOrder["OrderId"].ToString());
                                }
                                catch(Exception E) { sendEmailToAdmin("<br />" + E.Message, _drOrder["OrderId"].ToString()); }
                                continue;
                            }
                            if (_drOrder["Authorizationcode"] != null)
                            {
                                if (_drOrder["Authorizationcode"].ToString().Length > 2)
                                {
                                    _AuthCode = _drOrder["Authorizationcode"].ToString();
                                    _TransactionId = _drOrder["Confirmationcode"].ToString();
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
                                        runsql("update [order] set orderstatusid = 7  where orderid= " + _drOrder["OrderId"].ToString());
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
                                    runsql("update [order] set orderstatusid = 7 where orderid= " + _drOrder["OrderId"].ToString());
                                }
                            }


                        }

                        _dtOrderSKU = null;
                    }
                    catch (Exception E) { sendEmailToAdmin("<br />" + E.Message, _drOrder["OrderId"].ToString()); }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while uploading orders -- " + e.Message);
            }
        }
        public void RejectedOrdersCheck()
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
                            try
                            {
                                if (!Directory.Exists(_srtPath))
                                {
                                    Directory.CreateDirectory(_srtPath);

                                }
                                filenameCustomer = _srtPath + "\\req" + _drRejectedOrder["orderid"].ToString() + ".xml"; ;
                                StreamWriter fileCustomer = new StreamWriter(filenameCustomer);
                                fileCustomer.WriteLine(ds.Tables[0].Rows[0]["request"].ToString());
                                fileCustomer.Close();

                                filenameCustomer = _srtPath + "\\resp" + _drRejectedOrder["orderid"].ToString() + ".xml"; ;
                                StreamWriter fileCustomerRes = new StreamWriter(filenameCustomer);
                                fileCustomerRes.WriteLine(ds.Tables[0].Rows[0]["response"].ToString());
                                fileCustomerRes.Close();

                            }
                            catch
                            {
                            }
                            sendDeclineEmail(email, FirstName, LastName, _drRejectedOrder["BankAccountName"].ToString());
                        }
                        catch
                        {
                            Console.WriteLine("Error while sending rejected orders email.");
                        }
                        runsql("delete from [order] where orderid=" + _drRejectedOrder["OrderId"].ToString());

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
            webRequest.Timeout = System.Threading.Timeout.Infinite;
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
        public static void sendEmailToAdmin(string desc, string orderid)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(Helper.AppSettings["AdminEmail"].ToString()));
                message.CC.Add(new MailAddress(Helper.AppSettings["MonitorEmail"].ToString()));
                message.From = new MailAddress("info@trynono.com");
                message.Subject = "Error processing order -  Nonohair.de" + orderid;
                sb.Append("Please address this issue.<br /><br />");
                sb.Append(desc + ".<br /><br />");
                sb.Append("Thank you and have a great day!<br />");
                sb.Append("--------------------------------------------------------<br />");
                sb.Append("<br /><br />");
                sb.Append("Conversion Systems<br />");


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
            message.From = new MailAddress("info@nonohair.de");
            string sendemailto1 = DeclineToEmail;

            message.Subject = "Unfähig zur Abwicklung Ihrer Bestellung";
            sb.Append("Liebe").Append(" ").Append(FirstName).Append(" ").Append(LastName).Append(",<br /><br />");
            sb.Append("Danke für eine Bestellung mit nein! Nein! Haar.<br /><br />");
            sb.Append("Leider waren wir nicht in der Lage, Ihre Kreditkarte / Bankkonto zu genehmigen und schicken Sie Ihre Bestellung zur Verarbeitung.<br /><br />");
            sb.Append("Bitte besuchen Sie unsere Webseite (http://www.nonohair.de/) und eine Bestellung mit einer neuen Karte.<br /><br />");
            sb.Append("Vielen Dank und einen schönen Tag!<br />");
            sb.Append("--------------------------------------------------------<br />");
            sb.Append("<br /><br />");            
            sb.Append("cs@nonohair.de<br />");

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
            s3 = s3.Replace("_cus.cus", ".cus");
            s3 = s3.Replace("_det.det", ".det");
            s3 = s3.Replace("_ord.ord", ".ord");
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

    }

}
