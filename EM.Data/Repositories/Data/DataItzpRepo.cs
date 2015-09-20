using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class DataItzpRepo : RepositoryBase<Data_Itzp>, IDataItzpRepo
    {
        private readonly ICache cache;
        public DataItzpRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<Data_Itzp> GetJobs(string city, string section)
        {
            var list = GetList();
            var query = from j in list
                        select j;
            if (!string.IsNullOrEmpty(section))
            {
                query = query.Where(j => j.Section == section);
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(j => j.City == city);
            }
            return query.ToList();
        }

        public IList<Data_Itzp> GetRankJobs(int cnt)
        {
            var list = GetList();
            var query = from j in list
                        select j;
            return query.OrderByDescending(j => j.Salary).ThenByDescending(j => j.Weight).Take(cnt).ToList();
        }


        public IList<string> getCityList()
        {
            var list = GetList();
            return list.Select(j => j.City).Distinct().ToList();
        }

        private IList<Data_Itzp> GetList()
        {
            return cache.Get("DATA_ITZP", () =>
            {
                return this.GetAll().ToList();
            }, 60);
        }

    }
    public interface IDataItzpRepo : IRepository<Data_Itzp>
    {
        IList<Data_Itzp> GetJobs(string city, string section);

        IList<Data_Itzp> GetRankJobs(int cnt);

        IList<string> getCityList();
    }
}
