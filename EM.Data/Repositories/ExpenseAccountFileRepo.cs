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
    public class ExpenseAccountFileRepo : RepositoryBase<EM_ExpenseAccount_File>, IExpenseAccountFileRepo
    {
        public ExpenseAccountFileRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<EM_ExpenseAccount_File> GetListByExpenseAccountId(int Id)
        {
            var List = DapperHelper.SqlQuery<EM_ExpenseAccount_File>(@"select * from EM_ExpenseAccount_File where ExpenseAccountId=@ExpenseAccountId and Status=@Default", new { ExpenseAccountId = Id, Default = (int)ExpenseAccountFileStatus.Default }).ToList();
            return List;
        }

        public async Task UpdateFileExpenseAccountId(int ExpenseAccountId,string Ids)
        {
            var result = await DapperHelper.SqlExecuteAsync(string.Format(@"update EM_ExpenseAccount_File set ExpenseAccountId=@ExpenseAccountId,Status=@Default where Id in ({0}) and Status=@NoRelated",  Ids), new { ExpenseAccountId = ExpenseAccountId, Default = ExpenseAccountFileStatus.Default, NoRelated =ExpenseAccountFileStatus.NoRelated});
        }

        public async Task< bool> UpdateDeleteStatus(int Id)
        {
            var result = await DapperHelper.SqlExecuteAsync(@"update EM_ExpenseAccount_File set Status=@Deleted where Id=@Id", new { Id = Id, Deleted = ExpenseAccountFileStatus.Deleted});
            return result > 0;
        }

         public async  Task<ExpenseAccountFileDTO> GetDtos(int Id)
        {
            return (await DapperHelper.SqlQueryAsync<ExpenseAccountFileDTO>(@"select a.FileName,a.UpLoader,a.FilePath,a.Remark,b.EANumber,c.CompanyName from EM_ExpenseAccount_File a 
left join EM_ExpenseAccount b on a.ExpenseAccountId=b.Id
left join EM_Company c on b.CompanyId=c.Id
where a.Id=@Id", new { Id = Id })).FirstOrDefault();
        }
    }


    public interface IExpenseAccountFileRepo : IRepository<EM_ExpenseAccount_File>
    {
        List<EM_ExpenseAccount_File> GetListByExpenseAccountId(int Id);

        Task UpdateFileExpenseAccountId(int ExpenseAccountId, string Ids);
         Task<bool> UpdateDeleteStatus(int Id);

         Task<ExpenseAccountFileDTO> GetDtos(int Id);

         //bool CheckFileName(int Id, string FileName, string Uploader);

    }
}
