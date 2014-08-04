using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using EDWrapper;
using EDWrapper.Models;
using System.Configuration;

namespace DoloventEmailDirect
{
    class Batch
    {
        static string SerializeObj<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string 
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        static T DeserializeObj<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here 

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
        
        static DataTable getDataTable(string StoredProcedureName)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];

            string strMessage;
            SqlCommand oCmd = null;
            SqlDataAdapter da;

            DataTable dt = null;
            try
            {
                oCmd = new SqlCommand();
                oCmd.CommandText = StoredProcedureName;
                oCmd.CommandType = CommandType.StoredProcedure;

                oCmd.Connection = new SqlConnection(connstr);
                oCmd.Prepare();

                dt = new DataTable();
                da = new SqlDataAdapter(oCmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running procedure:  " + oCmd.CommandText + ". Error---" + ex.Message;
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return dt;
        }

        static void runsql(string sql)
        {
            string connstr = System.Configuration.ConfigurationSettings.AppSettings["connectionstringProd"];
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connstr);
            conn.Open();
            SqlCommand cm = new SqlCommand(sql, conn);
            cm.ExecuteNonQuery();
            conn.Close();
        }

        void UpdateEmailDirectStatus_SubscribeEmailList(string Created, string emailAddress)
        {
            try
            {
                string sqlquery = "update [dbo].[signups] set EmailDirectStatus=1 where CAST(CONVERT(VARCHAR(20),Created,113) AS DATETIME) = CONVERT(VARCHAR(20),CAST('" + Created + "' AS DATETIME),113) and [Email]  ='" + emailAddress + "'";
                runsql(sqlquery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void InsertEmailDirectLog(string ImportType, string ImportRequest, string ImportResponse, string requestEmail)
        {
            string SQLQuery = "";
            if (ImportRequest.Equals(""))
            {
                SQLQuery = "INSERT INTO [EmailDirectImportLogs] ([EDImportDate],[ImportType] , [ImportResponse], [RequestEmail])  VALUES (GETDATE(),'" + ImportType + "', '" + ImportResponse + "', '" + requestEmail + "')";
            }
            else
            {
                SQLQuery = "INSERT INTO [EmailDirectImportLogs] ([EDImportDate],[ImportType] ,[ImportRequest]  ,[ImportResponse], [RequestEmail])  VALUES (GETDATE(),'" + ImportType + "','" + ImportRequest + "','" + ImportResponse + "', '" + requestEmail + "')";
            }

            try
            {
                string runquery = SQLQuery.Replace("utf-16", "utf-8");
                runsql(runquery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void getAllPublicationList()
        {
            string EmailDirectAPIKey = System.Configuration.ConfigurationSettings.AppSettings["ed-apikey"];
            try
            {
                List<PublicationDetails> PublicationDetailsList = new List<PublicationDetails>();
                PublicationDetailsList = EDWrapper.PublicationManager.GetAllPublications();
                Console.WriteLine(PublicationDetailsList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error get All List Details EmailDirect");
            }
        }

        void getAllList()
        {
            string EmailDirectAPIKey = System.Configuration.ConfigurationSettings.AppSettings["ed-apikey"];

            try
            {
                List<List> AllList = new List<List>();
                AllList = EDWrapper.ListManager.GetAllLists(EmailDirectAPIKey);
                Console.WriteLine(AllList.Count);
                // "CS Signup List" 20 LIST ID               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error get All List Details EmailDirect");
            }

            //string orderno = "STONE-5005729"; //   "STONE-5007108";
            //EDWrapper.Models.OrderDetails OrderDetailsObj = EDWrapper.OrderManager.GetOrder(orderno, EmailDirectAPIKey);
            //Console.WriteLine(OrderDetailsObj.EmailAddress);
        }

        //void AddNewList(string ListName, string ListDescription)
        //{
        //    string EmailDirectAPIKey = System.Configuration.ConfigurationSettings.AppSettings["ed-apikey"];
        //    ListDetails LD = ListManager.AddNewList(ListName, ListDescription, EmailDirectAPIKey);
        //    Console.WriteLine(LD.ListID.ToString() + " " + LD.Name);
        //}

        void LoadSubscribeEmailToDirectEmail()
        {
            // string ID = "";
            string emailAddress = "";
            string FirstName = "";
            string LastName = "";
            string Country = "";
            string SignUpDate = "";
            string EmailDirectAPIKey = System.Configuration.ConfigurationSettings.AppSettings["ed-apikey"];
            bool emailSubscribed = false;

            string XMLRequest = "";
            string XMLResponse = "";

            DataTable dt2 = getDataTable("GetSubscribeEmailForEmailDirect");
            if (dt2.Rows.Count > 0)
            {
                Console.WriteLine("Total Rows Found " + dt2.Rows.Count.ToString());
                foreach (DataRow _drdt2 in dt2.Rows)
                {
                    if (_drdt2["Created"].ToString() != null && _drdt2["Email"].ToString() != null)
                    {
                        emailAddress = _drdt2["Email"].ToString();
                        FirstName = _drdt2["FirstName"].ToString();
                        LastName = _drdt2["LastName"].ToString();
                        Country = _drdt2["Country"].ToString();
                        SignUpDate = _drdt2["Created"].ToString();
                    }
                    
                    Subscriber checkSubscriber = null;
                    try
                    {
                        checkSubscriber = EDWrapper.SubscriberManager.GetSubscriber(emailAddress, EmailDirectAPIKey);
                        if (checkSubscriber == null)
                        {
                            // functions just return null if an element is not found - Brain Email Direct
                            emailSubscribed = false;
                        }
                        else if (checkSubscriber.Status == "Active")
                        {
                            emailSubscribed = true;
                        }
                        else
                        {
                            emailSubscribed = false;
                        }
                    }
                    catch (EDWrapperException ex)
                    {
                        emailSubscribed = false;
                        InsertEmailDirectLog("EmailSubscribedError", XMLRequest, XMLResponse, emailAddress);
                        Console.WriteLine("Error Code " + ex.EDError.ErrorCode + " Message " + ex.EDError.Message);
                    }

                                        
                    EDWrapper.Models.SubscriberDetails emailSubscriber = new SubscriberDetails();
                    emailSubscriber.EmailAddress = emailAddress;
                    emailSubscriber.Lists = new List<int>() { 20 };   // "CS Signup List" 20 LIST ID
                    emailSubscriber.Publications = new List<int>() { 2 }; // Publication “Linpharma, Inc. – Newsletter” (API ID: 2)
                    emailSubscriber.SourceID = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["SOURCEID"]);

                    List<EDWrapper.Models.CustomField> CustomFieldLists = new List<EDWrapper.Models.CustomField>();
                    EDWrapper.Models.CustomField cs1 = new CustomField();
                    cs1.FieldName = "FirstName";
                    cs1.Value = FirstName;
                    CustomFieldLists.Add(cs1);
                    EDWrapper.Models.CustomField cs2 = new CustomField();
                    cs2.FieldName = "LastName";
                    cs2.Value = LastName;
                    CustomFieldLists.Add(cs2);
                    EDWrapper.Models.CustomField cs3 = new CustomField();
                    cs3.FieldName = "Country";
                    cs3.Value = Country;
                    CustomFieldLists.Add(cs3);

                    EDWrapper.Models.CustomField cs4 = new CustomField();
                    cs4.FieldName = "DateOfSignUp";
                    cs4.Value = SignUpDate;
                    CustomFieldLists.Add(cs4); 

                    EDWrapper.Models.CustomField cs5 = new CustomField();
                    cs5.FieldName = "CLNOClientType";
                    cs5.Value = System.Configuration.ConfigurationSettings.AppSettings["CLNOClientType"];
                    CustomFieldLists.Add(cs5);                     
                   
                    emailSubscriber.CustomFields = CustomFieldLists;

                    XMLRequest = SerializeObj<SubscriberDetails>(emailSubscriber);
                    // Console.WriteLine(XMLRequest);
                    try
                    {
                        //if (emailSubscribed == false)
                        //{
                            try
                            {
                                Subscriber newSubscriber = EDWrapper.SubscriberManager.AddSubscriber(emailSubscriber, EmailDirectAPIKey);
                                XMLResponse = SerializeObj<Subscriber>(newSubscriber);
                                Console.WriteLine(XMLResponse);
                                InsertEmailDirectLog("EmailSubscribed", XMLRequest, XMLResponse, emailAddress);
                                UpdateEmailDirectStatus_SubscribeEmailList(SignUpDate, emailAddress);
                            }
                            catch (EDWrapperException ex)
                            {
                                ErrorResult errResponse = new ErrorResult();
                                errResponse = ex.EDError;
                                XMLResponse = SerializeObj<ErrorResult>(errResponse);
                                InsertEmailDirectLog("EmailSubscribedError", XMLRequest, XMLResponse, emailAddress);
                                Console.WriteLine("Error Subscribe Adding email to EmailDirect" + emailAddress);
                            }
                        // }
                        //else
                        //{
                        //    try
                        //    {
                        //        Subscriber newSubscriber = EDWrapper.SubscriberManager.UpdateSubscriber(emailSubscriber, EmailDirectAPIKey);
                        //        XMLResponse = SerializeObj<Subscriber>(newSubscriber);
                        //        Console.WriteLine(XMLResponse);
                        //        InsertEmailDirectLog("EmailSubscribed", XMLRequest, XMLResponse, emailAddress);
                        //        UpdateEmailDirectStatus_SubscribeEmailList(SignUpDate, emailAddress);
                        //    }
                        //    catch (EDWrapperException ex)
                        //    {
                        //        ErrorResult errResponse = new ErrorResult();
                        //        errResponse = ex.EDError;
                        //        XMLResponse = SerializeObj<ErrorResult>(errResponse);
                        //        InsertEmailDirectLog("EmailSubscribedError", XMLRequest, XMLResponse, emailAddress);
                        //        Console.WriteLine("Error Subscribe Adding email to EmailDirect" + emailAddress);
                        //    }
                        //}                        
                        SubscriberProperties scProperty = EDWrapper.SubscriberManager.GetProperties(emailAddress);
                        Console.Write(scProperty.EmailAddress + " " + scProperty.CustomFields[1].ToString() + " " + scProperty.CustomFields[2].ToString());
                    }
                    catch (EDWrapperException ex2)
                    {
                        ErrorResult errResponse = new ErrorResult();
                        errResponse = ex2.EDError;
                        XMLResponse = SerializeObj<ErrorResult>(errResponse);
                        InsertEmailDirectLog("EmailSubscribedError", XMLRequest, XMLResponse, emailAddress);
                        Console.WriteLine("Error Subscribe Adding email to EmailDirect" + emailAddress);
                    }

                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start Importing data to Email Direct");
            Batch StartBatch = new Batch();
            // StartBatch.AddNewList("CS Signup List", "Conversion Systems Sign Up List"); // List ID 20
            // StartBatch.getAllList();             
            // StartBatch.getAllPublicationList();
            //  StartBatch.LoadEmailOnly("abc@email.com");
                        
            StartBatch.LoadSubscribeEmailToDirectEmail();

            Console.WriteLine("End Data Imported to Email Direct");
        }
    }
}
