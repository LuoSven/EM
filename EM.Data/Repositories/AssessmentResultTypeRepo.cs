using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class AssessmentResultTypeRepo: RepositoryBase<Assessment_ResultType>, IAssessmentResultTypeRepo
    {
        private readonly ICache cache;
        public AssessmentResultTypeRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }


        public string GetDesc(string resultType)
        {
            string analysis = string.Empty;
            var query = from it in DataContext.Assessment_ResultType
                        where it.TypeName == resultType
                        select it.Description;
            if (query.Any())
            {
                analysis = query.FirstOrDefault();
            }
            return analysis;
        }


        public string GetFunctions(string type)
        {
            var query = from it in DataContext.Assessment_ResultType
                        where it.TypeName == type
                        select it.Functions;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return string.Empty;
        }

        #region Dapper
        public async Task<object> GetFunctionIdNamePairs(string type)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select ItemId,ItemName from DictItem
where Type = 'Function'
and ItemId in (select Item from dbo.SplitToStrArray((select top 1 Functions from Assessment_ResultType where TypeName = @typeName),','))
");
                var list = await conn.QueryAsync<FunctionIdNamePairs_TempClass>(sql, new { @typeName = type });
                return list.ToList();
            }
        }

        class FunctionIdNamePairs_TempClass 
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
        }
        #endregion

    }

    public interface IAssessmentResultTypeRepo : IRepository<Assessment_ResultType>
    {
        string GetDesc(string resultType);

        string GetFunctions(string type);

        #region Dapper
        Task<object> GetFunctionIdNamePairs(string type);
        #endregion

    }
}
