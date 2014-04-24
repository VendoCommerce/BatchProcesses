using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
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
using CSBusiness.OrderManagement;
using CSBusiness;
using CSCore.Utils;



namespace Com.ConversionSystems
{
    public class Batch : Com.ConversionSystems.UI.BasePage
    {
        public bool DoBatch()
        {
            bool _breturn = false;
           
            Hashtable SkyesInventory = new Hashtable();
            
            try
            {
                Hashtable AllItems = new OrderManager().GetBatchProcessOrders();
                List<Order> orders = (List<Order>)AllItems["allOrders"];
                foreach (Order orderItem in orders)
                {
                    try
                    {
                        
                            CommonHelper.HttpPost(Helper.AppSettings["SiteUrl"] + orderItem.VersionName + "/authorizeorder.aspx?oid=" + orderItem.OrderId, "");    
                        
                        
                    }
                    catch (Exception e)
                    {   
                    }
                }   
            }
            catch (Exception e)
            {
                return _breturn;
            }
            return _breturn;
        }
        public static void Main(string[] args)
        {   
            Batch StartBatch = new Batch();
            Console.WriteLine("MaxiVision Batch - Started");
            Console.WriteLine("Please Wait - ");
            StartBatch.DoBatch();
            Console.WriteLine("MaxiVision Batch  - End");
            Console.WriteLine("Task Completed - ");
            
        }
    }
  
}
