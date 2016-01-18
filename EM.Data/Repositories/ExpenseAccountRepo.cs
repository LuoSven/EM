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
         
            var dateSql = " and b.{0} >= @SDate and b.{0} <=@EDate";
            var sql =@" select  distinct a.EANumber, a.Id,a.ApproveStatus,a.ModifyDate,a.Name,a.SumMoney ,a.ApplyDate  from EM_ExpenseAccount a
join EM_ExpenseAccount_Detail b on a.Id=b.ExpenseAccountId
where 1=1 ";
            //不是管理员只能看到自己公司的
            if(UserInfo.RoleType!=(int)RoleType.Admin)
            {
              sql+=string.Format("and  b.CompanyId='{0}'  ", UserInfo.CompanyIds);
            }

            //根据角色进行分类的过滤
            //老板只能看5大类汇总
            //录入人和admin能看所有(admin是拥有父类的权限，录入人只有子类的权限，父类可以同时查看报表)
            sql += @" and b.CateId in (select * from dbo.FC_GetRoleChildrenCateIds('" + UserInfo.RoleType + "')) ";

            //公司权限
            //admin和分总都是只能看到自己角色的公司 或者 员工并且选了公司 
            if (UserInfo.RoleType != (int)RoleType.Staff || (UserInfo.RoleType == (int)RoleType.Staff && sm.CompanyId.HasValue))
            {
                sql += " and b.CompanyId in ("+UserInfo.CompanyIds+") ";
            }
            else
            {
                //录入人
                //If(未选择公司)
                if (!sm.CompanyId.HasValue)
                {
                    //{对应的公司Ids 或 自己录入的单据}
                    sql += " and ( b.CompanyId in (" + UserInfo.CompanyIds + ") or a.Creater='"+UserInfo.UserName+"' ) ";
                }

            }

            if (sm.CompanyId.HasValue)
                sql += " and b.CompanyId = @CompanyId ";
            if (!string.IsNullOrEmpty(sm.Creater))
                sql += " and a.Creater like '%'+@Creater+'%' ";
            if (!string.IsNullOrEmpty(sm.EANumber))
                sql += " and a.EANumber like '%'+@EANumber+'%' ";
            if (!string.IsNullOrEmpty(sm.Name))
                sql += " and a.Name like '%'+@Name+'%' ";
            if (sm.CateId.HasValue)
                sql += " and b.CateId In (select * from [dbo].[FC_GetChildCateIds](@CateId)) ";
            if (sm.ApproveStatus.HasValue)
                sql += " and a.ApproveStatus = @ApproveStatus ";

            //审核的是不能看到草稿箱的
            if (IsFromApprove)
            {
                sql += " and a.ApproveStatus != "+(int)ExpenseAccountApproveStatus.Created;
            }

            sm.SDate = sm.SDate.HasValue ? sm.SDate : DateTime.Now.AddYears(-10);
            sm.EDate = sm.EDate.HasValue ? sm.EDate : DateTime.Now.AddYears(10);
            var dateType = (ExpenseAccountDateType)Enum.ToObject(typeof(ExpenseAccountDateType), sm.DateType);
            switch (dateType)
            {
                case ExpenseAccountDateType.CreateDate:
                    sql += string.Format(dateSql, "CreateDate");
                    break;
                case ExpenseAccountDateType.OccurDate:
                    sql += string.Format(dateSql, "OccurDate");
                    break;
                case ExpenseAccountDateType.ApplyDate:
                    sql += string.Format(dateSql, "ApplyDate");
                    break;
                case ExpenseAccountDateType.ModifyDate:
                    sql += string.Format(dateSql, "ModifyDate");
                    break;
            }




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
        public  int UpdataApproveStatus(int Id, int ApproveStatus, string Message,string UserName)
        {
            var sql = "update EM_ExpenseAccount set ApproveStatus=@ApproveStatus ";
            if (ApproveStatus==(int)ExpenseAccountApproveStatus.FailApproved)
            {
                sql += " ,RefusedMessage=@Message ";
            }
            sql += "where Id=@Id ";
           var result= DapperHelper.SqlExecute(sql, new {Id,ApproveStatus,Message });
           if (result>0)
           {
               var ApproveHistory = new EM_ExpenseAccount_ApproveHistory() { 
                   ExpenseAccountId = Id,
                   Status = ApproveStatus,
                   FailReason = Message,
                   Creater = UserName,
                   Modifier = UserName, 
                   CreateDate = DateTime.Now, 
                   ModifyDate = DateTime.Now };
               ApproveHistory.FailReason = ApproveHistory.FailReason ?? "";
               DapperHelper.SqlExecute(@"INSERT INTO EM_ExpenseAccount_ApproveHistory
           (ExpenseAccountId
           ,Status
           ,FailReason
           ,Creater
           ,Modifier
           ,CreateDate
           ,ModifyDate)
     VALUES
           (@ExpenseAccountId
           ,@Status
           ,@FailReason
           ,@Creater
           ,@Modifier
           ,@CreateDate
           ,@ModifyDate)", ApproveHistory);
           }
           return result;
        }
    }


    public interface IExpenseAccountRepo : IRepository<EM_ExpenseAccount>
    {
        PagedResult<ExpenseAccountListDTO> GetListByDto(ExpenseAccountSM sm, AccountVM UserInfo, int Page, int PageSize, bool IsFromApprove = false);

        Task<string> GetNewPublicId();


        int UpdataApproveStatus(int Id, int ApproveStatus, string Message, string UserName);

   
    }
}
