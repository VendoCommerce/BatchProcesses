using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using Com.ConversionSystems.DataAccess;
using Com.ConversionSystems.Utility;
using Com.ConversionSystems.GoldCanyon;
using ConversionSystems.Providers;
using ConversionSystems.Providers.MediaChase.OrderProcessing;





namespace Com.ConversionSystems.GoldCanyon
{
    public class Batch : Com.ConversionSystems.UI.BasePage
    {
        private DataTable _dtOrders = null;
        private ArrayList AllOrders = new ArrayList();
        private DataTable _dtCustomerBillingAddress = null;
        private DataTable _dtCustomerShippingAddress = null;
        private DataTable _dtOrderSKU = null;
        private DataTable _dtSalesTaxRate = null;
        private int _intOrderID = 0;
        private int _intBillingAddress = 0;
        private int _intShippingAddress = 0;
        int _intRecords = 0;
        private Hashtable UpSell = new Hashtable();
        private string _srtPath = "C:\\batchstaging\\BatchProcesses\\TryMyPurmist";
        private string filenameCustomer = "";
        private string filenameLineItem = "";
        private string ActualfilenameCustomer = "";
        private string ActualfilenameLineItem = "";
        public static string file1;
        public static string file2;
        public static string file3;

        public static int count1;
        public static int count2;
        public static int count3;
        public static int totalLineItem = 0;
        private string ActualfilenameOrder = "";
        private string ActualfilenameDetail = "";
        private ArrayList skuList = new ArrayList();
        LogData log = new LogData();        
        private DataTable Orders
        {
            get
            {
                if (_dtOrders == null)
                {
                    DAL.SQLServer.GetOrdersForXMLBatch(DateTime.ParseExact(DateTime.Today.AddDays(-1).ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture), out _dtOrders);
                                        
                    // DAL.SQLServer.GetOrdersForXMLBatch(DateTime.ParseExact(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture), out _dtOrders);                    
                    //DAL.SQLServer.GetOrdersForXMLBatch(DateTime.Now.AddDays(-1), out _dtOrders);
                    //DAL.SQLServer.GetOrdersForXMLBatch(new DateTime(2009, 06, 29), out _dtOrders);
                }
                return _dtOrders;
           } 
        }
        private DataTable CustomerBillingAddress
        {
            get
            {
                if (_dtCustomerBillingAddress == null)
                {
                    DAL.SQLServer.GetCustomerAddress(_intBillingAddress, out _dtCustomerBillingAddress);
                }
                return _dtCustomerBillingAddress;
            }
        }
        private DataTable CustomerShippingAddress
        {
            get
            {
                if (_dtCustomerShippingAddress == null)
                {
                    DAL.SQLServer.GetCustomerAddress(_intShippingAddress, out _dtCustomerShippingAddress);
                }
                return _dtCustomerShippingAddress;
            }
        }
        private DataTable OrderSKU
        {
            get
            {
                if (_dtOrderSKU == null)
                {
                    DAL.SQLServer.GetOrderSKU(_intOrderID, out _dtOrderSKU);
                }
                return _dtOrderSKU;
            }
        }
        private bool IsMultiPay(string skuid)
        {
            if (skuid.Equals("336") || skuid.Equals("345") || skuid.Equals("346"))
            {
                return true;
            }
            return false;

        }
        
        // public bool DoVeripurBatch()
        public bool DoTryMyPurmistBatch()
        {
            bool _breturn = false;
            int LineIdCounter = 0;
            decimal subTotal = 0;
            decimal Total = 0;
            string skuCode = "";
            if (!Directory.Exists(_srtPath))
            {
                Directory.CreateDirectory(_srtPath);

            }
            Console.WriteLine(_srtPath);
            
            DataTable dtSeq = null;
            DAL.SQLServer.GetSequenceNumber(out dtSeq);
            string sq = dtSeq.Rows[0]["sequencenumber"].ToString();

            ActualfilenameCustomer = "VPCNVSh" + DateTime.Now.ToString("yyyyMMdd") + "-" + sq.PadLeft(5, '0') + ".txt";
            filenameCustomer = _srtPath + "\\" + ActualfilenameCustomer;
            file1 = filenameCustomer;
            StreamWriter fileCustomer = new StreamWriter(filenameCustomer);
            fileCustomer.Flush();

            ActualfilenameLineItem = "VPCNVSd" + DateTime.Now.ToString("yyyyMMdd") + "-" + sq.PadLeft(5, '0') + ".txt";
            filenameLineItem = _srtPath + "\\" + ActualfilenameLineItem;
            file2 = filenameLineItem;
            StreamWriter fileLineItem = new StreamWriter(filenameLineItem);
            fileLineItem.Flush();


            //DAL.SQLServer.GetOrdersForXMLBatch(DateTime.Now.AddDays(-1), out _dtOrders);
            try
            {                  
               foreach (DataRow _drOrder in Orders.Rows)
                {

                    subTotal = 0;
                   Order ord = new Order();
                   _breturn = true;
                   LineIdCounter = 0;
                   _intOrderID = Helper.ToInt32(_drOrder["OrderId"].ToString());
                   
                   ord.OrderNumber = _drOrder["OrderId"].ToString();
                   ord.SourceCode = "CNVS";
                   ord.OrderType = "WEB";
                   ord.MediaCode = "TVURL";

                    try
                    {
                        if (_drOrder["uid"] != null)
                        {
                            if (!_drOrder["uid"].ToString().Equals(""))
                            {
                                if (_drOrder["uid"].ToString().ToUpper().Contains("_BR"))
                                {
                                    ord.MediaCode = "SEMB";
                                }
                                else if (_drOrder["uid"].ToString().ToUpper().Contains("_NB"))
                                {
                                    ord.MediaCode = "SEMN";
                                }
                            }
                        }
                    }
                    catch 
                    {    
                    }                   
                   ord.ProductID_AdCode = "MPTV08";
                   ord.DeliveryMethod = "";
                   ord.ShipSeperately = "Y";
                   ord.ApplyTo1stInstallment = "Y";
                   ord.TransactionTime = Convert.ToDateTime(_drOrder["Created"]).ToString("hhmmss");
                   ord.TransactionDate = Convert.ToDateTime(_drOrder["Created"]).ToString("MMddyyyy");
                   ord.NumberOfPayments = "1";

                    //Order payment Information
                   ord.CreditCardNumber = em.Decrypt(_drOrder["CreditCardNumber"].ToString());
                   ord.CreditCardType = Helper.CCPaymentType[_drOrder["CreditCardName"].ToString()].ToString();
                   ord.CreditCardExpiration = Convert.ToDateTime(_drOrder["CreditCardExpired"]).ToString("MMyyyy");

                   ord.AuthCode = _drOrder["AuthorizationCode"].ToString();
                   ord.TransactionId = _drOrder["ConfirmationCode"].ToString();

                   ord.AuthDate = Convert.ToDateTime(_drOrder["Created"]).ToString("MMddyyyy");             
                   ord.Shipping = Math.Round(Convert.ToDecimal(_drOrder["shippingcost"].ToString()), 2).ToString();
                   ord.Tax = Math.Round(Convert.ToDecimal(_drOrder["tax"].ToString()), 2).ToString();
                   ord.Discout = "0";
                    bool surCharge = false;
                   //Billing Information
                   _intBillingAddress = Helper.ToInt32(_drOrder["BillingAddressId"].ToString());
                            foreach (DataRow _drBillingInfo in CustomerBillingAddress.Rows)
                            {
                                ord.FirstName = _drBillingInfo["FirstName"].ToString();
                                ord.LastName = _drBillingInfo["LastName"].ToString();
                                ord.Address1 = _drBillingInfo["Address1"].ToString();
                                ord.Address2 = _drBillingInfo["Address2"].ToString();
                                ord.Address3 = "";
                                ord.City = _drBillingInfo["City"].ToString();
                                ord.State = _drBillingInfo["StateProvince"].ToString();
                                ord.PostalCode = _drBillingInfo["ZipPostalCode"].ToString();
                                ord.Email = _drOrder["Email"].ToString();
                                ord.Phone = _drBillingInfo["PhoneNumber"].ToString();
                                ord.Country = _drBillingInfo["CountryCode"].ToString().Trim();
                                
                                //ord.TransactionId -  Ask Kevin
                                _dtCustomerBillingAddress = null;
                                break;
                            }
                    //Shipping nformation
                   _intShippingAddress = Helper.ToInt32(_drOrder["CustomerShippingAddressId"].ToString());
                            foreach (DataRow _drShippingInfo in CustomerShippingAddress.Rows)
                            {
                                ord.ShipToFirstName = _drShippingInfo["FirstName"].ToString();
                                    ord.ShipToLastName = _drShippingInfo["LastName"].ToString();
                                    ord.ShipToPhone = _drShippingInfo["PhoneNumber"].ToString();
                                    ord.ShipToAddress1 = _drShippingInfo["Address1"].ToString();
                                    ord.ShipToAddress2 = _drShippingInfo["Address2"].ToString();
                                    ord.ShipToAddress3 = "";
                                    ord.ShipToCity = _drShippingInfo["City"].ToString();
                                    ord.ShipToState = _drShippingInfo["StateProvince"].ToString();
                                    
                                    ord.ShipToPostalCode = _drShippingInfo["ZipPostalCode"].ToString();
                                    ord.ShipToCountry = _drShippingInfo["CountryCode"].ToString().ToString().Trim();
                                    if (ord.ShipToState.Equals("AK") || ord.ShipToState.Equals("HI") || ord.ShipToState.Equals("VI") || ord.ShipToState.Equals("GU") || ord.ShipToState.Equals("PR") || ord.ShipToCountry.Equals("CA"))
                                    {
                                        surCharge = true;
                                    }
                                    //ord.SalesTaxRate ------------ Check with Kevin
                                
                                _dtCustomerShippingAddress = null;
                                break;                                                              
                            }

                            bool _boolMultiPay = false;
                            int count = 1;
                            skuList.Clear();
                            foreach (DataRow _drOrderSKU in OrderSKU.Rows)
                            {
                                int skuid = Convert.ToInt32( _drOrderSKU["skuid"].ToString());
                                skuList.Add(_drOrderSKU["skuid"].ToString());
                                totalLineItem++;
                                ord.ItemNumber = _drOrderSKU["skucode"].ToString();
                                ord.ItemPrice = Math.Round(Convert.ToDecimal(_drOrderSKU["FullAmount"].ToString()), 2).ToString();                                

                                ord.ItemQuantity = _drOrderSKU["OrderSkuQuantity"].ToString();
                                fileLineItem.WriteLine(ord.OrderNumber + "\t" + count.ToString() + "\t" + ord.ItemNumber + "\t" + ord.ItemQuantity + "\t" + ord.ItemPrice + "\t" + ord.SpecialHandlingCharge + "\t" + ord.ShipSeperately + "\t" + ord.ApplyTo1stInstallment + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler);
                                count++;
                                try
                                {
                                    subTotal += Convert.ToDecimal(_drOrderSKU["FullAmount"].ToString()) * Convert.ToDecimal(_drOrderSKU["OrderSkuQuantity"].ToString());
                                }
                                catch (Exception  ex)
                                {                                    
                                }                             
                            }
                            Total = Convert.ToDecimal(_drOrder["SubTotal"].ToString()) + Convert.ToDecimal(_drOrder["ShippingCost"].ToString()) + Convert.ToDecimal(_drOrder["Tax"].ToString());
                            //if (skuList.Contains("637"))
                            //{
                            //    fileLineItem.WriteLine(ord.OrderNumber + "\t" + count.ToString() + "\t" + "MPMKITD30" + "\t" + "1" + "\t" + "0.00" + "\t" + ord.SpecialHandlingCharge + "\t" + ord.ShipSeperately + "\t" + ord.ApplyTo1stInstallment + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler);
                            //    count++;
                            //}
                            if (surCharge)
                            {
                                fileLineItem.WriteLine(ord.OrderNumber + "\t" + count.ToString() + "\t" + "NUSSURCHG" + "\t" + "1" + "\t" + "0.00" + "\t" + ord.SpecialHandlingCharge + "\t" + ord.ShipSeperately + "\t" + ord.ApplyTo1stInstallment + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler);
                                count++;
                                totalLineItem++;
                            }
                            //if(Convert.ToDecimal(ord.Shipping) == 19.95m && !surCharge)
                            //{
                            //    fileLineItem.WriteLine(ord.OrderNumber + "\t" + count.ToString() + "\t" + "RUSH2" + "\t" + "1" + "\t" + "0.00" + "\t" + ord.SpecialHandlingCharge + "\t" + ord.ShipSeperately + "\t" + ord.ApplyTo1stInstallment + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler);
                            //    count++;
                            //}                            

                            ord.OrderBaseAmount = Math.Round(subTotal, 2).ToString();
                            ord.OrderTotal = (Convert.ToDecimal(ord.OrderBaseAmount) + Convert.ToDecimal(ord.Shipping) + Convert.ToDecimal(ord.Tax)).ToString();

                            if (skuList.Contains("637"))
                            {
                                ord.NumberOfPayments = "4";
                            }
                            else if (skuList.Contains("650"))
                            {
                                ord.NumberOfPayments = "4";
                                //ord.ProductID_AdCode = "MPTV03";
                            }
                            else if (skuList.Contains("648"))
                            {
                                ord.NumberOfPayments = "3";
                                //ord.ProductID_AdCode = "MPTV02";
                            }
                            else if (skuList.Contains("652"))
                            {
                                ord.NumberOfPayments = "5";
                            }
                            else if (skuList.Contains("666"))
                            {
                                ord.NumberOfPayments = "5";
                            }
                            else
                            {
                                ord.NumberOfPayments = "1";
                            }

                            if (skuList.Contains("650"))
                            {                                
                                ord.ProductID_AdCode = "MPTV03";
                            }
                            else if (skuList.Contains("648") || skuList.Contains("649"))
                            {                                
                                ord.ProductID_AdCode = "MPTV02";
                            }
                            else if ( skuList.Contains("651") || skuList.Contains("652"))
                            {
                                   ord.ProductID_AdCode = "MPTV08";
                            }
                            else if (skuList.Contains("665") || skuList.Contains("667") || skuList.Contains("668"))
                            {
                                ord.ProductID_AdCode = "MPTV11"; // triple_g1 triple_g2 ps_triple_g1 ps_triple_g2 H2, H3 PS_H2 PS_H3
                            }

                            if (_drOrder["BankAccountName"].ToString().ToLower().Contains("/f3/"))
                            {
                                ord.ProductID_AdCode = "MPTV04";
                            }
                            else if (_drOrder["BankAccountName"].ToString().ToLower().Contains("/h2/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/h3/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/h4/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/ps_h2/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/ps_h3/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/ps_h4/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/triple_g1/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/triple_g2/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/ps_triple_g1/") || _drOrder["BankAccountName"].ToString().ToLower().Contains("/ps_triple_g2/"))
                            {
                                ord.ProductID_AdCode = "MPTV11";
                            }                            
                            fileCustomer.WriteLine(ord.OrderNumber + "\t" + ord.FirstName + "\t" + ord.LastName + "\t" + ord.Address1 + "\t" + ord.Address2 + "\t" + ord.Address3 + "\t" + ord.City + "\t" + ord.State + "\t" + ord.PostalCode + "\t" + ord.Country + "\t" + ord.Phone + "\t" + ord.CreditCardType + "\t" + ord.CreditCardNumber + "\t" + ord.CreditCardExpiration + "\t" + ord.AuthCode + "|" + _drOrder["CustomerId"].ToString() + "|" + "" +"|"+ Total.ToString("n2") +"|" +ord.TransactionId + "\t" + ord.AuthDate + "\t" + ord.SourceCode + "|" + ord.OrderType + "|" + ord.MediaCode + "\t" + ord.ProductID_AdCode + "\t" + ord.InboundDialed + "\t" + ord.TransactionTime + "\t" + ord.TransactionDate + "\t" + ord.OperatorCode + "\t" + ord.ShipToFirstName + "\t" + ord.ShipToLastName + "\t" + ord.ShipToAddress1 + "\t" + ord.ShipToAddress2 + "\t" + ord.ShipToAddress3 + "\t" + ord.ShipToCity + "\t" + ord.ShipToState + "\t" + ord.ShipToPostalCode + "\t" + ord.ShipToCountry + "\t" + ord.NumberOfPayments + "\t" + ord.DeliveryMethod + "\t" + ord.OrderBaseAmount + "\t" + ord.Shipping + "\t" + ord.Tax + "\t" + ord.Discout + "\t" + ord.OrderTotal + "\t" + ord.Email + "\t" + ord.MicrNumber + "\t" + ord.CheckNumber + "\t" + ord.BankName + "\t" + ord.BillingCity + "\t" + ord.Phone + "\t" + ord.BankAccountType + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler + "\t" + ord.Filler);

                            DAL.SQLServer.UpdateOrderStatus(ord.OrderNumber);
                            // update [Order] set [OrderStatusId]=2, [BatchtoFile] =1 where OrderID

                    _dtOrderSKU = null;
                    ord.clear();                   
                }
                
                fileLineItem.WriteLine("TRAILER RECORD" + "\t" + ActualfilenameLineItem.Replace(".txt","") + "\t" + DateTime.Now.ToString("yyyyMMdd") + "\t" + DateTime.Now.ToString("hhmmss") + "\t" + (totalLineItem + 1) + "\t" + sq.PadLeft(5,'0'));
                fileCustomer.WriteLine("TRAILER RECORD" + "\t" + ActualfilenameCustomer.Replace(".txt", "") + "\t" + DateTime.Now.ToString("yyyyMMdd") + "\t" + DateTime.Now.ToString("hhmmss") + "\t" + (Orders.Rows.Count + 1) + "\t" + sq.PadLeft(5, '0'));
                DAL.SQLServer.AddSequenceNumber(Convert.ToInt32(sq)+1);
               fileCustomer.Flush();
               fileCustomer.Close();
               fileLineItem.Flush();
               fileLineItem.Close();
               log.LogToFile("File Successfully created ");                
            }
            catch (Exception e)
            {
                fileCustomer.Flush();
                fileCustomer.Close();
                fileLineItem.Flush();
                fileLineItem.Close();
                log.LogToFile("Error creating file ---" + e.Message);
                _breturn = false;
                return _breturn; 
            }
            return _breturn;
        }
        public void DoEncryption()
        {
            //Check this before going forward completely
            string EncryptionFileKey = "C:\\enc\\encryptionkey.asc";
            PGPEncryption _oEncrypt = new PGPEncryption();
            //_oEncrypt.Encrypt(filename,EncryptionFileKey,filename);
        }
        public void MakeUpsells()
        {
            UpSell.Add("402", "402");
            UpSell.Add("403", "403");
            UpSell.Add("404", "404");
        }
        static string key1;
        static Mediachase.eCF.BusLayer.Common.Util.EncryptionManager em;


        public static string fixpath(string s)
        {
            string s1 = s;
            if (s1.IndexOf(".txt") != -1) { s1 = s1.ToLower().Replace("veripur", "veripur1"); }            
            return s1;
        }
        public static void encryptfile(string s)
        {

            string s21 = s;
            s21 = s21.Replace(".txt", "_txt.txt");
            string s2 = s;
            s2 = fixpath(s2);

            //if (File.Exists(s2))
            //{
            //    File.Delete(s2);
            //}
            //File.Move(s, s2);

            if (File.Exists(s21))
            {
                File.Delete(s21);
            }
            // File.Move(s, s21);
            File.Copy(s, s21);


            string s1;
            s1 = "-e -u --sign-~Aniketh Parmar <aniketh@conversionsystems.com>~ -r ~comp@fosdickcorp.com~ --always-trust --yes ~filepath~";
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
            st2z = st2z.Replace(".det", ".gpg");
            st2z = st2z.Replace(".ord", ".gpg");

            st21z = st21z.Replace(".txt", ".gpg");
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


            string stb1 = stb;
            stb1 = stb1.Replace("batchstaging", "ftp");


            stb1 = stb1.Replace("_cus", "");
            stb1 = stb1.Replace("_det", "");
            stb1 = stb1.Replace("_ord", "");
            stb1 = stb1.Replace("_txt", "");

            if (File.Exists(stb1))
            {
                File.Delete(stb1);
            }
            File.Move(stb, stb1);



            string s3 = s21;
            s3 = s3.Replace("_txt.txt",".txt");
            s3 = s3.Replace("_det.det",".det");
            s3 = s3.Replace("_ord.ord",".ord");
            if (File.Exists(s3))
            {
                File.Delete(s3);
            }
            File.Move(s21, s3);

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


        public static void sendemail(string s1, string s2, int i1a, int i2a)
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


            message.Subject = "TryMyPurmist batch file upload - " + DateTime.Now;
            string st;
            st = "Hello, ~~The following TryMyPurmist batch files were created: <filename1>,<filename2>~~This file was transmitted at <date and time of upload>~~There were <XX1> lines in <filename1>, <XX2> lines in <filename2>.~~Thank You,~~Conversion Systems";

            st = st.Replace("<filename1>", afterslash(s1));
            st = st.Replace("<filename2>", afterslash(s2));            
            st = st.Replace("<date and time of upload>", System.DateTime.Now.ToString());
            st = st.Replace("<XX1>", i1a.ToString());
            st = st.Replace("<XX2>", i2a.ToString());
            
            st = st.Replace("~", ((char)(13)).ToString() + ((char)(10)).ToString());
            message.Body = st;
            SmtpClient client = new SmtpClient();
            Helper.SendMail(message);
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
        public static void uploadFile(string FTPAddress, string filePath, string username, string password)
        {
            //Create FTP request
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/" + Path.GetFileName(filePath));

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            //Load the file
            FileStream stream = File.OpenRead(filePath);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            //Upload file
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            
        }
        public static void Main(string[] args)
        {
            key1= System.Configuration.ConfigurationSettings.AppSettings["encryptionkey"];
            Mediachase.eCF.BusLayer.Common.Configuration.FrameworkConfig.EncryptionPrivateKey = key1;
            em = new Mediachase.eCF.BusLayer.Common.Util.EncryptionManager();

            Batch StartBatch = new Batch();
            //  StartBatch.MakeUpsells();
            Console.WriteLine("TryMyPurmist Batch - Started");
            Console.WriteLine("Please Wait - ");
            Console.WriteLine("Start - Batching Control Version");
            // StartBatch.DoVeripurBatch();
            StartBatch.DoTryMyPurmistBatch();
            Console.WriteLine("End - Batching Control Version");
            
            Console.WriteLine("Start - Encryption Process");
            encryptfile(file1);
            encryptfile(file2);
            Console.WriteLine("Start - Successfulyy Encrypted");
            
            //upload files
            //Console.WriteLine("Start - Upload Process");
            //uploadFile(Helper.AppSettings["ftpServerName"].ToString(), file1.ToLower().Replace("batchstaging", "ftp") + ".pgp", Helper.AppSettings["ftpUserName"].ToString(), Helper.AppSettings["ftpPassword"].ToString());

            count1 = LineCount(file1);
            count2 = LineCount(file2);
            //count3 = LineCount(file3);

            sendemail(file1, file2, count1-1, count2-1);

            Console.WriteLine("Task Completed - ");            
            
        }
    }
  
}
