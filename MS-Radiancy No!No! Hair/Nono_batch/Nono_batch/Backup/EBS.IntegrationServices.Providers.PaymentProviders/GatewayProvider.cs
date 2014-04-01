using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using EBS.IntegrationServices.Providers;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{
    public abstract class GatewayProvider
    {

        public static GatewayProvider gatewayProvider = null;

        private static void CreateProvider(string defaultProvider)
        {
            gatewayProvider = (GatewayProvider)ProviderConfiguration.CreateObject(defaultProvider);
        }

        public static GatewayProvider Instance(string defaultProvider)
        {

            if (gatewayProvider == null)
            {
                CreateProvider(defaultProvider);
            }

            return gatewayProvider;

        }

        public abstract Response PerformRequest(Request request);
        public abstract Response PerformVoidRequest(Request request);

    }
}
