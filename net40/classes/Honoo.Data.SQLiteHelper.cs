/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

/*
 * Setting password is supported for the System.Data.SQLite 1.0.111.0 or earlier.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;

namespace Honoo.Data
{
    /// <summary>
    /// SQLite helper.
    /// </summary>
    public static class SQLiteHelper
    {
        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static SQLiteConnection BuildConnection(SQLiteConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }
            return new SQLiteConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns></returns>
        public static SQLiteConnection BuildConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="dataSource">Data file path.</param>
        /// <param name="password">Password.</param>
        /// <returns></returns>
        public static SQLiteConnection BuildConnection(string dataSource, string password)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder() { DataSource = dataSource };
            if (!string.IsNullOrEmpty(password)) { connectionStringBuilder.Password = password; }
            return new SQLiteConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static string BuildConnectionString(SQLiteConnectionStringBuilder connectionStringBuilder)
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
        /// <param name="dataSource">Data file path.</param>
        /// <param name="password">Password.</param>
        /// <returns></returns>
        public static string BuildConnectionString(string dataSource, string password)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder() { DataSource = dataSource };
            if (!string.IsNullOrEmpty(password)) { connectionStringBuilder.Password = password; }
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
        public static int FillDataSet(DataSet dataSet, SQLiteConnection connection, string selectCommandText)
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
        public static int FillDataSet(DataSet dataSet, SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            using (var dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand?.Parameters.AddRange(parameters); }
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
        public static int FillDataTable(DataTable dataTable, SQLiteConnection connection, string selectCommandText)
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
        public static int FillDataTable(DataTable dataTable, SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            using (var dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand?.Parameters.AddRange(parameters); }
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
        public static SQLiteDataAdapter GetDataAdapter(SQLiteConnection connection, string selectCommandText)
        {
            return new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
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
        public static SQLiteDataAdapter GetDataAdapter(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            var dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand?.Parameters.AddRange(parameters); }
            return dataAdapter;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(SQLiteConnection connection, string selectCommandText)
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
        public static DataSet GetDataSet(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            var dataSet = new DataSet();
            using (var dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand?.Parameters.AddRange(parameters); }
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
        public static DataTable GetDataTable(SQLiteConnection connection, string selectCommandText)
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
        public static DataTable GetDataTable(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            var dataTable = new DataTable();
            using (var dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand?.Parameters.AddRange(parameters); }
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
        public static SQLiteCommand GetCommand(SQLiteConnection connection, CommandType commandType, string commandText)
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
        public static SQLiteCommand GetCommand(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] parameters)
        {
            var command = new SQLiteCommand(commandText, connection) { CommandType = commandType };
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
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText)
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
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(commandText, connection) { CommandType = commandType })
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
        public static object ExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText)
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
        public static object ExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(commandText, connection) { CommandType = commandType })
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
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText)
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
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
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
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            int result = 0;
            Exception exception = null;
            using (SQLiteTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (var command = new SQLiteCommand(commandText, connection) { CommandType = commandType })
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
        public static object TransactionExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText)
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
        public static object TransactionExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
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
        public static object TransactionExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            object result = null;
            Exception exception = null;
            using (SQLiteTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (var command = new SQLiteCommand(commandText, connection) { CommandType = commandType })
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
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="manifest">Dump manifest.</param>
        /// <param name="textWriter">TextWriter.</param>
        public static void Dump(SQLiteConnection connection, SQLiteDumpManifest manifest, TextWriter textWriter)
        {
            Dump(connection, manifest, textWriter, null, null, out _);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="manifest">Dump manifest.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void Dump(SQLiteConnection connection,
                                SQLiteDumpManifest manifest,
                                TextWriter textWriter,
                                SQLiteWrittenCallback written,
                                object userState,
                                out bool cancelled)
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
            written?.Invoke(0, summary.Total, SQLiteDumpProjectType.Summary, string.Empty, userState, ref cancel);
            if (!cancel)
            {
                using (DataSet ds = GetDataSet(connection, SQLiteCommandText.ShowTables() + SQLiteCommandText.ShowViews() + SQLiteCommandText.ShowTriggers()))
                {
                    if (!cancel && summary.TableCount > 0)
                    {
                        DumpTables(ds.Tables[0], manifest.Tables, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.ViewCount > 0)
                    {
                        DumpViews(ds.Tables[1], manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                    if (!cancel && summary.TriggerCount > 0)
                    {
                        DumpTriggers(ds.Tables[2], manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                    }
                }
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
        /// <param name="fileSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        /// <param name="encoding">File encoding.</param>
        public static void DumpToFiles(SQLiteConnection connection, SQLiteDumpManifest manifest, string folder, long fileSize, Encoding encoding)
        {
            DumpToFiles(connection, manifest, folder, encoding, fileSize, null, null, out _);
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
        public static void DumpToFiles(SQLiteConnection connection,
                                       SQLiteDumpManifest manifest,
                                       string folder,
                                       Encoding encoding,
                                       long splitFileSize,
                                       SQLiteWrittenCallback written,
                                       object userState,
                                       out bool cancelled)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (manifest is null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (splitFileSize < 1024 * 1024)
            {
                throw new ArgumentException("File size cannot be less than 1 MB.");
            }
            var summary = BuildSummary(connection, manifest);
            bool cancel = false;
            long index = 0;
            string file = Path.Combine(folder, "!schema.sql");
            using (var stream = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
            {
                using (var textWriter = new StreamWriter(stream, encoding))
                {
                    textWriter.Write(summary.Text);
                    written?.Invoke(0, summary.Total, SQLiteDumpProjectType.Summary, string.Empty, userState, ref cancel);
                    if (!cancel)
                    {
                        using (DataSet ds = GetDataSet(connection, SQLiteCommandText.ShowTables() + SQLiteCommandText.ShowViews() + SQLiteCommandText.ShowTriggers()))
                        {
                            if (!cancel && summary.TableCount > 0)
                            {
                                DumpTables(ds.Tables[0], manifest.Tables, textWriter, ref index, summary.Total, written, userState, ref cancel);
                            }
                            if (!cancel && summary.ViewCount > 0)
                            {
                                DumpViews(ds.Tables[1], manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                            }
                            if (!cancel && summary.TriggerCount > 0)
                            {
                                DumpTriggers(ds.Tables[2], manifest.Triggers, textWriter, ref index, summary.Total, written, userState, ref cancel);
                            }
                            textWriter.Flush();
                        }
                    }
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
        public static SQLiteDumpManifest GetDumpManifest(SQLiteConnection connection)
        {
            var manifest = new SQLiteDumpManifest();
            using (DataTable dt = GetDataTable(connection, "SELECT type, name FROM `SQLite_master`;"))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string type = (string)dr["type"];
                        if (type == "table")
                        {
                            manifest.Tables.Add(new SQLiteTableDumpProject((string)dr["name"], false, true));
                        }
                        else if (type == "view")
                        {
                            manifest.Views.Add(new SQLiteDumpProject((string)dr["name"], false));
                        }
                        else
                        {
                            manifest.Triggers.Add(new SQLiteDumpProject((string)dr["name"], false));
                        }
                    }
                }
            }
            return manifest;
        }

        private static SQLiteSummary BuildSummary(SQLiteConnection connection, SQLiteDumpManifest manifest)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (manifest is null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }
            var summary = new SQLiteSummary();
            var union = new List<string>();
            foreach (SQLiteTableDumpProject table in manifest.Tables)
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
            foreach (SQLiteDumpProject view in manifest.Views)
            {
                if (!view.Ignore)
                {
                    summary.ViewCount++;
                }
            }
            foreach (SQLiteDumpProject trigger in manifest.Triggers)
            {
                if (!trigger.Ignore)
                {
                    summary.TriggerCount++;
                }
            }
            summary.Total = summary.TableCount + summary.ViewCount + summary.TriggerCount + summary.RecordCount;
            var tmp = new StringBuilder();
            tmp.AppendLine("/*");
            tmp.AppendLine(" * Dump by Honoo.Data.SQLiteHelper");
            tmp.AppendLine(" * https://github.com/LokiHonoo/development-resources");
            tmp.AppendLine(" * This code page is published by the MIT license.");
            tmp.AppendLine(" * ");
            tmp.AppendLine(" * DataSource     : " + connection.DataSource);
            tmp.AppendLine(" * Server Version : " + connection.ServerVersion);
            tmp.AppendLine(" * Database       : " + connection.Database);
            tmp.AppendLine(" * ");
            tmp.AppendLine(" * Table          : " + summary.TableCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine(" * View           : " + summary.ViewCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine(" * Trigger        : " + summary.TriggerCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine(" * Record         : " + summary.RecordCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine(" * ");
            tmp.AppendLine(" * Dump Time(UTC) : " + DateTime.UtcNow);
            tmp.AppendLine(" * ");
            tmp.AppendLine(" * Use the console or database tool to recover data.");
            tmp.AppendLine(" * If the target database has a table with the same name, the table data is overwritten.");
            tmp.AppendLine(" */");
            tmp.AppendLine();
            summary.Text = tmp.ToString();
            return summary;
        }

        private static void DumpRecords(SQLiteConnection connection,
                                        List<SQLiteTableDumpProject> tables,
                                        TextWriter textWriter,
                                        ref long index,
                                        long total,
                                        SQLiteWrittenCallback written,
                                        object userState,
                                        ref bool cancel)
        {
            var tmp = new StringBuilder();
            textWriter.WriteLine("PRAGMA foreign_keys = OFF;");
            textWriter.WriteLine();
            foreach (SQLiteTableDumpProject table in tables)
            {
                if (cancel)
                {
                    break;
                }
                if (!table.Ignore && table.IncludingRecord)
                {
                    using (var command = GetCommand(connection, CommandType.Text, "SELECT * FROM `" + table.TableName + "`;"))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            var dataTypes = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dataTypes.Add(reader.GetDataTypeName(i));
                            }
                            if (reader.HasRows)
                            {
                                tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                                tmp.AppendLine("-- Records of \"" + table.TableName + "\"");
                                tmp.AppendLine("-- --------------------------------------------------------");
                                while (reader.Read())
                                {
                                    if (cancel)
                                    {
                                        break;
                                    }
                                    if (table.TableName == "sqlite_sequence")
                                    {
                                        string name = reader.GetString(0);
                                        long seq = reader.GetInt64(1);
                                        tmp.AppendLine("INSERT OR IGNORE INTO `sqlite_sequence` VALUES('" + name + "', " + seq + ");");
                                        tmp.AppendLine("UPDATE `sqlite_sequence` SET seq=" + seq + " WHERE name='" + name + "';");
                                    }
                                    else
                                    {
                                        tmp.Append("INSERT INTO `" + table.TableName + "` VALUES");
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
                                                    case byte[] value: tmp.Append("X'" + BitConverter.ToString(value).Replace("-", string.Empty) + "'"); break;
                                                    default: tmp.Append("'" + val.ToString() + "'"); break;
                                                }
                                            }
                                            if (i < reader.FieldCount - 1)
                                            {
                                                tmp.Append(',');
                                            }
                                        }
                                        tmp.AppendLine(");");
                                    }
                                    textWriter.Write(tmp);
                                    tmp.Clear();
                                    index++;
                                    written?.Invoke(index, total, SQLiteDumpProjectType.Record, table.TableName, userState, ref cancel);
                                }
                            }
                            reader.Close();
                        }
                        textWriter.WriteLine();
                    }
                }
            }

            textWriter.WriteLine("PRAGMA foreign_keys = ON;");
        }

        private static void DumpRecords(SQLiteConnection connection,
                                        List<SQLiteTableDumpProject> tables,
                                        string folder,
                                        Encoding encoding,
                                        long splitFileSize,
                                        ref long index,
                                        long total,
                                        SQLiteWrittenCallback written,
                                        object userState,
                                        ref bool cancel)
        {
            foreach (SQLiteTableDumpProject table in tables)
            {
                if (cancel)
                {
                    break;
                }
                if (!table.Ignore && table.IncludingRecord)
                {
                    using (SQLiteCommand command = GetCommand(connection, CommandType.Text, "SELECT * FROM `" + table.TableName + "`;"))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.Default))
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

        private static void DumpRecords(string tableName,
                                        SQLiteDataReader reader,
                                        string folder,
                                        Encoding encoding,
                                        long splitFileSize,
                                        ref long index,
                                        long total,
                                        SQLiteWrittenCallback written,
                                        object userState,
                                        ref bool cancel)
        {
            var tmp = new StringBuilder();
            int sn = 0;
            string file = Path.Combine(folder, "records@" + tableName + ".sql");
            var stream = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
            var streamWriter = new StreamWriter(stream, encoding);
            streamWriter.WriteLine("PRAGMA foreign_keys = OFF;");
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
                if (tableName == "sqlite_sequence")
                {
                    string name = reader.GetString(0);
                    long seq = reader.GetInt64(1);
                    tmp.AppendLine("INSERT OR IGNORE INTO `sqlite_sequence` VALUES('" + name + "', " + seq + ");");
                    tmp.AppendLine("UPDATE `sqlite_sequence` SET seq=" + seq + " WHERE name='" + name + "';");
                }
                else
                {
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
                                case byte[] value: tmp.Append("X'" + BitConverter.ToString(value).Replace("-", string.Empty) + "'"); break;
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
                        streamWriter.WriteLine("PRAGMA foreign_keys = ON;");
                        streamWriter.Flush();
                        streamWriter.Close();
                        streamWriter.Dispose();
                        stream.Close();
                        stream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "records@" + tableName + "@p" + sn + ".sql");
                        stream = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                        streamWriter = new StreamWriter(stream, encoding);
                        streamWriter.WriteLine("PRAGMA foreign_keys = OFF;");
                        streamWriter.WriteLine();
                        streamWriter.WriteLine("-- ----------------------------------------------------------------------------------------------------------------");
                        streamWriter.WriteLine("-- Records of \"" + tableName + "\"");
                        streamWriter.WriteLine("-- --------------------------------------------------------");
                    }
                }
                streamWriter.Write(tmp);
                tmp.Clear();
                index++;
                written?.Invoke(index, total, SQLiteDumpProjectType.Record, tableName, userState, ref cancel);
            }
            streamWriter.WriteLine();
            streamWriter.WriteLine("PRAGMA foreign_keys = ON;");
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
            stream.Close();
            stream.Dispose();
        }

        private static void DumpTables(DataTable tableCreates,
                                       List<SQLiteTableDumpProject> tables,
                                       TextWriter textWriter,
                                       ref long index,
                                       long total,
                                       SQLiteWrittenCallback written,
                                       object userState,
                                       ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (SQLiteTableDumpProject table in tables)
            {
                if (cancel)
                {
                    break;
                }
                if (!table.Ignore && table.TableName != "sqlite_sequence")
                {
                    tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                    tmp.AppendLine("-- Table structure for \"" + table.TableName + "\"");
                    tmp.AppendLine("-- --------------------------------------------------------");
                    string tableCreate = (string)tableCreates.Select("name='" + table.TableName + "'")[0]["sql"];
                    tmp.AppendLine("DROP TABLE IF EXISTS `" + table.TableName + "`;");
                    tmp.AppendLine(tableCreate + ";");
                    tmp.AppendLine();
                    textWriter.Write(tmp);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, SQLiteDumpProjectType.Table, table.TableName, userState, ref cancel);
                }
            }
        }

        private static void DumpTriggers(DataTable triggerCreates,
                                         List<SQLiteDumpProject> triggers,
                                         TextWriter textWriter,
                                         ref long index,
                                         long total,
                                         SQLiteWrittenCallback written,
                                         object userState,
                                         ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (SQLiteDumpProject trigger in triggers)
            {
                if (cancel)
                {
                    break;
                }
                if (!trigger.Ignore)
                {
                    string triggerCreate = (string)triggerCreates.Select("name='" + trigger + "'")[0]["sql"];
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
                    written?.Invoke(index, total, SQLiteDumpProjectType.Trigger, trigger.Name, userState, ref cancel);
                }
            }
        }

        private static void DumpViews(DataTable viewCreates,
                                      List<SQLiteDumpProject> views,
                                      TextWriter textWriter,
                                      ref long index,
                                      long total,
                                      SQLiteWrittenCallback written,
                                      object userState,
                                      ref bool cancel)
        {
            var tmp = new StringBuilder();
            foreach (SQLiteDumpProject view in views)
            {
                if (cancel)
                {
                    break;
                }
                if (!view.Ignore)
                {
                    string viewCreate = (string)viewCreates.Select("name='" + view + "'")[0]["sql"];
                    tmp.AppendLine("-- ----------------------------------------------------------------------------------------------------------------");
                    tmp.AppendLine("-- View structure for \"" + view + "\"");
                    tmp.AppendLine("-- --------------------------------------------------------");
                    tmp.AppendLine("DROP VIEW IF EXISTS `" + view + "`;");
                    tmp.AppendLine(viewCreate + ";");
                    tmp.AppendLine();
                    textWriter.Write(tmp);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, SQLiteDumpProjectType.View, view.Name, userState, ref cancel);
                }
            }
        }

        #endregion Dump

        #region Private class

        private struct SQLiteSummary
        {
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
    public delegate void SQLiteWrittenCallback(long written, long total, SQLiteDumpProjectType projectType, string association, object userState, ref bool cancel);

    /// <summary>
    /// Note the type of dumping in the progress report.
    /// </summary>
    [Flags]
    public enum SQLiteDumpProjectType
    {
        /// <summary>Summary header.</summary>
        Summary = 1,

        /// <summary>Table schema.</summary>
        Table = 2,

        /// <summary>Trigger.</summary>
        Trigger = 4,

        /// <summary>View.</summary>
        View = 8,

        /// <summary>Record.</summary>
        Record = 16,
    }

    /// <summary>
    /// Dump manifest.
    /// </summary>
    public sealed class SQLiteDumpManifest
    {
        /// <summary>
        /// Tables dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public List<SQLiteTableDumpProject> Tables { get; } = new List<SQLiteTableDumpProject>();

        /// <summary>
        /// Triggers dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public List<SQLiteDumpProject> Triggers { get; } = new List<SQLiteDumpProject>();

        /// <summary>
        /// Views dump project.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:不要公开泛型列表", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        public List<SQLiteDumpProject> Views { get; } = new List<SQLiteDumpProject>();
    }

    /// <summary>
    /// Dump project.
    /// </summary>
    public sealed class SQLiteDumpProject
    {
        /// <summary>
        /// Dump project.
        /// </summary>
        /// <param name="name">Project name.</param>
        /// <param name="ignore">Ignore this project.</param>
        public SQLiteDumpProject(string name, bool ignore)
        {
            Name = name;
            Ignore = ignore;
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
    public sealed class SQLiteTableDumpProject
    {
        /// <summary>
        /// Table dump project.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="ignore">Ignore this project.</param>
        /// <param name="includingRecord">Dump records.</param>
        public SQLiteTableDumpProject(string tableName, bool ignore, bool includingRecord)
        {
            TableName = tableName;
            Ignore = ignore;
            IncludingRecord = includingRecord;
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
    /// Command text.
    /// </summary>
    public static class SQLiteCommandText
    {
        #region Database

        /// <summary>
        /// Displays the database version.
        /// </summary>
        /// <returns></returns>
        public static string ShowVersion()
        {
            return "SELECT SQLite_VERSION();";
        }

        #endregion Database

        #region Table

        /// <summary>
        /// Displays tables in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTables()
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'table';";
        }

        /// <summary>
        /// Displays tables in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTables(string like)
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'table' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTriggers()
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'trigger';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like)
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'trigger' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like, string table)
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'trigger' AND `tbl_name` = '" + table + "' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowViews()
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'view';";
        }

        /// <summary>
        /// Displays views in the current database.
        /// </summary>
        /// <param name="like">Part of the view name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowViews(string like)
        {
            return "SELECT * FROM `SQLite_master` WHERE `type` = 'view' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Compress database files to recycle wasted space in the database.
        /// </summary>
        /// <returns></returns>
        public static string Vacuum()
        {
            return "VACUUM;";
        }

        #endregion Table
    }

    #endregion CommandText
}