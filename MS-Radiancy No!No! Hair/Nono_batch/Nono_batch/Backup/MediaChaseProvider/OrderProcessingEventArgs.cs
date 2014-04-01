using System;
using System.Collections.Generic;
using System.Text;

namespace ConversionSystems.Providers.MediaChase.OrderProcessing
{
    public class OrderProcessingEventArgs : EventArgs
    {

        public OrderProcessingEventArgs(int count)
        {
            _count = count;
        }
        public OrderProcessingEventArgs(Exception ex)
        {
            _ex = ex;
        }
        public OrderProcessingEventArgs(string message)
        {
            _statusMessage = message;
        }
        public OrderProcessingEventArgs(string email, string firstName, string lastName)
        {
            _email = email;
            _lastName = lastName;
            _firstName = firstName;
        }
        public OrderProcessingEventArgs()
        { }

        public string OrderID
        {
            get { return _orderID; }
            set { _orderID = value; }
        }
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
            }
        }
        public Exception ex
        {
            set { _ex = value; }
            get { return _ex; }
        }
        public string statusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; }
        }
        public string statusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }
        private string _firstName, _lastName, _email;
        public string email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string lastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string firstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        private int _count;
        private Exception _ex;
        private string _statusMessage, _statusCode;
        private string _orderID;
    }
}
