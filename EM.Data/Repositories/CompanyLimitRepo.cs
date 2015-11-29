using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Model.VMs;
using EM.Utils;
using EM.Common;
using EM.Model.DTOs;
using EM.Data.Dapper;
using EM.Model.SMs;

namespace EM.Data.Repositories
{
    public class CompanyLimitRepo : RepositoryBase<EM_Company_Limit>, ICompanyLimitRepo
    {
        public CompanyLimitRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<CompanyLimitDTO> GetList(CompanyLimitSM sm)
        {

            var sql = @"select a.Id,b.CompanyName,c.CateName,a.LimitSum,a.ModifyDate,a.Modifier from EM_Company_Limit a
join EM_Company b on a.CompanyId=b.Id
join EM_Charge_Cate c on a.CateId=c.Id
 where 1=1 ";

            if (sm.CompanyId != 0)
            {
                sql += " and a.CompanyId=@CompanyId";
            } if (sm.CateId != 0)
            {
                sql += " and a.CateId=@CateId";
            } if (sm.SeasonType != 0)
            {
                sql += " and a.SeasonType=@SeasonType";
            }
            if (sm.SDate.HasValue)
            {
                sql += " and a.ModifyDate>=@SDate";
            }
            if (sm.EDate.HasValue)
            {
                sql += " and a.ModifyDate<=@EDate";
            }
            var result = DapperHelper.SqlQuery<CompanyLimitDTO>(sql, sm).ToList();
            return result;
        }

    }


    public interface ICompanyLimitRepo : IRepository<EM_Company_Limit>
    {
         List<CompanyLimitDTO> GetList(CompanyLimitSM sm);
    }
}
