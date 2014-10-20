using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.IO;

namespace ConversionSystems.Celluscience.Utility
{
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // the ErrorData class is based on the Data Object pattern found
    // in J2EE.  it's basically a user defined type.  it has a shape
    // in that it has properties, but it's not really intended to have
    // any behavior.  it's supposed to serve more as a data cartridge
    // that can be used to transfer data around in a lightweight way.
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public class LogData
    {
        private const string _FILENAME = "LogData.cs";
        private DateTime _dtInstantiationTime = new DateTime();
        private long _lngId = 0;
        private string _strAppName = "";
        private string _strFileName = "";
        private string _strFunction = "";
        private int _intPhase = 0;
        private string _strMessage = "";
        private int _intErrorCode = 0;
        private string _strUid = "";
        private string _strUrl = "";
        private bool _bIsError = false;
        private int _intSeverity = 0;
        private Exception _exError;
        private SqlConnection _sqlConnection;

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // public constructor methods (note the constructor chaining)
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public LogData()
        {
            // we're going to give this thing an id so we can
            // uniquely identify the object if it ends up in some
            // kind of collection.  perhaps we'll have a function
            // that outputs this ErrorData object as an XML node
            // that can be used to generate some rendering.  an Id
            // will be helpful for that.
            _lngId = DateTime.Now.Ticks;
        }

        public LogData(Exception Error)
            : this()
        {
            this.Error = Error;
        }

        public LogData(string User)
        {
            // we're going to give this thing an id so we can
            // uniquely identify the object if it ends up in some
            // kind of collection.  perhaps we'll have a function
            // that outputs this ErrorData object as an XML node
            // that can be used to generate some rendering.  an Id
            // will be helpful for that.
            _lngId = DateTime.Now.Ticks;
            this.Uid = User;
        }

        public LogData(string User, Exception Error)
            : this()
        {
            this.Error = Error;
            this.Uid = User;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
            string Message, int ErrorCode)
            : this()
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
            this.ErrorCode = ErrorCode;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
            string Message, Exception Error)
            : this(Error)
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
            string Message, int ErrorCode, Exception Error)
            : this(Error)
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
            this.ErrorCode = ErrorCode;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
          string Message, Exception Error, string User)
            : this(Error)
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
            this.Uid = User;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
           string Message, int ErrorCode, Exception Error, string User)
            : this(Error)
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
            this.ErrorCode = ErrorCode;
            this.Uid = User;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
            string Message, int ErrorCode, string Uid, string Url, int Severity)
            : this()
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
            this.ErrorCode = ErrorCode;
            this.Uid = Uid;
            this.Url = Url;
            this.Severity = Severity;
        }

        public LogData(string AppName, string FileName, string Function, int Phase,
            string Message, int ErrorCode, string Uid, string Url, int Severity,
            Exception Error)
            : this(Error)
        {
            this.AppName = AppName;
            this.FileName = FileName;
            this.Function = Function;
            this.Phase = Phase;
            this.Message = Message;
            this.ErrorCode = ErrorCode;
            this.Uid = Uid;
            this.Url = Url;
            this.Severity = Severity;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // public properties
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public DateTime InstantiationTime
        {
            get { return DateTime.Now; }
        }
        public long Id
        {
            get { return _lngId; }
        }
        public bool IsError
        {
            get { return _bIsError; }
            set { _bIsError = value; }
        }
        public string AppName
        {
            get
            {
                if (_strAppName == "")
                {
                    return Helper.AppName;
                }
                else
                {
                    return _strAppName;
                }
            }
            set { _strAppName = value; }
        }
        public string FileName
        {
            get { return _strFileName; }
            set { _strFileName = value; }
        }
        public string Function
        {
            get { return _strFunction; }
            set { _strFunction = value; }
        }
        public int Phase
        {
            get { return _intPhase; }
            set { _intPhase = value; }
        }
        public string Message
        {
            get { return _strMessage; }
            set { _strMessage = value; }
        }
        public int ErrorCode
        {
            get { return _intErrorCode; }
            set { _intErrorCode = value; }
        }
        public string Uid
        {
            get { return _strUid; }
            set { _strUid = value; }
        }
        public string Url
        {
            get { return _strUrl; }
            set { _strUrl = value; }
        }
        public int Severity
        {
            get { return _intSeverity; }
            set { _intSeverity = value; }
        }
        public Exception Error
        {
            get { return _exError; }
            set { _exError = value; }
        }

        private NameValueCollection ErrorLogAPI
        {
            get
            {
                return (NameValueCollection)ConfigurationSettings.GetConfig("ErrorLogAPI");
            }
        }

        private bool IsLogToDatabase
        {
            get
            {
                return (this.ErrorLogAPI["LogDatabase"] == "1");
            }
        }
        private bool IsLogToEmail
        {
            get
            {
                return (this.ErrorLogAPI["LogEmail"] == "1");
            }
        }
        private string ErrorEmail
        {
            get
            {
                return this.ErrorLogAPI["MonitorEmail"];
            }
        }
        private string ShadowEmail
        {
            get
            {
                return this.ErrorLogAPI["ShadowEmail"];
            }
        }

        // Public Methods

        public int LogToDatabase()
        {
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            LogData Err;
            SqlCommand oCmd = null;
            int intResult;

            int intAppLogId = 0;

            try
            {
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "CreateApplicationLog";
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = 1;
                // set the parameters
                oCmd.Parameters.Add(new SqlParameter("@ipInstanceID", SqlDbType.BigInt, 24, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Id));
                oCmd.Parameters.Add(new SqlParameter("@ipInstantiationTime", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, InstantiationTime));
                oCmd.Parameters.Add(new SqlParameter("@ipFileName", SqlDbType.VarChar, 256, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, FileName));
                oCmd.Parameters.Add(new SqlParameter("@ipFunction", SqlDbType.VarChar, 512, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Function));
                oCmd.Parameters.Add(new SqlParameter("@ipPhase", SqlDbType.Int, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Phase));
                oCmd.Parameters.Add(new SqlParameter("@ipMessage", SqlDbType.VarChar, 4000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, "ERROR occurred running the application, no exception specified"));
                oCmd.Parameters.Add(new SqlParameter("@ipErrorCode", SqlDbType.VarChar, 100, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ErrorCode));
                if (Error == null)
                {
                    oCmd.Parameters.Add(new SqlParameter("@ipStackTrace", SqlDbType.Text, 1000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ""));
                }
                else
                {
                    oCmd.Parameters.Add(new SqlParameter("@ipStackTrace", SqlDbType.Text, 1000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Error.ToString()));
                }
                oCmd.Parameters.Add(new SqlParameter("@ipUserID", SqlDbType.VarChar, 256, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Uid));
                oCmd.Parameters.Add(new SqlParameter("@ipSeverity", SqlDbType.Int, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Severity));
                oCmd.Parameters.Add(new SqlParameter("@ipIsError", SqlDbType.Bit, 1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, IsError));
                oCmd.Parameters.Add(new SqlParameter("@ipAppAcronym", SqlDbType.VarChar, 32, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppAcronym));
                oCmd.Parameters.Add(new SqlParameter("@ipAppName", SqlDbType.VarChar, 256, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppName));
                oCmd.Parameters.Add(new SqlParameter("@ipAppVersion", SqlDbType.VarChar, 32, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppVersion));
                oCmd.Parameters.Add(new SqlParameter("@ipAppReleaseType", SqlDbType.VarChar, 16, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppReleaseType));
                oCmd.Parameters.Add(new SqlParameter("@ipProducer", SqlDbType.VarChar, 16, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.Producer));
                oCmd.Parameters.Add(new SqlParameter("@ipAppReleaseDate", SqlDbType.VarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppReleaseDate));

                oCmd.Parameters.Add(new SqlParameter("@opApplicationLogID", SqlDbType.Int));
                oCmd.Parameters["@opApplicationLogID"].Direction = ParameterDirection.Output;
                if (InstantiationTime == DateTime.MinValue) { oCmd.Parameters["@ipInstantiationTime"].Value = DBNull.Value; }

                intPhase = 2;
                oCmd.Connection = this.SqlConnection;
                oCmd.Prepare();
                intResult = oCmd.ExecuteNonQuery();
                intAppLogId = Helper.ToInt32(oCmd.Parameters["@opApplicationLogID"].Value.ToString());

            }
            catch (Exception exception)
            {
                intAppLogId = 0;
            }
            finally
            {

            }
            oCmd = null;
            return intAppLogId;
        }

        public int LogToDatabase(Exception ex)
        {
            bool bReturn = false;
            int intPhase = -1;
            string strMessage;
            LogData Err;
            SqlCommand oCmd = null;
            int intResult;

            int intAppLogId = 0;

            try
            {
                intPhase = 0;
                oCmd = new SqlCommand();
                oCmd.CommandText = "CreateApplicationLog";
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = 1;
                // set the parameters
                oCmd.Parameters.Add(new SqlParameter("@ipInstanceID", SqlDbType.BigInt, 24, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Id));
                oCmd.Parameters.Add(new SqlParameter("@ipInstantiationTime", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, InstantiationTime));
                oCmd.Parameters.Add(new SqlParameter("@ipFileName", SqlDbType.VarChar, 256, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, FileName));
                oCmd.Parameters.Add(new SqlParameter("@ipFunction", SqlDbType.VarChar, 512, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Function));
                oCmd.Parameters.Add(new SqlParameter("@ipPhase", SqlDbType.Int, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Phase));
                oCmd.Parameters.Add(new SqlParameter("@ipMessage", SqlDbType.VarChar, 4000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ex.ToString()));
                oCmd.Parameters.Add(new SqlParameter("@ipErrorCode", SqlDbType.VarChar, 100, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ErrorCode));
                if (Error == null)
                {
                    oCmd.Parameters.Add(new SqlParameter("@ipStackTrace", SqlDbType.Text, 1000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, ""));
                }
                else
                {
                    oCmd.Parameters.Add(new SqlParameter("@ipStackTrace", SqlDbType.Text, 1000, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Error.ToString()));
                }
                oCmd.Parameters.Add(new SqlParameter("@ipUserID", SqlDbType.VarChar, 256, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Uid));
                oCmd.Parameters.Add(new SqlParameter("@ipSeverity", SqlDbType.Int, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Severity));
                oCmd.Parameters.Add(new SqlParameter("@ipIsError", SqlDbType.Bit, 1, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, IsError));
                oCmd.Parameters.Add(new SqlParameter("@ipAppAcronym", SqlDbType.VarChar, 32, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppAcronym));
                oCmd.Parameters.Add(new SqlParameter("@ipAppName", SqlDbType.VarChar, 256, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppName));
                oCmd.Parameters.Add(new SqlParameter("@ipAppVersion", SqlDbType.VarChar, 32, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppVersion));
                oCmd.Parameters.Add(new SqlParameter("@ipAppReleaseType", SqlDbType.VarChar, 16, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppReleaseType));
                oCmd.Parameters.Add(new SqlParameter("@ipProducer", SqlDbType.VarChar, 16, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.Producer));
                oCmd.Parameters.Add(new SqlParameter("@ipAppReleaseDate", SqlDbType.VarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, Helper.AppReleaseDate));

                oCmd.Parameters.Add(new SqlParameter("@opApplicationLogID", SqlDbType.Int));
                oCmd.Parameters["@opApplicationLogID"].Direction = ParameterDirection.Output;
                if (InstantiationTime == DateTime.MinValue) { oCmd.Parameters["@ipInstantiationTime"].Value = DBNull.Value; }

                intPhase = 2;
                oCmd.Connection = this.SqlConnection;
                oCmd.Prepare();
                intResult = oCmd.ExecuteNonQuery();
                intAppLogId = Helper.ToInt32(oCmd.Parameters["@opApplicationLogID"].Value.ToString());

            }
            catch (Exception exception)
            {
                intAppLogId = 0;
            }
            finally
            {

            }
            oCmd = null;
            return intAppLogId;
        }

        public bool LogToEvent(string strMessage, EventLogEntryType eType)
        {
            /*bool bResult = false;
            if (this.IsLogToEventLog && this.CanLogEvent)
            {
                int intFlag = -1;
                EventLog myLog = null;
                try
                {
                    myLog = new EventLog();
                    string strSource = "WTG Applications";
                    intFlag = -2;
                    if (!EventLog.SourceExists(strSource))
                    {
                        intFlag = -3;
                        EventLog.CreateEventSource(strSource, "Application");
                    }

                    intFlag = -4;
                    myLog.Source = strSource;

                    intFlag = -5;
                    myLog.WriteEntry("Application: " + this.ApplicationName + "\n" + strMessage, eType);
                    bResult = true;
                }
                catch (Exception ex)
                {
                    this.CanLogEvent = false;
                    string strErrorMessage = "Apps: " + this.ApplicationName + ", ErrorLog.LogToEvent, Flag: " + intFlag + ", Message: " + ex.Message;
                    this.LogToEmail(this.ErrorEmail, "", "", this.ErrorEmail, strErrorMessage);

                    Debug.WriteLine(strErrorMessage);
                    Debug.WriteLine(ex.StackTrace);
                    Trace.Write(strErrorMessage);
                    Trace.Write(ex.StackTrace);
                }
                finally
                {
                    if (myLog != null)
                    {
                        myLog.Close();
                        myLog.Dispose();
                    }
                    myLog = null;
                }
            }
            return bResult;*/
            return false;
        }

        public void LogToEmailAndDatabase(string StringException)
        {
            if (Error == null)
            {
                LogToDatabase(new Exception(StringException));
            }
            else
            {
                LogToDatabase(Error);
            }

        }
        public bool LogToFile(string AdditionalInfo)
        {
            bool bResult = false;
            StreamWriter log;


            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now);
            if (Error != null)
            {
                sb.Append("Exception error:" + Error.ToString() + "-");
            }
            sb.Append("-" + AdditionalInfo + "-");

            try
            {
                if (!File.Exists(Helper.AppSettings["ErrorLogFile"]))
                {
                    log = new StreamWriter(Helper.AppSettings["ErrorLogFile"]);
                }
                else
                {
                    log = File.AppendText(Helper.AppSettings["ErrorLogFile"]);
                }
                log.WriteLine(sb.ToString());
                log.Close();

            }
            catch (Exception ex)
            {
                bResult = false;
            }

            return bResult;
        }

        private string ConnectionString
        {
            get
            {
                if (ErrorLogAPI["Mode"] == "Prd")
                {
                    return ErrorLogAPI["Prd"].ToString();
                }
                else if (ErrorLogAPI["Mode"] == "Tst")
                {
                    return ErrorLogAPI["Tst"].ToString();
                }
                else
                {
                    return ErrorLogAPI["Dev"].ToString();
                }
            }
        }

        private SqlConnection SqlConnection
        {
            get
            {
                int intPhase = 0;
                LogData Err;
                try
                {
                    intPhase = -1;
                    if (_sqlConnection == null) { _sqlConnection = new SqlConnection(this.ConnectionString); }
                    intPhase = -2;
                    if (_sqlConnection.State != ConnectionState.Open) { _sqlConnection.Open(); }
                }
                catch (Exception ex)
                {
                    string strMessage = "ConnectionString: " + this.ConnectionString;
                    switch (intPhase)
                    {
                        case -1:
                            strMessage += "\n\rProblem creating database connection.";
                            break;
                        case -2:
                            strMessage += "\n\rProblem opening a database connection.";
                            break;
                        default:
                            strMessage += "\n\rProblem with database detected.";
                            break;
                    }
                    // may consider logging here.

                    Err = new LogData(Helper.AppName, FileName, "Connection", intPhase, strMessage, ex);
                    /*this.AddError(Err); Err.LogToDatabase(ex);
                    throw ex;*/
                }
                return _sqlConnection;
            }
        }

        private string Truncate(string str, int intLength)
        {
            string strResult = "";
            if (str != null)
            {
                if (str.Length > intLength)
                    strResult = str.Substring(0, intLength);
                else
                    strResult = str;
            }
            return strResult;
        }
    }
}
