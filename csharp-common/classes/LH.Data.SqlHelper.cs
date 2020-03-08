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
using System.Data.SqlClient;
using System.Globalization;
using System.Xml;

namespace LH.Data
{
    /// <summary>
    /// Sql extension.
    /// </summary>
    internal static class SqlHelper
    {
        #region ConnectionBehavior

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        internal static SqlConnectionBehavior DataAdapterConnectionBehavior { get; set; } = SqlConnectionBehavior.Auto;

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        internal static SqlConnectionBehavior DataReaderConnectionBehavior { get; set; } = SqlConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Execute, XmlReader. Default Manual.
        /// </summary>
        internal static SqlConnectionBehavior ExecuteConnectionBehavior { get; set; } = SqlConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        internal static SqlConnectionBehavior TransactionConnectionBehavior { get; set; } = SqlConnectionBehavior.Manual;

        #endregion ConnectionBehavior

        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        internal static SqlConnection BuildConnection(SqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }

            return new SqlConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns></returns>
        internal static SqlConnection BuildConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="dataSource">Name or network address of the data server.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="catalog">The name of the database associated with the connection.</param>
        /// <returns></returns>
        internal static SqlConnection BuildConnection(string dataSource, string userID, string password, string catalog)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                IntegratedSecurity = false,
                PersistSecurityInfo = false,
                DataSource = dataSource,
                UserID = userID,
                Password = password,
                InitialCatalog = catalog
            };
            return new SqlConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="server">Name or network address of the data server.</param>
        /// <param name="port">Server port.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="catalog">The name of the database associated with the connection.</param>
        /// <returns></returns>
        internal static SqlConnection BuildConnection(string server, uint port, string userID, string password, string catalog)
        {
            string dataSource = string.Format(CultureInfo.InvariantCulture, "{0},{1}", server, port);
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                IntegratedSecurity = false,
                PersistSecurityInfo = false,
                DataSource = dataSource,
                UserID = userID,
                Password = password,
                InitialCatalog = catalog
            };
            return new SqlConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        internal static string BuildConnectionString(SqlConnectionStringBuilder connectionStringBuilder)
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
        /// <param name="dataSource">Name or network address of the data server.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="catalog">The name of the database associated with the connection.</param>
        /// <returns></returns>
        internal static string BuildConnectionString(string dataSource, string userID, string password, string catalog)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                IntegratedSecurity = false,
                PersistSecurityInfo = false,
                DataSource = dataSource,
                UserID = userID,
                Password = password,
                InitialCatalog = catalog
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
        /// <param name="catalog">The name of the database associated with the connection.</param>
        /// <returns></returns>
        internal static string BuildConnectionString(string server, uint port, string userID, string password, string catalog)
        {
            string dataSource = string.Format(CultureInfo.InvariantCulture, "{0},{1}", server, port);
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                IntegratedSecurity = false,
                PersistSecurityInfo = false,
                DataSource = dataSource,
                UserID = userID,
                Password = password,
                InitialCatalog = catalog
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
        internal static int FillDataSet(DataSet dataSet, SqlConnection connection, string selectCommandText) => FillDataSet(dataSet, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static int FillDataSet(DataSet dataSet, SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static int FillDataTable(DataTable dataTable, SqlConnection connection, string selectCommandText) => FillDataTable(dataTable, connection, selectCommandText, null);

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static int FillDataTable(DataTable dataTable, SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static SqlDataAdapter GetDataAdapter(SqlConnection connection, string selectCommandText) => new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static SqlDataAdapter GetDataAdapter(SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            SqlDataAdapter result = new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { result.SelectCommand.Parameters.AddRange(parameters); }
            return result;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        internal static DataSet GetDataSet(SqlConnection connection, string selectCommandText) => GetDataSet(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static DataSet GetDataSet(SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataSet result = new DataSet();
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static DataTable GetDataTable(SqlConnection connection, string selectCommandText) => GetDataTable(connection, selectCommandText, null);

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static DataTable GetDataTable(SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataTable result = new DataTable();
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
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
        internal static SqlDataReader GetDataReader(SqlConnection connection, string commandText) => GetDataReader(connection, commandText, null);

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static SqlDataReader GetDataReader(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            CommandBehavior commandBehavior;
            if (DataReaderConnectionBehavior == SqlConnectionBehavior.Manual)
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
            SqlCommand command = new SqlCommand(commandText, connection);
            if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
            return command.ExecuteReader(commandBehavior);
        }

        #endregion DataReader

        #region XmlReader

        /// <summary>
        /// Get XmlReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static XmlReader GetXmlReader(SqlConnection connection, string commandText) => GetXmlReader(connection, commandText, null);

        /// <summary>
        /// Get XmlReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static XmlReader GetXmlReader(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            XmlReader result;
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                result = command.ExecuteXmlReader();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
            return result;
        }

        #endregion XmlReader

        #region Execute

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static int ExecuteNonQuery(SqlConnection connection, string commandText) => ExecuteNonQuery(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static int ExecuteNonQuery(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (SqlCommand command = connection.CreateCommand())
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
        internal static void ExecuteProcedure(SqlConnection connection, string procedure) => ExecuteProcedure(connection, procedure, null);

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static void ExecuteProcedure(SqlConnection connection, string procedure, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            using (SqlCommand command = new SqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
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
        internal static object ExecuteScalar(SqlConnection connection, string commandText) => ExecuteScalar(connection, commandText, null);

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static object ExecuteScalar(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result;
            using (SqlCommand command = connection.CreateCommand())
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
        internal static int TransactionExecuteNonQuery(SqlConnection connection, string commandText) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.ReadCommitted, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        internal static int TransactionExecuteNonQuery(SqlConnection connection, string commandText, params SqlParameter[] parameters) =>
            TransactionExecuteNonQuery(connection, IsolationLevel.ReadCommitted, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static int TransactionExecuteNonQuery(SqlConnection connection, IsolationLevel iso, string commandText) =>
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static int TransactionExecuteNonQuery(SqlConnection connection, IsolationLevel iso, string commandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SqlTransaction transaction = connection.BeginTransaction(iso))
            {
                using (SqlCommand command = connection.CreateCommand())
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
        internal static void TransactionExecuteProcedure(SqlConnection connection, string procedure) =>
            TransactionExecuteProcedure(connection, IsolationLevel.ReadCommitted, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        internal static void TransactionExecuteProcedure(SqlConnection connection, string procedure, params SqlParameter[] parameters) =>
            TransactionExecuteProcedure(connection, IsolationLevel.ReadCommitted, procedure, parameters);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        internal static void TransactionExecuteProcedure(SqlConnection connection, IsolationLevel iso, string procedure) =>
            TransactionExecuteProcedure(connection, iso, procedure, null);

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static void TransactionExecuteProcedure(SqlConnection connection, IsolationLevel iso, string procedure, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SqlTransaction transaction = connection.BeginTransaction(iso))
            {
                using (SqlCommand command = new SqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
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
        internal static object TransactionExecuteScalar(SqlConnection connection, string commandText) =>
            TransactionExecuteScalar(connection, IsolationLevel.ReadCommitted, commandText, null);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        internal static object TransactionExecuteScalar(SqlConnection connection, string commandText, params SqlParameter[] parameters) =>
            TransactionExecuteScalar(connection, IsolationLevel.ReadCommitted, commandText, parameters);

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static object TransactionExecuteScalar(SqlConnection connection, IsolationLevel iso, string commandText) =>
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static object TransactionExecuteScalar(SqlConnection connection, IsolationLevel iso, string commandText, params SqlParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SqlTransaction transaction = connection.BeginTransaction(iso))
            {
                using (SqlCommand command = connection.CreateCommand())
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
    internal enum SqlConnectionBehavior
    {
        /// <summary>Does not open automatically. If the connection is not open  when the querying, an exception is thrown.</summary>
        Manual,

        /// <summary>If the connection is not open, automatically open and close when the query is complete. If the connection is already open, keep it turned on when the query is complete.</summary>
        Auto
    }

    #endregion ConnectionBehavior

    #region CommandText

    /// <summary>
    /// Return command text.
    /// </summary>
    internal static class SqlCommandText
    {
        #region Engine

        /// <summary>
        /// Displays database files.
        /// </summary>
        internal static string ShowAltFiles() => "SELECT * FROM sysaltfiles";

        /// <summary>
        /// Displays character set and collation.
        /// </summary>
        internal static string ShowCharsets() => "SELECT * FROM syscharsets";

        /// <summary>
        /// Displays configuration options.
        /// </summary>
        internal static string ShowConfigures() => "SELECT * FROM sysconfigures";

        /// <summary>
        /// Displays current config options.
        /// </summary>
        internal static string ShowCurConfigs() => "SELECT * FROM syscurconfigs";

        /// <summary>
        /// Displays file groups.
        /// </summary>
        internal static string ShowFileGroups() => "SELECT * FROM sysfilegroups";

        /// <summary>
        /// Displays file, sysfiles is a virtual table.
        /// </summary>
        internal static string ShowFiles() => "SELECT * FROM sysfiles";

        /// <summary>
        /// Displays external Keywords.
        /// </summary>
        internal static string ShowForeignKeys() => "SELECT * FROM sysforeignkeys";

        /// <summary>
        /// Displays languages.
        /// </summary>
        internal static string ShowLanguages() => "SELECT * FROM syslanguages";

        /// <summary>
        /// Displays login account information.
        /// </summary>
        internal static string ShowLogins() => "SELECT * FROM syslogins";

        /// <summary>
        /// Displays members.
        /// </summary>
        internal static string ShowMembers() => "SELECT * FROM sysmembers";

        /// <summary>
        /// Displays objects of all types.
        /// </summary>
        internal static string ShowObjects() => "SELECT * FROM sysobjects;";

        /// <summary>
        /// Displays server login information.
        /// </summary>
        internal static string ShowOleDbUsers() => "SELECT * FROM sysoledbusers";

        /// <summary>
        /// Displays permissions.
        /// </summary>
        internal static string ShowPermissions() => "SELECT * FROM syspermissions;";

        /// <summary>
        /// Displays processes.
        /// </summary>
        internal static string ShowProcesses() => "SELECT * FROM sysprocesses;";

        /// <summary>
        /// Displays Remote login account.
        /// </summary>
        internal static string ShowRemoteLogins() => "SELECT * FROM sysremotelogins;";

        /// <summary>
        /// Displays objects of the specified type.
        /// </summary>
        /// <param name="xType">
        /// <para>C = CHECK. D = DEFAULT. F = FOREIGN KEY. L = Log.</para>
        /// <para>FN = function non-vector. IF = Inlined table function. P = Procedure. PK = PRIMARY KEY.</para>
        /// <para>RF = Replication filter stored procedure. S = System table. TF = table function. TR = Trigger.</para>
        /// <para>U = User table. UQ = UNIQUE .V = View. X = Extended stored procedure.</para>
        /// </param>
        internal static string ShowSysObject(string xType) => "SELECT * FROM sysobjects WHERE xtype = N'" + xType + "';";

        /// <summary>
        /// Displays system data types and user defined data types.
        /// </summary>
        internal static string ShowTypes() => "SELECT * FROM systypes;";

        /// <summary>
        /// Displays users.
        /// </summary>
        internal static string ShowUsers() => "SELECT * FROM sysusers;";

        #endregion Engine

        #region Database

        /// <summary>
        /// Creates a database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to create.</param>
        /// <returns></returns>
        internal static string CreateDatabase(string database) => "CREATE DATABASE [" + database + "];";

        /// <summary>
        /// Deletes the database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to delete.</param>
        /// <returns></returns>
        internal static string DropDatabase(string database) => "IF EXISTS (SELECT name FROM sysdatabases WHERE name = N'" + database + "') DROP DATABASE [" + database + "];";

        /// <summary>
        /// Displays all visible database names.
        /// </summary>
        /// <returns></returns>
        internal static string ShowDatabases() => "SELECT * FROM sysdatabases;";

        #endregion Database

        #region Table

        /// <summary>
        /// Displays all types of columns.
        /// </summary>
        internal static string ShowColumns() => "SELECT * FROM syscolumns;";

        /// <summary>
        /// Displays the columns of the specified type, and the data types provided by the current data server or data source can be queried through the systypes table.
        /// </summary>
        /// <param name="xType">syscolumns xtype。</param>
        internal static string ShowColumns(int xType) => "SELECT * FROM syscolumns WHERE xtype = " + xType + ";";

        /// <summary>
        /// Displays the creation script for the stored procedure with the specified name.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns></returns>
        internal static string ShowCreateProcedure(string procedure) => "EXEC sp_helptext N'" + procedure + "';";

        /// <summary>
        /// Displays basic information about the specified kind of stored procedure.
        /// </summary>
        /// <returns></returns>
        /// <param name="category">Set to 0 to represent the user stored procedure.</param>
        internal static string ShowProcedures(int category) => "SELECT * FROM sysobjects WHERE xtype= N'P' AND category = " + category + ";";

        /// <summary>
        /// Display basic information for all stored procedures.
        /// </summary>
        /// <returns></returns>
        internal static string ShowProcedures() => "SELECT * FROM sysobjects WHERE xtype= N'P';";

        #endregion Table
    }

    #endregion CommandText
}