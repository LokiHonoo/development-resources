﻿/*
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
        #region ConnectionBehavior

        private static OdbcConnectionBehavior _dataAdapterConnectionBehavior = OdbcConnectionBehavior.Auto;
        private static OdbcConnectionBehavior _dataReaderConnectionBehavior = OdbcConnectionBehavior.Manual;
        private static OdbcConnectionBehavior _executeConnectionBehavior = OdbcConnectionBehavior.Manual;
        private static OdbcConnectionBehavior _transactionConnectionBehavior = OdbcConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        public static OdbcConnectionBehavior DataAdapterConnectionBehavior
        {
            get => _dataAdapterConnectionBehavior;
            set => _dataAdapterConnectionBehavior = value;
        }

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        public static OdbcConnectionBehavior DataReaderConnectionBehavior
        {
            get => _dataReaderConnectionBehavior;
            set => _dataReaderConnectionBehavior = value;
        }

        /// <summary>
        /// Connection open/close behavior when using Execute. Default Manual.
        /// </summary>
        public static OdbcConnectionBehavior ExecuteConnectionBehavior
        {
            get => _executeConnectionBehavior;
            set => _executeConnectionBehavior = value;
        }

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        public static OdbcConnectionBehavior TransactionConnectionBehavior
        {
            get => _transactionConnectionBehavior;
            set => _transactionConnectionBehavior = value;
        }

        #endregion ConnectionBehavior

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
        /// <param name="selectCommandText">Sql command.</param>
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                result = dataAdapter.Fill(dataSet);
            }
            return result;
        }

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int FillDataTable(DataTable dataTable, OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                result = dataAdapter.Fill(dataTable);
            }
            return result;
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static OdbcDataAdapter GetDataAdapter(OdbcConnection connection, string selectCommandText)
        {
            return new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(OdbcConnection connection, string selectCommandText)
        {
            return GetDataSet(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(OdbcConnection connection, string selectCommandText)
        {
            return GetDataTable(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataTable result = new DataTable();
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(result);
            }
            return result;
        }

        #endregion DataAdapter

        #region DataReader

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static OdbcDataReader GetDataReader(OdbcConnection connection, string commandText)
        {
            return GetDataReader(connection, commandText, null);
        }

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static OdbcDataReader GetDataReader(OdbcConnection connection, string commandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            CommandBehavior commandBehavior;
            if (_dataReaderConnectionBehavior == OdbcConnectionBehavior.Manual)
            {
                if (connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
                }
                commandBehavior = CommandBehavior.Default;
            }
            else
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    commandBehavior = CommandBehavior.CloseConnection;
                }
                else
                {
                    commandBehavior = CommandBehavior.Default;
                }
            }
            OdbcCommand command = new OdbcCommand(commandText, connection);
            if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
            return command.ExecuteReader(commandBehavior);
        }

        #endregion DataReader

        #region Execute

        /// <summary>
        /// Execute the sql command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OdbcConnection connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, null);
        }

        /// <summary>
        /// Execute the sql command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OdbcConnection connection, string commandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (OdbcCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                result = command.ExecuteNonQuery();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
            return result;
        }

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        public static void ExecuteProcedure(OdbcConnection connection, string procedure)
        {
            ExecuteProcedure(connection, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        public static void ExecuteProcedure(OdbcConnection connection, string procedure, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            using (OdbcCommand command = new OdbcCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                command.ExecuteNonQuery();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Execute the sql command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static object ExecuteScalar(OdbcConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, null);
        }

        /// <summary>
        /// Execute the sql command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static object ExecuteScalar(OdbcConnection connection, string commandText, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result;
            using (OdbcCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                result = command.ExecuteScalar();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
            return result;
        }

        #endregion Execute

        #region Transaction

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(OdbcConnection connection, string commandText)
        {
            return TransactionExecuteNonQuery(connection, commandText, IsolationLevel.ReadCommitted, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(OdbcConnection connection, string commandText, IsolationLevel isolationLevel)
        {
            return TransactionExecuteNonQuery(connection, commandText, isolationLevel, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(OdbcConnection connection, string commandText, IsolationLevel isolationLevel, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_transactionConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OdbcTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (OdbcCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
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
            if (state != ConnectionState.Open) { connection.Close(); }
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
        /// Executing stored procedure by transaction. Auto rollback if failed.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        public static void TransactionExecuteProcedure(OdbcConnection connection, string procedure)
        {
            TransactionExecuteProcedure(connection, procedure, IsolationLevel.ReadCommitted, null);
        }

        /// <summary>
        /// Executing stored procedure by transaction. Auto rollback if failed.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        public static void TransactionExecuteProcedure(OdbcConnection connection, string procedure, IsolationLevel isolationLevel)
        {
            TransactionExecuteProcedure(connection, procedure, isolationLevel, null);
        }

        /// <summary>
        /// Executing stored procedure by transaction. Auto rollback if failed.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <param name="parameters">Parameters.</param>
        public static void TransactionExecuteProcedure(OdbcConnection connection, string procedure, IsolationLevel isolationLevel, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_transactionConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OdbcTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (OdbcCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedure;
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        command.ExecuteNonQuery();
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
            if (state != ConnectionState.Open) { connection.Close(); }
            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(OdbcConnection connection, string commandText)
        {
            return TransactionExecuteScalar(connection, commandText, IsolationLevel.ReadCommitted, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(OdbcConnection connection, string commandText, IsolationLevel isolationLevel)
        {
            return TransactionExecuteScalar(connection, commandText, isolationLevel, null);
        }

        /// <summary>
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(OdbcConnection connection, string commandText, IsolationLevel isolationLevel, params OdbcParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_transactionConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OdbcTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (OdbcCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
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
            if (state != ConnectionState.Open) { connection.Close(); }
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

    #region ConnectionBehavior

    /// <summary>
    /// The mode of the connection when querying.
    /// </summary>
    public enum OdbcConnectionBehavior
    {
        /// <summary>Does not open automatically. If the connection is not open  when the querying, an exception is thrown.</summary>
        Manual,

        /// <summary>If the connection is not open, automatically open and close when the query is complete. If the connection is already open, keep it turned on when the query is complete.</summary>
        Auto
    }

    #endregion ConnectionBehavior
}