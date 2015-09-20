using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;
using System.Data.Entity;

namespace Topuc22Top.Data.Repositories
{
    public class StuBasicRepo : RepositoryBase<TB_S_Basic>, IStuBasicRepo
    {
        public StuBasicRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public TB_S_Basic GetByUserID(int userID)
        {
            var query = from m in DataContext.TB_S_Basic
                        where m.UserId == userID
                        select m;
            return query.FirstOrDefault();
        }


        public async Task<BasicDTO> GetByUserIDAsync(int userID)
        {
            string sql = string.Format("select b.Id,   a.UserName  as Name, b.FamilyLocation,b.PolicitalStatus,b.Height ,case when isnull(b.Email,'') = '' then a.RegisterEmail else b.Email end as Email ,b.UserId,b.Gender,b.Birthday,b.Mobile,b.FamilyLocation,b.CurrentLocation,b.PolicitalStatus,b.Height,b.BloodType  from TB_S_Account  a left join TB_S_Basic  b on a.UserId=b.UserId where a.UserId= '{0}'", userID);
            var query = DataContext.Database.SqlQuery<BasicDTO>(sql);
            var model =  query.FirstOrDefault();
            return model;
        }

        public async Task UpdateAsync(BasicDTO model)
        {
            var target = DataContext.TB_S_Basic.Where(o => o.UserId == model.UserId).FirstOrDefault();
            if (target != null) 
            { 
                target.Birthday = model.Birthday;
                target.CurrentLocation = model.CurrentLocation;
                target.Email = model.Email;
                target.Gender = model.Gender.Value;
                target.ModifyDate = DateTime.Now;
                target.Mobile = model.Mobile;
                target.FamilyLocation = model.FamilyLocation;
                if (model.Height.HasValue)
                    target.Height = (short)model.Height;
                if (model.PolicitalStatus.HasValue)
                target.PolicitalStatus = model.PolicitalStatus;
            }
            var account = DataContext.TB_S_Account.Where(p => p.UserId == model.UserId).FirstOrDefault();
            if (account != null) 
            { 
                account.UserName = model.Name;
            }

            DataContext.SaveChanges();
        }

        public async Task CreateAsync(BasicDTO model)
        {
            var target = DataContext.TB_S_Basic.Where(o => o.UserId == model.UserId).FirstOrDefault();
            if(target==null)
            {
                target = new TB_S_Basic();
                target.CurrentLocation = model.CurrentLocation;
                target.Email = model.Email;
                target.UserId = model.UserId.Value;
                target.Gender = model.Gender.Value;
                target.Mobile = model.Mobile;
                target.Birthday = model.Birthday;
                target.FamilyLocation = model.FamilyLocation;
                target.Height = (short)model.Height;
                target.PolicitalStatus = model.PolicitalStatus;
                target.ModifyDate = DateTime.Now;
                target.CreateDate = DateTime.Now;
                DataContext.TB_S_Basic.Add(target);

                var account = DataContext.TB_S_Account.Where(p => p.UserId == model.UserId).FirstOrDefault();
                if (account != null)
                {
                    account.UserName = model.Name;
                }

                await DataContext.SaveChangesAsync();
            }
            
        }
        public string[] GetEmailAndUserNameByUserId(int id)
        {
            string[] result = new string[2];
            var Email = "";
            var UserName = "";
            var t = GetByUserID(id);
            IStuAccountRepo stuAccountRepo = new StuAccountRepo(new Data.Infrastructure.DatabaseFactory());
            var StuAccount = stuAccountRepo.GetById(id);
            if (t != null && !string.IsNullOrEmpty(t.Email))
                Email = t.Email;
            else
            {
                Email = StuAccount.RegisterEmail;
            }
            if (!string.IsNullOrEmpty(StuAccount.UserName))
                UserName = StuAccount.UserName;
            result[0] = Email;
            result[1] = UserName;
            return result;
        }
    }
    public interface IStuBasicRepo : IRepository<TB_S_Basic>
    {

        /// <summary>
        /// 根据id获取Email和学生名，默认是Basic内的邮箱，无则返回account内邮箱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string[] GetEmailAndUserNameByUserId(int id);

        /// <summary>
        /// 通过UserID获取基本信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        TB_S_Basic GetByUserID(int userID);


       /// <summary>
       /// 更新基本信息
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        Task UpdateAsync(BasicDTO model);

        /// <summary>
        /// 创建基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CreateAsync(BasicDTO model);


        Task<BasicDTO> GetByUserIDAsync(int userID);
    }
}
