using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;

namespace KeepAlive
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            PingURLs();
        }


        private static void PingURLs()
        {
            try
            {
                Console.WriteLine("Starting to ping urls:");
                string[] urls = KeepAlive.Properties.Settings.Default.RequestURL.Split(new char[] { '|' });

                foreach (string url in urls)
                {
                    Console.WriteLine(string.Format("Requesting: {0}",url));
                    WebRequest request = HttpWebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Console.WriteLine(string.Format("Request was successful, conect length: {0}", response.ContentLength.ToString()));
                }
                Console.WriteLine("Exiting application.");
                Application.Exit();
            }
            catch (Exception)
            {
            }

        }
    }
}
