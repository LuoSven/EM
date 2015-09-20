using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Text;

namespace EM.Data.Dapper
{
    public static class DapperHelper
    {

        public static void InitDapperTableNameMapper(Func<Type, string> funcTableNameGetter)
        {
            SqlMapperExtensions.TableNameMapper += new SqlMapperExtensions.TableNameMapperDelegate(funcTableNameGetter);
        }

        public static SqlConnection GetOpenConnection(string connectionStr, bool mars = false)
        {
            if (mars)
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(connectionStr);
                scsb.MultipleActiveResultSets = true;
                connectionStr = scsb.ConnectionString;
            }
            var connection = new SqlConnection(connectionStr);
            connection.Open();
            return connection;
        }

        public static SqlConnection Get22Connection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DapperConnection"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        #region 查询方法
        public static async Task<IEnumerable<T>> SqlQuery22Async<T>(string Sql, object param = null)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = Sql;
                var list = await conn.QueryAsync<T>(sql, param);
                return list;
            }
        }
        public static IEnumerable<T> SqlQuery22<T>(string Sql, object param = null)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = Sql;
                var list = conn.Query<T>(sql, param);
                return list;
            }
        }
        #endregion

        #region 执行方法

        #region 同步
        public static int SqlExecute22(string Sql, object param = null)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = Sql;
                var result = conn.Execute(sql, param);
                return result;
            }
        }

        /// <summary>
        /// 运用事务处理系列sql,有一句失败则全部回滚
        /// </summary>
        /// <param name="Sqls"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int[] SqlExecute22WithTransaction(List<Tuple<string, object>> Sqls)//string Sql, object param = null
        {
            if (Sqls.Count != 0)
                using (var conn = DapperHelper.Get22Connection())
                {
                    var trans = conn.BeginTransaction();
                    var FirstSql = Sqls[0].Item1;
                    var Firstparam = Sqls[0].Item2;
                    int[] result = new int[Sqls.Count];
                    result[0] = conn.Execute(FirstSql, Firstparam, trans);
                    for (int i = 1; i < Sqls.Count; i++)
                    {
                        result[i] = conn.Execute(Sqls[i].Item1, Sqls[i].Item2, trans);
                    }
                    if (result.AsList().Contains(0))
                        trans.Rollback();
                    else
                        trans.Commit();
                    return result;
                }
            return null;
        }



        public static bool Update<T>(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
            where T : class
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Update(entityToUpdate, transaction, commandTimeout);
                return result;
            }
        }



        public static int Inert<T>(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
            where T : class
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Insert(entityToUpdate, transaction, commandTimeout);
                return (int)result;
            }
        }

        #endregion

        #region 异步

        public static async Task<int> SqlExecute22Async(string Sql, object param = null)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = Sql;
                var result = await conn.ExecuteAsync(sql, param);
                return result;
            }
        }


        public static async Task<int> InsertAsync<T>(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
    where T : class
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = await conn.InsertAsync(entityToUpdate, transaction, commandTimeout);
                return (int)result;
            }
        }

        public static   async Task<bool> UpdateAsync<T>(T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
     where T : class
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result =await conn.UpdateAsync(entityToUpdate, transaction, commandTimeout);
                return result;
            }
        }
        /// <summary>
        /// 运用事务处理系列sql,有一句失败则全部回滚
        /// </summary>
        /// <param name="Sqls"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<int[]> SqlExecute22AsyncWithTransaction(List<Tuple<string, object>> Sqls)//string Sql, object param = null
        {
            if (Sqls.Count != 0)
                using (var conn = DapperHelper.Get22Connection())
                {
                    var trans = conn.BeginTransaction();
                    var FirstSql = Sqls[0].Item1;
                    var Firstparam = Sqls[0].Item2;
                    int[] result = new int[Sqls.Count];
                    result[0] = await conn.ExecuteAsync(FirstSql, Firstparam, trans);
                    for (int i = 1; i < Sqls.Count; i++)
                    {
                        result[i] = await conn.ExecuteAsync(Sqls[i].Item1, Sqls[i].Item2, trans);
                    }
                    if (result.AsList().Contains(0))
                        trans.Rollback();
                    else
                        trans.Commit();
                    return result;
                }
            return null;
        }
        #endregion

        #endregion

        #region 工具
        public static string CheckParams(string Params)
        {
            if (Params == null)
                return "";
            string[] checkParams = { "'", "\"", "&", "<", };
            var Olength = Params.Length;
            for (int i = 0; i < checkParams.Length; i++)
            {
                Params = Params.Replace(checkParams[i], "");
            }
            return Params;
        }

        /// 对字符串进行sql格式化，并且符合like查询的格式。 
        /// </summary> 
        /// <param name="str">要转换的字符串</param> 
        /// <returns>格式化后的字符串</returns> 
        public static string ToSqlLink(this string sqlstr)
        {
            if (sqlstr == null) return "";
            StringBuilder str = new StringBuilder(sqlstr);
            str.Replace("'", "''");
            str.Replace("[", "[[]");
            str.Replace("%", "[%]");
            str.Replace("_", "[_]");
            return str.ToString();
        }
        #endregion

    }
}
