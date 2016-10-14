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
    public class ExpenseAccountRepo : RepositoryBase<EM_ExpenseAccount>, IExpenseAccountRepo
    {
        public ExpenseAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public  PagedResult<ExpenseAccountListDTO> GetListByDto(ExpenseAccountSM sm, AccountVM UserInfo, int Page, int PageSize, bool IsFromApprove = false)
        {


            var sql = @" select    distinct    @CompanyId   SearchCompanyId,a.EANumber, a.Id,a.ApproveStatus,a.ModifyDate,a.Name,a.SumMoney ,a.ApplyDate,a.Creater ,a.IsNotAccount,a.IsPublic from EM_ExpenseAccount a
left join EM_ExpenseAccount_Detail b on a.Id=b.ExpenseAccountId
where 1=1 ";
            sql += GetWhere(sm, UserInfo, IsFromApprove);
            var list =  DapperHelper.QueryWithPage<ExpenseAccountListDTO>(sql, sm, " ModifyDate desc ", Page, PageSize);
            return list;
        }

        public async Task<string> GetNewPublicId()
        {
           var  EANumber=(await  DapperHelper.SqlQueryAsync<string>("select top 1 EANumber from EM_ExpenseAccount where EANumber like '对公%' order by Id desc")).FirstOrDefault();
           EANumber=EANumber??"";
            var MaxNo = 0;
           int.TryParse(EANumber.Replace("对公", ""), out MaxNo);
            var NewNo="对公"+( MaxNo + 1).ToString();
            return NewNo;
        }
        public int UpdataApproveStatus(int Id, int ApproveStatus, string Message, string UserName, string Note = "")
        {
            var sql = "update EM_ExpenseAccount set ApproveStatus=@ApproveStatus ";
            if (ApproveStatus==(int)ExpenseAccountApproveStatus.FailApproved)
            {
                sql += " ,RefusedMessage=@Message ";
                sql += " ,ApproveDate=null ";
            }
            if (ApproveStatus == (int)ExpenseAccountApproveStatus.PassApproved)
            {
                sql += " ,ApproveDate=@Now ";
            }
            sql += " where Id=@Id ";
           var result= DapperHelper.SqlExecute(sql, new {Id,ApproveStatus,Message,Now=DateTime.Now });
           if (result>0)
           {
               AddApproveHistory(Id, ApproveStatus, Message, UserName, Note);
           }
           return result;
        }

        public void AddApproveHistory(int Id, int ApproveStatus, string Message, string UserName, string Note )
        {
            var ApproveHistory = new EM_ExpenseAccount_ApproveHistory()
            {
                ExpenseAccountId = Id,
                Status = ApproveStatus,
                FailReason = Message,
                Creater = UserName,
                Modifier = UserName,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Note=Note,
            };
            ApproveHistory.FailReason = ApproveHistory.FailReason ?? "";
            DapperHelper.SqlExecute(@"INSERT INTO EM_ExpenseAccount_ApproveHistory
           (ExpenseAccountId
           ,Status
           ,FailReason
           ,Creater
           ,Modifier
           ,CreateDate
           ,ModifyDate
           ,Note)
     VALUES
           (@ExpenseAccountId
           ,@Status
           ,@FailReason
           ,@Creater
           ,@Modifier
           ,@CreateDate
           ,@ModifyDate
           ,@Note)", ApproveHistory);
        }

        public List<ExpenseAccountMonthCateDTO> GetMonthCateList(MonthExpenseStatisticsSM sm)
        {
            var sql=@" select  SUM(Money) as SumMoney ,c.CateName , year(OccurDate) as ECYear ,month(OccurDate)  as ECMonth from EM_ExpenseAccount_Detail a
join EM_Charge_Cate b on a.CateId=b.Id
join EM_Charge_Cate c on b.ParentId=c.Id
where CateId in (select * from dbo.FC_GetRoleChildrenCateIds(@RoleType))
and a.CompanyId in ({0}) ";
            
            sql=string.Format(sql,sm.CompanyIds);
            if (sm.SDate.HasValue)
            {
                sql += " and a.OccurDate>=@SDate ";
            }
            if (sm.EDate.HasValue)
            {
                sql += " and a.OccurDate<=@EDate ";
            }
              sql+=@" group by c.CateName, year(OccurDate),month(OccurDate) order by ECYear, ECMonth";
          var result=  DapperHelper.SqlQuery<ExpenseAccountMonthCateDTO>(sql,sm).ToList();
          return result;
        }


        public List<ExpenseAccountExcelDTO> GetExcelListByDto(ExpenseAccountSM sm, AccountVM UserInfo)
        {


            var sql = @" select  b.OccurDate,a.Name,a.EANumber,c.CateName,b.Remark,b.Money  from EM_ExpenseAccount a
left join EM_ExpenseAccount_Detail b on a.Id=b.ExpenseAccountId
left join EM_Charge_Cate c on b.CateId=c.Id
where 1=1 ";
            sql += GetWhere(sm, UserInfo, false);
            sql += " order by a.ModifyDate desc  ";
            var list = DapperHelper.SqlQuery<ExpenseAccountExcelDTO>(sql, sm).ToList();
            return list;
        }
        /// <summary>
        /// 生成报销单的where条件
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GetWhere(ExpenseAccountSM sm, AccountVM userInfo, bool isFromApprove)
        {
            var sql = "";
            //根据不同的角色类型看到报销单也是不一样的
            switch ((RoleType)Enum.ToObject(typeof(RoleType), userInfo.RoleType))
            {
                case RoleType.Admin:
                    break;
                case RoleType.CompanyManager:
                    sql += string.Format("and ( b.CompanyId is null or b.CompanyId in ({0})  )", userInfo.CompanyIds);
                    break;
                case RoleType.Area:
                    sql += " and  a.Creater='" + userInfo.UserName + "'";
                    break;
                case RoleType.Staff:
                    sql += " and ( b.CompanyId in (" + userInfo.CompanyIds + ") or a.Creater='" + userInfo.UserName + "' ) ";
                    break;
            }


            //根据不同的角色类型看到报销单也是不一样的
            switch ((RoleViewRightType)Enum.ToObject(typeof(RoleViewRightType), userInfo.ViewRightType))
            {
                case RoleViewRightType.All:
                    break;
                case RoleViewRightType.Owner:
                    sql += " and  a.Creater='" + userInfo.UserName + "'  ";
                    break;
                case RoleViewRightType.OwnerAndCompany:
                    sql += " and ( b.CompanyId in (" + userInfo.CompanyIds + ") or a.Creater='" + userInfo.UserName + "' ) ";
                    break;
            }



            //根据角色进行分类的过滤
            //老板只能看5大类汇总
            //录入人和admin能看所有(admin是拥有父类的权限，录入人只有子类的权限，父类可以同时查看报表)
            sql += @" and  (b.CateId is null or b.CateId in (select * from dbo.FC_GetRoleChildrenCateIds('" + userInfo.RoleType + "')))   ";

            //添加部分特殊分类的逻辑
            if(!string.IsNullOrEmpty(userInfo.CateIds))
            {
                sql += @" and  b.CateId in("+userInfo.CateIds+")   ";  
            }
            //审核的是不能看到草稿箱的
            if (isFromApprove)
            {
                sql += " and a.ApproveStatus != " + (int)ExpenseAccountApproveStatus.Created;
            }
            sql += sm.SearchSql;
            return sql;
        }
        public bool IsCreater(string Ids,string userName)
        {
            var roleId = DapperHelper.SqlQuery<int>("select  RoleId from EM_User_Account where UserName=@userName", new { userName }).FirstOrDefault();
            if (StaticKey.AdminRoleIds.Contains(roleId))
                return true;
            return !DapperHelper.SqlQuery<int>(string.Format("select 1 from EM_ExpenseAccount where Creater <>@userName and Id in ({0})", Ids), new { userName }).Any();
        }

        public string GetEANumber(int Id)
        {
            return DapperHelper.SqlQuery<string>("select EANumber from EM_ExpenseAccount where   Id  =@Id", new { Id }).FirstOrDefault();
       
        }

        public PagedResult<ExpenseAccountListDTO> GetLostAccountListByDto(ExpenseAccountSM sm, AccountVM UserInfo, int Page, int PageSize)
        {
            var sql = @" select    distinct    @CompanyId   SearchCompanyId,a.EANumber, a.Id,a.ApproveStatus,a.ModifyDate,a.Name,a.SumMoney ,a.ApplyDate,a.Creater ,a.IsNotAccount,a.IsPublic from EM_ExpenseAccount a
left join EM_ExpenseAccount_Detail b on a.Id=b.ExpenseAccountId
where  b.id is null ";
            if (UserInfo.RoleType !=(int) RoleType.Admin)
            {
                sql += string.Format("  and a.Creater='{0}'" + UserInfo.UserName);
            }
            var list = DapperHelper.QueryWithPage<ExpenseAccountListDTO>(sql, sm, " ModifyDate desc ", Page, PageSize);
            return list;
        }
         
    }


    public interface IExpenseAccountRepo : IRepository<EM_ExpenseAccount>
    {
        PagedResult<ExpenseAccountListDTO> GetListByDto(ExpenseAccountSM sm, AccountVM UserInfo, int Page, int PageSize, bool IsFromApprove = false);


   
        /// <summary>
        /// 丢单找回
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="UserInfo"></param>
        /// <param name="Page"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        PagedResult<ExpenseAccountListDTO> GetLostAccountListByDto(ExpenseAccountSM sm, AccountVM UserInfo, int Page, int PageSize);
        /// <summary>
        /// 获取当前报表的导出excel对象
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        List<ExpenseAccountExcelDTO> GetExcelListByDto(ExpenseAccountSM sm, AccountVM UserInfo);

        Task<string> GetNewPublicId();
        /// <summary>
        /// 判断是否是创建者，管理员默认就是创建者
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool IsCreater(string Ids, string userName);

        /// <summary>
        /// 更新表单状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ApproveStatus"></param>
        /// <param name="Message"></param>
        /// <param name="UserName"></param>
        /// <param name="Note"></param>
        /// <returns></returns>
        int UpdataApproveStatus(int Id, int ApproveStatus, string Message, string UserName,string Note="");
        /// <summary>
        /// 添加变更记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ApproveStatus"></param>
        /// <param name="Message"></param>
        /// <param name="UserName"></param>
        void AddApproveHistory(int Id, int ApproveStatus, string Message, string UserName, string Note);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        List<ExpenseAccountMonthCateDTO> GetMonthCateList(MonthExpenseStatisticsSM sm);
        /// <summary>
        /// 根据Id获取报销号
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        string GetEANumber(int Id);
    }
}
