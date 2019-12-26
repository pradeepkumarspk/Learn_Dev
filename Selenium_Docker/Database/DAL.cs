using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Selenium_Docker.Database
{

    public class DAL
    {
        protected internal string _DSN = "";
        private bool _Debug = false;

        private string _DebugFile = "";
        public DAL(string dsn, bool debug = false, string debugFile = "")
        {
            this._Debug = debug;
            //dynamic DSN = "";
            this._DSN = dsn;
            this._DebugFile = debugFile;
        }

        public void ExecuteSQL(string sql)
        {
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public void ExecuteSQL(string sql, params SqlParameter[] param)
        {
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    if (param.Length > 0)
                        cmd.Parameters.AddRange(param);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        public void ExecuteCommand(SqlCommand cmd)
        {
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public bool ExecuteSQLWithTransaction(string sql)
        {
            bool success = false;
            SqlTransaction transaction = null;

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    try
                    {
                        con.Open();
                        transaction = con.BeginTransaction();
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        success = true;
                    }
                    catch (System.Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                    finally
                    {
                        transaction.Dispose();
                        con.Close();
                    }
                }
            }

            return success;
        }

        #region "ExecuteSQLSelect"

        public DataSet ExecuteSQLSelect(string sql)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        con.Open();
                        ds = new DataSet();
                        da.Fill(ds);
                        con.Close();
                    }
                }
            }
            return ds;
        }

        public List<T> ExecuteSQLSelect<T>(string sql) where T : class,new()
        {
            List<T> list = new List<T>();
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            list = DataReaderToList<T>(reader);
                        }
                    }
                }
            }
            return list;
        }

        public List<T> ExecuteSQLSelect<T>(SqlCommand cmd) where T : class,new()
        {
            List<T> list = new List<T>();
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        list = DataReaderToList<T>(reader);
                    }
                }
                con.Close();
            }
            return list;
        }

        public DataSet ExecuteSQLSelect(string sql, List<System.Data.SqlClient.SqlParameter> @params)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            con.Open();
                            cmd.Parameters.AddRange(@params.ToArray());
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            return ds;
        }

        public DataSet ExecuteSQLSelect(SqlCommand cmd)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Connection = con;
                    con.Open();
                    ds = new DataSet();
                    da.Fill(ds);
                    con.Close();
                }
            }

            return ds;
        }

        #endregion

        public DataTable ExecuteSQLSelectWithFill(SqlCommand cmd, string tableName)
        {

            DataTable dtResult = null;
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    cmd.Connection = con;
                    con.Open();
                    ds = new DataSet();
                    da.Fill(ds, tableName);

                    if (!ds.Tables.Contains(tableName))
                    {
                        DataTable[] dt = da.FillSchema(ds, SchemaType.Source);
                        dt[0].TableName = tableName;
                        dtResult = dt[0];
                    }
                    else
                    {
                        dtResult = ds.Tables[tableName];
                    }
                    con.Close();
                }
            }

            return dtResult;
        }

        public List<T> ExecuteSQLSelectWithFill<T>(SqlCommand cmd) where T : class,new()
        {
            List<T> list = new List<T>();

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                cmd.Connection = con;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        list = DataReaderToList<T>(reader);
                    }
                }
            }

            return list;
        }

        public List<T> ExecuteSQLSelectWithFill<T>(String sql) where T : class,new()
        {
            List<T> list = new List<T>();

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            list = DataReaderToList<T>(reader);
                        }
                    }
                    con.Close();
                }
            }

            return list;
        }

        public void ExecuteSQLBatch(List<string> sqlBatch)
        {
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    con.Open();
                    foreach (string Sql in sqlBatch)
                    {
                        cmd.CommandText = Sql;
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
        }

        public void ExecuteStoredProcedure(string sprocName, ref List<System.Data.SqlClient.SqlParameter> @params)
        {
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sprocName, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(@params.ToArray());
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }

                }
            }
        }

        public object ExecuteStoredProcedureScalar(string sprocName, List<System.Data.SqlClient.SqlParameter> @params)
        {
            Object returnValue = null;
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sprocName, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(@params.ToArray());
                        con.Open();
                        returnValue = cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }

                }
            }
            return returnValue;
        }

        public void ExecuteStoredProcedureWithTransaction(string sprocName, ref List<System.Data.SqlClient.SqlParameter> @params)
        {
            SqlTransaction transaction = null;
            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sprocName, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(@params.ToArray());
                        con.Open();
                        transaction = con.BeginTransaction();
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        //public void ExecuteStoredProcedureWithTransactionScope(ref List<CommandsWithParams> transactionScopeRequests)
        //{
        //    try
        //    {
        //        using (var scopeTransaction = new TransactionScope(TransactionScopeOption.Required,
        //            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromSeconds(60) }))
        //        {
        //            using (SqlConnection con = new SqlConnection(_DSN))
        //            {
        //                con.Open();
        //                foreach (CommandsWithParams request in transactionScopeRequests)
        //                {
        //                    using (SqlCommand cmd = new SqlCommand(request.CommandText, con))
        //                    {
        //                        cmd.CommandType = request.CommandType;
        //                        cmd.Parameters.AddRange(request.CommandParams.ToArray());
        //                        cmd.ExecuteNonQuery();
        //                    }
        //                }
        //            }
        //            scopeTransaction.Complete();
        //        }
        //    }
        //    catch (TransactionAbortedException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<T> ExecuteStoredProcedureWithFill<T>(string sprocName, List<System.Data.SqlClient.SqlParameter> @params) where T : class,new()
        {
            List<T> list = new List<T>();

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sprocName, con))
                {
                    try
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(@params.ToArray());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                list = DataReaderToList<T>(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Source = sprocName + ex.Source;
                        throw ex;
                    }
                }
            }

            return list;
        }

        public List<T> ExecuteStoredProcedureWithFill<T>(string sprocName, List<System.Data.SqlClient.SqlParameter> @params,
            TransactionOptions transactionOptions) where T : class,new()
        {
            List<T> list = new List<T>();
            using (var scopetransaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection con = new SqlConnection(_DSN))
                {
                    using (SqlCommand cmd = new SqlCommand(sprocName, con))
                    {
                        con.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(@params.ToArray());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                list = DataReaderToList<T>(reader);
                            }
                        }
                    }
                }
                scopetransaction.Complete();
            }
            return list;
        }

        public Tuple<List<T1>, List<T2>, List<T3>> ExecuteStoredProcedureWithFill<T1, T2, T3>(string sprocName, List<System.Data.SqlClient.SqlParameter> @params)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            var resultSet1 = new List<T1>();
            var resultSet2 = new List<T2>();
            var resultSet3 = new List<T3>();

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sprocName, con))
                {

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(@params.ToArray());
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            resultSet1 = DataReaderToList<T1>(reader);
                            reader.NextResult();
                            resultSet2 = DataReaderToList<T2>(reader);
                            reader.NextResult();
                            resultSet3 = DataReaderToList<T3>(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }

                }
            }

            return Tuple.Create<List<T1>, List<T2>, List<T3>>(resultSet1, resultSet2, resultSet3);
        }

        public Tuple<List<T1>, List<T2>> ExecuteStoredProcedureWithFill<T1, T2>(string sprocName, List<System.Data.SqlClient.SqlParameter> @params)
            where T1 : class, new()
            where T2 : class, new()
        {
            var resultSet1 = new List<T1>();
            var resultSet2 = new List<T2>();

            using (SqlConnection con = new SqlConnection(_DSN))
            {
                using (SqlCommand cmd = new SqlCommand(sprocName, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddRange(@params.ToArray());
                            con.Open();
                            using (var reader = cmd.ExecuteReader())
                            {
                                resultSet1 = DataReaderToList<T1>(reader);
                                reader.NextResult();
                                resultSet2 = DataReaderToList<T2>(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }

            return Tuple.Create<List<T1>, List<T2>>(resultSet1, resultSet2);
        }

        public void ExecuteUpdate(SqlCommand selectCmd, SqlCommand insertCmd, SqlCommand updateCmd, SqlCommand deleteCmd, DataTable objectData)
        {
            using (SqlConnection thisDB = new SqlConnection(_DSN))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = selectCmd;
                    da.SelectCommand.Connection = thisDB;
                    da.InsertCommand = insertCmd;
                    da.InsertCommand.Connection = thisDB;
                    da.DeleteCommand = deleteCmd;
                    da.DeleteCommand.Connection = thisDB;
                    da.UpdateCommand = updateCmd;
                    da.UpdateCommand.Connection = thisDB;

                    thisDB.Open();
                    da.Update(objectData);
                    objectData.AcceptChanges();
                }
            }
        }

        private static List<T> DataReaderToList<T>(IDataReader dr) where T : class,new()
        {
            List<T> list = new List<T>();
            T obj = default(T);
            var hashColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow schemaRow in dr.GetSchemaTable().Rows)
                hashColumns.Add(schemaRow["ColumnName"].ToString());

            while (dr.Read())
            {
                obj = new T();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (hashColumns.Contains(prop.Name) && !object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        private static bool ColumnExists(IDataReader reader, string columnName)
        {
            return reader.GetSchemaTable()
                         .Rows
                         .OfType<DataRow>()
                         .Any(row => row["ColumnName"].ToString().Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    //public class CommandsWithParams : ICommandsWithParams
    //{
    //    public string CommandText { get; set; }
    //    public CommandType CommandType { get; set; }
    //    public List<IDbDataParameter> CommandParams { get; set; }
    //}
}