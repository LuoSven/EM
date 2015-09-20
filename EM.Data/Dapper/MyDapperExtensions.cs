using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using Dapper;
using EM.Data;

namespace EM.Data.Dapper
{
    public static class MyDapperExtensions
    {
        public static async Task<PagedResult<T>> QueryWithPageAsync<T>(this IDbConnection conn, string sql, object param, string orders, int page, int pageSize) where T : class
        {
            try
            {

                int startIndex = (page - 1) * pageSize + 1;
                int endIndex = page * pageSize;
                string pagedSql = string.Format("with queryData as ({0}) " +

"SELECT  * " +
"FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY {1}) AS RowNum, * " +
  "        FROM      queryData " +
   "     ) AS result " +
"WHERE   RowNum >= {2} " +
"    AND RowNum <= {3}  " +
"ORDER BY RowNum", sql, orders, startIndex, endIndex);


                string countSql = string.Format("with queryData as ({0})  select count(1) from queryData", sql);
                int count = conn.QueryAsync<int>(countSql, param).Result.First();

                DynamicParameters dynParms = new DynamicParameters();
                dynParms.AddDynamicParams(param);
                var list = await conn.QueryAsync<T>(pagedSql, param);
                return list.ToPagedList(page, pageSize, count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static PagedResult<T> QueryWithPage<T>(this IDbConnection conn, string sql, object param, string orders, int page, int pageSize) where T : class
        {
            try
            {

                int startIndex = (page - 1) * pageSize + 1;
                int endIndex = page * pageSize;
                string pagedSql = string.Format("with queryData as ({0}) " +

"SELECT  * " +
"FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY {1}) AS RowNum, * " +
  "        FROM      queryData " +
   "     ) AS result " +
"WHERE   RowNum >= {2} " +
"    AND RowNum <= {3}  " +
"ORDER BY RowNum", sql, orders, startIndex, endIndex);


                string countSql = string.Format("with queryData as ({0})  select count(1) from queryData", sql);
                int count = conn.QueryAsync<int>(countSql, param).Result.First();

                DynamicParameters dynParms = new DynamicParameters();
                dynParms.AddDynamicParams(param);
                var list = conn.Query<T>(pagedSql, param);
                return list.ToPagedList(page, pageSize, count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
