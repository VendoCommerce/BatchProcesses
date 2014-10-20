using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConversionSystems.Celluscience.DataAccess;

namespace ConversionSystems.Celluscience.UI
{
  public  class BaseClass
    {
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
