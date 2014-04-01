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
        private SQLServerDALV3 _oSQLServerV3 = null;
        private SQLServerDALV2 _oSQLServerV2 = null;
        private SQLServerDALDirectMail _oSQLServerDALDirectMail = null;
        private SQLServerDALGlow _oSQLServerDALGlow = null;
        private SQLServerDALJoanSkin _oSQLServerDALJoanSkin = null;
        private SQLServerDALVersionA4 _oSQLServerDALVersionA4 = null;
        private SQLServerDALAcneComplexVersion2 _oSQLServerDALAcneComplexVersion2 = null;
        private SQLServerDALAcneComplexVersion3 _oSQLServerDALAcneComplexVersion3 = null;
        private SQLServerNoNoSkin _oSQLServerNoNoSkin = null;
        private SQLServerDALTzone _oSQLServerDALTzone = null;
        private SQLServerDALUK _oSQLServerDALUK = null;
        private SQLServerDALIreland _oSQLServerDALIreland = null;
        private SQLServerDALNoNoSkinUK _oSQLServerNoNoSkinUK = null;
        private SQLServerDALFR _oSQLServerDALFR = null;
        private SQLServerDALSpain _oSQLServerDALSpain = null;
        private SQLServerDALGermany _oSQLServerDALGermany = null;
        private SQLServerDALNeovaRemarketing _oSQLServerDALNeovaRemarketing = null;

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
        public SQLServerDALV3 SQLServerV3
        {
            get
            {
                if (_oSQLServerV3 == null) { _oSQLServerV3 = new SQLServerDALV3(); }
                return _oSQLServerV3;
            }
        }
        public SQLServerDALV2 SQLServerV2
        {
            get
            {
                if (_oSQLServerV2 == null) { _oSQLServerV2 = new SQLServerDALV2(); }
                return _oSQLServerV2;
            }
        }
        public SQLServerDALDirectMail SQLServerDALDirectMail
        {
            get
            {
                if (_oSQLServerDALDirectMail == null) { _oSQLServerDALDirectMail = new SQLServerDALDirectMail(); }
                return _oSQLServerDALDirectMail;
            }
        }
        public SQLServerDALGlow SQLServerDALGlow
        {
            get
            {
                if (_oSQLServerDALGlow == null) { _oSQLServerDALGlow = new SQLServerDALGlow(); }
                return _oSQLServerDALGlow;
            }
        }
        public SQLServerDALJoanSkin SQLServerDALJoanSkin
        {
            get
            {
                if (_oSQLServerDALJoanSkin == null) { _oSQLServerDALJoanSkin= new SQLServerDALJoanSkin(); }
                return _oSQLServerDALJoanSkin;
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
        public SQLServerDALAcneComplexVersion2 SQLServerDALAcneComplexVersion2
        {
            get
            {
                if (_oSQLServerDALAcneComplexVersion2 == null) { _oSQLServerDALAcneComplexVersion2 = new SQLServerDALAcneComplexVersion2(); }
                return _oSQLServerDALAcneComplexVersion2;
            }
        }
        public SQLServerDALAcneComplexVersion3 SQLServerDALAcneComplexVersion3
        {
            get
            {
                if (_oSQLServerDALAcneComplexVersion3 == null) { _oSQLServerDALAcneComplexVersion3 = new SQLServerDALAcneComplexVersion3(); }
                return _oSQLServerDALAcneComplexVersion3;
            }
        }
        public SQLServerNoNoSkin SQLServerNoNoSkin
        {
            get
            {
                if (_oSQLServerNoNoSkin == null) { _oSQLServerNoNoSkin = new SQLServerNoNoSkin(); }
                return _oSQLServerNoNoSkin;
            }
        }
        public SQLServerDALTzone SQLServerDALTzone
        {
            get
            {
                if (_oSQLServerDALTzone == null) { _oSQLServerDALTzone = new SQLServerDALTzone(); }
                return _oSQLServerDALTzone;
            }
        }
        public SQLServerDALUK SQLServerDALUK
        {
            get
            {
                if (_oSQLServerDALUK == null) { _oSQLServerDALUK = new SQLServerDALUK(); }
                return _oSQLServerDALUK;
            }
        }
        public SQLServerDALIreland SQLServerDALIreland
        {
            get
            {
                if (_oSQLServerDALIreland == null) { _oSQLServerDALIreland = new SQLServerDALIreland(); }
                return _oSQLServerDALIreland;
            }
        }
        public SQLServerDALNoNoSkinUK SQLServerDALNoNoSkinUK
        {
            get
            {
                if (_oSQLServerNoNoSkinUK == null) { _oSQLServerNoNoSkinUK = new SQLServerDALNoNoSkinUK(); }
                return _oSQLServerNoNoSkinUK;
            }
        }
        public SQLServerDALFR SQLServerDALFR
        {
            get
            {
                if (_oSQLServerDALFR == null) { _oSQLServerDALFR = new SQLServerDALFR(); }
                return _oSQLServerDALFR;
            }
        }
        public SQLServerDALSpain SQLServerDALSpain
        {
            get
            {
                if (_oSQLServerDALSpain == null) { _oSQLServerDALSpain = new SQLServerDALSpain(); }
                return _oSQLServerDALSpain;
            }
        }
        public SQLServerDALGermany SQLServerDALGermany
        {
            get
            {
                if (_oSQLServerDALGermany == null) { _oSQLServerDALGermany = new SQLServerDALGermany(); }
                return _oSQLServerDALGermany;
            }
        }
        public SQLServerDALNeovaRemarketing SQLServerDALNeovaRemarketing
        {
            get
            {
                if (_oSQLServerDALNeovaRemarketing == null) { _oSQLServerDALNeovaRemarketing = new SQLServerDALNeovaRemarketing(); }
                return _oSQLServerDALNeovaRemarketing;
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
