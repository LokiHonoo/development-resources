/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Data;
using System.Data.Odbc;

namespace LH.Data
{
    /// <summary>
    /// Odbc extension.
    /// </summary>
    public static class OdbcHelper
    {
        #region ConnectionBehavior

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        public static OdbcConnectionBehavior DataAdapterConnectionBehavior { get; set; } = OdbcConnectionBehavior.Auto;

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        public static OdbcConnectionBehavior DataReaderConnectionBehavior { get; set; } = OdbcConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Execute. Default Manual.
        /// </summary>
        public static OdbcConnectionBehavior ExecuteConnectionBehavior { get; set; } = OdbcConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        public static OdbcConnectionBehavior TransactionConnectionBehavior { get; set; } = OdbcConnectionBehavior.Manual;

        #endregion ConnectionBehavior

        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static OdbcConnection BuildConnection(OdbcConnectionStringBuilder connectionStringBuilder)
        {
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
            return connectionStringBuilder.ConnectionString;
        }

        #endregion Connection

        #region DataAdapter

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, OdbcConnection connection, string selectCommandText) => FillDataSet(dataSet, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int FillDataSet(DataSet dataSet, OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static int FillDataTable(DataTable dataTable, OdbcConnection connection, string selectCommandText) => FillDataTable(dataTable, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int FillDataTable(DataTable dataTable, OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static OdbcDataAdapter GetDataAdapter(OdbcConnection connection, string selectCommandText) => new OdbcDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
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
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(OdbcConnection connection, string selectCommandText) => GetDataSet(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DataSet GetDataSet(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(OdbcConnection connection, string selectCommandText) => GetDataTable(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DataTable GetDataTable(OdbcConnection connection, string selectCommandText, params OdbcParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static OdbcDataReader GetDataReader(OdbcConnection connection, string commandText) => GetDataReader(connection, commandText, null);

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static OdbcDataReader GetDataReader(OdbcConnection connection, string commandText, params OdbcParameter[] parameters)
        {
            CommandBehavior commandBehavior;
            if (DataReaderConnectionBehavior == OdbcConnectionBehavior.Manual)
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
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OdbcConnection connection, string commandText) => ExecuteNonQuery(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int ExecuteNonQuery(OdbcConnection connection, string commandText, params OdbcParameter[] parameters)
        {
            if (ExecuteConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        public static void ExecuteProcedure(OdbcConnection connection, string procedure) => ExecuteProcedure(connection, procedure, null);

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static void ExecuteProcedure(OdbcConnection connection, string procedure, params OdbcParameter[] parameters)
        {
            if (ExecuteConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static object ExecuteScalar(OdbcConnection connection, string commandText) => ExecuteScalar(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static object ExecuteScalar(OdbcConnection connection, string commandText, params OdbcParameter[] parameters)
        {
            if (ExecuteConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(OdbcConnection connection, string commandText) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.ReadCommitted, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(OdbcConnection connection, string commandText, params OdbcParameter[] parameters) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.ReadCommitted, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(OdbcConnection connection, IsolationLevel iso, string commandText) =>
            TransactionExecuteNonQuery(connection, iso, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int TransactionExecuteNonQuery(OdbcConnection connection, IsolationLevel iso, string commandText, params OdbcParameter[] parameters)
        {
            if (TransactionConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OdbcTransaction transaction = connection.BeginTransaction(iso))
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
                        transaction.Rollback();
                        exception = ex;
                    }
                }
            }
            if (state != ConnectionState.Open) { connection.Close(); }
            if (exception == null)
            {
                return result;
            }
            else
            {
                throw exception;
            }
        }

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        public static void TransactionExecuteProcedure(OdbcConnection connection, string procedure) =>
            TransactionExecuteProcedure(connection, IsolationLevel.ReadCommitted, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        public static void TransactionExecuteProcedure(OdbcConnection connection, string procedure, params OdbcParameter[] parameters) =>
            TransactionExecuteProcedure(connection, IsolationLevel.ReadCommitted, procedure, parameters);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        public static void TransactionExecuteProcedure(OdbcConnection connection, IsolationLevel iso, string procedure) =>
            TransactionExecuteProcedure(connection, iso, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static void TransactionExecuteProcedure(OdbcConnection connection, IsolationLevel iso, string procedure, params OdbcParameter[] parameters)
        {
            if (TransactionConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OdbcTransaction transaction = connection.BeginTransaction(iso))
            {
                using (OdbcCommand command = new OdbcCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(OdbcConnection connection, string commandText) =>
            TransactionExecuteScalar(connection, IsolationLevel.ReadCommitted, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(OdbcConnection connection, string commandText, params OdbcParameter[] parameters) =>
            TransactionExecuteScalar(connection, IsolationLevel.ReadCommitted, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(OdbcConnection connection, IsolationLevel iso, string commandText) =>
            TransactionExecuteScalar(connection, iso, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static object TransactionExecuteScalar(OdbcConnection connection, IsolationLevel iso, string commandText, params OdbcParameter[] parameters)
        {
            if (TransactionConnectionBehavior == OdbcConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OdbcTransaction transaction = connection.BeginTransaction(iso))
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
                        transaction.Rollback();
                        exception = ex;
                    }
                }
            }
            if (state != ConnectionState.Open) { connection.Close(); }
            if (exception == null)
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