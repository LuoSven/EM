using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;

using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyLifeRepo : RepositoryBase<TB_Enterprise_Life>, ICompanyLifeRepo
    {
        public CompanyLifeRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IList<TB_Enterprise_Life> GetList(int enterpriseId)
        {
            var query = from m in DataContext.TB_Enterprise_Life
                        where m.EnterpriseId == enterpriseId
                        select m;
            return query.OrderByDescending(x => x.CreateTime).ToList();
        }

        public PagedResult<TB_Enterprise_Life> GetList(int enterpriseId, int page, int pageSize)
        {
            var query = from m in DataContext.TB_Enterprise_Life
                        where m.EnterpriseId == enterpriseId
                        select m;
            int count = query.Count();
            return new PagedResult<TB_Enterprise_Life>()
            {
                Results = query.OrderByDescending(x => x.CreateTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList(),
                PageSize = pageSize,
                CurrentPage = page,
                RowCount = count
            };
        }

        public Tuple<int, int> GetLifeFavorCnt(int enterpriseId)
        {
            var lifeCnt = DataContext.TB_Enterprise_Life.Where(a => a.EnterpriseId == enterpriseId).Count();

            var favorCnt = lifeCnt > 0 ? DataContext.TB_Enterprise_Life.Where(a => a.EnterpriseId == enterpriseId).Sum(a => a.Favor) : 0;
            return new Tuple<int, int>(lifeCnt, favorCnt);
        }

        #region Dapper

        public async Task<IList<EtpLifeDynamicDTO>> GetEtpLifeDynamicList(string companyIds, int days)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var startDate = DateTime.Now.AddDays((days > 0 ? -days : days)).Date;
                //这里只取图片，视频已经作为Video显示
                string sql = string.Format(@"
select a.EnterpriseId, b.Name EnterpriseName, a.Photos, a.CreateTime
from TB_Enterprise_Life a
left join TB_Enterprise b
on a.EnterpriseId = b.EnterpriseId
where a.EnterpriseId in ({0}) and a.CreateTime >= @startDate
and b.Status <> 99
and ISNULL(a.Photos,'') <> ''",companyIds);
                var list = await conn.QueryAsync<EtpLifeDynamicDTO>(sql, new { @startDate = startDate });
                return list.ToList();
            }
        }

        #endregion
    }

    public interface ICompanyLifeRepo : IRepository<TB_Enterprise_Life>
    {
        IList<TB_Enterprise_Life> GetList(int enterpriseId);

        PagedResult<TB_Enterprise_Life> GetList(int enterpriseId, int page, int pageSize);

        Tuple<int, int> GetLifeFavorCnt(int enterpriseId);
        #region Dapper
        Task<IList<EtpLifeDynamicDTO>> GetEtpLifeDynamicList(string companyIds, int days);
        #endregion
    }
}
