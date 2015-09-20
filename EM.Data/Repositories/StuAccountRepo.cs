using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topuc.Framework.Logger;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using Topuc22Top.Model.ExtendEntities;
using Topuc22Top.Model.DTOs;
using Dapper;
namespace Topuc22Top.Data.Repositories
{
    public class StuAccountRepo : RepositoryBase<TB_S_Account>, IStuAccountRepo
    {
        public StuAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public StudentProfileResult GetStudentResult(int userId)
        {
            StudentProfileResult StuResult = new StudentProfileResult();

            TB_S_Account acc = DataContext.TB_S_Account.Where(a => a.UserId == userId).FirstOrDefault();
            if (acc != null)
            {
                StuResult.StuId = userId;
                StuResult.UserName = acc.UserName;
                StuResult.RegisterEmail = acc.RegisterEmail;

                StuResult.Account = DataContext.TB_S_Account.Where(s => s.UserId == userId).FirstOrDefault();
                StuResult.BasicInfo = DataContext.TB_S_Basic.Where(s => s.UserId == userId).FirstOrDefault();
                StuResult.Educations = DataContext.TB_Stu_Education.Where(e => e.UserId == userId).OrderByDescending(e => e.StartDate).ToList();
                StuResult.Internships = DataContext.TB_Stu_Internship.Where(s => s.UserId == userId).ToList();

                StuResult.ITSkills = DataContext.TB_Stu_ITSkill.Where(s => s.UserId == userId).ToList();

                StuResult.Languages = DataContext.TB_Stu_Language.Where(s => s.UserId == userId).ToList();
                StuResult.Projects = DataContext.TB_Stu_Project.Where(s => s.UserId == userId).ToList();

                StuResult.Rewards = DataContext.TB_Stu_Reward.Where(s => s.UserId == userId).ToList();

                StuResult.CampusJobs = DataContext.TB_Stu_CampusJob.Where(s => s.UserId == userId).ToList();

                StuResult.Certificates = DataContext.TB_Stu_Certificate.Where(s => s.UserId == userId).ToList();

                //StuResult.Personality = DataContext.TB_Stu_Personality.Where(e => e.UserId == userId).FirstOrDefault();

                StuResult.Evalution = DataContext.TB_Stu_Evaluation.Where(e => e.UserId == userId).FirstOrDefault();

                //StuResult.Others = DataContext.TB_Stu_Other.Where(e => e.UserId == userId).ToList();

                StuResult.Attachment = DataContext.TB_Stu_Attachment.Where(e => e.UserId == userId).ToList();

                StuResult.Objective = DataContext.TB_Stu_Objective.Where(x => x.UserId == userId).FirstOrDefault();

                StuResult.NetworkLink = DataContext.TB_Stu_NetworkLink.Where(v => v.UserId == userId).FirstOrDefault();

                return StuResult;
            }
            else
            {
                return null;
            }
        }

        public TB_S_Basic GetBasicInfo(int userId)
        {
            return DataContext.TB_S_Basic.Where(p => p.UserId == userId).FirstOrDefault();
        }

        public string GetStuName(int userID)
        {
            var query = (from m in DataContext.TB_S_Account
                         where m.UserId == userID
                         select m.UserName).FirstOrDefault();
            if (query == null)
            {
                return string.Empty;
            }
            else
                return query;
        }

        public TB_S_Account GetStudentAccountInfo(string userEmail, string password)
        {
            return DataContext.TB_S_Account.Where(sa => sa.RegisterEmail == userEmail && sa.Password == password).FirstOrDefault();
        }

        public int GetProfileCompletion(int userId)
        {
            var c = DataContext.TB_S_Account.Where(p => p.UserId == userId).Select(p => p.ProfileCompletion).FirstOrDefault();
            c = c ?? 0;if (c > 100) c = 100;
            return c.Value;
        }

        /// <summary>
        /// 更新学生简历完善度
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addRatioActionTypes">增加</param>
        /// <param name="subRatioActionTypes">扣除</param>
        /// <returns></returns>
        public void UpdateProfileCompletion(int userId, IEnumerable<StuProfileRatioType> addRatioActionTypes, IEnumerable<StuProfileRatioType> subRatioActionTypes)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var sAccount = context.TB_S_Account.Where(sa => sa.UserId == userId).FirstOrDefault();
            if (sAccount != null)
            {
                try
                {
                    if (addRatioActionTypes != null && addRatioActionTypes.Count() > 0)
                    {
                        foreach (var item in addRatioActionTypes)
                        {
                            var ratio = addRatioTypeAction(userId, item);
                            sAccount.ProfileCompletion = (sAccount.ProfileCompletion ?? 0) + ratio;
                        }
                    }
                    if (subRatioActionTypes != null && subRatioActionTypes.Count() > 0)
                    {
                        foreach (var item in subRatioActionTypes)
                        {
                            var ratio = SubRatioTypeAction(userId, item);
                            if (ratio > 0) ratio = -ratio; //避免误操作
                            sAccount.ProfileCompletion = (sAccount.ProfileCompletion ?? 0) + ratio;
                        }
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    AppLogger.Error("更新学生简历完善度发生异常", ex);
                    //异常 怎么办 这是不允许的 为了不影响功能 只能 遍历所有相关项
                    var ratio = GetProfileCompletionByCalcAllItems(userId);
                    if (ratio > 0)
                    {
                        sAccount.ProfileCompletion = ratio;
                        context.SaveChanges();
                    }
                }
            }
            //return sAccount.ProfileCompletion ?? 0;
        }

        /// <summary>
        /// 某项做了完善操作
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int addRatioTypeAction(int userId, StuProfileRatioType item)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var curTypeRatio = Convert.ToInt32(((StuProfileRatioType)item).GetEnumDescription());
            switch (((StuProfileRatioType)item).ToString())
            {
                case "TB_S_Basic":
                    {
                        var ent = context.TB_S_Basic.Where(p => p.UserId == userId).FirstOrDefault();
                        //if (ent != null && ent.CreateDate.AddMinutes(2) > DateTime.Now) //避免误操作，20150716机制更改，填写完整才算分
                            return curTypeRatio;
                    }
                    break;
                case "TB_Stu_Evaluation":
                    {
                        var ent = context.TB_Stu_Evaluation.Where(p => p.UserId == userId).FirstOrDefault();
                        //if (ent != null && !string.IsNullOrEmpty(ent.SelfEvaluation))
                        if (ent != null && ent.CreateDate.AddMinutes(2) > DateTime.Now) //避免误操作
                            return curTypeRatio;
                    }
                    break;
                case "TB_Stu_Education":
                    {
                        var count = context.TB_Stu_Education.Where(p => p.UserId == userId).Count();
                        if (count == 1)
                            return curTypeRatio;//新增第一项
                    }
                    break;
                case "TB_Stu_Internship":
                    {
                        var count = context.TB_Stu_Internship.Where(p => p.UserId == userId).Count();
                        if (count == 1)
                            return curTypeRatio;//新增第一项
                    }
                    break;
                case "TB_Stu_Language":
                    {
                        var count = context.TB_Stu_Language.Where(p => p.UserId == userId).Count();
                        if (count == 1)
                            return curTypeRatio;//新增第一项
                    }
                    break;
                case "TB_Stu_ITSkill":
                    {
                        var count = context.TB_Stu_ITSkill.Where(p => p.UserId == userId).Count();
                        if (count == 1)
                            return curTypeRatio;//新增第一项
                    }
                    break;
                case "TB_Stu_Certificate":
                    {
                        var count = context.TB_Stu_Certificate.Where(p => p.UserId == userId).Count();
                        if (count == 1)
                            return curTypeRatio;//新增第一项
                    }
                    break;
            }
            return 0;
        }

        /// <summary>
        /// 某项做了移除操作
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int SubRatioTypeAction(int userId,StuProfileRatioType item)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var curTypeRatio = Convert.ToInt32(((StuProfileRatioType)item).GetEnumDescription());
            switch (((StuProfileRatioType)item).ToString())
            {
                case "TB_S_Basic":
                    {
                        //禁止此操作
                        if (!context.TB_S_Basic.Where(p => p.UserId == userId).Any())
                            return -curTypeRatio;
                    }
                    break;
                case "TB_Stu_Evaluation":
                    {
                        var ent = context.TB_Stu_Evaluation.Where(p => p.UserId == userId).FirstOrDefault();
                        //if (ent == null || string.IsNullOrEmpty(ent.SelfEvaluation))
                        if (ent == null)
                            return -curTypeRatio; //清空了
                        //其实是禁止次操作
                    }
                    break;
                case "TB_Stu_Education":
                    {
                        var count = context.TB_Stu_Education.Where(p => p.UserId == userId).Count();
                        if (count == 0)
                            return -curTypeRatio; //移完了
                    }
                    break;
                case "TB_Stu_Internship":
                    {
                        var count = context.TB_Stu_Internship.Where(p => p.UserId == userId).Count();
                        if (count == 0)
                            return -curTypeRatio; //移完了
                    }
                    break;
                case "TB_Stu_Language":
                    {
                        var count = context.TB_Stu_Language.Where(p => p.UserId == userId).Count();
                        if (count == 0)
                            return -curTypeRatio; //移完了
                    }
                    break;
                case "TB_Stu_ITSkill":
                    {
                        var count = context.TB_Stu_ITSkill.Where(p => p.UserId == userId).Count();
                        if (count == 0)
                            return -curTypeRatio; //移完了
                    }
                    break;
                case "TB_Stu_Certificate":
                    {
                        var count = context.TB_Stu_Certificate.Where(p => p.UserId == userId).Count();
                        if (count == 0)
                            return -curTypeRatio; //移完了
                    }
                    break;
            }
            return 0;
        }


        /// <summary>
        /// 遍历所有相关项
        /// </summary>
        /// <returns></returns>
        private int GetProfileCompletionByCalcAllItems(int userId)
        {
            try
            {
                var context = ObjectContextHelper.TopUObjectContext;
                int completion = 0;
                completion += context.TB_S_Basic.Where(it => it.UserId == userId).Count() > 0 ?
                   Convert.ToInt32(StuProfileRatioType.TB_S_Basic.GetEnumDescription()) : 0;
                completion += context.TB_Stu_Evaluation.Where(it => it.UserId == userId).Count() > 0 ?
                    Convert.ToInt32(StuProfileRatioType.TB_Stu_Evaluation.GetEnumDescription()) : 0;
                completion += context.TB_Stu_Education.Where(it => it.UserId == userId).Count() > 0 ?
                    Convert.ToInt32(StuProfileRatioType.TB_Stu_Education.GetEnumDescription()) : 0;
                completion += context.TB_Stu_Internship.Where(it => it.UserId == userId).Count() > 0 ?
                    Convert.ToInt32(StuProfileRatioType.TB_Stu_Internship.GetEnumDescription()) : 0;
                completion += context.TB_Stu_Language.Where(it => it.UserId == userId).Count() > 0 ?
                    Convert.ToInt32(StuProfileRatioType.TB_Stu_Language.GetEnumDescription()) : 0;
                completion += context.TB_Stu_ITSkill.Where(it => it.UserId == userId).Count() > 0 ?
                    Convert.ToInt32(StuProfileRatioType.TB_Stu_ITSkill.GetEnumDescription()) : 0;
                completion += context.TB_Stu_Certificate.Where(it => it.UserId == userId).Count() > 0 ?
                    Convert.ToInt32(StuProfileRatioType.TB_Stu_Certificate.GetEnumDescription()) : 0;
                return completion;
            }
            catch (Exception ex)
            {
                AppLogger.Error("计算学生简历完善度（通过遍历所有相关项）发生异常", ex);
                return -1;
            }
        }

        public TB_S_Account GetUserByOpenId(string openId)
        {
            if (string.IsNullOrEmpty(openId)) return null;
            return DataContext.TB_S_Account.Where(sa => sa.WeChatOpenId == openId).FirstOrDefault();
        }
        public int GetUserIdByOpenId(string openId)
        {
            if (string.IsNullOrEmpty(openId))
                return 0;
            return (from m in DataContext.TB_S_Account
                    where m.WeChatOpenId == openId
                    select m.UserId).FirstOrDefault();
        }

        public bool isUserBindWeChat(string openId)
        {
            return DataContext.TB_S_Account.Any(sa => sa.WeChatOpenId == openId);
        }

        public DateTime? GetLastLoginTime(int userId)
        {
            return DataContext.TB_S_Account.Where(u => u.UserId == userId).First().LastLoginAt;
        }

        public bool CanApplyJobOfTouch(int userId) 
        {
            var query = from account in DataContext.TB_S_Account
                        join basic in DataContext.TB_S_Basic
                        on account.UserId equals basic.UserId into gBasic
                        join ev in DataContext.TB_Stu_Evaluation
                        on account.UserId equals ev.UserId into gEv
                        join edu in DataContext.TB_Stu_Education
                        on account.UserId equals edu.UserId into gEdu
                        where account.UserId == userId
                        && gBasic.Count() == 1
                        && gEv.Count() > 0
                        && gEdu.Count() > 0
                        select 1;
            return query.FirstOrDefault() == 1;
        }



        public async Task<bool> IsEmailRegisted(string email) 
        {
            return (from account in DataContext.TB_S_Account where account.RegisterEmail == email select 1).Any();
        }

        public async Task<TB_S_Account> CreateAsync(string email, string password, string openId, int? regType) 
        {
            TB_S_Account account = new TB_S_Account();
            account.UserId = 0;
            account.UserName = string.Empty;
            account.RegisterEmail = email;
            account.Password = password;
            account.AccountStatus = (int)StudentStatus.Active;
            account.CreateDate = DateTime.Now;
            account.ModifyDate = DateTime.Now;
            account.LoginTimes = 1;
            account.LastResumeModifyDate = DateTime.Now;
            account.LastLoginAt = DateTime.Now;
            account.WeChatOpenId = openId;
            account.RegType = regType;

            DataContext.TB_S_Account.Add(account);

            DataContext.SaveChanges();

            return account;
        }

        public async Task AddStuLoginTimes(int userId) 
        {
            var account = (from entity in DataContext.TB_S_Account where entity.UserId == userId select entity).FirstOrDefault();
            if (account != null) 
            {
                account.LastLoginAt = DateTime.Now;
                account.LoginTimes += 1;
                DataContext.SaveChanges();
            }
        }

        public void SaveResetPasswordRequest(int userId, Guid pwdKey, DateTime createDate, bool keySwitch)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            TB_Stu_FindPwd sFindPwd = new TB_Stu_FindPwd();
            sFindPwd.UserId = userId;
            sFindPwd.PwdKey = pwdKey;
            sFindPwd.KeyTime = createDate;
            sFindPwd.KeySwitch = keySwitch;
            context.TB_Stu_FindPwd.Add(sFindPwd);
            context.SaveChanges();
        }

        public void SaveResetPasswordShortCode(int userId, Guid pwdKey, string ShortCode, DateTime KeyTime)
        {
            //先清除，不是必要条件
            var del=Dapper.DapperHelper.SqlExecute22(string.Format("Delete from TB_Stu_FindPwd_ShortCode where UserId='{0}'  ", userId));

            List<Tuple<string, object>> Sqls = new List<Tuple<string, object>>();
            Sqls.Add(new Tuple<string, object>(@"INSERT INTO TB_Stu_FindPwd_ShortCode
           (PwdKey
           ,ShortPwdKey
           ,KeyTime
           ,UserId)
     VALUES
           (@PwdKey
           ,@ShortPwdKey
           ,@KeyTime
           ,@UserId )", new { PwdKey = pwdKey, ShortPwdKey = ShortCode, KeyTime = KeyTime, UserId = userId }));
            Sqls.Add(new Tuple<string, object>(@"INSERT INTO TB_Stu_FindPwd
           (PwdKey
           ,KeyTime
           ,KeySwitch
           ,UserId)
     VALUES
         (  @PwdKey
           ,@KeyTime
           ,@KeySwitch
           ,@UserId)", new { PwdKey = pwdKey, KeyTime = KeyTime, KeySwitch = 0, UserId = userId }));

            var result =  Dapper.DapperHelper.SqlExecute22WithTransaction(Sqls);


        }

        public async Task<StuSendCodeDTO> FindByMobileAndEmailDTO(string mobile,string Email)
        {
            var t = await Dapper.DapperHelper.SqlQuery22Async<StuSendCodeDTO>(@"select a.UserId,a.RegisterEmail,b.Mobile from TB_S_Account a
join TB_S_Basic  b on a.UserId=b.UserId 
where b.Mobile =@Mobile and a.RegisterEmail=@Email", new { Mobile=mobile,Email=Email });
            return t.FirstOrDefault();
        }

        public async Task<Tuple<bool,string>> GetPwdKeybyEmailAndShortCode(string email, string code)
        {
            var t = await Dapper.DapperHelper.SqlQuery22Async<StuConfirmCodeDTO>(@"select a.PwdKey,a.KeyTime,a.UserId from TB_Stu_FindPwd_ShortCode a
join TB_S_Account b on a.UserId =b.UserId
where a.ShortPwdKey=@ShortPwdKey and b.RegisterEmail=@RegisterEmail", new { ShortPwdKey = code, RegisterEmail = email });
            var s = t.FirstOrDefault();
            if (s!= null)
            {
               if(s.KeyTime<DateTime.Now)
                   return new Tuple<bool, string>(false, "验证码已经过期，请重新获取！");
              var result= await Dapper.DapperHelper.SqlExecute22Async(string.Format("Delete from TB_Stu_FindPwd_ShortCode where PwdKey='{0}'  ",s.PwdKey));
              if (result==1)
                   return new Tuple<bool, string>(true, s.PwdKey.ToString());
            }
            return  new Tuple<bool, string>(false,"验证码输入错误！");
        }

        public async Task<bool> ChangePasswordByToken(string Token, string Password)
        {
            List<Tuple<string, object>> Sqls = new List<Tuple<string, object>>();
            Sqls.Add(new Tuple<string, object>(@"update TB_S_Account   set Password=@Password  
              where TB_S_Account.UserId in (select a.UserId from TB_Stu_FindPwd a where a.PwdKey=@PwdKey )", new { Password = Password, PwdKey = Token }));
            Sqls.Add(new Tuple<string, object>(@"update TB_Stu_FindPwd  set KeySwitch=1,UpdateDate=@UpdateDate where PwdKey=@PwdKey ",  new { UpdateDate = DateTime.Now, PwdKey = Token }));

         var result=await Dapper.DapperHelper.SqlExecute22AsyncWithTransaction(Sqls);
            if(result!=null&&!result.AsList().Contains(0))
                return true;
            return false;
        }


         public  string GetOpenIdByUserId(string UserId)
        {
            return  Dapper.DapperHelper.SqlQuery22<string>("select WeChatOpenId  from TB_S_Account where UserId=@UserId", new { UserId = UserId }).FirstOrDefault();
         }

        public  int[] GetPushPositionUserIdList()
         {
             var list = Dapper.DapperHelper.SqlQuery22<int>(@"select distinct   a.UserId from TB_S_Account a
join ( select  max(Degree) as Degree, UserId from   TB_Stu_Education  where EndDate>=@EndDate   group by UserId ) b on    a.UserId=b.UserId
where RegisterEmail is not null  ", new { EndDate = new DateTime(DateTime.Now.Year - 1, 9, 1) }).ToArray();
             return list;
         }

     public  void UpdateLastResumeModifyDate(int userId)
        {
            var result = Dapper.DapperHelper.SqlExecute22("update TB_S_Account set LastResumeModifyDate=@LastResumeModifyDate where UserId=@UserId", new { LastResumeModifyDate = DateTime.Now, UserId =userId});
        }
    }
    public interface IStuAccountRepo : IRepository<TB_S_Account>
    {
        /// <summary>
        /// 获取学生个人信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        StudentProfileResult GetStudentResult(int userId);
        TB_S_Basic GetBasicInfo(int userId);
        /// <summary>
        /// 获取学生姓名
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        string GetStuName(int userID);

        /// <summary>
        /// 获取TB_S_Account 通过邮箱密码
        /// </summary>
        TB_S_Account GetStudentAccountInfo(string userEmail, string password);

        /// <summary>
        /// 获得简历完整度
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetProfileCompletion(int userId);

        TB_S_Account GetUserByOpenId(string openId);
        int GetUserIdByOpenId(string openId);

        bool isUserBindWeChat(string openId);

        DateTime? GetLastLoginTime(int userId);

        void  UpdateProfileCompletion(int userId, IEnumerable<StuProfileRatioType> addRatioActionTypes, IEnumerable<StuProfileRatioType> subRatioActionTypes);

        /// <summary>
        /// 移动端 学生 是否 可以 申请 职位
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="posId"></param>
        /// <returns></returns>
        bool CanApplyJobOfTouch(int userId);



        /// <summary>
        /// 已被注册 返回true
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsEmailRegisted(string email);

        /// <summary>
        /// 创建学生帐号，调用之前请先确认邮箱未被注册过
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<TB_S_Account> CreateAsync(string email, string password, string openId, int? regType);
        /// <summary>
        /// 增加登录次数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddStuLoginTimes(int userId);


        /// <summary>
        /// 把保存找回密码记录的操作放到DB层
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pwdKey"></param>
        /// <param name="createDate"></param>
        /// <param name="keySwitch"></param>
        void SaveResetPasswordRequest(int userId, Guid pwdKey, DateTime createDate, bool keySwitch);

        /// <summary>
        /// 保存找回密码-6位短码,目前最后一步和pc端通用，方便以后将找回密码短信服务放到Pc端，pc现有逻辑有问题
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pwdKey"></param>
        /// <param name="createDate"></param>
        /// <param name="keySwitch"></param>
        void SaveResetPasswordShortCode(int userId, Guid pwdKey, string ShortCode, DateTime KeyTime);

        /// <summary>
        /// 通过邮箱和手机进行匹配，找回密码时用
        /// </summary>
        /// <param name="moible"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        Task<StuSendCodeDTO> FindByMobileAndEmailDTO(string mobile, string Email);

        Task<Tuple<bool, string>> GetPwdKeybyEmailAndShortCode(string email, string code);

        Task<bool> ChangePasswordByToken(string Token, string Password);

        string GetOpenIdByUserId(string UserId);

        int[] GetPushPositionUserIdList();

        void UpdateLastResumeModifyDate(int userId);
    }
}
