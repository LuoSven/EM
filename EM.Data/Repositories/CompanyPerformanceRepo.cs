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
    public class CompanyPerformanceRepo : RepositoryBase<EM_Company_Performance>, ICompanyPerformanceRepo
    {
        public CompanyPerformanceRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<CompanyPerformanceDTO> GetList(CompanyPerformanceSM sm)
        {

            var sql = @"select b.CompanyName,a.CompanyId,a.CreateDate,a.Creater,a.Id,a.Modifier,a.ModifyDate,a.SalesPerformance,a.UploadDate from EM_Company_Performance a
join EM_Company b on a.CompanyId=b.Id
 where 1=1 ";

            if (sm.CompanyId != 0)
            {
                sql += "and a.CompanyId=@CompanyId";
            }
            if (sm.SDate.HasValue)
            {
                sql += "and a.UploadDate>=@SDate";
            }
            if (sm.EDate.HasValue)
            {
                sql += "and a.UploadDate<=@EDate";
            }
            var result = DapperHelper.SqlQuery<CompanyPerformanceDTO>(sql, sm).ToList();
            return result;
        }

    }


    public interface ICompanyPerformanceRepo : IRepository<EM_Company_Performance>
    {
         List<CompanyPerformanceDTO> GetList(CompanyPerformanceSM sm);
    }
}
