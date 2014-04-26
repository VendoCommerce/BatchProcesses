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
//using Com.ConversionSystems.DataAccess;
//using Com.ConversionSystems.Utility;
using CSBusiness.OrderManagement;
using CSBusiness;
using CSCore.Utils;
using CSBusiness.Payment;
using CSPaymentProvider;
using CSPaymentProvider.Providers;

namespace VOIDBatch
{
    public class Batch : Com.ConversionSystems.UI.BasePage
    {
        /// <summary>
        /// Process all transactions
        /// </summary>
        /// <returns></returns>
        public bool DoBatch()
        {
            bool _breturn = false;
            CSMasterDataSet.VoidQueueDataTable tblVoidQueue = new CSMasterDataSet.VoidQueueDataTable();
            CSMasterDataSetTableAdapters.VoidQueueTableAdapter adpVoidQueue = new CSMasterDataSetTableAdapters.VoidQueueTableAdapter();
            IPaymentProvider pProvider = VOIDBatch.Application_Code.DataAccess.ClientDAL.GetDefaultProvider();
            try
            {
                adpVoidQueue.Fill(tblVoidQueue);
                foreach (var item in tblVoidQueue.Rows)
                {
                    CSMasterDataSet.VoidQueueRow voidItem = (CSMasterDataSet.VoidQueueRow)item;
                    CSPaymentProvider.Request request = GetVoidRequestFromOrderRow(voidItem);
                    CSPaymentProvider.Response response = pProvider.PerformVoidRequest(request);
                    voidItem.Request = response.GatewayRequestRaw;
                    if (voidItem.IsRequestNull()) voidItem.Request = "NULL";
                    voidItem.Response = response.GatewayResponseRaw;
                    switch (response.ResponseType)
                    {
                        case CSPaymentProvider.TransactionResponseType.Approved:
                            voidItem.Succeeded = true;
                            break;
                        case CSPaymentProvider.TransactionResponseType.Denied:
                            voidItem.Succeeded = false;
                            break;
                        case CSPaymentProvider.TransactionResponseType.Error:
                            voidItem.Succeeded = false;
                            if (voidItem.IsCommentsNull()) voidItem.Comments = string.Empty;
                            voidItem.Comments = string.Format("{0} --- {1} : {2}", voidItem.Comments, DateTime.Now.ToString(), response.ReasonText);
                            break;
                    }
                    SaveTransaction(voidItem);
                }
            }
            catch (Exception e)
            {
                return _breturn;
            }
            return _breturn;
        }

        /// <summary>
        /// Create a Request object from transaction data
        /// </summary>
        /// <param name="voidItem"></param>
        /// <returns></returns>
        public CSPaymentProvider.Request GetVoidRequestFromOrderRow(CSMasterDataSet.VoidQueueRow voidItem)
        {
            CSPaymentProvider.Request request = new CSPaymentProvider.Request();
            request.TransactionID = voidItem.TransactionID;
            request.AuthCode = voidItem.TransactionNumber;
            request.InvoiceNumber = voidItem.OrderID.ToString();
            return request;
        }

        /// <summary>
        /// Save this transaction in db
        /// </summary>
        /// <param name="voidItem"></param>
        /// <returns></returns>
        public int SaveTransaction(CSMasterDataSet.VoidQueueRow voidItem)
        {
            CSMasterDataSetTableAdapters.VoidQueueTableAdapter adpVoidQueue = new CSMasterDataSetTableAdapters.VoidQueueTableAdapter();
            return adpVoidQueue.SaveTransaction(voidItem.Request, voidItem.Response, voidItem.Succeeded, voidItem.Comments, voidItem.VoidId);            
        }

        public static void Main(string[] args)
        {   
            Batch StartBatch = new Batch();
            Console.WriteLine("Void queue processing - Started");
            Console.WriteLine("Please Wait - ");
            StartBatch.DoBatch();
            Console.WriteLine("Void queue processing  - End");
            Console.WriteLine("Task Completed - ");
            
        }
    }
  
}
