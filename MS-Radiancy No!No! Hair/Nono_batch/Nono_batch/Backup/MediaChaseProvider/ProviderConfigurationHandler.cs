using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace ConversionSystems.Providers.MediaChase.OrderProcessing
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
