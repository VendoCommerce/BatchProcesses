using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using Com.ConversionSystems.Utility;

namespace Com.ConversionSystems.DataAccess
{
    public abstract class DAO : IDisposable
    {
        private string _FILENAME = "DAO.cs";
        private string _strConfigBlockName = "";
        private ArrayList _alistErrors = new ArrayList();
        protected int _intErrorCode = 0;
        protected string _strErrorMessage = "";

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // throughout the execution of our data access code it is
        // possible to run into exceptions or other forms of errors.
        // although we may handle them gracefully, we may want to put
        // them on some sort of stack so we can output them to the
        // to the application or subsequent procedures (whatever).  we
        // use an ArrayList to store these.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public ArrayList Errors
        {
            get { return _alistErrors; }
        }
        protected void ClearErrors()
        {
            _alistErrors.Clear();
        }
        protected void AddError(LogData Error)
        {
            _alistErrors.Add(Error);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // we store our connection information in the web.config file.
        // the config information is a "Config Block" or section that
        // can be retrieved by name.  the concrete DAO implementations
        // will need to specify what that block is.  without a proper
        // config block the DAO subclasses won't work.

        public virtual string DefaultConfigBlockName
        {
            get { return ""; }
        }
        public string ConfigBlockName
        {
            get { return "appSettings"; }// return (_strConfigBlockName.Length == 0) ? DefaultConfigBlockName : _strConfigBlockName; }
            set { if (value != null) { _strConfigBlockName = value; } }
        }
        public NameValueCollection AppSettings
        {
            get { return (NameValueCollection)ConfigurationManager.GetSection(this.ConfigBlockName); }
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // the LoadQuery method is used to load query templates that
        // are stored as text files in the application (in a folder
        // somewhere.  this allows you to write SQL in your favorite
        // editor and then place it in a text file so our DataAccess
        // objects can load it into Command objects for parameter
        // binding and execution.  this is a far superior technique to
        // writing SQL in actual compiled code because it's easier to
        // debug and unit test for one thing and because existing query
        // templates can be replace while the application is running.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

       

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // in our configuration block we have a debug name/value pair
        // that we use to indicate whether the application is in debug
        // mode or not.  this is nice for switching from dev/test to
        // prod databases with minimal effort.  sometimes we want to
        // know what mode we're in so we can send out or log debug info.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        protected string Mode
        {
            get
            {
                /*
                  string strMode = "";
                  string strReturn = "";

                  strMode = this.AppSettings["Mode"].ToLower();
                  if (strMode == null) { strMode = "Dev"; }
                  else
                  {
                      strMode = Helper.Decode(strMode, "Dev", "Dev",
                          Helper.Decode(strMode, "Tst", "Tst",
                              Helper.Decode(strMode, "Prd", "Prd",
                                  Helper.Decode(strMode, "Auto", Helper.ServerMode, "Dev"))));
                  }
                  strReturn = strMode;
                  return strReturn;*/
                return Helper.ServerMode;
            }
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // whenever we connect to a database we're going to need a
        // connection string.  we get ours from our config block.  there
        // are two connection strings in that block:  dev/test and prod.
        // we figure out which one to get by determining what debug
        // mode we're in.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public string ConnectionString
        {
            get
            {
                string strConnectionString = "";
                string strReturn = "";
                strConnectionString = this.AppSettings[this.Mode];
                if (strConnectionString != null) { strReturn = strConnectionString; }
                return strReturn;
            }
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // DbCommand objects usually if not always have a DbConnection
        // associated with them.  this method can be used to clean up
        // the connection associated with a specific DbCommand object.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        protected void DisposeConnection(DbCommand Command)
        {
            if (Command != null)
            {
                if (Command.Connection != null) { Command.Connection.Close(); }
                Command.Connection = null;
            }
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // the abstract methods below must be implemented by any class
        // that extends this DAO class.  it's sort of like an interface
        // in that we've created a bit of a contract between this DAO
        // class and any classes that inherit from it.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs);
        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DataTable dt);
        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DataSet ds);
        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DbParameter[] Outputs);
        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DbParameter[] Outputs, out DataTable dt);
        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DbParameter[] Outputs, out DataSet ds);
        public abstract bool ExecStoredProc(string ProcName, DbParameter[] Inputs, string OutParamName, out object Value);
        public abstract bool ExecSql(string Sql);
        public abstract bool ExecSql(string Sql, out DataTable dt);
        public abstract bool ExecSql(string Sql, out DataSet ds);
        public abstract bool ExecSql(string Sql, out DbDataReader dr);
        public abstract bool ExecSql(string Sql, out object Value);
        public abstract bool ExecCommand(DbCommand cmd);
        public abstract bool ExecCommand(DbCommand cmd, out DataTable dt);
        public abstract bool ExecCommand(DbCommand cmd, out DataSet ds);
        public abstract bool ExecCommand(DbCommand cmd, out DbDataReader dr);
        public abstract bool ExecCommand(DbCommand cmd, out object Value);

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // we're going to need to implement Dispose in our subclasses
        // to ensure that we clean up our objects in a timely fashion.
        // this is especially important for database stuff because they
        // can be resource hogs.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public virtual void Dispose() { }

    }



}
