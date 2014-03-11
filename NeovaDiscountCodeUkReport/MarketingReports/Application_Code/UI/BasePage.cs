using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Security.Principal;
using System.Text;
using System.Web;
using Com.ConversionSystems.DataAccess;
using Com.ConversionSystems.Utility;

namespace Com.ConversionSystems.UI
{
    public class BasePage 
    {
        private string _FILENAME = "BasePage.cs";
        private DAL _oDAL;
        
        public DAL DAL
        {
            get
            {
                if (_oDAL == null) { _oDAL = new DAL(); }
                return _oDAL;
            }
        }
        
        
        
    }
}