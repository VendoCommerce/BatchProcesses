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
using ConversionSystems.Celluscience.BatchUpload;

namespace ConversionSystems.Celluscience.BatchProcess
{
    class Batch : ConversionSystems.Celluscience.UI.BaseClass
    {
        static void Main(string[] args)
        {
            Batch CelluscienceBatch = new Batch();
            DateTime dt;
            dt = DateTime.ParseExact(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            //dt = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            //  dt = new DateTime(2010, 01, 19);
           // string EmployeeNumber = "CONV";
            string EmployeeNumber = Helper.DataServicesTxtBatchConfig["Field#24"]; 
            //   Console.WriteLine("\n Please Enter your assigned Employee Number(call center code).");
             //   EmployeeNumber = Console.ReadLine();
           
            //Batch Process
            Console.WriteLine("\n[1] Celluscience Batch Process Started.\n");
            CelluscienceBatch.DoCelluscienceBatch(dt, EmployeeNumber);
            Console.WriteLine("\n Celluscience Batch Process Completed. \n");

            ////Batch Process - GetCelluScience
            //Console.WriteLine("\n[1] GetCelluScience Batch Process Started.\n");
            //CelluscienceBatch.DoGetCelluscienceBatch(dt, EmployeeNumber);
            //Console.WriteLine("\n GetCelluScience Batch Process Completed. \n");

            ////Batch Process - TryCelluScience
            //Console.WriteLine("\n[1] TryCelluScience Batch Process Started.\n");
            //CelluscienceBatch.DoTryCelluscienceBatch(dt, EmployeeNumber);
            //Console.WriteLine("\n TryCelluScience Batch Process Completed. \n");

            //UPLOAD Process
            //Console.WriteLine("\n[2] Uploading Files to the Server. Please Wait... \n");
            //CelluscienceBatch.DoCelluscienceFileUpload(EmployeeNumber);
            //Console.WriteLine("\n Files uploaded to the Server.  \n");

            //
            //ENCRYPT Process
            Console.WriteLine("\n[2] Encrypting Files on the Server. Please Wait... \n");
            CelluscienceBatch.DoCelluscienceFileEncryption(EmployeeNumber);
            Console.WriteLine("\n Files Encrypted on the Server.  \n");
            //SEND-EMAIL Process
            Console.WriteLine("\n[2] Sending Email. Please Wait... \n");
            string _srtPath = Helper.DataServicesTxtBatchConfig["FilePath"];
            string txtfilename = "";
            string Actualfilename = "";
            Actualfilename = EmployeeNumber + DateTime.Now.ToString("MMddyyyy") + "_cellu.txt";
            txtfilename = _srtPath + "\\" + Actualfilename;
            sendemail(Actualfilename, LineCount(txtfilename));
            Console.WriteLine("\n Email Sent.  \n");
            Console.WriteLine("************************************************************");
            Console.WriteLine("Task Completed.");
            Console.WriteLine("************************************************************\n");
            Console.WriteLine("press any key to exit.... \n");
            Console.ReadKey();
        }

        public void DoCelluscienceBatch(DateTime dt,string EmpNumber)
        {
            BatchProcess batpro = new BatchProcess();
            batpro.ProcessBatch(dt, EmpNumber);
        }
        public void DoGetCelluscienceBatch(DateTime dt, string EmpNumber)
        {
            BatchProcess2 batpro2 = new BatchProcess2();
            batpro2.ProcessBatch(dt, EmpNumber);
        }
        public void DoTryCelluscienceBatch(DateTime dt, string EmpNumber)
        {
            BatchProcess3 batpro3 = new BatchProcess3();
            batpro3.ProcessBatch(dt, EmpNumber);
        }
        public void DoCelluscienceFileEncryption(string EmpNumber)
        {
            string _srtPath = Helper.DataServicesTxtBatchConfig["FilePath"];
            string txtfilename = "";
            string Actualfilename = "";
            Actualfilename = EmpNumber + DateTime.Now.ToString("MMddyyyy") + "_cellu.txt";
            txtfilename = _srtPath + "\\" + Actualfilename;
            encryptfile(txtfilename);
            encryptfileLori(txtfilename);
        }
        public void DoCelluscienceFileUpload(string EmpNumber)
        {
            string _srtPath = Helper.DataServicesTxtBatchConfig["FilePath"];
            string txtfilename = "";
            string Actualfilename = "";
            Actualfilename = EmpNumber + DateTime.Now.ToString("MMddyyyy") + "_cellu.txt";
            txtfilename = _srtPath + "\\" + Actualfilename;
            UploadBatchFile btUpload = new UploadBatchFile();
            btUpload.UploadFile(Actualfilename);  
        }
        public static void runapp(string appname, string args1)
        {
            ProcessStartInfo pi = new ProcessStartInfo(appname);
            pi.Arguments = args1;

            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            Process p = Process.Start(pi);
            StreamReader sr = p.StandardOutput;

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine("Read line: {0}", line);
            }
            p.WaitForExit();

        }
        public static string fixpath(string s)
        {
            string s1 = s;
            if (s1.IndexOf(".txt") != -1) { s1 = s1.ToLower().Replace("cellusciencebatchfiles", "cellusciencebatchfiles1"); }
            if (s1.IndexOf(".cus") != -1) { s1 = s1.ToLower().Replace("muradbatchfiles", "muradbatchfiles1"); }
            if (s1.IndexOf(".det") != -1) { s1 = s1.ToLower().Replace("muradbatchfiles", "muradbatchfiles2"); }
            if (s1.IndexOf(".ord") != -1) { s1 = s1.ToLower().Replace("muradbatchfiles", "muradbatchfiles3"); }
            return s1;
        }
        public static void encryptfile(string s)
        {

            string s21 = s;
            s21 = s21.Replace(".txt", "_txt.txt");
            s21 = s21.Replace(".cus", "_cus.cus");
            s21 = s21.Replace(".det", "_det.det");
            s21 = s21.Replace(".ord", "_ord.ord");

            string s2 = s;
            s2 = fixpath(s2);

            if (File.Exists(s2))
            {
                File.Delete(s2);
            }
            File.Move(s, s2);


            string s1;
            s1 = "-e -u --sign-~Aniketh Parmar <aniketh@conversionsystems.com>~ -r ~Laura Portanova for Conversion <laura@medesteausa.com>~ --always-trust --yes ~filepath~";
            s1 = s1.Replace("~", ((char)(34)).ToString());
            s1 = s1.Replace("filepath", s2);
            Console.WriteLine("C:\\gnupg\\gpg.exe " + s1);

            string appname = "C:\\gnupg\\gpg.exe";
            string args1 = s1;

            runapp(appname, args1);

            if (File.Exists(s21))
            {
                File.Delete(s21);
            }
            File.Move(s2, s21);

            string st2z = s2;
            string st21z = s21;
            st2z = st2z.Replace(".txt", ".gpg");
            st2z = st2z.Replace(".cus", ".gpg");
            st2z = st2z.Replace(".det", ".gpg");
            st2z = st2z.Replace(".ord", ".gpg");

            st21z = st21z.Replace(".txt", ".gpg");
            st21z = st21z.Replace(".cus", ".gpg");
            st21z = st21z.Replace(".det", ".gpg");
            st21z = st21z.Replace(".ord", ".gpg");

            if (File.Exists(st21z))
            {
                File.Delete(st21z);
            }
            File.Move(st2z, st21z);



            string sta, stb;




            sta = st21z;
            stb = st21z;
            stb = stb.Replace(".gpg", ".pgp");



            if (File.Exists(stb))
            {
                File.Delete(stb);
            }
            File.Move(sta, stb);


            string stb1 = stb.ToLower();
            stb1 = stb1.Replace("batchstaging\\batchprocesses\\cellusciencebatchfiles", "ftp\\BatchProcesses\\FullFast_Batch\\Celluscience Files");



            stb1 = stb1.Replace("_txt", ".txt");
            stb1 = stb1.Replace("_cus", ".cus");
            stb1 = stb1.Replace("_det", ".det");
            stb1 = stb1.Replace("_ord", ".ord");



            if (File.Exists(stb1))
            {
                File.Delete(stb1);
            }
            File.Move(stb, stb1);



            string s3 = s21;
            s3 = s3.Replace("_txt.txt", ".txt");
            s3 = s3.Replace("_cus.cus", ".cus");
            s3 = s3.Replace("_det.det", ".det");
            s3 = s3.Replace("_ord.ord", ".ord");
            if (File.Exists(s3))
            {
                File.Delete(s3);
            }

            File.Move(s21, s3);

        }
        public static void encryptfileLori(string s)
        {

            string s21 = s;
            s21 = s21.Replace(".txt", "_txt.txt");
            s21 = s21.Replace(".cus", "_cus.cus");
            s21 = s21.Replace(".det", "_det.det");
            s21 = s21.Replace(".ord", "_ord.ord");

            string s2 = s;
            s2 = fixpath(s2);

            if (File.Exists(s2))
            {
                File.Delete(s2);
            }
            File.Move(s, s2);


            string s1;
            s1 = "-e -u --sign-~Aniketh Parmar <aniketh@conversionsystems.com>~ -r ~Lori Remy <lremy@medesteausa.com>~ --always-trust --yes ~filepath~";
            s1 = s1.Replace("~", ((char)(34)).ToString());
            s1 = s1.Replace("filepath", s2);
            Console.WriteLine("C:\\gnupg\\gpg.exe " + s1);

            string appname = "C:\\gnupg\\gpg.exe";
            string args1 = s1;

            runapp(appname, args1);

            if (File.Exists(s21))
            {
                File.Delete(s21);
            }
            File.Move(s2, s21);

            string st2z = s2;
            string st21z = s21;
            st2z = st2z.Replace(".txt", ".gpg");
            st2z = st2z.Replace(".cus", ".gpg");
            st2z = st2z.Replace(".det", ".gpg");
            st2z = st2z.Replace(".ord", ".gpg");

            st21z = st21z.Replace(".txt", ".gpg");
            st21z = st21z.Replace(".cus", ".gpg");
            st21z = st21z.Replace(".det", ".gpg");
            st21z = st21z.Replace(".ord", ".gpg");

            if (File.Exists(st21z))
            {
                File.Delete(st21z);
            }
            File.Move(st2z, st21z);



            string sta, stb;




            sta = st21z;
            stb = st21z;
            stb = stb.Replace(".gpg", ".pgp");



            if (File.Exists(stb))
            {
                File.Delete(stb);
            }
            File.Move(sta, stb);


            string stb1 = stb.ToLower();
            stb1 = stb1.Replace("batchstaging\\batchprocesses\\cellusciencebatchfiles", "ftp\\BatchProcesses\\FullFast_Batch\\Lori Remy\\CelluScience Files");



            stb1 = stb1.Replace("_txt", ".txt");
            stb1 = stb1.Replace("_cus", ".cus");
            stb1 = stb1.Replace("_det", ".det");
            stb1 = stb1.Replace("_ord", ".ord");



            if (File.Exists(stb1))
            {
                File.Delete(stb1);
            }
            File.Move(stb, stb1);



            string s3 = s21;
            s3 = s3.Replace("_txt.txt", ".txt");
            s3 = s3.Replace("_cus.cus", ".cus");
            s3 = s3.Replace("_det.det", ".det");
            s3 = s3.Replace("_ord.ord", ".ord");
            if (File.Exists(s3))
            {
                File.Delete(s3);
            }

            File.Move(s21, s3);

        }
        public static int LineCount(string source)
        {
            if (source != null)
            {
                string text = source;
                int numOfLines = 0;

                FileStream FS = new FileStream(source, FileMode.Open,
                   FileAccess.Read, FileShare.Read);
                StreamReader SR = new StreamReader(FS);
                while (text != null)
                {
                    text = SR.ReadLine();
                    if (text != null)
                    {
                        ++numOfLines;
                    }
                }
                SR.Close();
                FS.Close();
                return (numOfLines);


            }
            else
            {
                // Handle a null source here
                return (0);
            }
        }
        public static string afterslash(string s)
        {
            string s1 = s;
            string s2 = "";
            bool b;

            b = false;

            int j;
            j = -1;


            for (int i = s.Length - 1; i >= 0; i--)
            {
                if ((s[i].ToString() == "\\") && (j == -1))
                {
                    j = i;
                }
            }

            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i].ToString() == "\\") && i == j)
                {
                    b = true;
                }
                else
                {
                    if (b == true) { s2 += s[i]; }
                }
            }

            return s2;
        }
        public static void sendemail(string s1, int i1a)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@conversionsystems.com");


            string sendemailto1 = System.Configuration.ConfigurationSettings.AppSettings["sendemailto"];
            string bcc1 = System.Configuration.ConfigurationSettings.AppSettings["bcc"];
            string cc1 = System.Configuration.ConfigurationSettings.AppSettings["cc"];


            string[] tok = new string[200];
            int numtok = 1;
            for (int i1 = 0; i1 < 200; i1++)
            {
                tok[i1] = "";
            }
            for (int i1 = 0; i1 < sendemailto1.Length; i1++)
            {
                if (sendemailto1[i1].ToString() == ";")
                {
                    numtok++;
                }
                else
                {
                    tok[numtok] += sendemailto1[i1];
                }
            }



            for (int i1 = 1; i1 <= numtok; i1++)
            {
                message.To.Add(new MailAddress(tok[i1]));
            }



            if (cc1 != "")
            {
                numtok = 1;
                for (int i1 = 0; i1 < 200; i1++)
                {
                    tok[i1] = "";
                }
                for (int i1 = 0; i1 < cc1.Length; i1++)
                {
                    if (cc1[i1].ToString() == ";")
                    {
                        numtok++;
                    }
                    else
                    {
                        tok[numtok] += cc1[i1];
                    }
                }
                for (int i1 = 1; i1 <= numtok; i1++)
                {
                    message.CC.Add(new MailAddress(tok[i1]));
                }
            }



            if (bcc1 != "")
            {
                numtok = 1;
                for (int i1 = 0; i1 < 200; i1++)
                {
                    tok[i1] = "";
                }
                for (int i1 = 0; i1 < bcc1.Length; i1++)
                {
                    if (bcc1[i1].ToString() == ";")
                    {
                        numtok++;
                    }
                    else
                    {
                        tok[numtok] += bcc1[i1];
                    }
                }



                for (int i1 = 1; i1 <= numtok; i1++)
                {
                    message.Bcc.Add(new MailAddress(tok[i1]));
                }
            }


            message.Subject = "CelluScience batch file upload";
            string st;
            st = "Hello, ~~The following CelluScience batch file was created: <filename1>,~~This file was transmitted at <date and time of upload>~~There were <XX1> lines in <filename1>.~~Thank You,~~Conversion Systems";

            st = st.Replace("<filename1>", s1);
            st = st.Replace("<date and time of upload>", System.DateTime.Now.ToString());
            st = st.Replace("<XX1>", i1a.ToString());

            st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            message.Body = st;
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }
    }
}
