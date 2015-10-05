using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Models.VMs;
using EM.Utils;
using EM.Common;
namespace EM.Data.Repositories
{
    public class UserAccountRepo : RepositoryBase<EM_User_Account>, IUserAccountRepo
    {
        public UserAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public AccountVm Login(AccountLoginVM accountVM)
        {
            var result = new AccountVm() { Message = "" };
            //拼接登陆信息
            string[] LoginInfo={BrowserHelper.GetIP(), BrowserHelper.GetOSVersion(),BrowserHelper.GetBrowser()};
            var LoginRecord = new EM_User_Login_Record()
            {
                LoginTime = DateTime.Now,
                LoginInfo = string.Join(StaticKey.Split,LoginInfo)
            };
            //判断是否登陆成功
            var account = DataContext.EM_User_Account.Where(o => o.LoginEmaill == accountVM.UserName).FirstOrDefault();
            if (account == null)
                result.Message = "输入的用户不存在";
            else if (account.Password != DESEncrypt.Encrypt(accountVM.Password))
                result.Message = "账号密码错误";

            if (result.Message!=string.Empty)
            {
                LoginRecord.IsLogin = false;
                LoginRecord.ErrorInfo = result.Message;
                LoginRecord.UserId = account == null ? 0 : account.UserId;
            }
            else
            {
                //登陆成功
                result.UserId = account.UserId;
                result.Mobile = account.Mobile;
                result.UserName = account.UserName;

                //获取系统信息

                var SystemType = Dapper.DapperHelper.SqlQuery<int>(@"select distinct d.SystemType from EM_User_Account a 
join EM_User_Role b on a.RoleId=b.id
join EM_User_Right c on b.id=c.RoleId and c.Permit=1
join EM_System_Program d on d.Id=c.ProgramId
where a.UserId=@UserId", new { UserId = account.UserId }).ToList();
                result.SystemIds = SystemType;
                
                LoginRecord.IsLogin = true;
                LoginRecord.UserId = account == null ? 0 : account.UserId;
            }

            DataContext.EM_User_Login_Record.Add(LoginRecord);
            DataContext.SaveChanges();
            return result;

        }


        
        public  Tuple<bool,string> IsRepeat(EM_User_Account model)
        {
            var result = new Tuple<bool, string>(true, "");
            var userId = Dapper.DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where Mobile=@Mobile", new { Mobile = model.Mobile }).FirstOrDefault();
            if (userId != 0)
                return new Tuple<bool, string>(false, "手机号重复，请重新输入"); 
            userId = Dapper.DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where LoginEmaill=@LoginEmaill", new { LoginEmaill = model.LoginEmaill }).FirstOrDefault();
            if (userId != 0)
                return new Tuple<bool, string>(false, "登陆邮箱重复，请重新输入"); 
            return new Tuple<bool, string>(true, ""); ;
        }
        public void LogOff(int UserId)
        {
            var LoginRecord = new EM_User_Login_Record()
            {
                UserId=UserId,
                LoginTime = DateTime.Now,
                LoginInfo ="",
                IsLogin=false,
                ErrorInfo="用户退出了系统"
                
            };
            DataContext.EM_User_Login_Record.Add(LoginRecord);
            DataContext.SaveChanges();
        }

    }
    public interface IUserAccountRepo : IRepository<EM_User_Account>
    {


        AccountVm Login(AccountLoginVM accountVM);

        /// <summary>
        /// 记录下登出时间
        /// </summary>
        /// <param name="UserId"></param>
        void LogOff(int UserId);


        Tuple<bool, string> IsRepeat(EM_User_Account model);
    }
}
