using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;

using Mediachase.eCF.BusFacade;
using Mediachase.eCF.BusLayer.Orders;
using Mediachase.eCF.BusLayer.SiteStructure.Products;
using Mediachase.eCF.ClientLib2;
using Mediachase.eCF.BusLayer.Shipping;
using System.Security.Cryptography;
using Mediachase.eCF.BusLayer.Objects;
/// <summary>
/// Summary description for OrderHelper
/// </summary>
public class OrderHelper
{
    public static bool AuthorizeOrder(int orderID)
    {
        EBS.IntegrationServices.Providers.PaymentProviders.Request _request =
                new EBS.IntegrationServices.Providers.PaymentProviders.Request();

        Order order = new Order();
        order.LoadByPrimaryKey(orderID);
        double Amount = (double)order.SubTotal + (double)order.ShippingCost + (double)order.Tax;

	
        _request.CardNumber = order.CreditCardNumber;
        _request.CurrencyCode = order.CurrencyId;
        DateTime expireDate = DateTime.MinValue;
        DateTime.TryParse(order.CreditCardExpired, out expireDate);
        _request.ExpireDate = string.Format("{0}{1}", zeropad(expireDate.Month.ToString("N0"), 2), Right(expireDate.Year.ToString(), 2));
        _request.Amount = Amount;
        _request.FirstName = order.BillingAddress.FirstName;
        _request.LastName = order.BillingAddress.LastName;
        _request.Address1 = order.BillingAddress.Address1;
        _request.City = order.BillingAddress.Address2;
        _request.State = order.BillingAddress.StateProvince;
        _request.InvoiceNumber = orderID.ToString();
        _request.ZipCode = order.BillingAddress.ZipPostalCode;
        _request.MethodType = EBS.IntegrationServices.Providers.PaymentProviders.PaymentMethodType.CreditCard;
        _request.CardCvv = order.CreditCardCSC;
        _request.CustomerID = order.Customer.CustomerId.ToString();

	
        //for testing
        
//	if (_request.CardCvv != "111")
  //      {

  //          order.OrderStatusId = 8; //Rejected
   //         order.Save();
    //        return false;
    //    }
	
        EBS.IntegrationServices.Providers.PaymentProviders.Response _response =
          EBS.IntegrationServices.Providers.PaymentProviders.GatewayProvider.Instance("PaymentProvider").PerformRequest(_request);


        if (_response != null && _response.ResponseType != EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
        {
            order.AuthorizationCode = _response.TransactionID;
            order.OrderStatusId = 8; //Rejected
            order.Save();
            return false;
        }
        else if (_response != null && _response.ResponseType == EBS.IntegrationServices.Providers.PaymentProviders.TransactionResponseType.Approved)
        {
            order.AuthorizationCode = _response.TransactionID;

            order.OrderStatusId = 7; //New Order
            //order.CreditCardNumber = string.Empty;
            order.Save();
            return true;
        }

        return true;
    }
    private static string Right(string param, int length)
    {
        string result = param.Substring(param.Length - length, length);
        return result;
    }
    private static string zeropad(string s, int i)
    {
        string s1 = s;
        while (s1.Length < i)
        {
            s1 = "0" + s1;
        }
        return s1;
    }
    private static void ExecuteSQLString(string s)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["EcfSqlConnection2"].ConnectionString);
        conn.Open();
        SqlCommand cm = new SqlCommand(s, conn);
        cm.CommandType = CommandType.Text;
        cm.ExecuteNonQuery();
        conn.Close();
    }
    public static bool UpdateOrder(OrderInformation currentOrderInformation)
    {
        Order order = new Order();
        if (order.LoadByPrimaryKey(currentOrderInformation.orderID))
        {
            AddressInfo addressInfo = new AddressInfo();
            addressInfo.Address1 = currentOrderInformation.billingAddress;
            addressInfo.City = currentOrderInformation.city;
            addressInfo.State = currentOrderInformation.state;
            addressInfo.PostalCode = currentOrderInformation.zipCode;
            addressInfo.PhoneNumber = currentOrderInformation.phone;
            addressInfo.FirstName = currentOrderInformation.firstName;
            addressInfo.LastName = currentOrderInformation.lastName;
            addressInfo.AddressId = order.BillingAddressId;

            string SQL = "UPDATE ADDRESSINFO SET FIRSTNAME = '{0}', LASTNAME = '{1}', CITY = '{2}', STATE = '{3}', POSTALCODE = '{4}', ADDRESS1 = '{5}', PHONENUMBER = '{6}' WHERE ADDRESSID = {7}";
            ExecuteSQLString(string.Format(SQL, addressInfo.FirstName, addressInfo.LastName, addressInfo.City, addressInfo.State, addressInfo.PostalCode, addressInfo.Address1, addressInfo.PhoneNumber, addressInfo.AddressId));
        }
        return true;
    }
    public class OrderInformation
    {
        private int _cvv;
        public int CVV
        {
            get { return _cvv; }
            set { _cvv = value; }
        }
        private DateTime _expireDate;
        public DateTime expireDate
        {
            get { return _expireDate; }
            set { _expireDate = value; }
        }
        private string _creditCardNumber;
        public string CreditCardNumber
        {
            get { return _creditCardNumber; }
            set { _creditCardNumber = value; }
        }
        private int _orderID;
        public int orderID
        {
            set { _orderID = value; }
            get { return _orderID; }
        }
        private string _firstName;
        public string firstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        private string _lastName;
        public string lastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        private string _billingAddress;
        public string billingAddress
        {
            get { return _billingAddress; }
            set { _billingAddress = value; }
        }
        private string _city;
        public string city
        {
            get { return _city; }
            set { _city = value; }
        }
        private string _state;
        public string state
        {
            get { return _state; }
            set { _state = value; }
        }
        private string _zipCode;
        public string zipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }
        private string _phone;
        public string phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        private string _email;
        public string email
        {
            get { return _email; }
            set { _email = value; }
        }
        private int _orderStatusID;
        public int orderStatusID
        {
            get { return _orderStatusID; }
            set { _orderStatusID = value; }
        }
        public OrderInformation(int orderID, DataSet ds)
        {
            _orderID = orderID;
            if (ds != null && ds.Tables.Count > 0)
            {
                _firstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
                _lastName = ds.Tables[0].Rows[0]["LastName"].ToString();
                _phone = ds.Tables[0].Rows[0]["PhoneNumber"].ToString();
                _billingAddress = ds.Tables[0].Rows[0]["Address1"].ToString();
                _city = ds.Tables[0].Rows[0]["City"].ToString();
                _email = ds.Tables[0].Rows[0]["Email"].ToString();
                _state = ds.Tables[0].Rows[0]["StateProvince"].ToString();
                _zipCode = ds.Tables[0].Rows[0]["ZipPostalCode"].ToString();
                _orderStatusID = int.Parse(ds.Tables[0].Rows[0]["OrderStatusID"].ToString());

            }
        }
        public OrderInformation(int orderID)
        {
            _orderID = orderID;
        }
    }
}
