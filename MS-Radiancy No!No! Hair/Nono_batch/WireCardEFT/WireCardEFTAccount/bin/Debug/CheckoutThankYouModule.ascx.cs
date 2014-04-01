using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.eCF.BusFacade;
using Mediachase.eCF.Web.BaseClasses;
using Mediachase.eCF.ClientLib2;
using Mediachase.eCF.ClientLib2.Util;
using System.Text;
using Mediachase.eCF.BusLayer.Objects;
using Mediachase.eCF.BusLayer.SiteStructure.Products;
using System.Data.SqlClient;
using Mediachase.eCF.BusLayer.Orders;
using com.majorsavings.smartsellonline;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace Mediachase.eCF.PublicStore.MasterTemplates.Default.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CheckoutThankYouModule : Mediachase.eCF.Web.BaseClasses.UserControlBase
    {
        bool ItemInfo = false;
        string table = " <tr><td valign=~top~ style=~padding-right: 40px; padding-top: 20px; padding-bottom: 20px;~>{Description}</td><td valign=~top~ style=~padding-right: 40px; padding-top: 20px; padding-bottom: 20px;~>{Quantity}</td><td valign=~top~ style=~padding-right: 40px; padding-top: 20px; padding-bottom: 20px;~>${Price}*</td></tr>";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (System.Configuration.ConfigurationSettings.AppSettings["SSLRedirect"].ToString().Equals("True"))
            {
                if (Request.Url.ToString().Contains("https://"))
                {
                    if (Request.Url.ToString().Contains("www"))
                    {
                        Response.Redirect((Request.Url.ToString().Replace("https:/", "http:/").Replace("index.aspx", "")));
                    }
                    else
                    {
                        Response.Redirect((Request.Url.ToString().Replace("https:/", "http:/").Replace("http://", "http://www.").Replace("index.aspx", "")));
                    }

                }
            }

            txtMessage.Text = "You have to check this out! Using ZQuiet has stopped my snoring - I think it could really help your snoring, too! It's simple, comfortable and affordable. Here's the website: www.ZQuiet.com - you should go there right now!";
            if (!this.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            //this.Page.Master.FindControl("TrackingModule1").Visible = false;
            DataBind();
            DisplayPage();
            LoadGoogleAnalytics();
            LoadDFAPixel();

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            _SuccessMesg.Text = "";
            if (!validation())
            {

                StringBuilder EmailBody = new StringBuilder();
                EmailBody.Append("<br />" + txtMessage.Text + " <br />");

                MailMessage _oMailMessage = new MailMessage(txtYourEmail.Text.Trim(), txtFriendsEmail.Text.Trim(), txtYourName.Text + " has sent you a message.", EmailBody.ToString());
                _oMailMessage.IsBodyHtml = true;
                _oMailMessage.Body = EmailBody.ToString();
                SendMail(_oMailMessage);

                _SuccessMesg.Text = "<div style=\"color:green;\">Your message has been sent!</div>";

                txtYourName.Text = "";
                txtYourEmail.Text = "";
                txtFriendsName.Text = "";
                txtFriendsEmail.Text = "";
                txtMessage.Text = "";

            }
            myHiddenInput.Value = "true";
        }

        private bool validation()
        {
            bool _bError = false;
            _YourNameError.Text = "";
            _YourEmailError.Text = "";
            _FriendsNameError.Text = "";
            _FriendsEmailError.Text = "";
            _MessageError.Text = "";
            if (txtYourName.Text.Trim().Equals(""))
            {
                _YourNameError.Text = "<div class=\"frienderror\">Please enter your name.</div>";
                _bError = true;
            }
            if (txtYourEmail.Text.Trim().Equals(""))
            {
                _YourEmailError.Text = "<div class=\"frienderror\">Please enter your email.</div>";
                _bError = true;
            }
            else if (txtYourEmail.Text.Trim() != "")
            {
                string patternStrict = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex reStrict = new Regex(patternStrict);

                if (!(reStrict.IsMatch(txtYourEmail.Text)))
                {
                    _YourEmailError.Text = "<div class=\"frienderror\">Please make sure email is valid.</div>";
                    _bError = true;
                }
            }
            if (txtFriendsName.Text.Trim().Equals(""))
            {
                _FriendsNameError.Text = "<div class=\"frienderror\">Please enter friend's email.</div>";
                _bError = true;
            }
            if (txtFriendsEmail.Text.Trim().Equals(""))
            {
                _FriendsEmailError.Text = "<div class=\"frienderror\">Please enter friend's email.</div>";
                _bError = true;
            }
            else if (txtFriendsEmail.Text.Trim() != "")
            {
                string patternStrict = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex reStrict = new Regex(patternStrict);

                if (!(reStrict.IsMatch(txtFriendsEmail.Text)))
                {
                    _FriendsEmailError.Text = "<div class=\"frienderror\">Please make sure email address is valid.</div>";
                    _bError = true;
                }
            }

            if (txtMessage.Text.Trim().Equals(""))
            {
                _MessageError.Text = "<div class=\"frienderror\">Please enter your message.</div>";
                _bError = true;
            }
            return _bError;

        }

        public static bool SendMail(MailMessage oMsg)
        {
            SmtpClient client;
            bool bResult = false;
            int intFlag = -1;
            try
            {
                intFlag = -2;
                oMsg.BodyEncoding = System.Text.Encoding.UTF8;


                intFlag = -3;

                oMsg.CC.Clear();
                oMsg.Bcc.Clear();


                intFlag = -4;

                client = new SmtpClient("67.59.160.215");
                client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Send(oMsg);
                bResult = true;
            }
            catch (Exception ex)
            {

            }
            return bResult;
        }
        
        public void LoadDFAPixel()
        {
            try
            {
				    string order_number = "";
                    OrderInfo order = (OrderInfo)ClientContext.Context["LatestOrder"];
                    Mediachase.eCF.BusLayer.Orders.Order _order;
                    _order = new Mediachase.eCF.BusLayer.Orders.Order();
                    _order.LoadByPrimaryKey(order.OrderId);
                                       

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<iframe src=~https://fls.doubleclick.net/activityi;src=2751412;type=zquie919;cat=zquie885;ord=TRANSACTION~?~ width=~1~ height=~1~ frameborder=~0~></iframe>");
                    sb.Replace("TRANSACTION", order.OrderId.ToString());
                    sb.Replace("~", ((char)(34)).ToString());
                    DFAPixel.Text = sb.ToString();
                    StringBuilder sb1 = new StringBuilder();
                    sb1.Append("<iframe src =~_SOURCE_~width=~1~ height=~1~ frameborder=~0~></iframe>");
                    sb1.Replace("_SOURCE_", "https://fls.doubleclick.net/activityi;src=2751412;type=zquie919;cat=zquie885;ord=TRANSACTION~?~");
                    sb1.Replace("TRANSACTION", order.OrderId.ToString());
                    sb1.Replace("~", ((char)(34)).ToString());
                    DFAPixelFrame.Text = sb1.ToString();
                    
             }
            catch (Exception e) { }
        }
        
        
        
        public void LoadGoogleAnalytics()
        {
            try
            {
                OrderInfo order = (OrderInfo)ClientContext.Context["LatestOrder"];
                Mediachase.eCF.BusLayer.Orders.Order _order;
                _order = new Mediachase.eCF.BusLayer.Orders.Order();
                _order.LoadByPrimaryKey(order.OrderId);
                _order = new Mediachase.eCF.BusLayer.Orders.Order();
                _order.LoadByPrimaryKey(order.OrderId);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<script>");
                sb.AppendLine("var pageTracker = _gat._getTracker('UA-8292597-1');");
                sb.AppendLine("pageTracker._trackPageview();");
                sb.AppendFormat("pageTracker._addTrans('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}' );\n", _order.OrderId, "", Math.Round(_order.TotalPrice, 2), Math.Round(_order.Tax, 2), Math.Round(_order.ShippingCost, 2), order.BillingAddress.City, order.BillingAddress.State, order.BillingAddress.Country);

                DataSet ds = null;
                string sql = "select * 	from dbo.v_OrderSKU where orderid = " + _order.OrderId;
                ds = getsql(sql);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    sb.AppendFormat("pageTracker._addItem('{0}','{1}','{2}','{3}','{4}','{5}');\n", _order.OrderId, row["SKUCode"].ToString(), row["SKUName"].ToString(), "", Math.Round(Convert.ToDouble(row["SKUPrice"].ToString()), 2), row["OrderSKUQuantity"].ToString());
                }

                sb.AppendLine("pageTracker._trackTrans();");
                sb.AppendLine("</script>");
                GoogleAnalytics.Text = sb.ToString();
            }
            catch (Exception e) { }
        }
        private DataSet getsql(string s)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["EcfSqlConnection"].ConnectionString);
            conn.Open();
            SqlDataAdapter adp = new SqlDataAdapter(s, conn);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            conn.Close();
            return ds;
        }
        private bool isnum(char c)
        {
            bool b = false;
            if ((((int)c) >= 48) && (((int)c) <= 57)) b = true;
            return b;
        }
        private int countnums(string s)
        {
            string s1 = s;

            int i;
            int j = 0;
            for (i = 0; i < s1.Length; i++)
            {
                if (isnum(s1[i]) == true) { j++; }
            }

            return j;

        }
        public string GetCleanPhoneNumber(string phone)
        {
            string result = string.Empty;

            int i = 0;
            if (phone.Length > 0)
            {
                i = countnums(phone);
            }

            int cnum = 0;

            foreach (char c in phone)
            {
                cnum++;
                if (!((i > 10) && (cnum == 1) && (c == '1')))
                {
                    if (char.IsDigit(c))
                        result += c;
                }
            }
            return result;
        }
        public void DisplayPage()
        {
            try
            {
                ItemInfo = false;
                OrderInfo order = (OrderInfo)ClientContext.Context["LatestOrder"];
                Mediachase.eCF.BusLayer.Orders.Order _order;
                _order = new Mediachase.eCF.BusLayer.Orders.Order();

                string orderID = Request["orderID"];
                if (order == null && string.IsNullOrEmpty(orderID) == false)
                {
                    _order.LoadByPrimaryKey(int.Parse(orderID));
                }
                else
                {

                    _order.LoadByPrimaryKey(order.OrderId);
                }
                if (_order == null)
                    Response.Redirect("~");
                StringBuilder sb = new StringBuilder();

                DataSet ds21 = getsql("select uid from [order] where orderid=" + _order.OrderId.ToString());
                LitPixel2.Text = "";
                if (ds21.Tables.Count > 0)
                {
                    if (ds21.Tables[0].Rows.Count > 0)
                    {
                        if (ds21.Tables[0].Rows[0].ItemArray[0] != null)
                        {
                            if (ds21.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper() == "ASN_ZZZ_0001")
                            {
                                LitPixel2.Text = "<img src='https://www.howlifeworks.com/Lib/conversion_tracker.aspx?Adv_ID=403' width='1' height='1' alt='' />";
                            }
                        }
                    }
                }

                //  foreach (OrderItem i in order.OrderItems)
                foreach (OrderSku i in _order.OrderSku)
                {
                    StringBuilder _sb = new StringBuilder();
                    _sb.Append(table);

                    // _sb.Replace("{Description}", i.ProductTitle);
                    _sb.Replace("{Description}", i.Sku.MetaFields["Everything_Description"].ToString());
                    //_sb.Replace("{Description}", i.Sku.Name);
                    if (i.SkuId == 347)
                    {
                        _sb.Replace("{Quantity}", i.Quantity.ToString());
                        _sb.Replace("{Price}*", "7.95".ToString());
                    }
                    else if (i.SkuId == 336)
                    {
                        ItemInfo = true;
                        _sb.Replace("{Quantity}", i.Quantity.ToString());
                        _sb.Replace("{Price}", "9.95".ToString());
                    }
                    else
                    {
                        _sb.Replace("{Quantity}", i.Quantity.ToString());
                        _sb.Replace("{Price}*", Math.Round(Convert.ToDouble((i.Quantity * i.Price).ToString()), 2).ToString());
                    }

                    sb.Append(_sb.ToString());
                }
                sb.Replace("~", ((char)(34)).ToString());
                LiteralTableRows.Text = sb.ToString();
                LiteralSubTotal.Text = Math.Round(_order.SubTotal, 2).ToString();
                LiteralShipping.Text = Math.Round(_order.ShippingCost, 2).ToString();
                LiteralTax.Text = Math.Round(_order.Tax, 2).ToString();
                LiteralTotal.Text = Math.Round((_order.SubTotal + _order.ShippingCost + _order.Tax), 2).ToString();

                DataSet shippingAddress = getsql("SELECT A.*, country.Code FROM [ORDERSHIPMENT] OS INNER JOIN ADDRESS A ON A.ADDRESSID = OS.ADDRESSID LEFT OUTER JOIN COUNTRY ON COUNTRY.COUNTRYID = A.COUNTRYID WHERE ORDERID = " + _order.OrderId);
                if (shippingAddress != null && shippingAddress.Tables.Count > 0)
                {
                    LiteralName.Text = shippingAddress.Tables[0].Rows[0]["FirstName"].ToString() + " " + shippingAddress.Tables[0].Rows[0]["LastName"].ToString();
                    LiteralAddress.Text = shippingAddress.Tables[0].Rows[0]["Address1"].ToString();
                    LiteralAddress2.Text = shippingAddress.Tables[0].Rows[0]["Address2"].ToString();
                    LiteralCity.Text = shippingAddress.Tables[0].Rows[0]["City"].ToString();
                    LiteralState.Text = shippingAddress.Tables[0].Rows[0]["StateProvince"].ToString();
                    LiteralZip.Text = shippingAddress.Tables[0].Rows[0]["ZipPostalCode"].ToString();
                    LiteralCountry.Text = shippingAddress.Tables[0].Rows[0]["Code"].ToString();
                }

                LiteralEmail.Text = _order.Email.ToString();

                DataSet billingAddress = getsql("SELECT A.*, country.Code FROM [ORDER] O INNER JOIN ADDRESS A ON A.ADDRESSID = O.BILLINGADDRESSID LEFT OUTER JOIN COUNTRY ON COUNTRY.COUNTRYID = A.COUNTRYID WHERE ORDERID = " + _order.OrderId);
                if (billingAddress != null && shippingAddress.Tables.Count > 0)
                {
                    LiteralName_b.Text = shippingAddress.Tables[0].Rows[0]["FirstName"].ToString() + " " + shippingAddress.Tables[0].Rows[0]["LastName"].ToString();
                    LiteralAddress_b.Text = shippingAddress.Tables[0].Rows[0]["Address1"].ToString();
                    LiteralAddress2_b.Text = shippingAddress.Tables[0].Rows[0]["Address2"].ToString();
                    LiteralCity_b.Text = shippingAddress.Tables[0].Rows[0]["City"].ToString();
                    LiteralState_b.Text = shippingAddress.Tables[0].Rows[0]["StateProvince"].ToString();
                    LiteralZip_b.Text = shippingAddress.Tables[0].Rows[0]["ZipPostalCode"].ToString();
                    LiteralCountry_b.Text = shippingAddress.Tables[0].Rows[0]["Code"].ToString();
                }

                ConversionSystems.MediaChase.Web.ProductHelper.SendOrderCompletedEmail(_order, ClientContext.Context.CurrentLanguage, RM, ItemInfo);


                //Get 3rd Party Upsell.
                broker mySSOLWS = new broker();
                WDIAuthHeader myAuthHeader = new WDIAuthHeader();

                mySSOLWS.Url = "https://secure.majorsavings.com/SmartSellOnlineWS/broker.asmx?wsdl";

                myAuthHeader.Username = "SleepWell";
                myAuthHeader.Password = "222Swc";
                myAuthHeader.AffiliateId = 98;

                mySSOLWS.WDIAuthHeaderValue = myAuthHeader;

                SSTeaser myTeaser = default(SSTeaser);
                string ccexp = Convert.ToDateTime(_order.CreditCardExpired).ToString("MMyyyy");
                string ccnum = _order.CreditCardNumber;
                string cctype = "";
                if (_order.CreditCardName.ToLower().Contains("visa"))
                {
                    cctype = "V";
                }
                else if (_order.CreditCardName.ToLower().Contains("AmericanExpress"))
                {
                    cctype = "A";
                }
                else if (_order.CreditCardName.ToLower().Contains("Discover"))
                {
                    cctype = "D";
                }
                else if (_order.CreditCardName.ToLower().Contains("MasterCard"))
                {
                    cctype = "M";
                }
                else if (_order.CreditCardName.ToLower().Contains("amex"))
                {
                    cctype = "A";
                }
                string countryCode = "";
                if (LiteralCountry_b.Text.ToLower().Contains("us"))
                {
                    countryCode = "1";
                }
                else if (LiteralCountry_b.Text.Contains("canada"))
                {
                    countryCode = "2";
                }

                string t = "ZQUIET2" + "" + _order.Email + LiteralName_b.Text + LiteralAddress_b.Text + "" + LiteralCity_b.Text + LiteralState_b.Text + LiteralZip_b.Text + countryCode + GetCleanPhoneNumber(billingAddress.Tables[0].Rows[0]["PhoneNumber"].ToString()) + cctype + ccnum + ccexp + _order.OrderId + "N";

                myTeaser = mySSOLWS.getTeaser("ZQUIET2", "", _order.Email, billingAddress.Tables[0].Rows[0]["FirstName"].ToString(), billingAddress.Tables[0].Rows[0]["LastName"].ToString(),
                                    LiteralAddress_b.Text, "", LiteralCity_b.Text, LiteralState_b.Text, LiteralZip_b.Text, countryCode, GetCleanPhoneNumber(billingAddress.Tables[0].Rows[0]["PhoneNumber"].ToString()), cctype, ccnum, ccexp, _order.OrderId, "N");
                test.Text = t;
                if (myTeaser.rc == 0)
                {
                    SmartSell.NavigateUrl = myTeaser.target;
                    SmartSell.ImageUrl = myTeaser.source;
                    SmartSell.Visible = true;
                }
                else
                {
                    SmartSell.Visible = false;
                }
                PanelItemInfo.Visible = ItemInfo;
            }
            catch (Exception e)
            {


            }
        }
    }
}
