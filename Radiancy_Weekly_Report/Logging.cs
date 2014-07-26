using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Radiancy_Weekly_Report
{
    class Logging
    {
        public static void LogToFile(string AdditionalInfo)
        {
            bool bResult = false;
            StreamWriter log;

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now);
            sb.Append("-" + AdditionalInfo + "-");
            try
            {
                if (!File.Exists(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"]))
                {
                    log = new StreamWriter(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"]);
                }
                else
                {
                    log = File.AppendText(System.Configuration.ConfigurationSettings.AppSettings["ErrorLogFile"]);
                }
                log.WriteLine(sb.ToString());
                log.Close();
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            //  return bResult;
        }


        public static DateTime EndOfDay(DateTime date)
        {
            return date.AddHours(21).AddMinutes(00).AddSeconds(00);
        }

        public static DateTime StartOfDay(DateTime date)
        {
            return date.AddDays(-1).AddHours(21).AddMinutes(00).AddSeconds(00);
        }
    }
}
