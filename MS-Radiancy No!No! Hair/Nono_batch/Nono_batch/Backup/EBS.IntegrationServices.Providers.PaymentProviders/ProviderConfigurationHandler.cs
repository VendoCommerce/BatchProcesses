using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{
    public class ProviderConfigurationHandler : IConfigurationSectionHandler
    {

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            ProviderConfiguration config = new ProviderConfiguration();
            config.LoadConfigurationFromXML(section);
            return config;
        }

    }
}
