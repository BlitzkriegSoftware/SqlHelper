#region "License"
// Blitzkrieg Software Superpack Libraries
// Copyright (c) 2002-2018 Stuart Williams (spookdejur@hotmail.com)
// MIT License 
#endregion

namespace Blitz.Lib.SqlHelper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Helpers for dealing with SQL Server
    /// </summary>
    public static class SqlHelper
    {
        #region "Constants"
        /// <summary>
        /// Default Timeout (seconds)
        /// </summary>
        public const int Timeout_Default = 3600;
        #endregion

        #region "Stored Procedures"

        /// <summary>
        /// Execute a stored procedure that returns rows
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="ProcedureName">SP Name</param>
        /// <param name="parameters">Stored Procedure Arguments</param>
        /// <param name="TimeOut">Timeout in seconds, with default</param>
        /// <returns>Datatable or null</returns>
        public static DataTable ExecuteStoredProcedureWithDataTable(string connectionString, string ProcedureName, List<SqlParameter> parameters, int TimeOut = Timeout_Default)
        {
            DataTable dt = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = ProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = TimeOut;
                    if (parameters != null) foreach (var p in parameters) command.Parameters.Add(p);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0)) dt = ds.Tables[0];
                }
                conn.Close();
            }

            return dt;
        }

        /// <summary>
        /// Executes a Stored Procedure with no rows returned
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="ProcedureName">SP Name</param>
        /// <param name="parameters">Stored Procedure Arguments</param>
        /// <param name="TimeOut">Timeout in seconds, with default</param>
        /// <returns>Rows affected (not always correct)</returns>
        public static int ExecuteStoredProcedureWithNoReturn(string connectionString, string ProcedureName, List<SqlParameter> parameters, int TimeOut = Timeout_Default)
        {
            int rows = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = ProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = TimeOut;
                    if (parameters != null) foreach (var p in parameters) command.Parameters.Add(p);
                    rows = command.ExecuteNonQuery();
                }
                conn.Close();
            }

            return rows;
        }

        /// <summary>
        /// Execute Stored Procedure With Parameters To Scaler
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="connectionString">Connection String</param>
        /// <param name="ProcedureName">SP Name</param>
        /// <param name="parameters">Stored Procedure Arguments</param>
        /// <param name="TimeOut">Timeout in seconds, with default</param>
        /// <returns></returns>
        public static T ExecuteStoredProcedureWithParametersToScaler<T>(string connectionString, string ProcedureName, List<SqlParameter> parameters, int TimeOut = Timeout_Default)
        {
            T data = default(T);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = ProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = TimeOut;
                    if (parameters != null) foreach (var p in parameters) command.Parameters.Add(p);
                    data = (T) command.ExecuteScalar();
                }
                conn.Close();
            }

            return data;
        }


        #endregion

        #region "SQL w. Parameters"

        /// <summary>
        /// Execute SQL with parameters with no return of data or values
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="SQL">SQL Statement</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="TimeOut">Timeout in seconds, with default</param>
        /// <returns>Rows affected</returns>
        public static int ExecuteSqlWithParametersNoReturn(string connectionString, string SQL, List<SqlParameter> parameters, int TimeOut = Timeout_Default)
        {
            System.Data.CommandType CmdType = System.Data.CommandType.Text;
            int iRows = 0;

            using (var conDB = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conDB.Open();
                using (SqlCommand command = new SqlCommand(SQL, conDB))
                {
                    command.CommandType = CmdType;
                    command.CommandTimeout = TimeOut;
                    if (parameters != null) foreach (var p in parameters) command.Parameters.Add(p);
                    var irows = command.ExecuteNonQuery();
                }
            }
            return iRows;
        }

        /// <summary>
        /// Execute SQL with parameters with a DataTable return
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="SQL">SQL Statement</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="TimeOut">Timeout in seconds, with default</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteSqlWithParametersToDataTable(string connectionString, string SQL, List<SqlParameter> parameters, int TimeOut = Timeout_Default)
        {
            System.Data.CommandType CmdType = System.Data.CommandType.Text;
            DataTable dt = null;

            using (var conDB = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conDB.Open();
                using (SqlCommand command = new SqlCommand(SQL, conDB))
                {
                    command.CommandType = CmdType;
                    command.CommandTimeout = TimeOut;
                    if (parameters != null) foreach (var p in parameters) command.Parameters.Add(p);
                    var rd = command.ExecuteReader();
                    dt = new DataTable();
                    dt.Load(rd);
                }
            }
            return dt;
        }

        /// <summary>
        /// Execute a parameterized SQL statement with a single value return
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="connectionString">Connection String</param>
        /// <param name="SQL"></param>
        /// <param name="parameters"></param>
        /// <param name="TimeOut"></param>
        /// <returns>T</returns>
        public static T ExecuteSqlWithParametersToScaler<T>(string connectionString, string SQL, List<SqlParameter> parameters, int TimeOut = Timeout_Default)
        {
            System.Data.CommandType CmdType = System.Data.CommandType.Text;
            T data = default(T);

            using (var conDB = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conDB.Open();
                using (SqlCommand command = new SqlCommand(SQL, conDB))
                {
                    command.CommandType = CmdType;
                    command.CommandTimeout = TimeOut;
                    if (parameters != null) foreach (var p in parameters) command.Parameters.Add(p);
                    var rd = command.ExecuteScalar();
                    data = (T)rd;
                }
            }
            return data;
        }

        #endregion

        #region "Utilities"


        /// <summary>
        /// Does data table have rows?
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>true if so</returns>
        public static bool HasRows(DataTable dt)
        {
            bool isOK = ((dt != null) && (dt.Rows != null) && (dt.Rows.Count > 0));
            return isOK;
        }

        /// <summary>
        /// Does data set have any tables?
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>true if so</returns>
        public static bool HasTables(DataSet ds)
        {
            bool isOK = false;
            if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0)) isOK = true;
            return isOK;
        }

        /// <summary>
        /// Fixes comma separated lists to be quoted correctly and removes last list delimiter for SQL 'IN' clauses
        /// </summary>
        /// <param name="inText">text to process</param>
        /// <param name="defaultValue">default value (can be empty)</param>
        /// <param name="textDelimiter">delimiter to wrap around each item</param>
        /// <returns>fixed up list</returns>
        public static string FixSqlInText(string inText, string defaultValue, char textDelimiter)
        {
            const char listDelimiter = ',';

            var workText = inText;
            if (string.IsNullOrWhiteSpace(workText)) workText = defaultValue;
            if (string.IsNullOrWhiteSpace(workText)) workText = string.Empty;

            if (!string.IsNullOrWhiteSpace(workText))
            {
                string[] buf = workText.Split(new char[] { listDelimiter }, StringSplitOptions.None);
                StringBuilder sb = new StringBuilder();
                foreach (string s in buf)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        sb.Append(textDelimiter);
                        sb.Append(s.Trim());
                        sb.Append(textDelimiter);
                        sb.Append(listDelimiter);
                    }
                }
                workText = sb.ToString();
                if ((!string.IsNullOrEmpty(workText)) && (workText.Length > 1))
                {
                    workText = workText.Trim();
                    workText = workText.Substring(0, workText.Length - 1); // Take off last listDelimiter
                }
            }
            return workText;
        }

        /// <summary>
        /// Clean a list of SQL Parameters to fix up strings by calling <see>SqlTextClean()</see> for each parameter that is 
        /// NText, NVarChar, Test, VarChar.
        /// </summary>
        /// <param name="parameters">Parameters to clean</param>
        /// <returns></returns>
        public static List<SqlParameter> CleanParameters(List<SqlParameter> parameters)
        {
            foreach (SqlParameter p in parameters)
            {
                if (p.Value != DBNull.Value)
                {
                    switch (p.SqlDbType)
                    {
                        case SqlDbType.NText:
                        case SqlDbType.NVarChar:
                        case SqlDbType.Text:
                        case SqlDbType.VarChar:
                            p.Value = SqlTextClean(p.Value.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
            return parameters;
        }


        /// <summary>
        /// Encodes apostrophe to two apostrophe keeping SQL statements from bombing
        /// </summary>
        /// <param name="inText">Text to clean</param>
        /// <returns>Clean text</returns>
        public static string SqlTextClean(string inText)
        {
            return inText.Replace("'", "''");
        }

        /// <summary>
        /// Parameter Builder
        /// </summary>
        /// <param name="name">Name, please include (at)</param>
        /// <param name="dbType">Type</param>
        /// <param name="value">Values</param>
        /// <returns>SqlParameter</returns>
        public static SqlParameter ParameterBuilder(string name, System.Data.SqlDbType dbType, object value)
        {
            var p = new SqlParameter(name, dbType);
            p.Value = value;
            return p;
        }

        #endregion
    }
}

