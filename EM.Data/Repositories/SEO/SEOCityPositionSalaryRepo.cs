using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class SEOCityPositionSalaryRepo : RepositoryBase<SEO_CityPositionSalary>, ISEOCityPositionSalaryRepo
    {
        public SEOCityPositionSalaryRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IList<CityPosistonSalary_Avg> GetCityPositionAvgSalary(int cityId, int takeNo)
        {
            //var dt1 = DateTime.Now.AddMonths(-6);
            var dt1 = new DateTime(2011,11,11);
            var query = (from entity in DataContext.SEO_CityPositionSalary
                         where entity.LastUpdatedDate > dt1 && entity.SalaryLower > 1000
                            && entity.TopucCityId == cityId
                        group entity by entity.PositionName into g
                        where g.Count() >= 5
                        select new CityPosistonSalary_Avg()
                        {
                             PosName = g.Key,
                             S_Avg= g.Select(p => p.SalaryLower + p.SalaryCeiling).Average()
                        }).OrderByDescending(p => p.S_Avg).Take(takeNo).OrderBy(p => p.S_Avg).ToList();
            query.ForEach(p => p.S_Avg = (int)p.S_Avg / 2);
            return query;
        }

        public int GetCitySalaryAvg(int cityId) 
        {
            try
            {
                //var dt1 = DateTime.Now.AddMonths(-6);
                var dt1 = new DateTime(2011, 11, 11);
                var query = from entity in DataContext.SEO_CityPositionSalary
                            where entity.LastUpdatedDate > dt1 && entity.SalaryLower > 1000
                            && entity.TopucCityId == cityId
                            select entity;
                return (int)query.Select(p => p.SalaryLower + p.SalaryCeiling).Average() / 2;
            }
            catch { return 0; }
        }

        public IList<CityCompanySalary_Avg> GetCityCompanyAvgSalary(int cityId, int takeNo)
        {
            //var dt1 = DateTime.Now.AddMonths(-6);
            var dt1 = new DateTime(2011, 11, 11);
            var query = (from entity in DataContext.SEO_CityPositionSalary
                         where entity.LastUpdatedDate > dt1 && entity.SalaryLower > 1000
                         && entity.TopucCityId == cityId
                         group entity by entity.CompanyName into g
                         select new CityCompanySalary_Avg()
                         {
                             CompanyName = g.Key,
                             S_Avg = g.Select(p => p.SalaryLower + p.SalaryCeiling).Average()
                         }).OrderByDescending(p => p.S_Avg).Take(takeNo).ToList();
            query.ForEach(p => p.S_Avg = (int)p.S_Avg / 2);
            return query;
        }

    }

    public interface ISEOCityPositionSalaryRepo : IRepository<SEO_CityPositionSalary>
    {
        IList<CityPosistonSalary_Avg> GetCityPositionAvgSalary(int cityId, int takeNo);
        IList<CityCompanySalary_Avg> GetCityCompanyAvgSalary(int cityId, int takeNo);
        int GetCitySalaryAvg(int cityId);
    }
}
