/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;
using System.Data;
using System.Data.Odbc;

namespace Honoo.Data
{
    /// <summary>
    /// Odbc helper.
    /// </summary>
    public static class OdbcHelper
    {
        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static OdbcConnection BuildConnection(OdbcConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }
            return new OdbcConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns></returns>
        public static OdbcConnection BuildConnection(string connectionString)
        {
            return new OdbcConnection(connectionString);
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static string BuildConnectionString(OdbcConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }
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
        public static int FillDataSet(DataSet dataSet, OdbcConnection connection, string selectCommandText)
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
        public static int FillDataSet(DataSet dataSet, OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static int FillDataTable(DataTable dataTable, OdbcConnection connection, string selectCommandText)
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
        public static int FillDataTable(DataTable dataTable, OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static OdbcDataAdapter GetDataAdapter(OdbcConnection connection, string selectCommandText)
        {
            return new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:检查 SQL 查询是否存在安全漏洞", Justification = "<挂起>")]
        public static OdbcDataAdapter GetDataAdapter(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            OdbcDataAdapter result = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { result.SelectCommand.Parameters.AddRange(parameters); }
            return result;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(OdbcConnection connection, string selectCommandText)
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
        public static DataSet GetDataSet(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            DataSet result = new DataSet();
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(result);
            }
            return result;
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command. Check SQL queries for security vulnerabilities.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(OdbcConnection connection, string selectCommandText)
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
        public static DataTable GetDataTable(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            DataTable result = new DataTable();
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(result);
            }
            return result;
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
        public static OdbcCommand GetCommand(OdbcConnection connection, CommandType commandType, string commandText)
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
        public static OdbcCommand GetCommand(OdbcConnection connection, CommandType commandType, string commandText, params OdbcParameter[] parameters)
        {
            OdbcCommand command = new OdbcCommand(commandText, connection) { CommandType = commandType };
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
        public static int ExecuteNonQuery(OdbcConnection connection, CommandType commandType, string commandText)
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
        public static int ExecuteNonQuery(OdbcConnection connection, CommandType commandType, string commandText, params OdbcParameter[] parameters)
        {
            using (OdbcCommand command = new OdbcCommand(commandText, connection) { CommandType = commandType })
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
        public static object ExecuteScalar(OdbcConnection connection, CommandType commandType, string commandText)
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
        public static object ExecuteScalar(OdbcConnection connection, CommandType commandType, string commandText, params OdbcParameter[] parameters)
        {
            using (OdbcCommand command = new OdbcCommand(commandText, connection) { CommandType = commandType })
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
        public static int TransactionExecuteNonQuery(OdbcConnection connection, CommandType commandType, string commandText)
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
        public static int TransactionExecuteNonQuery(OdbcConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
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
        public static int TransactionExecuteNonQuery(OdbcConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            int result = 0;
            Exception exception = null;
            using (OdbcTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (OdbcCommand command = new OdbcCommand(commandText, connection) { CommandType = commandType })
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
        public static object TransactionExecuteScalar(OdbcConnection connection, CommandType commandType, string commandText)
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
        public static object TransactionExecuteScalar(OdbcConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel)
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
        public static object TransactionExecuteScalar(OdbcConnection connection, CommandType commandType, string commandText, IsolationLevel isolationLevel, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            object result = null;
            Exception exception = null;
            using (OdbcTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (OdbcCommand command = new OdbcCommand(commandText, connection) { CommandType = commandType })
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