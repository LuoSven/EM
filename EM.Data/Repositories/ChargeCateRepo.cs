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
    public class ChargeCateRepo : RepositoryBase<EM_Charge_Cate>, IChargeCateRepo
    {
        public ChargeCateRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<KeyValueVM> GetList(int RoleTypeId)
        {
            var result = DataContext.EM_Charge_Cate.Where(o=>o.RoleTypes.Contains(RoleTypeId.ToString())).Select(o=>new KeyValueVM(){
                Value = o.CateName,
                Key=o.Id.ToString()
            }).ToList();
            return result;
        }
        public string GetCateName(int id)
        {
            var name = DapperHelper.SqlQuery<string>("select CateName from  EM_Charge_Cate where Id=@Id ", new { Id = id }).FirstOrDefault();
            return name ?? "";
        }

    }


    public interface IChargeCateRepo : IRepository<EM_Charge_Cate>
    {
        List<KeyValueVM> GetList(int RoleTypeId);

        string GetCateName(int id);
    }
}
