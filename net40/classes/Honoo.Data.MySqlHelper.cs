/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

/*
 * MySqlConnector is not supported for NET40. NET40 is need for the MySql.Data 6.9.12 or earlier.
 */

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace Honoo.Data
{
    /// <summary>
    /// MySql helper.
    /// </summary>
    public static class MySqlHelper
    {
        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static MySqlConnection BuildConnection(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }
            return new MySqlConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns></returns>
        public static MySqlConnection BuildConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="host">Name or network address of the data server.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="database">The name of the database associated with the connection.</param>
        /// <returns></returns>
        public static MySqlConnection BuildConnection(string host, string userID, string password, string database)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(nameof(host));
            }
            string server;
            uint port;
            string[] split = host.Split(':');
            if (split.Length > 1)
            {
                server = split[0];
                port = Convert.ToUInt32(split[1], CultureInfo.InvariantCulture);
            }
            else
            {
                server = host;
                port = 3306;
            }
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                PersistSecurityInfo = false,
                Server = server,
                Port = port,
                UserID = userID,
                Password = password,
                Database = database,
                AllowUserVariables = true
            };
            return new MySqlConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="server">Name or network address of the data server.</param>
        /// <param name="port">Server port.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="database">The name of the database associated with the connection.</param>
        /// <returns></returns>
        public static MySqlConnection BuildConnection(string server, uint port, string userID, string password, string database)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                PersistSecurityInfo = false,
                Server = server,
                Port = port,
                UserID = userID,
                Password = password,
                Database = database,
                AllowUserVariables = true
            };
            return new MySqlConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static string BuildConnectionString(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }
            return connectionStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="host">Name or network address of the data server.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="database">The name of the database associated with the connection.</param>
        /// <returns></returns>
        public static string BuildConnectionString(string host, string userID, string password, string database)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(nameof(host));
            }
            string server;
            uint port;
            string[] split = host.Split(':');
            if (split.Length > 1)
            {
                server = split[0];
                port = Convert.ToUInt32(split[1], CultureInfo.InvariantCulture);
            }
            else
            {
                server = host;
                port = 3306;
            }
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                PersistSecurityInfo = false,
                Server = server,
                Port = port,
                UserID = userID,
                Password = password,
                Database = database,
                AllowUserVariables = true
            };
            return connectionStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="server">Name or network address of the data server.</param>
        /// <param name="port">Server port.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="database">The name of the database associated with the connection.</param>
        /// <returns></returns>
        public static string BuildConnectionString(string server, uint port, string userID, string password, string database)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                PersistSecurityInfo = false,
                Server = server,
                Port = port,
                UserID = userID,
                Password = password,
                Database = database,
                AllowUserVariables = true
            };
            return connectionStringBuilder.ConnectionString;
        }

        #endregion Connection

        #region DataAdapter

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, MySqlConnection connection, string selectCommandText)
        {
            return FillDataSet(dataSet, connection, selectCommandText, null);
        }

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static int FillDataSet(DataSet dataSet, MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            using (var dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                return dataAdapter.Fill(dataSet);
            }
        }

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static int FillDataTable(DataTable dataTable, MySqlConnection connection, string selectCommandText)
        {
            return FillDataTable(dataTable, connection, selectCommandText, null);
        }

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static int FillDataTable(DataTable dataTable, MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            using (var dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                return dataAdapter.Fill(dataTable);
            }
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static MySqlDataAdapter GetDataAdapter(MySqlConnection connection, string selectCommandText)
        {
            return new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static MySqlDataAdapter GetDataAdapter(MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            var dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
            return dataAdapter;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(MySqlConnection connection, string selectCommandText)
        {
            return GetDataSet(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static DataSet GetDataSet(MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            var dataSet = new DataSet();
            using (var dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(dataSet);
            }
            return dataSet;
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(MySqlConnection connection, string selectCommandText)
        {
            return GetDataTable(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static DataTable GetDataTable(MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            var dataTable = new DataTable();
            using (var dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(dataTable);
            }
            return dataTable;
        }

        #endregion DataAdapter

        #region Command, DataReader, XmlReader

        /// <summary>
        /// Get Command. Create DataReader by Command.ExecuteReader(commandBehavior). Create XmlReader by Command.ExecuteXmlReader().
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static MySqlCommand GetCommand(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return GetCommand(connection, commandType, commandText, null);
        }

        /// <summary>
        /// Get Command. Create DataReader by Command.ExecuteReader(commandBehavior). Create XmlReader by Command.ExecuteXmlReader().
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static MySqlCommand GetCommand(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parameters)
        {
            var command = new MySqlCommand(commandText, connection) { CommandType = commandType };
            if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
            return command;
        }

        #endregion Command, DataReader, XmlReader

        #region Execute

        /// <summary>
        /// Execute the sql command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }

        /// <summary>
        /// Execute the sql command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parameters)
        {
            using (var command = new MySqlCommand(commandText, connection) { CommandType = commandType })
            {
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute the sql command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, null);
        }

        /// <summary>
        /// Execute the sql command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parameters)
        {
            using (var command = new MySqlCommand(commandText, connection) { CommandType = commandType })
            {
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                return command.ExecuteScalar();
            }
        }

        #endregion Execute

        #region Transaction

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return TransactionExecuteNonQuery(connection, commandType, commandText, IsolationLevel.ReadCommitted, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
        {
            return TransactionExecuteNonQuery(connection, commandType, commandText, isolationLevel, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static int TransactionExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params MySqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            int result = 0;
            Exception exception = null;
            using (MySqlTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (var command = new MySqlCommand(commandText, connection) { CommandType = commandType })
                {
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        result += command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                        exception = ex;
                    }
                }
            }
            if (exception is null)
            {
                return result;
            }
            else
            {
                throw exception;
            }
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return TransactionExecuteScalar(connection, commandType, commandText, IsolationLevel.ReadCommitted, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
        {
            return TransactionExecuteScalar(connection, commandType, commandText, isolationLevel, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <param name="commandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public static object TransactionExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params MySqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            object result = null;
            Exception exception = null;
            using (MySqlTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (var command = new MySqlCommand(commandText, connection) { CommandType = commandType })
                {
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        result = command.ExecuteScalar();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                        exception = ex;
                    }
                }
            }
            if (exception is null)
            {
                return result;
            }
            else
            {
                throw exception;
            }
        }

        #endregion Transaction

        #region Dump

        /// <summary>
        /// Dump database.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="manifest">Dump manifest.</param>
        /// <param name="textWriter">TextWriter.</param>
        public static void Dump(MySqlConnection connection, MySqlDumpManifest manifest, TextWriter textWriter)
        {
            Dump(connection, manifest, textWriter, null, null, out _);
        }

        /// <summary>
        /// Dump database.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="manifest">Dump manifest.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void Dump(MySqlConnection connection, MySqlDumpManifest manifest, TextWriter textWriter, MySqlWrittenCallback written, object userState, out bool cancelled)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (manifest is null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }
            if (textWriter is null)
            {
                throw new ArgumentNullException(nameof(textWriter));
            }
            var summary = BuildSummary(connection, manifest);
            bool cancel = false;
            long index = 0;
            textWriter.Write(summary.Text);
            written?.Invoke(0, summary.Total, MySqlDumpProjectType.Summary, string.Empty, userState, ref cancel);
            if (!cancel && summary.TableCount > 0)
            {
                DumpTables(connection, manifest.Tables, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            if (!cancel && summary.ViewCount > 0)
            {
                DumpViews(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            if (!cancel && summary.TriggerCount > 0)
            {
                DumpTriggers(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            if (!cancel && summary.FunctionCount > 0)
            {
                DumpFunctions(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            if (!cancel && summary.ProcedureCount > 0)
            {
                DumpProcedures(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            if (!cancel && summary.EventCount > 0)
            {
                DumpEvents(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            if (!cancel && summary.RecordCount > 0)
            {
                DumpRecords(connection, manifest.Tables, textWriter, ref index, summary.Total, written, userState, ref cancel);
            }
            textWriter.Flush();
            cancelled = cancel;
        }

        /// <summary>
        /// Dump database to file(s).
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="manifest">Dump manifest.</param>
        /// <param name="folder">Save to folder.</param>
        /// <param name="encoding">File encoding.</param>
        /// <param name="splitFileSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        public static void DumpToFiles(MySqlConnection connection, MySqlDumpManifest manifest, string folder, Encoding encoding, long splitFileSize)
        {
            DumpToFiles(connection, manifest, folder, encoding, splitFileSize, null, null, out _);
        }

        /// <summary>
        /// Dump database to file(s).
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="manifest">Dump manifest.</param>
        /// <param name="folder">Save to folder.</param>
        /// <param name="encoding">File encoding.</param>
        /// <param name="splitFileSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void DumpToFiles(MySqlConnection connection,
                                       MySqlDumpManifest manifest,
                                       string folder,
                                       Encoding encoding,
                                       long splitFileSize,
                                       MySqlWrittenCallback written,
                                       object userState,
                                       out bool cancelled)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (splitFileSize < 1024 * 1024)
            {
                throw new ArgumentException("File size cannot be less than 1 MB.");
            }
            //
            var summary = BuildSummary(connection, manifest);
            bool cancel = false;
            long index = 0;
            string file = Path.Combine(folder, "!schema.sql");
            using (var stream = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
            {
                using (var textWriter = new StreamWriter(stream, encoding))
                {
                    textWriter.Write(summary.Text);
                    written?.Invoke(0, summary.Total, MySqlDumpProjectType.Summary, string.Empty, userState, ref cancel);
                    if (!cancel && summary.TableCount > 0)
                    {
                        DumpTables(connection, manifest.Tables, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.ViewCount > 0)
                    {
                        DumpViews(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.TriggerCount > 0)
                    {
                        DumpTriggers(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.FunctionCount > 0)
                    {
                        DumpFunctions(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.ProcedureCount > 0)
                    {
                        DumpProcedures(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.EventCount > 0)
                    {
                        DumpEvents(connection, manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    textWriter.Flush();
                }
            }
            if (!cancel && summary.RecordCount > 0)
            {
                DumpRecords(connection, manifest.Tables, folder, encoding, splitFileSize, ref index, summary.Total, written, userState, ref cancel);
            }
            cancelled = cancel;
        }

        /// <summary>
        /// Get dump manifest.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <returns></returns>
        public static MySqlDumpManifest GetDumpManifest(MySqlConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            var manifest = new MySqlDumpManifest();
            var sql = new StringBuilder();
            sql.AppendLine(MySqlCommandText.ShowTableStatus());
            sql.AppendLine(MySqlCommandText.ShowTriggers());
            sql.AppendLine(MySqlCommandText.ShowFunctionStatus(connection.Database));
            sql.AppendLine(MySqlCommandText.ShowProcedureStatus(connection.Database));
            sql.AppendLine(MySqlCommandText.ShowEvents(connection.Database));
            using (DataSet ds = GetDataSet(connection, sql.ToString()))
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ENGINE"] != DBNull.Value)
                        {
                            manifest.Tables.Add(new MySqlTableDumpProject((string)dr["Name"], false, true));
                        }
                        else
                        {
                            manifest.Views.Add(new MySqlDumpProject((string)dr["Name"], false));
                        }
                    }
                }
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    manifest.Triggers.Add(new MySqlDumpProject((string)dr["Trigger"], false));
                }
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    manifest.Functions.Add(new MySqlDumpProject((string)dr["Name"], false));
                }
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    manifest.Procedures.Add(new MySqlDumpProject((string)dr["Name"], false));
                }
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    manifest.Events.Add(new MySqlDumpProject((string)dr["Name"], false));
                }
            }
            return manifest;
        }

        private static MySqlSummary BuildSummary(MySqlConnection connection, MySqlDumpManifest manifest)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (manifest is null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }
            var summary = new MySqlSummary();
            var union = new List<string>();
            foreach (MySqlTableDumpProject table in manifest.Tables)
            {
                if (!table.Ignore)
                {
                    if (table.IncludingRecord)
                    {
                        union.Add("SELECT COUNT(*) AS `count` FROM `" + table.TableName + "`");
                    }
                    summary.TableCount++;
                }
            }
            if (union.Count > 0)
            {
                using (DataTable dt = GetDataTable(connection, string.Join(" UNION ALL ", union) + ";"))
                {
                    summary.RecordCount = long.Parse(dt.Compute("SUM(count)", string.Empty).ToString(), CultureInfo.InvariantCulture);
                }
            }
            foreach (MySqlDumpProject view in manifest.Views)
            {
                if (!view.Ignore)
                {
                    summary.ViewCount++;
                }
            }
            foreach (MySqlDumpProject trigger in manifest.Triggers)
            {
                if (!trigger.Ignore)
                {
                    summary.TriggerCount++;
                }
            }
            foreach (MySqlDumpProject function in manifest.Functions)
            {
                if (!function.Ignore)
                {
                    summary.FunctionCount++;
                }
            }
            foreach (MySqlDumpProject procedure in manifest.Procedures)
            {
                if (!procedure.Ignore)
                {
                    summary.ProcedureCount++;
                }
            }
            foreach (MySqlDumpProject event_ in manifest.Events)
            {
                if (!event_.Ignore)
                {
                    summary.EventCount++;
                }
            }
            summary.Total = summary.TableCount
                + summary.ViewCount
                + summary.TriggerCount
                + summary.FunctionCount
                + summary.ProcedureCount
                + summary.EventCount
                + summary.RecordCount;
            using (DataTable dt = GetDataTable(connection, "SELECT @@version, @@character_set_server, @@collation_server;"))
            {
                var tmp = new StringBuilder();
                tmp.AppendLine("/*");
                tmp.AppendLine(" * Dump by Honoo.Data.MySqlHelper");
                tmp.AppendLine(" * https://github.com/LokiHonoo/development-resources");
                tmp.AppendLine(" * This code page is published by the MIT license.");
                tmp.AppendLine(" * ");
                tmp.AppendLine(" * DataSource     : " + connection.DataSource);
                tmp.AppendLine(" * Server Version : " + (string)dt.Rows[0][0]);
                tmp.AppendLine(" * Character_set  : " + (string)dt.Rows[0][1]);
                tmp.AppendLine(" * Collation      : " + (string)dt.Rows[0][2]);
                tmp.AppendLine(" * Database       : " + connection.Database);
                tmp.AppendLine(" * ");
                tmp.AppendLine(" * Table          : " + summary.TableCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * View           : " + summary.ViewCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * Trigger        : " + summary.TriggerCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * Function       : " + summary.FunctionCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * Procedure      : " + summary.ProcedureCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * Event          : " + summary.EventCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * Record         : " + summary.RecordCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine(" * ");
                tmp.AppendLine(" * Dump Time(UTC) : " + DateTime.UtcNow);
                tmp.AppendLine(" * ");
                tmp.AppendLine(" * se the console or database tool to recover data.");
                tmp.AppendLine(" * If the target database has a table with the same name, the table data is overwritten.");
                tmp.AppendLine(" */");
                tmp.AppendLine();
                summary.Text = tmp.ToString();
            }
            return summary;
        }

        private static void DumpEvents(MySqlConnection connection,
                                       List<MySqlDumpProject> events,
                                       TextWriter textWriter,
                                       ref long index,
                                       long total,
                                       MySqlWrittenCallback written,
                                       object userState,
                                       ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (MySqlDumpProject event_ in events)
            {
                if (cancel)
                {
                    break;
                }
                if (!event_.Ignore)
                {
                    using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateEvent(event_.Name)))
                    {
                        string eventCreate = (string)create.Rows[0][3];
                        tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                        tmp.AppendLine("-- Event structure for \"" + event_ + "\"");
                        tmp.AppendLine("-- --------------------------------------------------------");
                        tmp.AppendLine("DROP EVENT IF EXISTS `" + event_ + "`;");
                        tmp.AppendLine("DELIMITER ;;");
                        tmp.AppendLine(eventCreate + ";;");
                        tmp.AppendLine("DELIMITER ;");
                        tmp.AppendLine();
                        textWriter.Write(tmp);
                        tmp.Clear();
                        index++;
                        written?.Invoke(index, total, MySqlDumpProjectType.Event, event_.Name, userState, ref cancel);
                    }
                }
            }
        }

        private static void DumpFunctions(MySqlConnection connection,
                                          List<MySqlDumpProject> functions,
                                          TextWriter textWriter,
                                          ref long index,
                                          long total,
                                          MySqlWrittenCallback written,
                                          object userState,
                                          ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (MySqlDumpProject function in functions)
            {
                if (cancel)
                {
                    break;
                }
                if (!function.Ignore)
                {
                    using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateFunction(function.Name)))
                    {
                        string functionCreate = (string)create.Rows[0][2];
                        tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                        tmp.AppendLine("-- Function structure for \"" + function + "\"");
                        tmp.AppendLine("-- --------------------------------------------------------");
                        tmp.AppendLine("DROP FUNCTION IF EXISTS `" + function + "`;");
                        tmp.AppendLine("DELIMITER ;;");
                        tmp.AppendLine(functionCreate + ";;");
                        tmp.AppendLine("DELIMITER ;");
                        tmp.AppendLine();
                        textWriter.Write(tmp);
                        tmp.Clear();
                        index++;
                        written?.Invoke(index, total, MySqlDumpProjectType.Function, function.Name, userState, ref cancel);
                    }
                }
            }
        }

        private static void DumpProcedures(MySqlConnection connection,
                                           List<MySqlDumpProject> procedures,
                                           TextWriter textWriter,
                                           ref long index,
                                           long total,
                                           MySqlWrittenCallback written,
                                           object userState,
                                           ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (MySqlDumpProject procedure in procedures)
            {
                if (cancel)
                {
                    break;
                }
                if (!procedure.Ignore)
                {
                    using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateProcedure(procedure.Name)))
                    {
                        string procedureCreate = (string)create.Rows[0][2];
                        tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                        tmp.AppendLine("-- Procedure structure for \"" + procedure + "\"");
                        tmp.AppendLine("-- --------------------------------------------------------");
                        tmp.AppendLine("DROP PROCEDURE IF EXISTS `" + procedure + "`;");
                        tmp.AppendLine("DELIMITER ;;");
                        tmp.AppendLine(procedureCreate + ";;");
                        tmp.AppendLine("DELIMITER ;");
                        tmp.AppendLine();
                        textWriter.Write(tmp);
                        tmp.Clear();
                        index++;
                        written?.Invoke(index, total, MySqlDumpProjectType.Procedure, procedure.Name, userState, ref cancel);
                    }
                }
            }
        }

        private static void DumpRecords(MySqlConnection connection,
                                        List<MySqlTableDumpProject> tables,
                                        string folder,
                                        Encoding encoding,
                                        long splitFileSize,
                                        ref long index,
                                        long total,
                                        MySqlWrittenCallback written,
                                        object userState,
                                        ref bool cancel)
        {
            foreach (MySqlTableDumpProject table in tables)
            {
                if (cancel)
                {
                    break;
                }
                if (!table.Ignore && table.IncludingRecord)
                {
                    using (var command = GetCommand(connection, CommandType.Text, "SELECT * FROM `" + table.TableName + "`;"))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            if (reader.HasRows)
                            {
                                DumpRecords(table.TableName, reader, folder, encoding, splitFileSize, ref index, total, written, userState, ref cancel);
                            }
                            reader.Close();
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:为了清晰起见，请指定 StringComparison", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        private static void DumpRecords(MySqlConnection connection,
                                        List<MySqlTableDumpProject> tables,
                                        TextWriter textWriter,
                                        ref long index,
                                        long total,
                                        MySqlWrittenCallback written,
                                        object userState,
                                        ref bool cancel)
        {
            var tmp = new StringBuilder();
            textWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
            textWriter.WriteLine();
            foreach (MySqlTableDumpProject table in tables)
            {
                if (cancel)
                {
                    break;
                }
                if (!table.Ignore && table.IncludingRecord)
                {
                    string tableName = table.TableName;
                    using (var command = GetCommand(connection, CommandType.Text, "SELECT * FROM `" + tableName + "`;"))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            if (reader.HasRows)
                            {
                                tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                                tmp.AppendLine("-- Records of \"" + tableName + "\"");
                                tmp.AppendLine("-- --------------------------------------------------------");
                                while (reader.Read())
                                {
                                    if (cancel)
                                    {
                                        break;
                                    }
                                    tmp.Append("INSERT INTO `" + tableName + "` VALUES");
                                    tmp.Append('(');
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        object val = reader.GetValue(i);
                                        if (reader.IsDBNull(i))
                                        {
                                            tmp.Append("NULL");
                                        }
                                        else
                                        {
                                            switch (val)
                                            {
                                                case bool value: tmp.Append(value ? 1 : 0); break;
                                                case sbyte value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case byte value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case short value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case ushort value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case int value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case uint value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case long value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case ulong value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case double value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case float value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case decimal value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                case byte[] value: tmp.Append("UNHEX('" + BitConverter.ToString(value).Replace("-", string.Empty) + "')"); break;
                                                default: tmp.Append("'" + val.ToString() + "'"); break;
                                            }
                                        }
                                        if (i < reader.FieldCount - 1)
                                        {
                                            tmp.Append(',');
                                        }
                                    }
                                    tmp.AppendLine(");");
                                    textWriter.Write(tmp);
                                    tmp.Clear();
                                    index++;
                                    written?.Invoke(index, total, MySqlDumpProjectType.Record, tableName, userState, ref cancel);
                                }
                            }
                            reader.Close();
                        }
                    }
                }
            }
            textWriter.WriteLine();
            textWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 1;");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:为了清晰起见，请指定 StringComparison", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        private static void DumpRecords(string tableName,
                                        MySqlDataReader reader,
                                        string folder,
                                        Encoding encoding,
                                        long splitFileSize,
                                        ref long index,
                                        long total,
                                        MySqlWrittenCallback written,
                                        object userState,
                                        ref bool cancel)
        {
            var tmp = new StringBuilder();
            int sn = 0;
            string file = Path.Combine(folder, "records@" + tableName + ".sql");
            var stream = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
            var streamWriter = new StreamWriter(stream, encoding);
            streamWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("-- ----------------------------------------------------------------------------------------------------------------");
            streamWriter.WriteLine("-- Records of \"" + tableName + "\"");
            streamWriter.WriteLine("-- --------------------------------------------------------");
            while (reader.Read())
            {
                if (cancel)
                {
                    break;
                }
                tmp.Append("INSERT INTO `" + tableName + "` VALUES");
                tmp.Append('(');
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object val = reader.GetValue(i);
                    if (reader.IsDBNull(i))
                    {
                        tmp.Append("NULL");
                    }
                    else
                    {
                        switch (val)
                        {
                            case bool value: tmp.Append(value ? 1 : 0); break;
                            case sbyte value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case byte value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case short value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case ushort value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case int value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case uint value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case long value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case ulong value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case double value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case float value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case decimal value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                            case byte[] value: tmp.Append("UNHEX('" + BitConverter.ToString(value).Replace("-", string.Empty) + "')"); break;
                            default: tmp.Append("'" + val.ToString() + "'"); break;
                        }
                    }
                    if (i < reader.FieldCount - 1)
                    {
                        tmp.Append(',');
                    }
                }
                tmp.AppendLine(");");
                if (streamWriter.BaseStream.Length + (tmp.Length * 3) > splitFileSize)
                {
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 1;");
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                    stream.Close();
                    stream.Dispose();
                    sn++;
                    file = Path.Combine(folder, "records@" + tableName + "@p" + sn + ".sql");
                    stream = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                    streamWriter = new StreamWriter(stream, encoding);
                    streamWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("-- ----------------------------------------------------------------------------------------------------------------");
                    streamWriter.WriteLine("-- Records of \"" + tableName + "\"");
                    streamWriter.WriteLine("-- --------------------------------------------------------");
                }
                streamWriter.Write(tmp);
                tmp.Clear();
                index++;
                written?.Invoke(index, total, MySqlDumpProjectType.Record, tableName, userState, ref cancel);
            }
            streamWriter.WriteLine();
            streamWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 1;");
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
            stream.Close();
            stream.Dispose();
        }

        private static void DumpTables(MySqlConnection connection,
                                       List<MySqlTableDumpProject> tables,
                                       TextWriter textWriter,
                                       ref long index,
                                       long total,
                                       MySqlWrittenCallback written,
                                       object userState,
                                       ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (MySqlTableDumpProject table in tables)
            {
                if (cancel)
                {
                    break;
                }
                if (!table.Ignore)
                {
                    using (DataSet info = GetDataSet(connection, MySqlCommandText.ShowCreateTable(table.TableName) + MySqlCommandText.ShowTableStatus(table.TableName)))
                    {
                        string tableCreate = (string)info.Tables[0].Rows[0][1];
                        tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                        tmp.AppendLine("-- Table structure for \"" + table.TableName + "\"");
                        tmp.AppendLine("-- --------------------------------------------------------");
                        tmp.AppendLine("DROP TABLE IF EXISTS `" + table.TableName + "`;");
                        tmp.AppendLine(tableCreate + ";");
                        tmp.AppendLine();
                        textWriter.Write(tmp);
                        tmp.Clear();
                        index++;
                        written?.Invoke(index, total, MySqlDumpProjectType.Table, table.TableName, userState, ref cancel);
                    }
                }
            }
        }

        private static void DumpTriggers(MySqlConnection connection,
                                         List<MySqlDumpProject> triggers,
                                         TextWriter textWriter,
                                         ref long index,
                                         long total,
                                         MySqlWrittenCallback written,
                                         object userState,
                                         ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (MySqlDumpProject trigger in triggers)
            {
                if (cancel)
                {
                    break;
                }
                if (!trigger.Ignore)
                {
                    using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateTrigger(trigger.Name)))
                    {
                        string triggerCreate = (string)create.Rows[0][2];
                        tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                        tmp.AppendLine("-- Trigger structure for \"" + trigger + "\"");
                        tmp.AppendLine("-- --------------------------------------------------------");
                        tmp.AppendLine("DROP TRIGGER IF EXISTS `" + trigger + "`;");
                        tmp.AppendLine("DELIMITER ;;");
                        tmp.AppendLine(triggerCreate + ";;");
                        tmp.AppendLine("DELIMITER ;");
                        tmp.AppendLine();
                        textWriter.Write(tmp);
                        tmp.Clear();
                        index++;
                        written?.Invoke(index, total, MySqlDumpProjectType.Trigger, trigger.Name, userState, ref cancel);
                    }
                }
            }
        }

        private static void DumpViews(MySqlConnection connection,
                                      List<MySqlDumpProject> views,
                                      TextWriter textWriter,
                                      ref long index,
                                      long total,
                                      MySqlWrittenCallback written,
                                      object userState,
                                      ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (MySqlDumpProject view in views)
            {
                if (cancel)
                {
                    break;
                }
                if (!view.Ignore)
                {
                    using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateView(view.Name)))
                    {
                        string viewCreate = (string)create.Rows[0][1];
                        tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                        tmp.AppendLine("-- View structure for \"" + view + "\"");
                        tmp.AppendLine("-- --------------------------------------------------------");
                        tmp.AppendLine("DROP VIEW IF EXISTS `" + view + "`;");
                        tmp.AppendLine(viewCreate + ";");
                        tmp.AppendLine();
                        textWriter.Write(tmp);
                        tmp.Clear();
                        index++;
                        written?.Invoke(index, total, MySqlDumpProjectType.View, view.Name, userState, ref cancel);
                    }
                }
            }
        }

        #endregion Dump

        #region Private class

        private struct MySqlSummary
        {
            internal long EventCount { get; set; }
            internal long FunctionCount { get; set; }
            internal long ProcedureCount { get; set; }
            internal long RecordCount { get; set; }
            internal long TableCount { get; set; }
            internal string Text { get; set; }
            internal long Total { get; set; }
            internal long TriggerCount { get; set; }
            internal long ViewCount { get; set; }
        }

        #endregion Private class
    }

    #region Dump

    /// <summary>
    /// Delegate for dumping progress.
    /// </summary>
    /// <param name="written">Block index written.</param>
    /// <param name="total">The amount of dumping block.</param>
    /// <param name="projectType">The project type of dumping.</param>
    /// <param name="association">The name associated with dumping.</param>
    /// <param name="userState">User state.</param>
    /// <param name="cancel">Cancel dump.</param>
    public delegate void MySqlWrittenCallback(long written, long total, MySqlDumpProjectType projectType, string association, object userState, ref bool cancel);

    /// <summary>
    /// Note the type of dumping in the progress report.
    /// </summary>
    [Flags]
    public enum MySqlDumpProjectType
    {
        /// <summary>Summary header.</summary>
        Summary = 1,

        /// <summary>Table schema.</summary>
        Table = 2,

        /// <summary>View.</summary>
        View = 4,

        /// <summary>Trigger.</summary>
        Trigger = 8,

        /// <summary>Function.</summary>
        Function = 16,

        /// <summary>Procedure.</summary>
        Procedure = 32,

        /// <summary>Event.</summary>
        Event = 64,

        /// <summary>Record.</summary>
        Record = 128
    }

    /// <summary>
    /// Dump manifest.
    /// </summary>
    public sealed class MySqlDumpManifest
    {
        /// <summary>
        /// Events dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:简化集合初始化", Justification = "<挂起>")]
        public List<MySqlDumpProject> Events { get; } = new List<MySqlDumpProject>();

        /// <summary>
        /// Functions dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:简化集合初始化", Justification = "<挂起>")]
        public List<MySqlDumpProject> Functions { get; } = new List<MySqlDumpProject>();

        /// <summary>
        /// Procedures dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:简化集合初始化", Justification = "<挂起>")]
        public List<MySqlDumpProject> Procedures { get; } = new List<MySqlDumpProject>();

        /// <summary>
        /// Tables dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:简化集合初始化", Justification = "<挂起>")]
        public List<MySqlTableDumpProject> Tables { get; } = new List<MySqlTableDumpProject>();

        /// <summary>
        /// Triggers dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:简化集合初始化", Justification = "<挂起>")]
        public List<MySqlDumpProject> Triggers { get; } = new List<MySqlDumpProject>();

        /// <summary>
        /// Views dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:简化集合初始化", Justification = "<挂起>")]
        public List<MySqlDumpProject> Views { get; } = new List<MySqlDumpProject>();
    }

    /// <summary>
    /// Dump project.
    /// </summary>
    public sealed class MySqlDumpProject
    {
        /// <summary>
        /// Dump project.
        /// </summary>
        /// <param name="name">Project name.</param>
        /// <param name="ignore">Ignore this project.</param>
        public MySqlDumpProject(string name, bool ignore)
        {
            this.Name = name;
            this.Ignore = ignore;
        }

        /// <summary>
        /// Ignore this project. Default false.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Project name.
        /// </summary>
        public string Name { get; }
    }

    /// <summary>
    /// Table dump project.
    /// </summary>
    public sealed class MySqlTableDumpProject
    {
        /// <summary>
        /// Table dump project.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="ignore">Ignore this project.</param>
        /// <param name="includingRecord">Dump records.</param>
        public MySqlTableDumpProject(string tableName, bool ignore, bool includingRecord)
        {
            this.TableName = tableName;
            this.Ignore = ignore;
            this.IncludingRecord = includingRecord;
        }

        /// <summary>
        /// Ignore this project. Default false.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Dump including records.
        /// </summary>
        public bool IncludingRecord { get; set; }

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName { get; }
    }

    #endregion Dump

    #region CommandText

    /// <summary>
    /// Return command text.
    /// </summary>
    public static class MySqlCommandText
    {
        #region Database

        /// <summary>
        /// Displays the database version.
        /// </summary>
        /// <returns></returns>
        public static string ShowVersion()
        {
            return "SELECT @@version;";
        }

        #endregion Database

        #region Engine

        /// <summary>
        /// Displays the storage engine and default engine available after installation.
        /// </summary>
        /// <returns></returns>
        public static string ShowEngines()
        {
            return "SHOW ENGINES;";
        }

        /// <summary>
        /// Displays the error caused by the last execution statement.
        /// </summary>
        /// <returns></returns>
        public static string ShowErrors()
        {
            return "SHOW ERRORS;";
        }

        /// <summary>
        /// Displays the name and value of the global system variable.
        /// </summary>
        /// <returns></returns>
        public static string ShowGlobalVariables()
        {
            return "SHOW GLOBAL VARIABLES;";
        }

        /// <summary>
        /// Displays the name and value of the global system variable.
        /// </summary>
        /// <param name="like">Part of the variable name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowGlobalVariables(string like)
        {
            return "SHOW GLOBAL VARIABLES LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays the permissions for all user.
        /// </summary>
        /// <returns></returns>
        public static string ShowGrants()
        {
            return "SHOW GRANTS;";
        }

        /// <summary>
        /// Displays the permissions for the specified user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="host">Host.</param>
        /// <returns></returns>
        public static string ShowGrants(string user, string host)
        {
            return "SHOW GRANTS FOR `" + user + "`@`" + host + "`;";
        }

        /// <summary>
        /// Displays the status of the InnoDB storage engine.
        /// </summary>
        /// <returns></returns>
        public static string ShowInnoDbStatus()
        {
            return "SHOW INNODB STATUS;";
        }

        /// <summary>
        /// Displays the log.
        /// </summary>
        /// <returns></returns>
        public static string ShowLogs()
        {
            return "SHOW LOGS;";
        }

        /// <summary>
        /// Displays the different permissions supported by the server.
        /// </summary>
        /// <returns></returns>
        public static string ShowPrivileges()
        {
            return "SHOW PRIVILEGES;";
        }

        /// <summary>
        /// Displays all processes that are running in the system.
        /// </summary>
        /// <returns></returns>
        public static string ShowProcesslist()
        {
            return "SHOW PROCESSLIST;";
        }

        /// <summary>
        /// Displays information about some system-specific resources, such as the number of threads running.
        /// </summary>
        /// <returns></returns>
        public static string ShowStatus()
        {
            return "SHOW STATUS;";
        }

        /// <summary>
        /// Displays information about some system-specific resources, such as the number of threads running.
        /// </summary>
        /// <param name="like">Part of the variable name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowStatus(string like)
        {
            return "SHOW STATUS LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays the available storage engine and default engine after installation.
        /// </summary>
        /// <returns></returns>
        public static string ShowStorageEngines()
        {
            return "SHOW STORAGE ENGINES;";
        }

        /// <summary>
        /// Displays the name and value of the system variable.
        /// </summary>
        /// <returns></returns>
        public static string ShowVariables()
        {
            return "SHOW VARIABLES;";
        }

        /// <summary>
        /// Displays the name and value of the system variable.
        /// </summary>
        /// <param name="like">Part of the variable name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowVariables(string like)
        {
            return "SHOW VARIABLES LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays the warning generated by the last executed statement.
        /// </summary>
        /// <returns></returns>
        public static string ShowWarnings()
        {
            return "SHOW WARNINGS;";
        }

        #endregion Engine

        #region Database

        /// <summary>
        /// Creates a database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to create.</param>
        /// <returns></returns>
        public static string CreateDatabase(string database)
        {
            return "CREATE DATABASE `" + database + "`;";
        }

        /// <summary>
        /// Creates a database with the current connection and specifies the character set.
        /// </summary>
        /// <param name="database">The name of the database to create.</param>
        /// <param name="charset">Character set. Such for utf8.</param>
        /// <param name="collate">Collate. Such for utf8_general_ci.</param>
        /// <returns></returns>
        public static string CreateDatabase(string database, string charset, string collate)
        {
            return "CREATE DATABASE `" + database + "` DEFAULT CHARSET " + charset + " COLLATE " + collate + ";";
        }

        /// <summary>
        /// Deletes the database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to delete.</param>
        /// <returns></returns>
        public static string DropDatabase(string database)
        {
            return "DROP DATABASE IF EXISTS `" + database + "`;";
        }

        /// <summary>
        /// Displays the creation script for the database with the specified name.
        /// </summary>
        /// <param name="database">Database name.</param>
        /// <returns></returns>
        public static string ShowCreateDatabase(string database)
        {
            return "SHOW CREATE DATABASE `" + database + "`;";
        }

        /// <summary>
        /// Displays all visible database names.
        /// </summary>
        /// <returns></returns>
        public static string ShowDatabases()
        {
            return "SHOW DATABASES;";
        }

        #endregion Database

        #region Table

        /// <summary>
        /// Displays the column names in the table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string Desc(string table)
        {
            return "DESC `" + table + "`;";
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns></returns>
        public static string DropEvent(string eventName)
        {
            return "DROP EVENT IF EXISTS `" + eventName + "`;";
        }

        /// <summary>
        /// Deletes the stored procedure.
        /// </summary>
        /// <param name="function">The name of the function.</param>
        /// <returns></returns>
        public static string DropFunction(string function)
        {
            return "DROP FUNCTION IF EXISTS `" + function + "`;";
        }

        /// <summary>
        /// Deletes the stored procedure.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns></returns>
        public static string DropProcedure(string procedure)
        {
            return "DROP PROCEDURE IF EXISTS `" + procedure + "`;";
        }

        /// <summary>
        /// Deletes the trigger.
        /// </summary>
        /// <param name="trigger">The name of the trigger.</param>
        /// <returns></returns>
        public static string DropTrigger(string trigger)
        {
            return "DROP TRIGGER IF EXISTS `" + trigger + "`;";
        }

        /// <summary>
        /// Deletes the view.
        /// </summary>
        /// <param name="view">The name of the view.</param>
        /// <returns></returns>
        public static string DropView(string view)
        {
            return "DROP VIEW IF EXISTS `" + view + "`;";
        }

        /// <summary>
        /// Displays the column names in the table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowColumns(string table)
        {
            return "SHOW COLUMNS FROM `" + table + "`;";
        }

        /// <summary>
        /// Displays the creation script for the stored procedure with the specified name.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns></returns>
        public static string ShowCreateEvent(string eventName)
        {
            return "SHOW CREATE EVENT `" + eventName + "`;";
        }

        /// <summary>
        /// Displays the creation script for the function of the specified name.
        /// </summary>
        /// <param name="function">The name of the function.</param>
        /// <returns></returns>
        public static string ShowCreateFunction(string function)
        {
            return "SHOW CREATE FUNCTION `" + function + "`;";
        }

        /// <summary>
        /// Displays the creation script for the stored procedure with the specified name.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns></returns>
        public static string ShowCreateProcedure(string procedure)
        {
            return "SHOW CREATE PROCEDURE `" + procedure + "`;";
        }

        /// <summary>
        /// Displays the creation script for the table with the specified name.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowCreateTable(string table)
        {
            return "SHOW CREATE TABLE `" + table + "`;";
        }

        /// <summary>
        /// Displays the creation script for the trigger with the specified name.
        /// </summary>
        /// <param name="trigger">The name of the trigger.</param>
        /// <returns></returns>
        public static string ShowCreateTrigger(string trigger)
        {
            return "SHOW CREATE TRIGGER `" + trigger + "`;";
        }

        /// <summary>
        /// Displays the creation script for the view with the specified name.
        /// </summary>
        /// <param name="view">The name of the table.</param>
        /// <returns></returns>
        public static string ShowCreateView(string view)
        {
            return "SHOW CREATE VIEW `" + view + "`;";
        }

        /// <summary>
        /// Displays basic information about all events in all databases.
        /// </summary>
        /// <returns></returns>
        public static string ShowEvents()
        {
            return "SHOW EVENTS;";
        }

        /// <summary>
        /// Displays basic information about the events in the specified database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns></returns>
        public static string ShowEvents(string database)
        {
            return "SHOW EVENTS WHERE `Db` = '" + database + "';";
        }

        /// <summary>
        /// Displays basic information about all functions in all databases.
        /// </summary>
        /// <returns></returns>
        public static string ShowFunctionStatus()
        {
            return "SHOW FUNCTION STATUS;";
        }

        /// <summary>
        /// Displays basic information about the functions in the specified database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns></returns>
        public static string ShowFunctionStatus(string database)
        {
            return "SHOW FUNCTION STATUS WHERE `Db` = '" + database + "';";
        }

        /// <summary>
        /// Displays the index of the table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowIndex(string table)
        {
            return "SHOW INDEX FROM `" + table + "`;";
        }

        /// <summary>
        /// Displays basic information about stored procedures.
        /// </summary>
        /// <returns></returns>
        public static string ShowProcedureStatus()
        {
            return "SHOW PROCEDURE STATUS;";
        }

        /// <summary>
        /// Displays basic information about stored procedures in the specified database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns></returns>
        public static string ShowProcedureStatus(string database)
        {
            return "SHOW PROCEDURE STATUS WHERE `Db` = '" + database + "';";
        }

        /// <summary>
        /// Displays tables and views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTables()
        {
            return "SHOW TABLES;";
        }

        /// <summary>
        /// Displays tables and views in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTables(string like)
        {
            return "SHOW TABLES LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays the details of the tables and views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTableStatus()
        {
            return "SHOW TABLE STATUS;";
        }

        /// <summary>
        /// Displays the details of the tables and views in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTableStatus(string like)
        {
            return "SHOW TABLE STATUS LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTriggers()
        {
            return "SHOW TRIGGERS;";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like)
        {
            return "SHOW TRIGGERS LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like, string table)
        {
            return "SHOW TRIGGERS WHERE `Table` = '" + table + "' AND `Trigger` LIKE '" + like + "';";
        }

        /// <summary>
        /// Empty the table and initialize the table state.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string Truncate(string table)
        {
            return "TRUNCATE TABLE `" + table + "`;";
        }

        #endregion Table
    }

    #endregion CommandText
}