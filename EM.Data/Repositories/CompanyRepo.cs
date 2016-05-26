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

        public List<KeyValueVM> GetList(int roleId = 0, CompanyType companyTypeValue = CompanyType.Other)
        {
            var companyList = DataContext.EM_User_Role.Where(o => o.Id == roleId).Select(o => o.CompanyIds).FirstOrDefault().ToInts();
            var result = DataContext.EM_Company.Where(o => roleId == 0 || companyList.Contains(o.Id) && o.CompanyType == (int)companyTypeValue).Select(o => new KeyValueVM() { Key = o.Id.ToString(), Value = o.CompanyName }).ToList();
            return result;
        }


        public string GetCompanyName(int id)
        {
            var name = DapperHelper.SqlQuery<string>("select CompanyName from  EM_Company where Id=@Id ", new { Id = id }).FirstOrDefault();
            return name ?? "";
        }
        public string GetCompanysName(string ids,string SplitChar)
        {
            if (string.IsNullOrEmpty(ids))
                return string.Empty;
            var names = DapperHelper.SqlQuery<string>(string.Format("select CompanyName from  EM_Company where Id In ({0}) ",ids)).ToList();
            var result = string.Join(SplitChar, names);
            return result ;
        }

        public List<EM_Company> GetListDto(string Name="")
        {
            var sql="select * from EM_Company where 1=1";
             if(!string.IsNullOrEmpty(Name))
             {
                 sql+=" and CompanyName like '%'+@Name+'%'";
             }
          return  DapperHelper.SqlQuery<EM_Company>(sql, new { Name = Name }).ToList();
        }

    }


    public interface ICompanyRepo : IRepository<EM_Company>
    {
        List<EM_Company> GetListDto(string Name="");

        List<KeyValueVM> GetList(int roleId = 0, CompanyType CompanyTypeValue = CompanyType.Other);

        string GetCompanyName(int id);
        string GetCompanysName(string ids, string SplitChar);
    }
}
