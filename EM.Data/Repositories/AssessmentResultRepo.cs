using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using Topuc22Top.Model.DTOs;

namespace Topuc22Top.Data.Repositories
{
    public class AssessmentResultRepo : RepositoryBase<Assessment_Result>, IAssessmentResultRepo
    {
        private readonly ICache cache;
        public AssessmentResultRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public int GetCount()
        {
            return DataContext.Assessment_Result.Count();
        }
        public Assessment_Result GetLastestResult(int UserId)
        {
            var result = Dapper.DapperHelper.SqlQuery22<Assessment_Result>(@"select top(1) a.ResultId,a.AssessmentId ,a.Result,a.Adjust,a.ResultType,a.ResultAnalysis,a.CreateDate,a.ResultQIds from Assessment_Result  a
where UserId=@UserId
order by a.CreateDate desc ", new { UserId = UserId });
            return  result.FirstOrDefault();
        }

        public string GetFunctionsByResultId(int ResultId)
        {
            var Functions = Dapper.DapperHelper.SqlQuery22<string>(@"select b.Functions from Assessment_Result a
 join Assessment_ResultType b  on  a.ResultType=b.TypeName
 where a.ResultId=@ResultId", new { ResultId = ResultId }).FirstOrDefault();
            return Functions ?? "";
        }
    }

    public interface IAssessmentResultRepo : IRepository<Assessment_Result>
    {
        int GetCount();

        Assessment_Result GetLastestResult(int UserId);

        string GetFunctionsByResultId(int ResultId);
    }
}
