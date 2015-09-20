using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.DTOs;
using System;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyHomeRecommendRepo : RepositoryBase<CompanyHomeRecommendDTO>, ICompanyHomeRecommendRepo
    {
        public CompanyHomeRecommendRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public async Task<CompanyHomeRecommendDTO> GetById(int Id)
        {
            var result = await DapperHelper.SqlQuery22Async<CompanyHomeRecommendDTO>(@"select a.RecommendId,a.EnterpriseId, a.EnterpriseName ,a.Title,a.City1,a.Position1,a.PositionId1,a.City2,a.Position2,a.PositionId2,a.Sort,a.CreateTime from TB_Enterprise_HomeRecommend a
 where a.RecommendId=@Id", new { Id = Id });
            return result.FirstOrDefault();
        }
        public async Task<CompanyHomeRecommendDTO> GetByEtpId(int Id)
        {
            var result = await DapperHelper.SqlQuery22Async<CompanyHomeRecommendDTO>(@"select a.RecommendId,a.EnterpriseId, a.EnterpriseName ,a.Title,a.City1,a.Position1,a.PositionId1,a.City2,a.Position2,a.PositionId2,a.Sort,a.CreateTime from TB_Enterprise_HomeRecommend a
 where a.EnterpriseId=@Id", new { Id = Id });
            return result.FirstOrDefault();
        }
        public async Task<IList<CompanyHomeRecommendDTO>> GetList()
        {
            var query = await DapperHelper.SqlQuery22Async<CompanyHomeRecommendDTO>(@"select a.RecommendId,a.EnterpriseId, a.EnterpriseName ,a.Title,a.City1,a.Position1,a.PositionId1,a.City2,a.Position2,a.PositionId2,a.Sort,a.CreateTime ,case when  b.CertificationStatus=3 then 1 else 0 end as IsCert from TB_Enterprise_HomeRecommend a
join TB_Enterprise b on a.EnterpriseId=b.EnterpriseId
order by a.Sort  ");
            return query.ToList();
        }
        public bool DeleteHomeRecommend(int Id)
        {
            return  DapperHelper.SqlExecute22(@"delete from TB_Enterprise_HomeRecommend where RecommendId=@Id", new { Id = Id }) > 0 ? true : false;
        }
        public bool AddHomeRecommend(CompanyHomeRecommendDTO HomeRecommend)
        {
            HomeRecommend.CreateTime = DateTime.Now;
            HomeRecommend.ModifyTime = DateTime.Now;
            return DapperHelper.SqlExecute22(@"INSERT INTO [dbo].[TB_Enterprise_HomeRecommend]
           ([EnterpriseId]
           ,[EnterpriseName]
           ,[Title]
           ,[City1]
           ,[Position1]
           ,[PositionId1]
           ,[City2]
           ,[Position2]
           ,[PositionId2]
           ,[Sort]
           ,[CreateTime]
           ,[ModifyTime])
     VALUES
           ( @EnterpriseId
      , @EnterpriseName
      , @Title
      , @City1
      , @Position1
      , @PositionId1
      ,@City2
      ,@Position2
      , @PositionId2
      , @Sort
      , @CreateTime
      , @ModifyTime)", HomeRecommend) > 0 ? true : false;
        }
        public bool UpdateHomeRecommend(CompanyHomeRecommendDTO HomeRecommend)
        {
            HomeRecommend.ModifyTime = DateTime.Now;
            return DapperHelper.SqlExecute22(@"UPDATE TB_Enterprise_HomeRecommend
   SET EnterpriseId = @EnterpriseId
      ,EnterpriseName = @EnterpriseName
      ,Title = @Title
      ,City1 = @City1
      ,Position1 = @Position1
      ,PositionId1 = @PositionId1
      ,City2 = @City2
      ,Position2 = @Position2
      ,PositionId2 = @PositionId2
      ,Sort = @Sort
      ,CreateTime = @CreateTime
      ,ModifyTime = @ModifyTime
 WHERE  RecommendId=@RecommendId
", HomeRecommend) > 0 ? true : false;
        }

        public async Task<  CompanyHomeRecommendDTO >GetBySort(decimal sort)
        {
            var result = await DapperHelper.SqlQuery22Async<CompanyHomeRecommendDTO>(@"select a.RecommendId,a.EnterpriseId from TB_Enterprise_HomeRecommend a
 where a.Sort=@Id", new { Id =  sort.ToString("0.0") });
            return result.FirstOrDefault();
        }


    }
    public interface ICompanyHomeRecommendRepo : IRepository<CompanyHomeRecommendDTO>
    {
        Task<CompanyHomeRecommendDTO> GetById(int Id);
        Task<CompanyHomeRecommendDTO> GetByEtpId(int Id);
        Task<IList<CompanyHomeRecommendDTO>> GetList();
        bool DeleteHomeRecommend(int Id);

        bool AddHomeRecommend(CompanyHomeRecommendDTO HomeRecommend);

        bool UpdateHomeRecommend(CompanyHomeRecommendDTO HomeRecommend);
        Task<CompanyHomeRecommendDTO >GetBySort(decimal sort);
    }

}
