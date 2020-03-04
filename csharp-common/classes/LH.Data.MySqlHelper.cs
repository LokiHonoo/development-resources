/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

/*
 * PackageReference: MySqlConnector OR MySql.Data
 */

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace LH.Data
{
    /// <summary>
    /// MySql extension.
    /// </summary>
    public static class MySqlHelper
    {
        #region ConnectionBehavior

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        public static MySqlConnectionBehavior DataAdapterConnectionBehavior { get; set; } = MySqlConnectionBehavior.Auto;

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        public static MySqlConnectionBehavior DataReaderConnectionBehavior { get; set; } = MySqlConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Execute. Default Manual.
        /// </summary>
        public static MySqlConnectionBehavior ExecuteConnectionBehavior { get; set; } = MySqlConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        public static MySqlConnectionBehavior TransactionConnectionBehavior { get; set; } = MySqlConnectionBehavior.Manual;

        #endregion ConnectionBehavior

        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        public static MySqlConnection BuildConnection(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
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
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder()
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
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder()
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
            if (connectionStringBuilder == null)
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
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder()
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
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder()
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
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, MySqlConnection connection, string selectCommandText) => FillDataSet(dataSet, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static int FillDataSet(DataSet dataSet, MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (DataAdapterConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static int FillDataTable(DataTable dataTable, MySqlConnection connection, string selectCommandText) => FillDataTable(dataTable, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static int FillDataTable(DataTable dataTable, MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (DataAdapterConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static MySqlDataAdapter GetDataAdapter(MySqlConnection connection, string selectCommandText) => new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static MySqlDataAdapter GetDataAdapter(MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            MySqlDataAdapter result = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { result.SelectCommand.Parameters.AddRange(parameters); }
            return result;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(MySqlConnection connection, string selectCommandText) => GetDataSet(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static DataSet GetDataSet(MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (DataAdapterConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataSet result = new DataSet();
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static DataTable GetDataTable(MySqlConnection connection, string selectCommandText) => GetDataTable(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static DataTable GetDataTable(MySqlConnection connection, string selectCommandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (DataAdapterConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataTable result = new DataTable();
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        public static MySqlDataReader GetDataReader(MySqlConnection connection, string commandText) => GetDataReader(connection, commandText, null);

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static MySqlDataReader GetDataReader(MySqlConnection connection, string commandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            CommandBehavior commandBehavior;
            if (DataReaderConnectionBehavior == MySqlConnectionBehavior.Manual)
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
            MySqlCommand command = new MySqlCommand(commandText, connection);
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
        public static int ExecuteNonQuery(MySqlConnection connection, string commandText) => ExecuteNonQuery(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static int ExecuteNonQuery(MySqlConnection connection, string commandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (ExecuteConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (MySqlCommand command = connection.CreateCommand())
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
        public static void ExecuteProcedure(MySqlConnection connection, string procedure) => ExecuteProcedure(connection, procedure, null);

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        public static void ExecuteProcedure(MySqlConnection connection, string procedure, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (ExecuteConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            using (MySqlCommand command = new MySqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
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
        public static object ExecuteScalar(MySqlConnection connection, string commandText) => ExecuteScalar(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static object ExecuteScalar(MySqlConnection connection, string commandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (ExecuteConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result;
            using (MySqlCommand command = connection.CreateCommand())
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static int TransactionExecuteNonQuery(MySqlConnection connection, string commandText) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.RepeatableRead, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static int TransactionExecuteNonQuery(MySqlConnection connection, string commandText, params MySqlParameter[] parameters) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.RepeatableRead, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static int TransactionExecuteNonQuery(MySqlConnection connection, IsolationLevel iso, string commandText) =>
            TransactionExecuteNonQuery(connection, iso, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public static int TransactionExecuteNonQuery(MySqlConnection connection, IsolationLevel iso, string commandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (MySqlTransaction transaction = connection.BeginTransaction(iso))
            {
                using (MySqlCommand command = connection.CreateCommand())
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static void TransactionExecuteProcedure(MySqlConnection connection, string procedure) =>
            TransactionExecuteProcedure(connection, IsolationLevel.RepeatableRead, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static void TransactionExecuteProcedure(MySqlConnection connection, string procedure, params MySqlParameter[] parameters) =>
            TransactionExecuteProcedure(connection, IsolationLevel.RepeatableRead, procedure, parameters);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static void TransactionExecuteProcedure(MySqlConnection connection, IsolationLevel iso, string procedure) =>
            TransactionExecuteProcedure(connection, iso, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public static void TransactionExecuteProcedure(MySqlConnection connection, IsolationLevel iso, string procedure, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (TransactionConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (MySqlTransaction transaction = connection.BeginTransaction(iso))
            {
                using (MySqlCommand command = new MySqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static object TransactionExecuteScalar(MySqlConnection connection, string commandText) =>
            TransactionExecuteScalar(connection, IsolationLevel.RepeatableRead, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static object TransactionExecuteScalar(MySqlConnection connection, string commandText, params MySqlParameter[] parameters) =>
            TransactionExecuteScalar(connection, IsolationLevel.RepeatableRead, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static object TransactionExecuteScalar(MySqlConnection connection, IsolationLevel iso, string commandText) =>
            TransactionExecuteScalar(connection, iso, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public static object TransactionExecuteScalar(MySqlConnection connection, IsolationLevel iso, string commandText, params MySqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (TransactionConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (MySqlTransaction transaction = connection.BeginTransaction(iso))
            {
                using (MySqlCommand command = connection.CreateCommand())
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
        public static string GetServerVersion(MySqlConnection connection)
        {
            return (string)ExecuteScalar(connection, "SELECT @@version;");
        }

        #endregion Features

        #region Dump

        /// <summary>
        /// Dump database with DataAdapter connection behavior. In order by Table, Trigger, View, Function, Procedure, Event, Record.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void Dump(MySqlConnection connection, TextWriter textWriter, out bool cancelled)
        {
            MySqlDumpSetting setting = GetDumpSetting(connection);
            Dump(connection, setting, textWriter, out cancelled, null, null);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. In order by Table, Trigger, View, Function, Procedure, Event, Record.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        public static void Dump(MySqlConnection connection, TextWriter textWriter, out bool cancelled, MySqlWrittenCallback written, object userState)
        {
            MySqlDumpSetting setting = GetDumpSetting(connection);
            Dump(connection, setting, textWriter, out cancelled, written, userState);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="setting">Dump setting.</param>
        /// <param name="textWriter">TextWriter.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void Dump(MySqlConnection connection, MySqlDumpSetting setting, TextWriter textWriter, out bool cancelled)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        public static void Dump(MySqlConnection connection, MySqlDumpSetting setting, TextWriter textWriter, out bool cancelled, MySqlWrittenCallback written, object userState)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }
            if (DataAdapterConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
            foreach (MySqlDumpTableSetting table in setting.Tables)
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
                    recordCount = long.Parse(dt.Compute("SUM(count)", string.Empty).ToString(), CultureInfo.InvariantCulture);
                    total += recordCount;
                }
            }
            total += setting.Tables.Count;
            total += setting.Views.Count;
            total += setting.Triggers.Count;
            total += setting.Functions.Count;
            total += setting.Procedures.Count;
            total += setting.Events.Count;
            //
            bool cancel = false;
            StringBuilder tmp = new StringBuilder();
            written?.Invoke(index, total, MySqlDumpType.Summary, string.Empty, ref cancel, userState);
            if (cancel) { goto end; }
            //
            using (DataTable dt = GetDataTable(connection, "SELECT @@version, @@character_set_server, @@collation_server;"))
            {
                tmp.AppendLine("/*");
                tmp.AppendLine("Dump by LH.Data.MySqlHelper");
                tmp.AppendLine();
                tmp.AppendLine("DataSource     : " + connection.DataSource);
                tmp.AppendLine("Server Version : " + (string)dt.Rows[0][0]);
                tmp.AppendLine("Character_set  : " + (string)dt.Rows[0][1]);
                tmp.AppendLine("Collation      : " + (string)dt.Rows[0][2]);
                tmp.AppendLine("Database       : " + connection.Database);
                tmp.AppendLine();
                tmp.AppendLine("Table          : " + setting.Tables.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Record         : " + recordCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Trigger        : " + setting.Triggers.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("View           : " + setting.Views.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Function       : " + setting.Functions.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Procedure      : " + setting.Procedures.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Event          : " + setting.Events.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine();
                tmp.AppendLine("Dump Time      : " + DateTime.Now);
                tmp.AppendLine();
                tmp.AppendLine("Use the console or database tool to recover data.");
                tmp.AppendLine("If the target database has a table with the same name, the table data is overwritten.");
                tmp.AppendLine("*/");
                tmp.AppendLine();
                textWriter.Write(tmp.ToString());
                tmp.Clear();
            }
            //
            foreach (MySqlDumpTableSetting table in setting.Tables)
            {
                using (DataSet info = GetDataSet(connection, MySqlCommandText.ShowCreateTable(table.TableName) + MySqlCommandText.ShowTableStatus(table.TableName)))
                {
                    string tableCreate = (string)info.Tables[0].Rows[0][1];
                    if (table.AutoIncrement > 0)
                    {
                        if (info.Tables[1].Rows[0]["Auto_increment"] != null)
                        {
                            tableCreate = tableCreate.Replace("AUTO_INCREMENT=" + (int)info.Tables[1].Rows[0]["Auto_increment"], "AUTO_INCREMENT=" + table.AutoIncrement, StringComparison.InvariantCulture);
                        }
                    }
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Table structure for " + table.TableName);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP TABLE IF EXISTS `" + table.TableName + "`;");
                    tmp.AppendLine(tableCreate + ";");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Table, table.TableName, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string trigger in setting.Triggers)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateTrigger(trigger)))
                {
                    string triggerCreate = (string)create.Rows[0][2];
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
                    written?.Invoke(index, total, MySqlDumpType.Trigger, trigger, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string view in setting.Views)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateView(view)))
                {
                    string viewCreate = (string)create.Rows[0][1];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- View structure for " + view);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP VIEW IF EXISTS `" + view + "`;");
                    tmp.AppendLine(viewCreate + ";");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.View, view, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string function in setting.Functions)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateFunction(function)))
                {
                    string functionCreate = (string)create.Rows[0][2];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Function structure for " + function);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP FUNCTION IF EXISTS `" + function + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(functionCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Function, function, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string procedure in setting.Procedures)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateProcedure(procedure)))
                {
                    string procedureCreate = (string)create.Rows[0][2];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Procedure structure for " + procedure);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP PROCEDURE IF EXISTS `" + procedure + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(procedureCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Procedure, procedure, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string event_ in setting.Events)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateEvent(event_)))
                {
                    string eventCreate = (string)create.Rows[0][3];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Event structure for " + event_);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP EVENT IF EXISTS `" + event_ + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(eventCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    textWriter.Write(tmp.ToString());
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Event, event_, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            if (recordCount > 0)
            {
                textWriter.WriteLine();
                textWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
                foreach (MySqlDumpTableSetting table in setting.Tables)
                {
                    if (table.IncludingRecord)
                    {
                        string tableName = table.TableName;
                        using (MySqlDataReader reader = GetDataReader(connection, "SELECT * FROM `" + tableName + "`;"))
                        {
                            if (reader.HasRows)
                            {
                                tmp.AppendLine();
                                tmp.AppendLine("-- ----------------------------");
                                tmp.AppendLine("-- Records of " + tableName);
                                tmp.AppendLine("-- ----------------------------");
                                while (reader.Read())
                                {
                                    tmp.Append("INSERT INTO `" + tableName + "` VALUES");
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
                                                case byte[] value: tmp.Append("X'" + BitConverter.ToString(value).Replace("-", string.Empty, StringComparison.InvariantCulture) + "'"); break;
                                                case bool value: tmp.Append(value ? 1 : 0); break;
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
                                    written?.Invoke(index, total, MySqlDumpType.Record, tableName, ref cancel, userState);
                                    if (cancel) { goto recordEnd; }
                                }
                            }
                            //reader.Close();
                        }
                    }
                }
            recordEnd:
                textWriter.WriteLine();
                textWriter.WriteLine("SET FOREIGN_KEY_CHECKS = 1;");
            }
        end:
            textWriter.Flush();
            if (connectionState != ConnectionState.Open) { connection.Close(); }
            //
            cancelled = cancel;
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. In order by Table, Trigger, View, Function, Procedure, Event, Record.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="folder">Save to folder.</param>
        /// <param name="encoding">File encoding.</param>
        /// <param name="segmentSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void DumpToFiles(MySqlConnection connection, string folder, Encoding encoding, long segmentSize, out bool cancelled)
        {
            MySqlDumpSetting setting = GetDumpSetting(connection);
            DumpToFiles(connection, setting, folder, encoding, segmentSize, out cancelled, null, null);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. In order by Table, Trigger, View, Function, Procedure, Event, Record.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="folder">Save to folder.</param>
        /// <param name="encoding">File encoding.</param>
        /// <param name="segmentSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        /// <param name="written">A delegate that report written progress.</param>

        /// <param name="userState">User state.</param>
        public static void DumpToFiles(MySqlConnection connection, string folder, Encoding encoding, long segmentSize, out bool cancelled, MySqlWrittenCallback written, object userState)
        {
            MySqlDumpSetting setting = GetDumpSetting(connection);
            DumpToFiles(connection, setting, folder, encoding, segmentSize, out cancelled, written, userState);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="setting">Dump setting.</param>
        /// <param name="folder">Save to folder.</param>
        /// <param name="encoding">File encoding.</param>
        /// <param name="segmentSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        public static void DumpToFiles(MySqlConnection connection, MySqlDumpSetting setting, string folder, Encoding encoding, long segmentSize, out bool cancelled)
        {
            DumpToFiles(connection, setting, folder, encoding, segmentSize, out cancelled, null, null);
        }

        /// <summary>
        /// Dump database with DataAdapter connection behavior. Specify items to dump.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="setting">Dump setting.</param>
        /// <param name="folder">Save to folder.</param>
        /// <param name="encoding">File encoding.</param>
        /// <param name="segmentSize">Each file does not exceed the specified size. Cannot specify a value less than 1 MB. Unit is byte.</param>
        /// <param name="cancelled">Indicates whether it is finished normally or has been canceled.</param>
        /// <param name="written">A delegate that report written progress.</param>
        /// <param name="userState">User state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public static void DumpToFiles(MySqlConnection connection, MySqlDumpSetting setting, string folder, Encoding encoding, long segmentSize, out bool cancelled, MySqlWrittenCallback written, object userState)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (segmentSize < 1024 * 1024)
            {
                throw new ArgumentException("Segment size cannot be less than 1 MB.");
            }
            if (DataAdapterConnectionBehavior == MySqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
            foreach (MySqlDumpTableSetting table in setting.Tables)
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
                    recordCount = long.Parse(dt.Compute("SUM(count)", string.Empty).ToString(), CultureInfo.InvariantCulture);
                    total += recordCount;
                }
            }
            total += setting.Tables.Count;
            total += setting.Views.Count;
            total += setting.Triggers.Count;
            total += setting.Functions.Count;
            total += setting.Procedures.Count;
            total += setting.Events.Count;
            //
            bool cancel = false;
            StringBuilder tmp = new StringBuilder();
            byte[] tmpBytes;
            string file;
            FileStream fileStream = null;
            int sn;
            written?.Invoke(index, total, MySqlDumpType.Summary, string.Empty, ref cancel, userState);
            if (cancel) { goto end; }
            //
            file = Path.Combine(folder, "!schema.sql");
            fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            sn = 0;
            using (DataTable dt = GetDataTable(connection, "SELECT @@version, @@character_set_server, @@collation_server;"))
            {
                tmp.AppendLine("/*");
                tmp.AppendLine("Dump by LH.Data.MySqlHelper");
                tmp.AppendLine();
                tmp.AppendLine("DataSource     : " + connection.DataSource);
                tmp.AppendLine("Server Version : " + (string)dt.Rows[0][0]);
                tmp.AppendLine("Character_set  : " + (string)dt.Rows[0][1]);
                tmp.AppendLine("Collation      : " + (string)dt.Rows[0][2]);
                tmp.AppendLine("Database       : " + connection.Database);
                tmp.AppendLine();
                tmp.AppendLine("Table          : " + setting.Tables.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Record         : " + recordCount.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Trigger        : " + setting.Triggers.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("View           : " + setting.Views.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Function       : " + setting.Functions.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Procedure      : " + setting.Procedures.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine("Event          : " + setting.Events.Count.ToString("n0", CultureInfo.InvariantCulture));
                tmp.AppendLine();
                tmp.AppendLine("Dump Time      : " + DateTime.Now);
                tmp.AppendLine();
                tmp.AppendLine("Use the console or database tool to recover data.");
                tmp.AppendLine("If the target database has a table with the same name, the table data is overwritten.");
                tmp.AppendLine("*/");
                tmp.AppendLine();
                tmpBytes = encoding.GetBytes(tmp.ToString());
                fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                tmp.Clear();
            }
            //
            foreach (MySqlDumpTableSetting table in setting.Tables)
            {
                using (DataSet info = GetDataSet(connection, MySqlCommandText.ShowCreateTable(table.TableName) + MySqlCommandText.ShowTableStatus(table.TableName)))
                {
                    string tableCreate = (string)info.Tables[0].Rows[0][1];
                    if (table.AutoIncrement > 0)
                    {
                        if (info.Tables[1].Rows[0]["Auto_increment"] != null)
                        {
                            tableCreate = tableCreate.Replace("AUTO_INCREMENT=" + (int)info.Tables[1].Rows[0]["Auto_increment"], "AUTO_INCREMENT=" + table.AutoIncrement, StringComparison.InvariantCulture);
                        }
                    }
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Table structure for " + table.TableName);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP TABLE IF EXISTS `" + table.TableName + "`;");
                    tmp.AppendLine(tableCreate + ";");
                    tmpBytes = encoding.GetBytes(tmp.ToString());
                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "!schema_" + sn + ".sql");
                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Table, table.TableName, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string trigger in setting.Triggers)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateTrigger(trigger)))
                {
                    string triggerCreate = (string)create.Rows[0][2];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Trigger structure for " + trigger);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP TRIGGER IF EXISTS `" + trigger + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(triggerCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    tmpBytes = encoding.GetBytes(tmp.ToString());
                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "!schema_" + sn + ".sql");
                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Trigger, trigger, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string view in setting.Views)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateView(view)))
                {
                    string viewCreate = (string)create.Rows[0][1];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- View structure for " + view);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP VIEW IF EXISTS `" + view + "`;");
                    tmp.AppendLine(viewCreate + ";");
                    tmpBytes = encoding.GetBytes(tmp.ToString());
                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "!schema_" + sn + ".sql");
                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.View, view, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string function in setting.Functions)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateFunction(function)))
                {
                    string functionCreate = (string)create.Rows[0][2];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Function structure for " + function);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP FUNCTION IF EXISTS `" + function + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(functionCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    tmpBytes = encoding.GetBytes(tmp.ToString());
                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "!schema_" + sn + ".sql");
                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Function, function, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string procedure in setting.Procedures)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateProcedure(procedure)))
                {
                    string procedureCreate = (string)create.Rows[0][2];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Procedure structure for " + procedure);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP PROCEDURE IF EXISTS `" + procedure + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(procedureCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    tmpBytes = encoding.GetBytes(tmp.ToString());
                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "!schema_" + sn + ".sql");
                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Procedure, procedure, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            foreach (string event_ in setting.Events)
            {
                using (DataTable create = GetDataTable(connection, MySqlCommandText.ShowCreateEvent(event_)))
                {
                    string eventCreate = (string)create.Rows[0][3];
                    tmp.AppendLine();
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("-- Event structure for " + event_);
                    tmp.AppendLine("-- ----------------------------");
                    tmp.AppendLine("DROP EVENT IF EXISTS `" + event_ + "`;");
                    tmp.AppendLine("DELIMITER ;;");
                    tmp.AppendLine(eventCreate + ";;");
                    tmp.AppendLine("DELIMITER ;");
                    tmpBytes = encoding.GetBytes(tmp.ToString());
                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                        sn++;
                        file = Path.Combine(folder, "!schema_" + sn + ".sql");
                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                    tmp.Clear();
                    index++;
                    written?.Invoke(index, total, MySqlDumpType.Event, event_, ref cancel, userState);
                    if (cancel) { goto end; }
                }
            }
            if (recordCount > 0)
            {
                byte[] foreignKeyChecksOffBytes = encoding.GetBytes("SET FOREIGN_KEY_CHECKS = 0;" + Environment.NewLine);
                byte[] foreignKeyChecksOnBytes = encoding.GetBytes(Environment.NewLine + "SET FOREIGN_KEY_CHECKS = 1;");
                foreach (MySqlDumpTableSetting table in setting.Tables)
                {
                    if (table.IncludingRecord)
                    {
                        using (MySqlDataReader reader = GetDataReader(connection, "SELECT * FROM `" + table.TableName + "`;"))
                        {
                            if (reader.HasRows)
                            {
                                file = Path.Combine(folder, "record_" + table.TableName + ".sql");
                                fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                                sn = 0;
                                //
                                fileStream.Write(foreignKeyChecksOffBytes, 0, foreignKeyChecksOffBytes.Length);
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
                                                case byte[] value: tmp.Append("X'" + BitConverter.ToString(value).Replace("-", string.Empty, StringComparison.InvariantCulture) + "'"); break;
                                                case bool value: tmp.Append(value ? 1 : 0); break;
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
                                                default: tmp.Append("'" + val.ToString() + "'"); break;
                                            }
                                        }
                                        if (i < reader.FieldCount - 1)
                                        {
                                            tmp.Append(",");
                                        }
                                    }
                                    tmp.AppendLine(");");
                                    tmpBytes = encoding.GetBytes(tmp.ToString());
                                    if (fileStream.Length + tmpBytes.Length > segmentSize)
                                    {
                                        fileStream.Close();
                                        fileStream.Dispose();
                                        sn++;
                                        file = Path.Combine(folder, "record_" + table.TableName + "_" + sn + ".sql");
                                        fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                                    }
                                    fileStream.Write(tmpBytes, 0, tmpBytes.Length);
                                    tmp.Clear();
                                    index++;
                                    written?.Invoke(index, total, MySqlDumpType.Record, table.TableName, ref cancel, userState);
                                    if (cancel) { goto recordEnd; }
                                }
                            recordEnd:
                                fileStream.Write(foreignKeyChecksOnBytes, 0, foreignKeyChecksOnBytes.Length);
                                if (cancel) { goto end; }
                            }
                            //reader.Close();
                        }
                    }
                }
            }
        end:
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose();
            }
            if (connectionState != ConnectionState.Open) { connection.Close(); }
            //
            cancelled = cancel;
        }

        /// <summary>
        /// Get dump setting.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <returns></returns>
        public static MySqlDumpSetting GetDumpSetting(MySqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            MySqlDumpSetting setting = new MySqlDumpSetting();
            StringBuilder sql = new StringBuilder();
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
                            setting.Tables.Add(new MySqlDumpTableSetting((string)dr["Name"], 0, true));
                        }
                        else
                        {
                            setting.Views.Add((string)dr["Name"]);
                        }
                    }
                }
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    setting.Triggers.Add((string)dr["Trigger"]);
                }
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    setting.Functions.Add((string)dr["Name"]);
                }
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    setting.Procedures.Add((string)dr["Name"]);
                }
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    setting.Events.Add((string)dr["Name"]);
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
    public enum MySqlConnectionBehavior
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
    public delegate void MySqlWrittenCallback(long written, long total, MySqlDumpType dumpType, string name, ref bool cancel, object userState);

    /// <summary>
    /// Note the type of dumping in the progress report.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "<Pending>")]
    public enum MySqlDumpType
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
        View = 16,

        /// <summary>Function.</summary>
        Function = 32,

        /// <summary>Procedure.</summary>
        Procedure = 64,

        /// <summary>Event.</summary>
        Event = 128
    }

    /// <summary>
    /// Dump setting.
    /// </summary>
    public sealed class MySqlDumpSetting
    {
        /// <summary>
        /// Dump events at specified list.
        /// </summary>
        public List<string> Events { get; } = new List<string>();

        /// <summary>
        /// Dump functions at specified list.
        /// </summary>
        public List<string> Functions { get; } = new List<string>();

        /// <summary>
        /// Dump procedures at specified list.
        /// </summary>
        public List<string> Procedures { get; } = new List<string>();

        /// <summary>
        /// Dump table setting.
        /// </summary>
        public List<MySqlDumpTableSetting> Tables { get; } = new List<MySqlDumpTableSetting>();

        /// <summary>
        /// Dump triggers at specified list.
        /// </summary>
        public List<string> Triggers { get; } = new List<string>();

        /// <summary>
        /// Dump views at specified list.
        /// </summary>
        public List<string> Views { get; } = new List<string>();
    }

    /// <summary>
    /// Dump table setting.
    /// </summary>
    public sealed class MySqlDumpTableSetting
    {
        /// <summary>
        /// Dump table setting.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="autoIncrement">Reset AUTO_INCREMENT=value. Set to 0 without changing.</param>
        /// <param name="includingRecord">Dump records.</param>
        public MySqlDumpTableSetting(string tableName, int autoIncrement, bool includingRecord)
        {
            this.TableName = tableName;
            this.AutoIncrement = autoIncrement;
            this.IncludingRecord = includingRecord;
        }

        /// <summary>
        /// Reset AUTO_INCREMENT=value. Set to 0 without changing.
        /// </summary>
        public int AutoIncrement { get; }

        /// <summary>
        /// Dump records.
        /// </summary>
        public bool IncludingRecord { get; }

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
        #region Engine

        /// <summary>
        /// Displays the storage engine and default engine available after installation.
        /// </summary>
        /// <returns></returns>
        public static string ShowEngines() => "SHOW ENGINES;";

        /// <summary>
        /// Displays the error caused by the last execution statement.
        /// </summary>
        /// <returns></returns>
        public static string ShowErrors() => "SHOW ERRORS;";

        /// <summary>
        /// Displays the name and value of the global system variable.
        /// </summary>
        /// <returns></returns>
        public static string ShowGlobalVariables() => "SHOW GLOBAL VARIABLES;";

        /// <summary>
        /// Displays the name and value of the global system variable.
        /// </summary>
        /// <param name="like">Part of the variable name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowGlobalVariables(string like) => "SHOW GLOBAL VARIABLES LIKE '" + like + "';";

        /// <summary>
        /// Displays the permissions for all user.
        /// </summary>
        /// <returns></returns>
        public static string ShowGrants() => "SHOW GRANTS;";

        /// <summary>
        /// Displays the permissions for the specified user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="host">Host.</param>
        /// <returns></returns>
        public static string ShowGrants(string user, string host) => "SHOW GRANTS FOR `" + user + "`@`" + host + "`;";

        /// <summary>
        /// Displays the status of the InnoDB storage engine.
        /// </summary>
        /// <returns></returns>
        public static string ShowInnoDbStatus() => "SHOW INNODB STATUS;";

        /// <summary>
        /// Displays the log.
        /// </summary>
        /// <returns></returns>
        public static string ShowLogs() => "SHOW LOGS;";

        /// <summary>
        /// Displays the different permissions supported by the server.
        /// </summary>
        /// <returns></returns>
        public static string ShowPrivileges() => "SHOW PRIVILEGES;";

        /// <summary>
        /// Displays all processes that are running in the system.
        /// </summary>
        /// <returns></returns>
        public static string ShowProcesslist() => "SHOW PROCESSLIST;";

        /// <summary>
        /// Displays information about some system-specific resources, such as the number of threads running.
        /// </summary>
        /// <returns></returns>
        public static string ShowStatus() => "SHOW STATUS;";

        /// <summary>
        /// Displays information about some system-specific resources, such as the number of threads running.
        /// </summary>
        /// <param name="like">Part of the variable name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowStatus(string like) => "SHOW STATUS LIKE '" + like + "';";

        /// <summary>
        /// Displays the available storage engine and default engine after installation.
        /// </summary>
        /// <returns></returns>
        public static string ShowStorageEngines() => "SHOW STORAGE ENGINES;";

        /// <summary>
        /// Displays the name and value of the system variable.
        /// </summary>
        /// <returns></returns>
        public static string ShowVariables() => "SHOW VARIABLES;";

        /// <summary>
        /// Displays the name and value of the system variable.
        /// </summary>
        /// <param name="like">Part of the variable name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowVariables(string like) => "SHOW VARIABLES LIKE '" + like + "';";

        /// <summary>
        /// Displays the warning generated by the last executed statement.
        /// </summary>
        /// <returns></returns>
        public static string ShowWarnings() => "SHOW WARNINGS;";

        #endregion Engine

        #region Database

        /// <summary>
        /// Creates a database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to create.</param>
        /// <returns></returns>
        public static string CreateDatabase(string database) => "CREATE DATABASE `" + database + "`;";

        /// <summary>
        /// Creates a database with the current connection and specifies the character set.
        /// </summary>
        /// <param name="database">The name of the database to create.</param>
        /// <param name="charset">Character set. Such for utf8.</param>
        /// <param name="collate">Collate. Such for utf8_general_ci.</param>
        /// <returns></returns>
        public static string CreateDatabase(string database, string charset, string collate) => "CREATE DATABASE `" + database + "` DEFAULT CHARSET " + charset + " COLLATE " + collate + ";";

        /// <summary>
        /// Deletes the database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to delete.</param>
        /// <returns></returns>
        public static string DropDatabase(string database) => "DROP DATABASE IF EXISTS `" + database + "`;";

        /// <summary>
        /// Displays the creation script for the database with the specified name.
        /// </summary>
        /// <param name="database">Database name.</param>
        /// <returns></returns>
        public static string ShowCreateDatabase(string database) => "SHOW CREATE DATABASE `" + database + "`;";

        /// <summary>
        /// Displays all visible database names.
        /// </summary>
        /// <returns></returns>
        public static string ShowDatabases() => "SHOW DATABASES;";

        #endregion Database

        #region Table

        /// <summary>
        /// Displays the column names in the table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string Desc(string table) => "DESC `" + table + "`;";

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="event_">The name of the event.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        public static string DropEvent(string event_) => "DROP EVENT IF EXISTS `" + event_ + "`;";

        /// <summary>
        /// Deletes the stored procedure.
        /// </summary>
        /// <param name="function">The name of the function.</param>
        /// <returns></returns>
        public static string DropFunction(string function) => "DROP FUNCTION IF EXISTS `" + function + "`;";

        /// <summary>
        /// Deletes the stored procedure.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns></returns>
        public static string DropProcedure(string procedure) => "DROP PROCEDURE IF EXISTS `" + procedure + "`;";

        /// <summary>
        /// Deletes the trigger.
        /// </summary>
        /// <param name="trigger">The name of the trigger.</param>
        /// <returns></returns>
        public static string DropTrigger(string trigger) => "DROP TRIGGER IF EXISTS `" + trigger + "`;";

        /// <summary>
        /// Deletes the view.
        /// </summary>
        /// <param name="view">The name of the view.</param>
        /// <returns></returns>
        public static string DropView(string view) => "DROP VIEW IF EXISTS `" + view + "`;";

        /// <summary>
        /// Displays the column names in the table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowColumns(string table) => "SHOW COLUMNS FROM `" + table + "`;";

        /// <summary>
        /// Displays the creation script for the stored procedure with the specified name.
        /// </summary>
        /// <param name="event_">The name of the event.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        public static string ShowCreateEvent(string event_) => "SHOW CREATE EVENT `" + event_ + "`;";

        /// <summary>
        /// Displays the creation script for the function of the specified name.
        /// </summary>
        /// <param name="function">The name of the function.</param>
        /// <returns></returns>
        public static string ShowCreateFunction(string function) => "SHOW CREATE FUNCTION `" + function + "`;";

        /// <summary>
        /// Displays the creation script for the stored procedure with the specified name.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns></returns>
        public static string ShowCreateProcedure(string procedure) => "SHOW CREATE PROCEDURE `" + procedure + "`;";

        /// <summary>
        /// Displays the creation script for the table with the specified name.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowCreateTable(string table) => "SHOW CREATE TABLE `" + table + "`;";

        /// <summary>
        /// Displays the creation script for the trigger with the specified name.
        /// </summary>
        /// <param name="trigger">The name of the trigger.</param>
        /// <returns></returns>
        public static string ShowCreateTrigger(string trigger) => "SHOW CREATE TRIGGER `" + trigger + "`;";

        /// <summary>
        /// Displays the creation script for the view with the specified name.
        /// </summary>
        /// <param name="view">The name of the table.</param>
        /// <returns></returns>
        public static string ShowCreateView(string view) => "SHOW CREATE VIEW `" + view + "`;";

        /// <summary>
        /// Displays basic information about all events in all databases.
        /// </summary>
        /// <returns></returns>
        public static string ShowEvents() => "SHOW EVENTS;";

        /// <summary>
        /// Displays basic information about the events in the specified database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns></returns>
        public static string ShowEvents(string database) => "SHOW EVENTS WHERE `Db` = '" + database + "';";

        /// <summary>
        /// Displays basic information about all functions in all databases.
        /// </summary>
        /// <returns></returns>
        public static string ShowFunctionStatus() => "SHOW FUNCTION STATUS;";

        /// <summary>
        /// Displays basic information about the functions in the specified database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns></returns>
        public static string ShowFunctionStatus(string database) => "SHOW FUNCTION STATUS WHERE `Db` = '" + database + "';";

        /// <summary>
        /// Displays the index of the table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowIndex(string table) => "SHOW INDEX FROM `" + table + "`;";

        /// <summary>
        /// Displays basic information about stored procedures.
        /// </summary>
        /// <returns></returns>
        public static string ShowProcedureStatus() => "SHOW PROCEDURE STATUS;";

        /// <summary>
        /// Displays basic information about stored procedures in the specified database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns></returns>
        public static string ShowProcedureStatus(string database) => "SHOW PROCEDURE STATUS WHERE `Db` = '" + database + "';";

        /// <summary>
        /// Displays tables and views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTables() => "SHOW TABLES;";

        /// <summary>
        /// Displays tables and views in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTables(string like) => "SHOW TABLES LIKE '" + like + "';";

        /// <summary>
        /// Displays the details of the tables and views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTableStatus() => "SHOW TABLE STATUS;";

        /// <summary>
        /// Displays the details of the tables and views in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTableStatus(string like) => "SHOW TABLE STATUS LIKE '" + like + "';";

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTriggers() => "SHOW TRIGGERS;";

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like) => "SHOW TRIGGERS LIKE '" + like + "';";

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like, string table) => "SHOW TRIGGERS WHERE `Table` = '" + table + "' AND `Trigger` LIKE '" + like + "';";

        /// <summary>
        /// Empty the table and initialize the table state.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string Truncate(string table) => "TRUNCATE TABLE `" + table + "`;";

        #endregion Table
    }

    #endregion CommandText
}