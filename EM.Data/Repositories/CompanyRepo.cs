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
    public class CompanyRepo : RepositoryBase<EM_Company>, ICompanyRepo
    {
        public CompanyRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<KeyValueVM> GetList(int roleId=0)
        {
            var companyList = DataContext.EM_User_Role.Where(o => o.id == roleId).Select(o => o.CompanyIds).FirstOrDefault();
            var result = DataContext.EM_Company.Where(o => roleId==0||companyList.Contains(o.Id.ToString())).Select(o => new KeyValueVM() { Key = o.Id.ToString(), Value = o.CompanyName }).ToList();
            return result;
        }

    }


    public interface ICompanyRepo : IRepository<EM_Company>
    {
        List<KeyValueVM> GetList(int roleId=0);
    }
}
