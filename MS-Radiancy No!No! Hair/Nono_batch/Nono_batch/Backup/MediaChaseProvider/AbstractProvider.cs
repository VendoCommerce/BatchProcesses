using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;


namespace ConversionSystems.Providers.MediaChase.OrderProcessing
{
    public abstract class AbstractProvider
    {
        public delegate void OrderProcessingEventHandler(object sender, OrderProcessingEventArgs e);
        public abstract event OrderProcessingEventHandler Status;
        public abstract event OrderProcessingEventHandler Error;
        public abstract event OrderProcessingEventHandler Completed;

        private static AbstractProvider abstractProvider = null;

        private static void CreateProvider(string configSection)
        {
            abstractProvider = (AbstractProvider)ProviderConfiguration.CreateObject(configSection);
        }

        public static AbstractProvider Instance(string configSection)
        {

            if (abstractProvider == null)
            {
                CreateProvider(configSection);
            }

            return abstractProvider;

        }


        public abstract Response PerformRequest(Request request);

        public abstract Response[] PerformRequestAdvanced(Request request);

        public abstract Response[] PerformRequestAdvanced(Request request, bool useTemp);

        public abstract void RaiseStatusEvent(OrderProcessingEventArgs args);


    }
}
