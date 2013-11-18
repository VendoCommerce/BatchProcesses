using System;
using System.Data;
using System.Configuration;
using System.Web;
using Com.ConversionSystems.Utility;

namespace Com.ConversionSystems.DataAccess
{
    public class DAL : IDisposable
    {
        private string _FILENAME = "DAL.cs";
        private SQLServerDAL _oSQLServer = null;
        
        //private OracleDAL _oOracleDAL = null;

        public DAL()
        {
        }

        public SQLServerDAL SQLServer
        {
            get
            {
                if (_oSQLServer == null) { _oSQLServer = new SQLServerDAL(); }
                return _oSQLServer;
            }
        }
        
        //public OracleDAL OracleDAL
        //{
        //    get
        //    {
        //        if (_oOracleDAL == null) { _oOracleDAL = new OracleDAL(); }
        //        return _oOracleDAL;
        //    }
        //}

        public void Dispose()
        {
            if (_oSQLServer != null)
            {
                _oSQLServer.Dispose();
                _oSQLServer = null;
            }
        }

    }
}
