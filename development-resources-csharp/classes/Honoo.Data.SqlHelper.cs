/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml;

namespace Honoo.Data
{
    /// <summary>
    /// Sql helper.
    /// </summary>
    public static class SqlHelper
    {
        #region ConnectionBehavior

        private static SqlConnectionBehavior _dataAdapterConnectionBehavior = SqlConnectionBehavior.Auto;
        private static SqlConnectionBehavior _dataReaderConnectionBehavior = SqlConnectionBehavior.Manual;
        private static SqlConnectionBehavior _executeConnectionBehavior = SqlConnectionBehavior.Manual;
        private static SqlConnectionBehavior _transactionConnectionBehavior = SqlConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        public static SqlConnectionBehavior DataAdapterConnectionBehavior
        {
            get => _dataAdapterConnectionBehavior;
            set => _dataAdapterConnectionBehavior = value;
        }

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        public static SqlConnectionBehavior DataReaderConnectionBehavior
        {
            get => _dataReaderConnectionBehavior;
            set => _dataReaderConnectionBehavior = value;
        }

        /// <summary>
        /// Connection open/close behavior when using Execute. Default Manual.
        /// </summary>
        public static SqlConnectionBehavior ExecuteConnectionBehavior
        {
            get => _executeConnectionBehavior;
            set => _executeConnectionBehavior = value;
        }

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        public static SqlConnectionBehavior TransactionConnectionBehavior
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
        public static SqlConnection BuildConnection(SqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is null)
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
        public static SqlConnection BuildConnection(string connectionString)
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
        public static SqlConnection BuildConnection(string dataSource, string userID, string password, string catalog)
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
        public static SqlConnection BuildConnection(string server, uint port, string userID, string password, string catalog)
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
        public static string BuildConnectionString(SqlConnectionStringBuilder connectionStringBuilder)
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
        /// <param name="dataSource">Name or network address of the data server.</param>
        /// <param name="userID">User id.</param>
        /// <param name="password">Password.。</param>
        /// <param name="catalog">The name of the database associated with the connection.</param>
        /// <returns></returns>
        public static string BuildConnectionString(string dataSource, string userID, string password, string catalog)
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
        public static string BuildConnectionString(string server, uint port, string userID, string password, string catalog)
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, SqlConnection connection, string selectCommandText)
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
        public static int FillDataSet(DataSet dataSet, SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static int FillDataTable(DataTable dataTable, SqlConnection connection, string selectCommandText)
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
        public static int FillDataTable(DataTable dataTable, SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static SqlDataAdapter GetDataAdapter(SqlConnection connection, string selectCommandText)
        {
            return new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static SqlDataAdapter GetDataAdapter(SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            SqlDataAdapter result = new SqlDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { result.SelectCommand.Parameters.AddRange(parameters); }
            return result;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(SqlConnection connection, string selectCommandText)
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
        public static DataSet GetDataSet(SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="selectCommandText">Sql command.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(SqlConnection connection, string selectCommandText)
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
        public static DataTable GetDataTable(SqlConnection connection, string selectCommandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_dataAdapterConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static SqlDataReader GetDataReader(SqlConnection connection, string commandText)
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
        public static SqlDataReader GetDataReader(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            CommandBehavior commandBehavior;
            if (_dataReaderConnectionBehavior == SqlConnectionBehavior.Manual)
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
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static XmlReader GetXmlReader(SqlConnection connection, string commandText)
        {
            return GetXmlReader(connection, commandText, null);
        }

        /// <summary>
        /// Get XmlReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static XmlReader GetXmlReader(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// Execute the sql command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlConnection connection, string commandText)
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
        public static int ExecuteNonQuery(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        public static void ExecuteProcedure(SqlConnection connection, string procedure)
        {
            ExecuteProcedure(connection, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        public static void ExecuteProcedure(SqlConnection connection, string procedure, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// Execute the sql command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlConnection connection, string commandText)
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
        public static object ExecuteScalar(SqlConnection connection, string commandText, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_executeConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
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
        /// Execute the sql command by transaction. Auto rollback if failed. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql command.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(SqlConnection connection, string commandText)
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
        public static int TransactionExecuteNonQuery(SqlConnection connection, string commandText, IsolationLevel isolationLevel)
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
        public static int TransactionExecuteNonQuery(SqlConnection connection, string commandText, IsolationLevel isolationLevel, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_transactionConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SqlTransaction transaction = connection.BeginTransaction(isolationLevel))
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
        public static void TransactionExecuteProcedure(SqlConnection connection, string procedure)
        {
            TransactionExecuteProcedure(connection, procedure, IsolationLevel.ReadCommitted, null);
        }

        /// <summary>
        /// Executing stored procedure by transaction. Auto rollback if failed.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="isolationLevel">The transaction isolation level of the connection.</param>
        public static void TransactionExecuteProcedure(SqlConnection connection, string procedure, IsolationLevel isolationLevel)
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
        public static void TransactionExecuteProcedure(SqlConnection connection, string procedure, IsolationLevel isolationLevel, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_transactionConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SqlTransaction transaction = connection.BeginTransaction(isolationLevel))
            {
                using (SqlCommand command = connection.CreateCommand())
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
        public static object TransactionExecuteScalar(SqlConnection connection, string commandText)
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
        public static object TransactionExecuteScalar(SqlConnection connection, string commandText, IsolationLevel isolationLevel)
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
        public static object TransactionExecuteScalar(SqlConnection connection, string commandText, IsolationLevel isolationLevel, params SqlParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_transactionConnectionBehavior == SqlConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SqlTransaction transaction = connection.BeginTransaction(isolationLevel))
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
    public enum SqlConnectionBehavior
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
    public static class SqlCommandText
    {
        #region Engine

        /// <summary>
        /// Displays database files.
        /// </summary>
        public static string ShowAltFiles()
        {
            return "SELECT * FROM sysaltfiles";
        }

        /// <summary>
        /// Displays character set and collation.
        /// </summary>
        public static string ShowCharsets()
        {
            return "SELECT * FROM syscharsets";
        }

        /// <summary>
        /// Displays configuration options.
        /// </summary>
        public static string ShowConfigures()
        {
            return "SELECT * FROM sysconfigures";
        }

        /// <summary>
        /// Displays current config options.
        /// </summary>
        public static string ShowCurConfigs()
        {
            return "SELECT * FROM syscurconfigs";
        }

        /// <summary>
        /// Displays file groups.
        /// </summary>
        public static string ShowFileGroups()
        {
            return "SELECT * FROM sysfilegroups";
        }

        /// <summary>
        /// Displays file, sysfiles is a virtual table.
        /// </summary>
        public static string ShowFiles()
        {
            return "SELECT * FROM sysfiles";
        }

        /// <summary>
        /// Displays external Keywords.
        /// </summary>
        public static string ShowForeignKeys()
        {
            return "SELECT * FROM sysforeignkeys";
        }

        /// <summary>
        /// Displays languages.
        /// </summary>
        public static string ShowLanguages()
        {
            return "SELECT * FROM syslanguages";
        }

        /// <summary>
        /// Displays login account information.
        /// </summary>
        public static string ShowLogins()
        {
            return "SELECT * FROM syslogins";
        }

        /// <summary>
        /// Displays members.
        /// </summary>
        public static string ShowMembers()
        {
            return "SELECT * FROM sysmembers";
        }

        /// <summary>
        /// Displays objects of all types.
        /// </summary>
        public static string ShowObjects()
        {
            return "SELECT * FROM sysobjects;";
        }

        /// <summary>
        /// Displays server login information.
        /// </summary>
        public static string ShowOleDbUsers()
        {
            return "SELECT * FROM sysoledbusers";
        }

        /// <summary>
        /// Displays permissions.
        /// </summary>
        public static string ShowPermissions()
        {
            return "SELECT * FROM syspermissions;";
        }

        /// <summary>
        /// Displays processes.
        /// </summary>
        public static string ShowProcesses()
        {
            return "SELECT * FROM sysprocesses;";
        }

        /// <summary>
        /// Displays Remote login account.
        /// </summary>
        public static string ShowRemoteLogins()
        {
            return "SELECT * FROM sysremotelogins;";
        }

        /// <summary>
        /// Displays objects of the specified type.
        /// </summary>
        /// <param name="xType">
        /// <para>C = CHECK. D = DEFAULT. F = FOREIGN KEY. L = Log.</para>
        /// <para>FN = function non-vector. IF = Inlined table function. P = Procedure. PK = PRIMARY KEY.</para>
        /// <para>RF = Replication filter stored procedure. S = System table. TF = table function. TR = Trigger.</para>
        /// <para>U = User table. UQ = UNIQUE .V = View. X = Extended stored procedure.</para>
        /// </param>
        public static string ShowSysObject(string xType)
        {
            return "SELECT * FROM sysobjects WHERE xtype = N'" + xType + "';";
        }

        /// <summary>
        /// Displays system data types and user defined data types.
        /// </summary>
        public static string ShowTypes()
        {
            return "SELECT * FROM systypes;";
        }

        /// <summary>
        /// Displays users.
        /// </summary>
        public static string ShowUsers()
        {
            return "SELECT * FROM sysusers;";
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
            return "CREATE DATABASE [" + database + "];";
        }

        /// <summary>
        /// Deletes the database using the current connection.
        /// </summary>
        /// <param name="database">The name of the database to delete.</param>
        /// <returns></returns>
        public static string DropDatabase(string database)
        {
            return "IF EXISTS (SELECT name FROM sysdatabases WHERE name = N'" + database + "') DROP DATABASE [" + database + "];";
        }

        /// <summary>
        /// Displays all visible database names.
        /// </summary>
        /// <returns></returns>
        public static string ShowDatabases()
        {
            return "SELECT * FROM sysdatabases;";
        }

        #endregion Database

        #region Table

        /// <summary>
        /// Displays all types of columns.
        /// </summary>
        public static string ShowColumns()
        {
            return "SELECT * FROM syscolumns;";
        }

        /// <summary>
        /// Displays the columns of the specified type, and the data types provided by the current data server or data source can be queried through the systypes table.
        /// </summary>
        /// <param name="xType">syscolumns xtype。</param>
        public static string ShowColumns(int xType)
        {
            return "SELECT * FROM syscolumns WHERE xtype = " + xType + ";";
        }

        /// <summary>
        /// Displays the creation script for the stored procedure with the specified name.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns></returns>
        public static string ShowCreateProcedure(string procedure)
        {
            return "EXEC sp_helptext N'" + procedure + "';";
        }

        /// <summary>
        /// Displays basic information about the specified kind of stored procedure.
        /// </summary>
        /// <returns></returns>
        /// <param name="category">Set to 0 to represent the user stored procedure.</param>
        public static string ShowProcedures(int category)
        {
            return "SELECT * FROM sysobjects WHERE xtype= N'P' AND category = " + category + ";";
        }

        /// <summary>
        /// Display basic information for all stored procedures.
        /// </summary>
        /// <returns></returns>
        public static string ShowProcedures()
        {
            return "SELECT * FROM sysobjects WHERE xtype= N'P';";
        }

        #endregion Table
    }

    #endregion CommandText
}