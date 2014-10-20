using System;
using System.Data;
using System.Configuration;
using System.Web;
using ConversionSystems.Celluscience.Utility;

namespace ConversionSystems.Celluscience.DataAccess
{
  public class DAL : IDisposable
    {
        private SQLServerDAL _oSQLServer = null;
        private SQLServerDAL_Get _oSQLServerDAL_Get = null;
        private SQLServerDAL_Try _oSQLServerDAL_Try = null;

        public SQLServerDAL SQLServer
        {
            get
            {
                if (_oSQLServer == null) { _oSQLServer = new SQLServerDAL(); }
                return _oSQLServer;
            }
        }
        public SQLServerDAL_Get SQLServerDAL_Get
        {
            get
            {
                if (_oSQLServerDAL_Get == null) { _oSQLServerDAL_Get = new SQLServerDAL_Get(); }
                return _oSQLServerDAL_Get;
            }
        }
        public SQLServerDAL_Try SQLServerDAL_Try
        {
            get
            {
                if (_oSQLServerDAL_Try == null) { _oSQLServerDAL_Try = new SQLServerDAL_Try(); }
                return _oSQLServerDAL_Try;
            }
        }

        public void Dispose()
        {
            if (_oSQLServer != null)
            {
                _oSQLServer.Dispose();
                _oSQLServer = null;
            }
            if (_oSQLServerDAL_Get != null)
            {
                _oSQLServerDAL_Get.Dispose();
                _oSQLServerDAL_Get = null;
            }
        }
    }
}
