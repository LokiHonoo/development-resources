/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Honoo.Data
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
        public static int FillDataSet(DataSet dataSet, SQLiteConnection connection, string selectCommandText)
        {
            return FillDataSet(dataSet, connection, selectCommandText, null);
        }

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int FillDataSet(DataSet dataSet, SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static int FillDataTable(DataTable dataTable, SQLiteConnection connection, string selectCommandText)
        {
            return FillDataTable(dataTable, connection, selectCommandText, null);
        }

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int FillDataTable(DataTable dataTable, SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static SQLiteDataAdapter GetDataAdapter(SQLiteConnection connection, string selectCommandText)
        {
            return new SQLiteDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
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
        public static DataSet GetDataSet(SQLiteConnection connection, string selectCommandText)
        {
            return GetDataSet(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static DataSet GetDataSet(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static DataTable GetDataTable(SQLiteConnection connection, string selectCommandText)
        {
            return GetDataTable(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(SQLiteConnection connection, string selectCommandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static SQLiteDataReader GetDataReader(SQLiteConnection connection, string commandText)
        {
            return GetDataReader(connection, commandText, null);
        }

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static SQLiteDataReader GetDataReader(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static int ExecuteNonQuery(SQLiteConnection connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, null);
        }

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static void ExecuteProcedure(SQLiteConnection connection, string procedure)
        {
            ExecuteProcedure(connection, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        public static void ExecuteProcedure(SQLiteConnection connection, string procedure, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static object ExecuteScalar(SQLiteConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, null);
        }

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static object ExecuteScalar(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
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
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, string commandText)
        {
            return TransactionExecuteNonQuery(connection, commandText, null);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static int TransactionExecuteNonQuery(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (TransactionConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SQLiteCommand command = connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("BEGIN TRANSACTION;");
                sql.Append(commandText);
                sql.AppendLine(";");
                sql.AppendLine("COMMIT;");
                command.CommandText = sql.ToString();
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                try
                {
                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    using (SQLiteCommand rollback = connection.CreateCommand())
                    {
                        command.CommandText = "ROLLBACK;";
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    exception = ex;
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
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        public static void TransactionExecuteProcedure(SQLiteConnection connection, string procedure)
        {
            TransactionExecuteProcedure(connection, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        public static void TransactionExecuteProcedure(SQLiteConnection connection, string procedure, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("BEGIN TRANSACTION;");
                sql.Append(procedure);
                sql.AppendLine(";");
                sql.AppendLine("COMMIT;");
                command.CommandText = sql.ToString();
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    using (SQLiteCommand rollback = connection.CreateCommand())
                    {
                        command.CommandText = "ROLLBACK;";
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    exception = ex;
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
        public static object TransactionExecuteScalar(SQLiteConnection connection, string commandText)
        {
            return TransactionExecuteScalar(connection, commandText, null);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        public static object TransactionExecuteScalar(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (TransactionConnectionBehavior == SQLiteConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (SQLiteCommand command = connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("BEGIN TRANSACTION;");
                sql.Append(commandText);
                sql.AppendLine(";");
                sql.AppendLine("COMMIT;");
                command.CommandText = sql.ToString();
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                try
                {
                    result = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    using (SQLiteCommand rollback = connection.CreateCommand())
                    {
                        command.CommandText = "ROLLBACK;";
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    exception = ex;
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

        [SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        private static string BuildSummary(SQLiteConnection connection,
                                           SQLiteDumpManifest manifest,
                                           out long tableCount,
                                           out long viewCount,
                                           out long triggerCount,
                                           out long recordCount,
                                           out long total)
        {
            StringBuilder tmp = new StringBuilder();
            tableCount = 0;
            viewCount = 0;
            triggerCount = 0;
            recordCount = 0;
            List<string> union = new List<string>();
            foreach (SQLiteTableDumpProject table in manifest.Tables)
            {
                if (!table.Ignore)
                {
                    if (table.IncludingRecord)
                    {
                        union.Add("SELECT COUNT(*) AS `count` FROM `" + table.TableName + "`");
                    }
                    tableCount++;
                }
            }
            if (union.Count > 0)
            {
                using (DataTable dt = GetDataTable(connection, string.Join(" UNION ALL ", union) + ";"))
                {
                    recordCount = long.Parse(dt.Compute("SUM(count)", string.Empty).ToString(), CultureInfo.InvariantCulture);
                }
            }
            foreach (SQLiteDumpProject view in manifest.Views)
            {
                if (!view.Ignore)
                {
                    viewCount++;
                }
            }
            foreach (SQLiteDumpProject trigger in manifest.Triggers)
            {
                if (!trigger.Ignore)
                {
                    triggerCount++;
                }
            }
            total = tableCount + viewCount + triggerCount + recordCount;
            //
            tmp.AppendLine("/*");
            tmp.AppendLine("Dump by Honoo.Data.SQLiteHelper");
            tmp.AppendLine("https://github.com/LokiHonoo/development-resources");
            tmp.AppendLine("This code page is published under the terms of the MIT license.");
            tmp.AppendLine();
            tmp.AppendLine("DataSource     : " + connection.DataSource);
            tmp.AppendLine("Server Version : " + connection.ServerVersion);
            tmp.AppendLine("Database       : " + connection.Database);
            tmp.AppendLine();
            tmp.AppendLine("Table          : " + tableCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine("Trigger        : " + triggerCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine("View           : " + viewCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine("Record         : " + recordCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine();
            tmp.AppendLine("Dump Time      : " + DateTime.Now);
            tmp.AppendLine();
            tmp.AppendLine("Use the console or database tool to recover data.");
            tmp.AppendLine("If the target database has a table with the same name, the table data is overwritten.");
            tmp.AppendLine("*/");
            tmp.AppendLine();
            return tmp.ToString();
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
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
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
                    recordCount = long.Parse(dt.Compute("SUM(count)", string.Empty).ToString(), CultureInfo.InvariantCulture);
                    total += recordCount;
                }
            }
            total += setting.Tables.Count;
            total += setting.Views.Count;
            total += setting.Triggers.Count;
            //
            bool cancel = false;
            StringBuilder tmp = new StringBuilder();
            written?.Invoke(index, total, SQLiteDumpType.Summary, string.Empty, userState, ref cancel);
            if (cancel) { goto end; }
            //
            tmp.AppendLine("/*");
            tmp.AppendLine("Dump by Honoo.Data.SQLiteHelper");
            tmp.AppendLine();
            tmp.AppendLine("DataSource     : " + connection.DataSource);
            tmp.AppendLine("Server Version : " + connection.ServerVersion);
            tmp.AppendLine("Database       : " + connection.Database);
            tmp.AppendLine();
            tmp.AppendLine("Table          : " + setting.Tables.Count.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine("Record         : " + recordCount.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine("View           : " + setting.Views.Count.ToString("n0", CultureInfo.InvariantCulture));
            tmp.AppendLine("Trigger        : " + setting.Triggers.Count.ToString("n0", CultureInfo.InvariantCulture));
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
                    written?.Invoke(index, total, SQLiteDumpType.Table, table.TableName, userState, ref cancel);
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
                    written?.Invoke(index, total, SQLiteDumpType.Trigger, trigger, userState, ref cancel);
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
                    written?.Invoke(index, total, SQLiteDumpType.View, view, userState, ref cancel);
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
                                                written?.Invoke(index, total, SQLiteDumpType.Record, table.TableName, userState, ref cancel);
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
                                        tmp.Append('(');
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
                                                    case int value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                    case double value: tmp.Append(value.ToString(CultureInfo.InvariantCulture)); break;
                                                    default: tmp.Append("'" + val.ToString() + "'"); break;
                                                }
                                            }
                                            if (i < reader.FieldCount - 1)
                                            {
                                                tmp.Append(',');
                                            }
                                        }
                                        tmp.AppendLine(");");
                                        textWriter.Write(tmp.ToString());
                                        tmp.Clear();
                                        index++;
                                        written?.Invoke(index, total, SQLiteDumpType.Record, table.TableName, userState, ref cancel);
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
        /// Get dump manifest.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <returns></returns>
        public static SQLiteDumpManifest GetDumpManifest(SQLiteConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            SQLiteDumpManifest manifest = new SQLiteDumpManifest();
            using (DataTable dt = GetDataTable(connection, "SELECT type, name FROM `sqlite_master`;"))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string type = (string)dr["type"];
                        if (type == "table")
                        {
                            manifest.Tables.Add(new SQLiteTableDumpProject((string)dr["name"], false, 0, true));
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
        public List<SQLiteTableDumpProject> Tables { get; } = new List<SQLiteTableDumpProject>();

        /// <summary>
        /// Triggers dump project.
        /// </summary>
        public List<SQLiteDumpProject> Triggers { get; } = new List<SQLiteDumpProject>();

        /// <summary>
        /// Views dump project.
        /// </summary>
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
        internal SQLiteDumpProject(string name, bool ignore)
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
    public sealed class SQLiteTableDumpProject
    {
        /// <summary>
        /// Table dump project.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="ignore">Ignore this project.</param>
        /// <param name="sequence">Reset Sequence=value. Set to 0 without changing.</param>
        /// <param name="includingRecord">Dump records.</param>
        internal SQLiteTableDumpProject(string tableName, bool ignore, int? sequence, bool includingRecord)
        {
            this.TableName = tableName;
            this.Ignore = ignore;
            this.Sequence = sequence;
            this.IncludingRecord = includingRecord;
        }

        /// <summary>
        /// Reset Sequence=value. Set to 0 without changing.
        /// </summary>
        public int? Sequence { get; }

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
            return "SELECT SQLITE_VERSION();";
        }

        #endregion Database

        #region Table

        /// <summary>
        /// Displays tables in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTables()
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'table';";
        }

        /// <summary>
        /// Displays tables in the current database.
        /// </summary>
        /// <param name="like">Part of the table name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTables(string like)
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'table' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowTriggers()
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'trigger';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like)
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'trigger' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays triggers in the current database.
        /// </summary>
        /// <param name="like">Part of the trigger name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns></returns>
        public static string ShowTriggers(string like, string table)
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'trigger' AND `tbl_name` = '" + table + "' AND `name` LIKE '" + like + "';";
        }

        /// <summary>
        /// Displays views in the current database.
        /// </summary>
        /// <returns></returns>
        public static string ShowViews()
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'view';";
        }

        /// <summary>
        /// Displays views in the current database.
        /// </summary>
        /// <param name="like">Part of the view name. Follow the syntax rules for the "LIKE" keyword.</param>
        /// <returns></returns>
        public static string ShowViews(string like)
        {
            return "SELECT * FROM `sqlite_master` WHERE `type` = 'view' AND `name` LIKE '" + like + "';";
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