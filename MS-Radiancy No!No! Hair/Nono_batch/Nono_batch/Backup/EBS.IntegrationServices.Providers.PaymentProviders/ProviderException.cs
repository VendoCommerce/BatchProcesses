using System;
using System.Collections.Generic;
using System.Text;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{
    public class GatewayException : ApplicationException
    {

        public GatewayException()
            : base("A gateway exception has occured")
        {
        }

        public GatewayException(string Message)
            : base(Message)
        {
        }

        public GatewayException(string Message, Exception ex)
            : base(Message, ex)
        {

        }

    }
}
