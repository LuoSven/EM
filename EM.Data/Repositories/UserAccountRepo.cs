using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Models.VMs;
using EM.Utils;

namespace EM.Data.Repositories
{
    public class UserAccountRepo : RepositoryBase<EM_User_Account>, IUserAccountRepo
    {
        public UserAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public LoginResult Login(AccountLoginVM accountVM,string LoginInfo)
        {
            var result = new LoginResult() { Message = "" };
            var LoginRecord = new EM_User_Login_Record()
            {
                LoginInfo = LoginInfo,
                LoginTime = DateTime.Now
            };
            var account = DataContext.EM_User_Account.Where(o => o.LoginEmaill == accountVM.UserName).FirstOrDefault();
            if (account == null)
                result.Message = "输入的用户不存在";
            if (account.Password != DESEncrypt.Encrypt(accountVM.Password))
                result.Message = "账号密码错误";

            if (result.Message!=string.Empty)
            {
                LoginRecord.IsLogin = false;
                LoginRecord.ErrorInfo = result.Message;
                LoginRecord.UserId = account == null ? 0 : account.UserId;
            }
            else
            {
                result.UserId = account.UserId;
                result.Mobile = account.Mobile;
                result.UserName = account.UserName;
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

    }
    public interface IUserAccountRepo : IRepository<EM_User_Account>
    {


        LoginResult Login(AccountLoginVM accountVM, string LoginInfo);

        Tuple<bool, string> IsRepeat(EM_User_Account model);
    }
}
