using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using EBS.IntegrationServices.Providers.PaymentProviders;
using System.Globalization;

namespace CS.ErrorFramework.LogError
{
    public class LogError
    {

        public static void LogErrorToFile(string logMessage, TextWriter writer)
        {
            writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            writer.WriteLine("  :");
            writer.WriteLine("  :{0}", logMessage);
            writer.WriteLine("-------------------------------");
        }

    }
}
