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
            //s1 += "		<Parameter key=[[[RequestType[[[>UniversalDirectOrderAppending</Parameter>~";
            s1 += "		<Parameter key=[[[UDIAuthToken[[[>e89cfdcd07830045cf08c2a05cd7423c170515b4466a06a5c0469a0921703042064788d0c0774cca014320473d0aa980920f1b191ab9cfab74e200f4604b970b42802e63ce88f400733aeecc0d90e048e20866e02cdbf283a60599490f890df6c040d7082540d202c0ece6942e11d9330a1c7042a509b0605a35ef00665c92b</Parameter>~";

            DataSet ds80 = getsql("select keycode from ordersku,OrderMotionSkuInfo where OrderMotionSkuInfo.usekeycode=1 and OrderMotionSkuInfo.sku=ordersku.skuid and orderid=" + orderid1);

            s1 += "		<Parameter key=[[[Keycode[[[>" + ds80.Tables[0].Rows[0].ItemArray[0].ToString() + "</Parameter>~";
            s1 += "		<Parameter key=[[[VerifyFlag[[[>" + verifyflag + "</Parameter>~";
            s1 += "		<Parameter key=[[[QueueFlag[[[>False</Parameter>~";
            s1 += "	</UDIParameter>~";
            s1 += "	<Header>~";
            s1 += "		<OrderID>@orderid@</OrderID>~";
            s1 += "		<OrderDate>@orderdate@</OrderDate>~";
            s1 += "		<OriginType>3</OriginType>~";
            s1 += "		<StoreCode>www.thecontour.tv</StoreCode>~";
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
            s1 += "	<Payment type=[[[1[[[>~";
            s1 += "		<CardNumber>@cardnumber@</CardNumber>~";
            s1 += "		<CardVerification>@cvv@</CardVerification>~";
            s1 += "		<CardExpDateMonth>@cardexpmonth@</CardExpDateMonth>~";
            s1 += "		<CardExpDateYear>@cardexpyear@</CardExpDateYear>~";
            s1 += "		<RealTimeCreditCardProcessing>False</RealTimeCreditCardProcessing>~";
            s1 += "		<CardStatus>11</CardStatus>~";
            s1 += "		<CardAuthCode/>~";
            s1 += "		<CardTransactionID>@CardAuthCode@</CardTransactionID>~";

            string pay1 = "";
            PaymentPlanSKU = new ArrayList();
            foreach (DataRow r in OrderSKU.Rows)
            {
                PaymentPlanSKU.Add(r["SkuId"].ToString());
            }
            if (PaymentPlanSKU.Contains("330") || PaymentPlanSKU.Contains("396"))
            {
                pay1 = "2";
            }
            else
            {
                pay1 = "0";
            }

            s1 += " <PaymentPlanID>" + pay1.ToString() + "</PaymentPlanID>~";

            s1 += "	</Payment>~";
            s1 += "	<OrderDetail>@orderdetailstuff@</OrderDetail>~";

            s1 += "";
            s1 += "	<CustomFields>~";
            s1 += "	    <Report>~";
            s1 += "		    <Field fieldID=[[[1[[[>@rep@</Field>~";
            s1 += "	    </Report>~";
            s1 += "	</CustomFields>~";
            s1 += "</UDOARequest>~";
            s1 = s1.Replace("[[[", ((char)(34)).ToString());
            s1 = s1.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            return s1;
        }
        public void sendxml1(string verifyflag, string ordernumber)
        {
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
            string stb = "<LineItem lineNumber=[[[@1[[[>~<ItemCode>@2</ItemCode>~<Quantity>@3</Quantity>~<UnitPrice>@4</UnitPrice>~<Recurrence patternID=[[[@5[[[></Recurrence>~@6~</LineItem>";
            stb = stb.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            stb = stb.Replace("[[[", ((char)(34)).ToString());

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                string s1 = s1a;
                string cc1 = "";
                string cc2 = "";

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
                s1 = s1.Replace("@CardAuthCode@", _TransactionId);// Will enter something in future.


                //s1 = s1.Replace("@keycode@", fixstring(r["keycode"]));
                s1 = s1.Replace("@orderid@", "CS" + fixstring(r["orderid"]));


                s1 = s1.Replace("@shipfirstname@", fixstring(r["shipfirstname"]));
                s1 = s1.Replace("@shiplastname@", fixstring(r["shiplastname"]));
                s1 = s1.Replace("@shipaddress@", fixstring(r["shipaddress"]));
                s1 = s1.Replace("@shipaddress2@", fixstring(r["shipaddress2"]));
                s1 = s1.Replace("@shipcity@", fixstring(r["shipcity"]));
                s1 = s1.Replace("@shipstate@", fixstring(r["shipstate"]));
                s1 = s1.Replace("@shipzip@", fixstring(r["shipzip"]));
                dsCountry = getsql("select * from country where countryid = " + r["shipcountry"]);
                s1 = s1.Replace("@shipcountry@", dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim());

                cc1 = r["creditcardnumber"].ToString();
                cc2 = em.Decrypt(cc1);

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
                foreach (DataRow r1 in OrderSKU.Rows)
                {

                    if (r1["skuid"].ToString().Contains("389") || r1["skuid"].ToString().Contains("390") || r1["skuid"].ToString().Contains("337") || r1["skuid"].ToString().Contains("392") || r1["skuid"].ToString().Contains("393") || r1["skuid"].ToString().Contains("394") || r1["skuid"].ToString().Contains("395"))
                    {
                        continue;
                    }
                    cnt++;
                    stc1 = stc;
                    stc += stb;
                    stc = stc.Replace("@1", cnt.ToString());
                    stc = stc.Replace("@2", fixstring(r1["skucode"].ToString()).Replace(" ", "").Replace("-", ""));
                    qty1 = Convert.ToInt32(r1["OrderSkuQuantity"].ToString());
                    qty2 = Convert.ToInt32(r1["OrderSkuQuantity"].ToString());
                    price1 = Convert.ToDecimal(r1["Fullprice"].ToString());
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
                    stc = stc.Replace("@4", fixstring(price1.ToString()));
                    stc = stc.Replace("@5", "");
                    stc = stc.Replace("@5", "");
                    try
                    {
                        DateTime w = new DateTime();
                        if (Convert.ToDecimal(r["shippingamount"].ToString()) < 8)
                        {
                            w = d.AddDays(2);
                            stc = stc.Replace("@6", "<WaitDate>" + w.ToString("MM-dd-yyyy") + "</WaitDate>");
                        }
                        else
                        {
                            stc = stc.Replace("@6", "");
                        }
                    }
                    catch { }
                    if (r1["skuid"].ToString().Contains("337") || r1["skuid"].ToString().Contains("338"))
                    {
                        rep1 = "yes";
                    }
                }
                string cardstatus = "";
                cardstatus = "0";
                string st1 = "";
                int cnt39 = 0;

                if (PaymentPlanSKU.Contains("389") || PaymentPlanSKU.Contains("390") || PaymentPlanSKU.Contains("337") || PaymentPlanSKU.Contains("394"))
                {
                    s1 = s1.Replace("@methodcode@", "12");
                }
                else if (PaymentPlanSKU.Contains("392") || PaymentPlanSKU.Contains("393") || PaymentPlanSKU.Contains("395"))
                {
                    s1 = s1.Replace("@methodcode@", "13");
                }
                else
                {
                    s1 = s1.Replace("@methodcode@", "0");
                }
                if (r["sid"] !=null)
                {
                    try
                    {
                        s1 = s1.Replace("@siteid@", Helper.SID[r["sid"].ToString().ToUpper()].ToString());
                    }
                    catch (Exception)
                    {
                        s1 = s1.Replace("@siteid@", "");
                                                
                    }
                    
                }
                else
                {
                    s1 = s1.Replace("@siteid@", "");
                }
                total1z = Convert.ToDecimal(r["totalamount"].ToString());
                shipping1z = Convert.ToDecimal(r["shippingamount"].ToString());
                s1 = s1.Replace("@total@", fixstring(total1z.ToString()));
                s1 = s1.Replace("@shippingamount@", fixstring(shipping1z.ToString()));
                s1 = s1.Replace("@rep@", rep1);
                stc = "<OrderDetail>" + stc + "</OrderDetail>";
                s1 = s1.Replace("<OrderDetail>@orderdetailstuff@</OrderDetail>", stc);
                string s2 = "";
                string url = "https://api.omx.ordermotion.com/hdde/xml/udi.asp";
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
                                    runsql("update [order] set orderstatusid = 2, Authorizationcode='" + _TransactionId + "',  Request1='" + ClearAccents(fixQuot(s1)) + "', Response1 = '" + ClearAccents(fixQuot(response1)) + "', tax=" + salesTax.ToString("N2") + "where orderid= " + orderid1);
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
            }

        }

        public bool DoAuthorization(string orderid1)
        {
            bool auth = true;
            DataSet ds = new DataSet();
            ds = getsql("exec getorders4 " + orderid1.ToString());
            String post_url = "https://secure.authorize.net/gateway/transact.dll";

            Hashtable post_values = new Hashtable();

            //the API Login ID and Transaction Key must be replaced with valid values
            post_values.Add("x_login", "8A8u6smD2");
            post_values.Add("x_tran_key", "3Bua8YJM876e9v5H");

            post_values.Add("x_delim_data", "TRUE");
            post_values.Add("x_delim_char", '|');
            post_values.Add("x_relay_response", "FALSE");

            post_values.Add("x_type", "AUTH_ONLY");
            post_values.Add("x_method", "CC");

            string cc = em.Decrypt(ds.Tables[0].Rows[0]["creditcardnumber"].ToString());
            post_values.Add("x_card_num", cc);
            post_values.Add("x_exp_date", (fixstring(ds.Tables[0].Rows[0]["creditcardexpiredmonth"]) + "" + fixstring(ds.Tables[0].Rows[0]["creditcardexpiredyear"])));
            post_values.Add("x_test_request", "FALSE");
            post_values.Add("x_amount", ds.Tables[0].Rows[0]["totalamount"].ToString());
            post_values.Add("x_description", ClearAccents(ds.Tables[0].Rows[0]["billfirstname"].ToString() + " " + ds.Tables[0].Rows[0]["billlastname"].ToString() + "thecontour.tv - CS"));

            post_values.Add("x_first_name", ClearAccents(ds.Tables[0].Rows[0]["billfirstname"].ToString()));
            post_values.Add("x_last_name", ClearAccents(ds.Tables[0].Rows[0]["billlastname"].ToString()));
            post_values.Add("x_address", ClearAccents(fixstring(ds.Tables[0].Rows[0]["billaddress"])));
            post_values.Add("x_state", fixstring(ds.Tables[0].Rows[0]["billstate"]));
            post_values.Add("x_zip", fixstring(ds.Tables[0].Rows[0]["billzip"]));
            DataSet dsCountry = getsql("select * from country where countryid = " + ds.Tables[0].Rows[0]["billcountry"]);
            post_values.Add("x_country", fixstring(dsCountry.Tables[0].Rows[0]["Code"].ToString().Trim()));
            // Additional fields can be added here as outlined in the AIM integration
            // guide at: http://developer.authorize.net

            // This section takes the input fields and converts them to the proper format
            // for an http post.  For example: "x_login=username&x_tran_key=a1B2c3D4"
            String post_string = "";
            foreach (DictionaryEntry field in post_values)
            {
                post_string += field.Key + "=" + field.Value + "&";
            }
            post_string = post_string.TrimEnd('&');

            // create an HttpWebRequest object to communicate with Authorize.net
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
            objRequest.Method = "POST";
            objRequest.ContentLength = post_string.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            // post data is sent as a stream
            StreamWriter myWriter = null;
            myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(post_string);
            myWriter.Close();

            // returned values are returned as a stream, then read into a string
            String post_response;
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
            {
                post_response = responseStream.ReadToEnd();
                responseStream.Close();
            }

            // the response string is broken into an array
            // The split character specified here must match the delimiting character specified above
            Array response_array = post_response.Split('|');

            // the results are output to the screen in the form of an html numbered list.
            int i = 0;
            foreach (string value in response_array)
            {
                i++;
                if (i == 1)
                {
                    if (value != "1")
                    {
                        auth = false;
                    }
                }
                if (i == 7)
                {
                    _TransactionId = value;
                }
            }
            // individual elements of the array could be accessed to read certain response
            // fields.  For example, response_array[0] would return the Response Code,
            // response_array[2] would return the Response Reason Code.
            // for a list of response fields, please review the AIM Implementation Guide        
            if (_TransactionId == null || _TransactionId.ToString().Equals(""))
            {
                auth = false;
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
                    _TransactionId = "";
                    try
                    {
                        if (_drOrder["OrderId"].ToString() != null)
                        {
                            sendxml1("True", _drOrder["OrderId"].ToString());
                            if (_drOrder["Authorizationcode"] != null)
                            {
                                if (_drOrder["Authorizationcode"].ToString().Length > 2)
                                {
                                    _TransactionId = _drOrder["Authorizationcode"].ToString();
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
                    catch (Exception e) { }
                
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
        public static void sendDeclineEmail(string DeclineToEmail, string FirstName, string LastName, string version)
        {
            StringBuilder sb = new StringBuilder();
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(DeclineToEmail));
            message.From = new MailAddress("info@thecontour.tv");
            string sendemailto1 = DeclineToEmail;

                message.Subject = "Contour Abs - Unable to Process Your Order";
                sb.Append("Dear").Append(" ").Append(FirstName).Append(" ").Append(LastName).Append(",<br /><br />");
                sb.Append("Thank you for placing an order with Contour Abs.<br /><br />");
                sb.Append("Unfortunately, we were not able to authorize your credit card and submit your order for processing.<br /><br />");
                sb.Append("Please visit our Website (http://www.thecontour.tv) and place an order with a new card.<br /><br />");
                sb.Append("Thank you and have a great day!<br />");
                sb.Append("--------------------------------------------------------<br />");
                sb.Append("<br /><br />");
                sb.Append("Contour Abs<br />");
                
           

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
            Console.WriteLine("ContourAbs Batch - Started");
            Console.WriteLine("Please Wait - ");
            StartBatch.UploadOrdersToOrderMotion();
            Console.WriteLine("End - ContourAbs Batch - Started");
            StartBatch.RejectedOrdersCheck();
            Console.WriteLine("Task Completed - ");            

            
        }
    }
  
}
