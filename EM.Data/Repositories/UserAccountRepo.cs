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
    public class UserAccountRepo : RepositoryBase<EM_User_Account>, IUserAccountRepo
    {
        public UserAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public AccountVM Login(AccountLoginVM accountVM)
        {
            var result = new AccountVM() { Message = "" };
            //拼接登陆信息
            string[] LoginInfo = { accountVM.UserName,accountVM.Password, BrowserHelper.GetIP(), BrowserHelper.GetOSVersion(), BrowserHelper.GetBrowser() };
            var LoginRecord = new EM_User_Login_Record()
            {
                LoginTime = DateTime.Now,
                LoginInfo = string.Join(StaticKey.Split,LoginInfo)
            };
            //判断是否登陆成功
            var account = DataContext.EM_User_Account.Where(o => o.LoginEmail == accountVM.UserName).FirstOrDefault();
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
join EM_User_Role b on a.RoleId=b.Id
join EM_User_Right c on b.id=c.RoleId and c.Permit=1
join EM_System_Program d on d.Id=c.ProgramId
where a.UserId=@UserId", new { UserId = account.UserId }).ToList();
                var CompanyIdsAndRoleType = DapperHelper.SqlQuery<AccountVM>(@"select CompanyIds  ,RoleType   from EM_User_Role where Id=@RoleId ", new { RoleId = account.RoleId.Value }).FirstOrDefault();

                result.SystemIds = SystemType;
                result.UserRole = account.RoleId.Value;
                result.CompanyIds = CompanyIdsAndRoleType.CompanyIds ?? "0";
                result.RoleType = CompanyIdsAndRoleType.RoleType;
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
            var userId = Dapper.DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where Mobile=@Mobile and UserId<>@UserId", model).FirstOrDefault();
            if (userId != 0)
                return new Tuple<bool, string>(false, "手机号重复，请重新输入"); 
            userId = Dapper.DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where LoginEmail=@LoginEmail", new { LoginEmail = model.LoginEmail }).FirstOrDefault();
            if (userId != 0)
                return new Tuple<bool, string>(false, "登陆邮箱重复，请重新输入"); 
            return new Tuple<bool, string>(true, ""); ;
        }
        public bool IsEmailRepeat(string LoginEmail, int UserId)
        {
            var userId = Dapper.DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where LoginEmail=@LoginEmail and UserId<>@UserId", new { UserId = UserId, LoginEmail = LoginEmail }).FirstOrDefault();
            if (userId != 0)
                return true;
            return false ;
        }


        public bool IsMobileRepeat(string Mobile, int UserId)
        {
            var result = new Tuple<bool, string>(true, "");
            var userId = Dapper.DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where Mobile=@Mobile and UserId<>@UserId", new { UserId = UserId, Mobile =Mobile}).FirstOrDefault();
            if (userId != 0)
                return true;
            return false;
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

        public async Task<List<AccountDetailDTO>> GetUserList(SystemUserSM sm)
        {
            var sql = @"select a.UserId, a.UserName,a.LoginEmail,a.Mobile,b.Name as RoleName,a.Status,a.ModifyTime from EM_User_Account a
left join EM_User_Role b on a.RoleId=b.id ";
            sql += string.IsNullOrEmpty(sm.UserName) ? "" : " and a.UserName like '%'+@UserName+'%' ";
            sql += string.IsNullOrEmpty(sm.LoginEmail) ? "" : " and a.LoginEmail like '%'+@LoginEmail+'%' ";
            sql += string.IsNullOrEmpty(sm.RoleId) ? "" : " and a.RoleId =@RoleId ";
            sql += " order by b.RoleType,a.UserId ";
           var result = (await DapperHelper.SqlQueryAsync<AccountDetailDTO>(sql,sm)).ToList();
           return result;

        }

        public async Task<AccountDetailDTO> GetByIdDto(int UserId)
        {
            var user = (await Dapper.DapperHelper.SqlQueryAsync<AccountDetailDTO>("select * from EM_User_Account where UserId=@UserId", new { UserId = UserId })).FirstOrDefault();
            return user;
        }


        public string ChangePassword(int UserId,string OPassword,string NPassword)
        {
             var result= DapperHelper.SqlQuery<int>("select UserId from EM_User_Account where UserId=@UserId and Password=@Password", new { UserId = UserId, Password =DESEncrypt.Encrypt(OPassword)}).FirstOrDefault();
            if(result!=0)
            {
                result = DapperHelper.SqlExecute(@"update EM_User_Account set Password=@Password where UserId=@UserId", new { UserId = UserId, Password = DESEncrypt.Encrypt(NPassword) });
                return result > 0 ? "" : "保存失败,请重试";
            }
            return "旧密码错误，请重新输入";
        }

        public List<KeyValueVM> GetSelectList()
        {
            var Sql = @"select a.UserId as [Key], a.UserName+'('+b.Name+')' as Value from EM_User_Account a 
join  EM_User_Role b on a.RoleId=b.Id 
           order by  a.UserId ";
            var result =  DapperHelper.SqlQuery<KeyValueVM>(Sql).ToList();
            return result;
        }
    }
    public interface IUserAccountRepo : IRepository<EM_User_Account>
    {


        AccountVM Login(AccountLoginVM accountVM);

        /// <summary>
        /// 记录下登出时间
        /// </summary>
        /// <param name="UserId"></param>
        void LogOff(int UserId);


        Tuple<bool, string> IsRepeat(EM_User_Account model);

        Task<List<AccountDetailDTO>> GetUserList(SystemUserSM sm);
        Task<AccountDetailDTO> GetByIdDto(int UserId);

        string ChangePassword(int UserId,string OPassword,string NPassword);

        bool IsEmailRepeat(string LoginEmail, int UserId);

        bool IsMobileRepeat(string Mobile, int UserId);

        List<KeyValueVM> GetSelectList();
    }
}
