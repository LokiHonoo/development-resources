﻿/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

/*
 * PackageReference: System.Data.SQLite
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace LH.Data
{
    /// <summary>
    /// SQLite extension.
    /// </summary>
    public static class SQLiteHelper
    {
        #region ConnectionBehavior

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        public static SQLiteConnectionBehavior DataAdapterConnectionBehavior { get; set; } = SQLiteConnectionBehavior.Auto;

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        public static SQLiteConnectionBehavior DataReaderConnectionBehavior { get; set; } = SQLiteConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Execute. Default Manual.
        /// </summary>
        public static SQLiteConnectionBehavior ExecuteConnectionBehavior { get; set; } = SQLiteConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        public static SQLiteConnectionBehavior TransactionConnectionBehavior { get; set; } = SQLiteConnectionBehavior.Manual;

        #endregion ConnectionBehavior

        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static SQLiteConnection BuildConnection(SQLiteConnectionStringBuilder connectionStringBuilder)
        {
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
        /// <param name="password">Password. Warning: Setting password is not supported on the NETSTANDARD and NETCORE. Set this argument to 'null'.</param>
        /// <returns></returns>
        public static SQLiteConnection BuildConnection(string dataSource, string password)
        {
            SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder() { DataSource = dataSource };
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
            return connectionStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="dataSource">Data file path.</param>
        /// <param name="password">Password. Warning: Setting password is not supported on the NETSTANDARD and NETCORE. Set this argument to 'null'.</param>
        /// <returns></returns>
        public static string BuildConnectionString(string dataSource, string password)
        {
            SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder() { DataSource = dataSource };
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
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, SQLiteConnection connection, string selectCommandText) => FillDataSet(dataSet, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int FillDataSet(DataSet dataSet, SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static int FillDataTable(DataTable dataTable, SQLiteConnection connection, string selectCommandText) => FillDataTable(dataTable, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int FillDataTable(DataTable dataTable, SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static SQLiteDataAdapter GetDataAdapter(SQLiteConnection connection, string selectCommandText) => new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static SQLiteDataAdapter GetDataAdapter(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            SQLiteDataAdapter result = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { result.SelectCommand.Parameters.AddRange(parameters); }
            return result;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(SQLiteConnection connection, string selectCommandText) => GetDataSet(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DataSet GetDataSet(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataSet result = new DataSet();
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static DataTable GetDataTable(SQLiteConnection connection, string selectCommandText) => GetDataTable(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DataTable GetDataTable(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (DataAdapterConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataTable result = new DataTable();
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static SQLiteDataReader GetDataReader(SQLiteConnection connection, string commandText) => GetDataReader(connection, commandText, null);

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static SQLiteDataReader GetDataReader(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            CommandBehavior commandBehavior;
            if (DataReaderConnectionBehavior == SQLiteConnectionBehavior.Manual)
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
            SQLiteCommand command = new SQLiteCommand(commandText, connection);
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
        public static int ExecuteNonQuery(SQLiteConnection connection, string commandText) => ExecuteNonQuery(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static int ExecuteNonQuery(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (ExecuteConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (SQLiteCommand command = connection.CreateCommand())
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
        /// <param name="procedure">Sql query.</param>
        public static void ExecuteProcedure(SQLiteConnection connection, string procedure) => ExecuteProcedure(connection, procedure, null);

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static void ExecuteProcedure(SQLiteConnection connection, string procedure, params SQLiteParameter[] parameters)
        {
            if (ExecuteConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            using (SQLiteCommand command = new SQLiteCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
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
        public static object ExecuteScalar(SQLiteConnection connection, string commandText) => ExecuteScalar(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static object ExecuteScalar(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (ExecuteConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result;
            using (SQLiteCommand command = connection.CreateCommand())
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
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, string commandText) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.RepeatableRead, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.RepeatableRead, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, IsolationLevel iso, string commandText) =>
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
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, IsolationLevel iso, string commandText, params SQLiteParameter[] parameters)
        {
            if (TransactionConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SQLiteTransaction transaction = connection.BeginTransaction(iso))
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        result = command.ExecuteNonQuery();
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
        public static void TransactionExecuteProcedure(SQLiteConnection connection, string procedure) =>
            TransactionExecuteProcedure(connection, IsolationLevel.RepeatableRead, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        public static void TransactionExecuteProcedure(SQLiteConnection connection, string procedure, params SQLiteParameter[] parameters) =>
            TransactionExecuteProcedure(connection, IsolationLevel.RepeatableRead, procedure, parameters);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        public static void TransactionExecuteProcedure(SQLiteConnection connection, IsolationLevel iso, string procedure) =>
            TransactionExecuteProcedure(connection, iso, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static void TransactionExecuteProcedure(SQLiteConnection connection, IsolationLevel iso, string procedure, params SQLiteParameter[] parameters)
        {
            if (TransactionConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SQLiteTransaction transaction = connection.BeginTransaction(iso))
            {
                using (SQLiteCommand command = new SQLiteCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
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
        public static object TransactionExecuteScalar(SQLiteConnection connection, string commandText) =>
            TransactionExecuteScalar(connection, IsolationLevel.RepeatableRead, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters) =>
            TransactionExecuteScalar(connection, IsolationLevel.RepeatableRead, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(SQLiteConnection connection, IsolationLevel iso, string commandText) =>
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
        public static object TransactionExecuteScalar(SQLiteConnection connection, IsolationLevel iso, string commandText, params SQLiteParameter[] parameters)
        {
            if (TransactionConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SQLiteTransaction transaction = connection.BeginTransaction(iso))
            {
                using (SQLiteCommand command = connection.CreateCommand())
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

        #region Features

        /// <summary>
        /// Get real-time server version with Execute connection behavior.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <returns></returns>
        public static string GetServerVersion(SQLiteConnection connection)
        {
            return (string)ExecuteScalar(connection, "SELECT SQLITE_VERSION();");
        }

        #endregion Features

        #region Dump

        /// <summary>
        /// Dump database with DataAdapter connection behavior. In order by Table, Trigger, View, Record.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void Dump(SQLiteConnection connection, TextWriter textWriter, out bool cancelled)
        {
            SQLiteDumpSetting setting = GetDumpSetting(connection);
            Dump(connection, setting, textWriter, out cancelled, null, null);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. In order by Table, Trigger, View, Record.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        public static void Dump(SQLiteConnection connection, TextWriter textWriter, out bool cancelled, SQLiteWrittenCallback written, object userState)
        {
            SQLiteDumpSetting setting = GetDumpSetting(connection);
            Dump(connection, setting, textWriter, out cancelled, written, userState);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="setting">Dump setting.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void Dump(SQLiteConnection connection, SQLiteDumpSetting setting, TextWriter textWriter, out bool cancelled)
        {
            Dump(connection, setting, textWriter, out cancelled, null, null);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="setting">Dump setting.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        public static void Dump(SQLiteConnection connection, SQLiteDumpSetting setting, TextWriter textWriter, out bool cancelled, SQLiteWrittenCallback written, object userState)
        {
            if (DataAdapterConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            ConnectionState connectionState = connection.State;
            if (connectionState != ConnectionState.Open) { connection.Open(); }
            //
            long index = 0;
            long total = 0;
            long recordCount = 0;
            List<string> union = new List<string>();
            foreach (SQLiteDumpTableSetting table in setting.Tables)
            {
                if (table.IncludingRecord)
                {
                    union.Add("SELECT COUNT(*) AS `count` FROM `" + table.TableName + "`");
                }
            }
            if (union.Count > 0)
            {
                using (DataTable dt = GetDataTable(connection, string.Join(" UNION ALL ", union) + ";"))
                {
                    recordCount = long.Parse(dt.Compute("SUM(count)", string.Empty).ToString());
                    total += recordCount;
                }
            }
            total += setting.Tables.Count;
            total += setting.Views.Count;
            total += setting.Triggers.Count;
            //
            bool cancel = false;
            StringBuilder tmp = new StringBuilder();
            written?.Invoke(index, total, SQLiteDumpType.Summary, string.Empty, ref cancel, userState);
            if (cancel) { goto end; }
            //
            tmp.AppendLine("/*");
            tmp.AppendLine("Dump by LH.Data.SQLiteHelper");
            tmp.AppendLine();
            tmp.AppendLine("DataSource     : " + connection.DataSource);
            tmp.AppendLine("Server Version : " + connection.ServerVersion);
            tmp.AppendLine("Database       : " + connection.Database);
            tmp.AppendLine();
            tmp.AppendLine("Table          : " + setting.Tables.Count.ToString("n0"));
            tmp.AppendLine("Record         : " + recordCount.ToString("n0"));
            tmp.AppendLine("View           : " + setting.Views.Count.ToString("n0"));
            tmp.AppendLine("Trigger        : " + setting.Triggers.Count.ToString("n0"));
            tmp.AppendLine();
            tmp.AppendLine("Dump Time      : " + DateTime.Now);
            tmp.AppendLine();
            tmp.AppendLine("Use the console or database tool to recover data.");
            tmp.AppendLine("If the target database has a table with the same name, the table data is overwritten.");
            tmp.AppendLine("*/");
            tmp.AppendLine();
            textWriter.Write(tmp.ToString());
            tmp.Clear();
            //
            using (DataSet ds = GetDataSet(connection, SQLiteCommandText.ShowTables() + SQLiteCommandText.ShowTriggers() + SQLiteCommandText.ShowViews()))
            {
                foreach (SQLiteDumpTableSetting table in setting.Tables)
                {
                    string tableCreate = (string)ds.Tables[0].Select("name='" + table.TableName + "'")[0]["sql"];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Table structure for " + table.TableName);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP TABLE IF EXISTS `" + table.TableName + "`;");
                    tmp.AppendLine(tableCreate + ";");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, SQLiteDumpType.Table, table.TableName, ref cancel, userState);
                    if (cancel) { goto end; }
                }
                foreach (string trigger in setting.Triggers)
                {
                    string triggerCreate = (string)ds.Tables[1].Select("name='" + trigger + "'")[0]["sql"];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Trigger structure for " + trigger);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP TRIGGER IF EXISTS `" + trigger + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(triggerCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, SQLiteDumpType.Trigger, trigger, ref cancel, userState);
                    if (cancel) { goto end; }
                }
                foreach (string view in setting.Views)
                {
                    string viewCreate = (string)ds.Tables[1].Select("name='" + view + "'")[0]["sql"];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- View structure for " + view);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP VIEW IF EXISTS `" + view + "`;");
                    tmp.AppendLine(viewCreate + ";");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, SQLiteDumpType.View, view, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            if (recordCount > 0)
            {
                textWriter.WriteLine();
                textWriter.WriteLine("PRAGMA foreign_keys = OFF;");
                foreach (SQLiteDumpTableSetting table in setting.Tables)
                {
                    if (table.IncludingRecord)
                    {
                        if (table.TableName == "sqlite_sequence")
                        {
                            using (DataTable dt = GetDataTable(connection, "SELECT name, seq FROM sqlite_sequence;"))
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    tmp.AppendLine();
                                    tmp.AppendLine("-- ----------------------------");
                                    tmp.AppendLine("-- Records of sqlite_sequence");
                                    tmp.AppendLine("-- ----------------------------");
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        string name = (string)dr["name"];
                                        foreach (SQLiteDumpTableSetting t in setting.Tables)
                                        {
                                            if (t.TableName == name)
                                            {
                                                if (t.Sequence.HasValue && t.Sequence.Value > 0)
                                                {
                                                    tmp.AppendLine("INSERT INTO `sqlite_sequence` VALUES('" + name + "'," + t.Sequence.Value + ");");
                                                }
                                                else if (!t.Sequence.HasValue)
                                                {
                                                    tmp.AppendLine("INSERT INTO `sqlite_sequence` VALUES('" + name + "',NULL);");
                                                }
                                                else
                                                {
                                                    if (dr["seq"] == DBNull.Value)
                                                    {
                                                        tmp.AppendLine("INSERT INTO `sqlite_sequence` VALUES('" + name + "',NULL);");
                                                    }
                                                    else
                                                    {
                                                        tmp.AppendLine("INSERT INTO `sqlite_sequence` VALUES('" + name + "'," + dr["seq"].ToString() + ");");
                                                    }
                                                }
                                                textWriter.Write(tmp.ToString());
                                                tmp.Clear();
                                                index++;
                                                written?.Invoke(index, total, SQLiteDumpType.Record, table.TableName, ref cancel, userState);
                                                if (cancel) { goto recordEnd; }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (SQLiteDataReader reader = GetDataReader(connection, "SELECT * FROM `" + table.TableName + "`;"))
                            {
                                if (reader.HasRows)
                                {
                                    tmp.AppendLine();
                                    tmp.AppendLine("-- ----------------------------");
                                    tmp.AppendLine("-- Records of " + table.TableName);
                                    tmp.AppendLine("-- ----------------------------");
                                    while (reader.Read())
                                    {
                                        tmp.Append("INSERT INTO `" + table.TableName + "` VALUES");
                                        tmp.Append("(");
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            object val = reader.GetValue(i);
                                            if (val == DBNull.Value)
                                            {
                                                tmp.Append("NULL");
                                            }
                                            else
                                            {
                                                switch (val)
                                                {
                                                    case byte[] value: tmp.Append("X'" + BitConverter.ToString(value).Replace("-", string.Empty) + "'"); break;
                                                    case int value: tmp.Append(value.ToString()); break;
                                                    case double value: tmp.Append(value.ToString()); break;
                                                    default: tmp.Append("'" + val.ToString() + "'"); break;
                                                }
                                            }
                                            if (i < reader.FieldCount - 1)
                                            {
                                                tmp.Append(",");
                                            }
                                        }
                                        tmp.AppendLine(");");
                                        textWriter.Write(tmp.ToString());
                                        tmp.Clear();
                                        index++;
                                        written?.Invoke(index, total, SQLiteDumpType.Record, table.TableName, ref cancel, userState);
                                        if (cancel) { goto recordEnd; }
                                    }
                                }
                                //reader.Close();
                            }
                        }
                    }
                }
            recordEnd:
                textWriter.WriteLine();
                textWriter.WriteLine("PRAGMA foreign_keys = ON;");
            }
        end:
            textWriter.Flush();
            if (connectionState != ConnectionState.Open) { connection.Close(); }
            //
            cancelled = cancel;
        }

        /// <summary>
        /// Get dump setting.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <returns></returns>
        public static SQLiteDumpSetting GetDumpSetting(SQLiteConnection connection)
        {
            SQLiteDumpSetting setting = new SQLiteDumpSetting();
            using (DataTable dt = GetDataTable(connection, "SELECT type, name FROM `sqlite_master`;"))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string type = (string)dr["type"];
                        if (type == "table")
                        {
                            setting.Tables.Add(new SQLiteDumpTableSetting((string)dr["name"], 0, true));
                        }
                        else if (type == "view")
                        {
                            setting.Views.Add((string)dr["name"]);
                        }
                        else
                        {
                            setting.Triggers.Add((string)dr["name"]);
                        }
                    }
                }
            }
            return setting;
        }

        #endregion Dump
    }

    #region ConnectionBehavior

    /// <summary>
    /// The mode of the connection when querying.
    /// </summary>
    public enum SQLiteConnectionBehavior
    {
        /// <summary>Does not open automatically. If the connection is not open  when the querying, an exception is thrown.</summary>
        Manual,

        /// <summary>If the connection is not open, automatically open and close when the query is complete. If the connection is already open, keep it turned on when the query is complete.</summary>
        Auto
    }

    #endregion ConnectionBehavior

    #region Dump

    /// <summary>
    /// Delegate for dumping progress.
    /// </summary>
    /// <param name="written">Block index written.</param>
    /// <param name="total">The amount of dumping block.</param>
    /// <param name="dumpType">The type of dumping.</param>
    /// <param name="name">The name associated with dumping.</param>
    /// <param name="cancel">Cancel dump.</param>
    /// <param name="userState">User state.</param>
    public delegate void SQLiteWrittenCallback(long written, long total, SQLiteDumpType dumpType, string name, ref bool cancel, object userState);

    /// <summary>
    /// Note the type of dumping in the progress report.
    /// </summary>
    public enum SQLiteDumpType
    {
        /// <summary>Does not belong to any type. This type does not appear during the dumping process.</summary>
        None = 0,

        /// <summary>Got dumping summary at the beginning.</summary>
        Summary = 1,

        /// <summary>Table schema.</summary>
        Table = 2,

        /// <summary>Record.</summary>
        Record = 4,

        /// <summary>Trigger.</summary>
        Trigger = 8,

        /// <summary>View.</summary>
        View = 16
    }

    /// <summary>
    /// Dump setting.
    /// </summary>
    public sealed class SQLiteDumpSetting
    {
        /// <summary>
        /// Dump table setting.
        /// </summary>
        public List<SQLiteDumpTableSetting> Tables = new List<SQLiteDumpTableSetting>();

        /// <summary>
        /// Dump triggers at specified list.
        /// </summary>
        public List<string> Triggers = new List<string>();

        /// <summary>
        /// Dump views at specified list.
        /// </summary>
        public List<string> Views = new List<string>();
    }

    /// <summary>
    /// Dump table setting.
    /// </summary>
    public sealed class SQLiteDumpTableSetting
    {
        /// <summary>
        /// Dump records.
        /// </summary>
        public bool IncludingRecord;

        /// <summary>
        /// Reset Sequence=value. Set to 0 without changing.
        /// </summary>
        public int? Sequence;

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName;

        /// <summary>
        /// Dump table setting.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="sequence">Reset Sequence=value. Set to 0 without changing.</param>
        /// <param name="includingRecord">Dump records.</param>
        public SQLiteDumpTableSetting(string tableName, int? sequence, bool includingRecord)
        {
            this.TableName = tableName;
            this.Sequence = sequence;
            this.IncludingRecord = includingRecord;
        }
    }

    #endregion Dump

    #region CommandText

    /// <summary>
    /// Command text.
    /// </summary>
    public static class SQLiteCommandText
    {
        #region Table

        /// <summary>
        /// Displays tables in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTables() => "SELECT * FROM `sqlite_master` WHERE `type` = 'table';";

        /// <summary>
        /// Displays tables in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTables(string like) => "SELECT * FROM `sqlite_master` WHERE `type` = 'table' AND `name` LIKE '" + like + "';";

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTriggers() => "SELECT * FROM `sqlite_master` WHERE `type` = 'trigger';";

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like) => "SELECT * FROM `sqlite_master` WHERE `type` = 'trigger' AND `name` LIKE '" + like + "';";

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like, string table) => "SELECT * FROM `sqlite_master` WHERE `type` = 'trigger' AND `tbl_name` = '" + table + "' AND `name` LIKE '" + like + "';";

        /// <summary>
        /// Displays views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowViews() => "SELECT * FROM `sqlite_master` WHERE `type` = 'view';";

        /// <summary>
        /// Displays views in the current database.
        /// </summary>
        /// <param name="like">Part of the view name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowViews(string like) => "SELECT * FROM `sqlite_master` WHERE `type` = 'view' AND `name` LIKE '" + like + "';";

        /// <summary>
        /// Compress database files to recycle wasted space in the database.
        /// </summary>
        /// <returns></returns>
        public static string Vacuum() => "VACUUM";

        #endregion Table
    }

    #endregion CommandText
}