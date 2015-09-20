using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;

using Dapper;
using Topuc22Top.Data.Dapper;
using Dapper.Contrib.Extensions;
using System;
using Topuc22Top.Model.VMs;

namespace Topuc22Top.Data.Repositories
{
    public class StuCertificateRepo : RepositoryBase<TB_Stu_Certificate>, IStuCertificateRepo
    {
        public StuCertificateRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            
        }

        public async Task<StuCertificateDTO> FindAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,CertificateDate,CertificateName,UserId from TB_Stu_Certificate where Id = @id";
                var model = await conn.QueryAsync<StuCertificateDTO>(sql, new { @id = id });
                return model.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<StuCertificateDTO>> FindListByUserAsync(int userId) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,CertificateDate,CertificateName,UserId,CertificateDegree from TB_Stu_Certificate where UserId = @userId";
                var model = await conn.QueryAsync<StuCertificateDTO>(sql, new { @userId = userId });
                return model.ToList();
            }
        }

        public async Task<int> CreateAsync(TB_Stu_Certificate model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                model.CreateDate = DateTime.Now;
                model.ModifyDate = DateTime.Now;
                return await conn.InsertAsync(model);
            }
        }

        public async Task UpdateAsync(StuCertificateDTO model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var target = await conn.GetAsync<TB_Stu_Certificate>(model.Id);
                target.CertificateDate = model.CertificateDate;
                target.CertificateName = model.CertificateName;
                target.ModifyDate = DateTime.Now;
                await conn.UpdateAsync(target);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"delete from TB_Stu_Certificate where Id = @id";
                await conn.ExecuteAsync(sql, new { @id = id });
            }
        }


        public List< DictItemVm> GetCertificateList()
        {
            var s = DapperHelper.SqlQuery22<DictItemVm>(@"select a.ItemId as id ,a.ItemName as name from DictItem a
where  a.Type='Certification' and   a.ParentItemId=0
").ToList();
            foreach (var item in s)
            {
                var items= DapperHelper.SqlQuery22<DictItemVm>(@"select a.ItemId as id ,a.ItemName as name from DictItem a
where  a.Type='Certification' and   a.ParentItemId=@ID
",new{ID=item.id}).ToList();
                item.items = items;
            }
            return s;
        }

        public async Task<bool> IsExistAsync(TB_Stu_Certificate model)
        {
            var id = (await DapperHelper.SqlQuery22Async<int>(@"select Id from TB_Stu_Certificate  a where  a.UserId=@UserId and a.CertificateName=@CertificateName", new { UserId = model.UserId, CertificateName = model.CertificateName })).FirstOrDefault();
            if (id == null || id == 0)
                return false;
            return true;
        }
    }

    public interface IStuCertificateRepo : IRepository<TB_Stu_Certificate>
    {
        Task<StuCertificateDTO> FindAsync(int id);
        Task<IEnumerable<StuCertificateDTO>> FindListByUserAsync(int userId);

        Task<int> CreateAsync(TB_Stu_Certificate model);

        Task UpdateAsync(StuCertificateDTO model);

        Task DeleteAsync(int id);

        List<Topuc22Top.Model.VMs.DictItemVm> GetCertificateList();

        Task<bool> IsExistAsync(TB_Stu_Certificate model);

    }

}
