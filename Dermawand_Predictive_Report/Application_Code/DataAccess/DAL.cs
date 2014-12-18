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
        private SQLServerDALVersionA4 _oSQLServerDALVersionA4 = null;
        private SQLServerNoNoSkin _oSQLServerNoNoSkin = null;
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
        public SQLServerDALVersionA4 SQLServerDALVersionA4
        {
            get
            {
                if (_oSQLServerDALVersionA4 == null) { _oSQLServerDALVersionA4 = new SQLServerDALVersionA4(); }
                return _oSQLServerDALVersionA4;
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
