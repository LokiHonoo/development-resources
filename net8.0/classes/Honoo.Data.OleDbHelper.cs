﻿/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;
using System.Data;
using System.Data.OleDb;

namespace Honoo.Data
{
    /// <summary>
    /// OleDb helper.
    /// </summary>
    internal static class OleDbHelper
    {
        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        internal static OleDbConnection BuildConnection(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            ArgumentNullException.ThrowIfNull(connectionStringBuilder);
            return new OleDbConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        internal static OleDbConnection BuildConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        internal static string BuildConnectionString(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            ArgumentNullException.ThrowIfNull(connectionStringBuilder);
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
        internal static int FillDataSet(DataSet dataSet, OleDbConnection connection, string selectCommandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static int FillDataSet(DataSet dataSet, OleDbConnection connection, string selectCommandText, params OleDbParameter[]? parameters)
        {
            using (var dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static int FillDataTable(DataTable dataTable, OleDbConnection connection, string selectCommandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static int FillDataTable(DataTable dataTable, OleDbConnection connection, string selectCommandText, params OleDbParameter[]? parameters)
        {
            using (var dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static OleDbDataAdapter GetDataAdapter(OleDbConnection connection, string selectCommandText)
        {
            return new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static OleDbDataAdapter GetDataAdapter(OleDbConnection connection, string selectCommandText, params OleDbParameter[]? parameters)
        {
            var dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand?.Parameters.AddRange(parameters); }
            return dataAdapter;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        internal static DataSet GetDataSet(OleDbConnection connection, string selectCommandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static DataSet GetDataSet(OleDbConnection connection, string selectCommandText, params OleDbParameter[]? parameters)
        {
            var dataSet = new DataSet();
            using (var dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static DataTable GetDataTable(OleDbConnection connection, string selectCommandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static DataTable GetDataTable(OleDbConnection connection, string selectCommandText, params OleDbParameter[]? parameters)
        {
            var dataTable = new DataTable();
            using (var dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static OleDbCommand GetCommand(OleDbConnection connection, CommandType commandType, string commandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static OleDbCommand GetCommand(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[]? parameters)
        {
            var command = new OleDbCommand(commandText, connection) { CommandType = commandType };
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
        internal static int ExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static int ExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[]? parameters)
        {
            using (var command = new OleDbCommand(commandText, connection) { CommandType = commandType })
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
        internal static object? ExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static object? ExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[]? parameters)
        {
            using (var command = new OleDbCommand(commandText, connection) { CommandType = commandType })
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
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText)
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
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params OleDbParameter[]? parameters)
        {
            ArgumentNullException.ThrowIfNull(connection);
            int result = 0;
            Exception? exception = null;
            using (OleDbTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (var command = new OleDbCommand(commandText, connection) { CommandType = commandType })
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
        internal static object? TransactionExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText)
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
        internal static object? TransactionExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        internal static object? TransactionExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params OleDbParameter[]? parameters)
        {
            ArgumentNullException.ThrowIfNull(connection);
            object? result = null;
            Exception? exception = null;
            using (OleDbTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (var command = new OleDbCommand(commandText, connection) { CommandType = commandType })
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
    }
}