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
    public class ChangeCateRepo : RepositoryBase<EM_Charge_Cate>, IChangeCateRepo
    {
        public ChangeCateRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<KeyValueVM> GetList()
        {
            var result = DataContext.EM_Charge_Cate.Select(o => new KeyValueVM() { Key = o.Id.ToString(), Value = o.CateName}).ToList();
            return result;
        }

    }


    public interface IChangeCateRepo : IRepository<EM_Charge_Cate>
    {
        List<KeyValueVM> GetList();
    }
}
