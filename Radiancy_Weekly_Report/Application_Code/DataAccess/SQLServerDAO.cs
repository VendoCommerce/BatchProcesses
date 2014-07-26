using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Com.ConversionSystems.Utility;

namespace Com.ConversionSystems.DataAccess
{
    public class SQLServerDAO : DAO
    {
        private const string _DEFAULT_CONFIG = "SQLServerDAO";
        private const string _FILENAME = "SQLServerDAO.cs";
        private SqlConnection _Connection = null;

        public SQLServerDAO() { }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // this version of the constructor takes in a config block
        // name in the case where there may be a non-default entry
        // to be used.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public SQLServerDAO(string strConfigBlockName)
            : this()
        {
            this.ConfigBlockName = strConfigBlockName;
        }

        public override string DefaultConfigBlockName
        {
            get
            {
                return _DEFAULT_CONFIG;
            }
        }


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // the Connection property is a pretty hard-working property
        // because it checks to see if we need a connection and if so,
        // it builds the thing for us.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public SqlConnection Connection
        {
            get
            {
                int intPhase = 0;
                LogData Err;
                try
                {
                    this.ClearErrors();
                    intPhase = -1;
                    string strConnectionString = this.AppSettings[this.Mode];

                    if (_Connection == null) { _Connection = new SqlConnection(strConnectionString); }
                    intPhase = -2;
                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
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

                    Err = new LogData(Helper.AppName, _FILENAME, "Connection", intPhase, strMessage, ex);
                    this.AddError(Err); Err.LogToDatabase(ex);
                    throw ex;
                }
                return _Connection;
            }
        }

        public SqlConnection GetConnection(string connectionName)
        {
                int intPhase = 0;
                LogData Err;
                try
                {
                    this.ClearErrors();
                    intPhase = -1;
                    string strConnectionString = this.AppSettings[connectionName];

                    if (_Connection == null) { _Connection = new SqlConnection(strConnectionString); }
                    intPhase = -2;
                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
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
                     
                    Err = new LogData(Helper.AppName, _FILENAME, "Connection", intPhase, strMessage, ex);
                    this.AddError(Err); Err.LogToDatabase(ex);
                    throw ex;
                }
                return _Connection;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // below are all the public overrides that satisfy the contract
        // with DAO abstract class and its abstract methods.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs)
        {
            bool bReturn = false;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            try
            {
                intPhase = -1;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -2;
                for (int idx = 0; idx < Inputs.Length; idx++)
                {
                    oCmd.Parameters.Add(Inputs[idx]);
                }
                intPhase = -3;
                bReturn = this.ExecCommand(oCmd);
            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[])", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DataTable dt)
        {
            bool bReturn = false;
            int intPhase = 0;
            dt = null;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -3;
                for (int idx = 0; idx < Inputs.Length; idx++)
                {
                    oCmd.Parameters.Add(Inputs[idx]);
                }
                bReturn = this.ExecCommand(oCmd, out dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[], out DataSet)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DataSet ds)
        {
            bool bReturn = false;
            int intPhase = 0;
            ds = null;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -3;
                for (int idx = 0; idx < Inputs.Length; idx++)
                {
                    oCmd.Parameters.Add(Inputs[idx]);
                }
                bReturn = this.ExecCommand(oCmd, out ds);
            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[], out DataSet)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DbParameter[] Outputs)
        {
            bool bReturn = false;
            int intPhase = 0;
            SqlCommand oCmd = null;
            Outputs = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -3;
                for (int idx = 0; idx < Inputs.Length; idx++)
                {
                    oCmd.Parameters.Add(Inputs[idx]);
                }

                intPhase = -4;
                bReturn = this.ExecCommand(oCmd);

                Outputs = new SqlParameter[oCmd.Parameters.Count];
                IEnumerator enumParams = oCmd.Parameters.GetEnumerator();
                int i = 0;
                while (enumParams.MoveNext())
                {
                    Outputs[i] = (SqlParameter)enumParams.Current;
                    i++;
                }
            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[], out DbParameter[])", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;

            return bReturn;
        }
        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DbParameter[] Outputs, out DataTable dt)
        {
            bool bReturn = false;
            int intPhase = 0;
            dt = null;
            SqlCommand oCmd = null;
            Outputs = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -3;
                for (int idx = 0; idx < Inputs.Length; idx++)
                {
                    oCmd.Parameters.Add(Inputs[idx]);
                }

                bReturn = this.ExecCommand(oCmd, out dt);

                Outputs = new SqlParameter[oCmd.Parameters.Count];
                IEnumerator enumParams = oCmd.Parameters.GetEnumerator();
                int i = 0;
                while (enumParams.MoveNext())
                {
                    Outputs[i] = (SqlParameter)enumParams.Current;
                    i++;
                }

            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[], out DbParameter[])", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs, out DbParameter[] Outputs, out DataSet ds)
        {
            bool bReturn = false;
            int intPhase = 0;
            ds = null;
            SqlCommand oCmd = null;
            Outputs = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -3;
                for (int idx = 0; idx < Inputs.Length; idx++)
                {
                    oCmd.Parameters.Add(Inputs[idx]);
                }

                bReturn = this.ExecCommand(oCmd, out ds);

                Outputs = new SqlParameter[oCmd.Parameters.Count];
                IEnumerator enumParams = oCmd.Parameters.GetEnumerator();
                int i = 0;
                while (enumParams.MoveNext())
                {
                    Outputs[i] = (SqlParameter)enumParams.Current;
                    i++;
                }

            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[], out DbParameter[])", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecStoredProc(string ProcName, DbParameter[] Inputs, string OutParamName, out object Value)
        {
            bool bReturn = false;
            Value = null;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand();
                oCmd.CommandText = ProcName;
                oCmd.CommandType = CommandType.StoredProcedure;

                intPhase = -3;
                for (int j = 0; j < Inputs.Length; j++) { oCmd.Parameters.Add(Inputs[j]); }

                intPhase = -4;
                bReturn = this.ExecCommand(oCmd);
                Value = oCmd.Parameters[OutParamName].Value;
                if (bReturn) { bReturn = (Value != null); }
            }
            catch (Exception ex)
            {
                strMessage = "Problem calling stored procedure:  " + ProcName + ".";
                Err = new LogData(Helper.AppName, _FILENAME,
                    "ExecStoredProc(string, DbParameter[], string, out object)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Parameters.Clear();
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
                oCmd = null;
            }
            return bReturn;
        }
        public override bool ExecSql(string Sql)
        {
            bool bReturn = false;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand(Sql);
                oCmd.CommandType = CommandType.Text;
                intPhase = -3;
                bReturn = this.ExecCommand(oCmd);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running Sql:  " + Sql + ".";
                Err = new LogData(Helper.AppName, _FILENAME, "ExecSql(string)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecSql(string Sql, out DataTable dt)
        {
            bool bReturn = false;
            dt = null;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand(Sql);
                oCmd.CommandType = CommandType.Text;
                intPhase = -3;
                bReturn = this.ExecCommand(oCmd, out dt);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running Sql:  " + Sql + ".";
                Err = new LogData(Helper.AppName, _FILENAME, "ExecSql(string, out DataSet)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }

        public override bool ExecSql(string Sql, out DataSet ds)
        {
            bool bReturn = false;
            ds = null;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand(Sql);
                oCmd.CommandType = CommandType.Text;
                intPhase = -3;
                bReturn = this.ExecCommand(oCmd, out ds);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running Sql:  " + Sql + ".";
                Err = new LogData(Helper.AppName, _FILENAME, "ExecSql(string, out DataSet)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
            }
            oCmd = null;
            return bReturn;
        }
        public override bool ExecSql(string Sql, out DbDataReader dr)
        {
            bool bReturn = false;
            dr = null;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand(Sql);
                oCmd.CommandType = CommandType.Text;
                intPhase = -3;
                oCmd.Connection = this.Connection;
                intPhase = -4;
                dr = oCmd.ExecuteReader();
                bReturn = true;
            }
            catch (Exception ex)
            {
                strMessage = "Problem running Sql:  " + Sql + ".";
                Err = new LogData(Helper.AppName, _FILENAME, "ExecSql(string, out DbDataReader)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
                oCmd = null;
            }
            return bReturn;
        }
        public override bool ExecSql(string Sql, out object Value)
        {
            bool bReturn = false;
            Value = null;
            int intPhase = 0;
            SqlCommand oCmd = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                oCmd = new SqlCommand(Sql);
                oCmd.CommandType = CommandType.Text;
                intPhase = -3;
                bReturn = this.ExecCommand(oCmd, out Value);
            }
            catch (Exception ex)
            {
                strMessage = "Problem running Sql:  " + Sql + ".";
                Err = new LogData(Helper.AppName, _FILENAME, "ExecSql(string, out object)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Connection = null;
                    oCmd.Dispose();
                }
                oCmd = null;
            }
            return bReturn;
        }
        public override bool ExecCommand(DbCommand cmd)
        {
            bool bReturn = false;
            int intPhase = 0;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                cmd.Connection = this.Connection;
                intPhase = -3;
                cmd.Prepare();
                intPhase = -4;
                int intResult = cmd.ExecuteNonQuery();
                bReturn = true;
            }
            catch (Exception ex)
            {
                if (cmd != null) { strMessage = "Command Text: " + cmd.CommandText; }
                Err = new LogData(Helper.AppName, _FILENAME, "ExecCommand(DbCommand)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                this.DisposeConnection(cmd);
            }
            return bReturn;
        }
        public override bool ExecCommand(DbCommand cmd, out DataTable dt)
        {
            bool bReturn = false;
            dt = null;
            int intPhase = 0;
            SqlDataAdapter da = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                cmd.Connection = this.Connection;
                intPhase = -3;
                cmd.Prepare();
                da = new SqlDataAdapter((SqlCommand)cmd);
                dt = new DataTable();
                intPhase = -4;
                da.Fill(dt);
                bReturn = true;
            }
            catch (Exception ex)
            {
                if (cmd != null) { strMessage = "Command Text: " + cmd.CommandText; }
                Err = new LogData(Helper.AppName, _FILENAME, "ExecCommand(DbCommand, out DataSet)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (da != null)
                {
                    da.SelectCommand = null;
                    da.Dispose();
                }
                da = null;
                this.DisposeConnection(cmd);
            }
            return bReturn;
        }
        public override bool ExecCommand(DbCommand cmd, out DataSet ds)
        {
            bool bReturn = false;
            ds = null;
            int intPhase = 0;
            SqlDataAdapter da = null;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                cmd.Connection = this.Connection;
                intPhase = -3;
                cmd.Prepare();
                da = new SqlDataAdapter((SqlCommand)cmd);
                ds = new DataSet();
                intPhase = -4;
                da.Fill(ds);
                bReturn = true;
            }
            catch (Exception ex)
            {
                if (cmd != null) { strMessage = "Command Text: " + cmd.CommandText; }
                Err = new LogData(Helper.AppName, _FILENAME, "ExecCommand(DbCommand, out DataSet)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (da != null)
                {
                    da.SelectCommand = null;
                    da.Dispose();
                }
                da = null;
                this.DisposeConnection(cmd);
            }
            return bReturn;
        }
        public override bool ExecCommand(DbCommand cmd, out DbDataReader dr)
        {
            bool bReturn = false;
            dr = null;
            int intPhase = 0;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                cmd.Connection = this.Connection;
                intPhase = -3;
                cmd.Prepare();
                intPhase = -4;
                dr = cmd.ExecuteReader();
                bReturn = (dr != null);
            }
            catch (Exception ex)
            {
                if (cmd != null) { strMessage = "Command Text: " + cmd.CommandText; }
                Err = new LogData(Helper.AppName, _FILENAME, "ExecCommand(DbCommand, out DbDataReader)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            finally
            {
                if (cmd != null) { cmd.Connection = null; }
            }
            return bReturn;
        }
        public override bool ExecCommand(DbCommand cmd, out object Value)
        {
            bool bReturn = false;
            Value = null;
            int intPhase = 0;
            LogData Err;
            string strMessage = "";
            intPhase = -1;
            try
            {
                intPhase = -2;
                cmd.Connection = this.Connection;
                intPhase = -3;
                cmd.Prepare();
                intPhase = -4;
                Value = cmd.ExecuteScalar();
                bReturn = (Value != null);
            }
            catch (Exception ex)
            {
                if (cmd != null) { strMessage = "Command Text: " + cmd.CommandText; }
                Err = new LogData(Helper.AppName, _FILENAME, "ExecCommand(DbCommand, out object)", intPhase, strMessage, ex);
                this.AddError(Err); Err.LogToDatabase(ex);
            }
            return bReturn;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Here we have implemented the Dispose in our subclass.
        // to ensure that we clean up our objects in a timely fashion.
        // this is especially important for database stuff because they
        // can be resource hogs.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override void Dispose()
        {
            if (_Connection != null)
            {
                try
                {
                    _Connection.Close();
                    _Connection.Dispose();
                }
                finally
                {
                    _Connection = null;
                }
            }
            GC.SuppressFinalize(this);
        }


    }
}
