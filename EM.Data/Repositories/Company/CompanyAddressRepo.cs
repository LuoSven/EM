using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc.Framework.Logger;
using System.Diagnostics;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyAddressRepo : RepositoryBase<TB_Enterprise_Address>, ICompanyAddressRepo
    {
        public CompanyAddressRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<IList<TB_Enterprise_Address>> GetEnterpriseAddressAsync(int enterpriseid, int? count = null)
        {

            using (TopucDB context = new TopucDB())
            {

                var qalist = from q in context.TB_Enterprise_Address
                             where q.EnterpriseId == enterpriseid
                             select q;

                if (count.HasValue)
                {
                    return await qalist.OrderByDescending(a => a.CreateTime).Take(count.Value).ToListAsync();
                }
                else
                {
                    return await qalist.OrderByDescending(a => a.CreateTime).ToListAsync();
                }

            }

        }

        public IList<TB_Enterprise_Address> GetList(int enterpriseId)
        {
            return DataContext.TB_Enterprise_Address.Where(x => x.EnterpriseId == enterpriseId).ToList();
        }

        public async Task<IList<TB_Enterprise_Address>> GetListAsync(int enterpriseId)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise_Address.Where(x => x.EnterpriseId == enterpriseId).OrderBy(a => a.AddressId).ToListAsync();
            }
        }

        public bool HasAddress(string address, int enterpriseId)
        {
            return DataContext.TB_Enterprise_Address.Where(a => a.Address == address && a.EnterpriseId == enterpriseId).Any();
        }

        public async Task<bool> HasCityAddressAsync(string city, int enterpriseId)
        {

            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise_Address.AnyAsync(x => x.EnterpriseId == enterpriseId && x.City != null && x.City.Contains(city));
            }
         
        }

        public  bool HasCityAddress(string city, int enterpriseId)
        {
            return  DataContext.TB_Enterprise_Address.Any(x => x.EnterpriseId == enterpriseId && x.City != null && x.City.Contains(city));
            
        }
    }
    public interface ICompanyAddressRepo : IRepository<TB_Enterprise_Address>
    {
        IList<TB_Enterprise_Address> GetList(int enterpriseId);
        Task<IList<TB_Enterprise_Address>> GetListAsync(int enterpriseId);
        Task<IList<TB_Enterprise_Address>> GetEnterpriseAddressAsync(int enterpriseid, int? count = null);

        bool HasAddress(string address, int enterpriseId);

        Task<bool> HasCityAddressAsync(string city, int enterpriseId);

        bool HasCityAddress(string city, int enterpriseId);
    }
}
