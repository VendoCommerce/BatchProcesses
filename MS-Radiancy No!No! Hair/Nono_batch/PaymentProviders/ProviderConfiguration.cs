using System;
using System.Xml;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace EBS.IntegrationServices.Providers.PaymentProviders
{

    public class ProviderConfiguration
    {

        private XmlAttributeCollection m_Attributes = null;
        private string m_DefaultProvider = string.Empty;
        private Dictionary<string, Provider> m_Providers = null;

        #region Properties

        public string DefaultProvider
        {
            get { return m_DefaultProvider; }
        }

        public Dictionary<string, Provider> Providers
        {
            get { return m_Providers; }
        }

        public XmlAttributeCollection Attributes
        {
            get { return m_Attributes; }
        }

        #endregion

        public static ProviderConfiguration GetProviderConfiguration(string ProviderSection)
        {
            return (ProviderConfiguration)ConfigurationSettings.GetConfig(ProviderSection);
        }

        public void LoadConfigurationFromXML(XmlNode node)
        {

            m_Attributes = node.Attributes;

            m_DefaultProvider = m_Attributes["defaultProvider"].Value;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "providers")
                {
                    GetProviders(childNode);
                }
            }

        }

        public void GetProviders(XmlNode node)
        {

            m_Providers = new Dictionary<string, Provider>();

            foreach (XmlNode providerNode in node.ChildNodes)
            {
                m_Providers.Add(providerNode.Attributes["name"].Value, new Provider(providerNode.Attributes));
            }

        }

        public static object CreateObject(string ConfigSection)
        {

            ProviderConfiguration config = (ProviderConfiguration)ConfigurationSettings.GetConfig(ConfigSection);
            Type typProvider = null;
            try
            {
                typProvider = Type.GetType(config.Providers[config.DefaultProvider].ProviderType, true);
            }
            catch (Exception e) { }
            
            return Activator.CreateInstance(typProvider);

        }

    }

}
