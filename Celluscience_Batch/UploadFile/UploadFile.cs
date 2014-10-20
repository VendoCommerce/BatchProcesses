using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using ConversionSystems.Celluscience.DataAccess;
using ConversionSystems.Celluscience.Utility;
using ConversionSystems.Celluscience.BatchProcess;

namespace ConversionSystems.Celluscience.BatchUpload
{
    public class UploadBatchFile : ConversionSystems.Celluscience.UI.BaseClass
    {
        LogData log = new LogData();

        public void UploadFile(string _FullFileName)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder Email = new StringBuilder();
            try
            {
              //  HttpWebRequest req = (HttpWebRequest)WebRequest.Create("HTTPS://webservices.datapakservices.com/OrderServiceTest");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("HTTPS://webservices.test.com/OrderServiceTest");

                req.Method = "POST";
                Stream s = req.GetRequestStream();
                byte[] bytes = File.ReadAllBytes(_FullFileName);
                s.Write(bytes, 0, bytes.Length);
                s.Close();
                WebResponse response = req.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseArray = reader.ReadToEnd();

                    //display the response and log to file
                    Console.WriteLine(responseArray);
                    log.LogToFile(responseArray); //Display response                    
                }

            }
            catch (Exception ex)
            {
                log.LogToFile("Error while uploading files to server - " + ex.Message + " - Informaton - " + sb.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
