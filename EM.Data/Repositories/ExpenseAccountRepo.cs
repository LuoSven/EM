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


            var sql = @" select    distinct  case when @CompanyId is null then null else @CompanyId end as SearchCompanyId,a.EANumber, a.Id,a.ApproveStatus,a.ModifyDate,a.Name,a.SumMoney ,a.ApplyDate,a.Creater ,a.IsNotAccount,a.IsPublic from EM_ExpenseAccount a
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
               AddApproveHistory(Id, ApproveStatus, Message, UserName);
           }
           return result;
        }

        public void AddApproveHistory(int Id, int ApproveStatus, string Message, string UserName)
        {
            var ApproveHistory = new EM_ExpenseAccount_ApproveHistory()
            {
                ExpenseAccountId = Id,
                Status = ApproveStatus,
                FailReason = Message,
                Creater = UserName,
                Modifier = UserName,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };
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
            sql += " order by a.EANumber,b.Id   ";
            var list = DapperHelper.SqlQuery<ExpenseAccountExcelDTO>(sql, sm).ToList();
            return list;
        }
        /// <summary>
        /// 生成报销单的where条件
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        private string GetWhere(ExpenseAccountSM sm, AccountVM UserInfo, bool IsFromApprove)
        {
            var sql = "";
            //根据不同的角色类型看到报销单也是不一样的
            switch ((RoleType)Enum.ToObject(typeof(RoleType), UserInfo.RoleType))
            {
                case RoleType.Admin:
                    break;
                case RoleType.CompanyManager:
                    sql += string.Format("and ( b.CompanyId is null or b.CompanyId in ({0})  )", UserInfo.CompanyIds);
                    break;
                case RoleType.Area:
                    sql += " and  a.Creater='" + UserInfo.UserName + "'";
                    break;
                case RoleType.Staff:
                    sql += " and ( b.CompanyId in (" + UserInfo.CompanyIds + ") or a.Creater='" + UserInfo.UserName + "' ) ";
                    break;
            }

            //根据角色进行分类的过滤
            //老板只能看5大类汇总
            //录入人和admin能看所有(admin是拥有父类的权限，录入人只有子类的权限，父类可以同时查看报表)
            sql += @" and  (b.CateId is null or b.CateId in (select * from dbo.FC_GetRoleChildrenCateIds('" + UserInfo.RoleType + "'))) ";
            //审核的是不能看到草稿箱的
            if (IsFromApprove)
            {
                sql += " and a.ApproveStatus != " + (int)ExpenseAccountApproveStatus.Created;
            }
            sql += sm.SearchSql;
            return sql;
        }
        public bool IsCreater(string Ids,string userName)
        {
            return !DapperHelper.SqlQuery<int>(string.Format("select* from EM_ExpenseAccount where Creater <>@userName and Id in ({0})", Ids), new { userName }).Any();
        }

        public string GetEANumber(int Id)
        {
            return DapperHelper.SqlQuery<string>("select EANumber from EM_ExpenseAccount where   Id  =@Id", new { Id }).FirstOrDefault();
       
        }
    }


    public interface IExpenseAccountRepo : IRepository<EM_ExpenseAccount>
    {
        PagedResult<ExpenseAccountListDTO> GetListByDto(ExpenseAccountSM sm, AccountVM UserInfo, int Page, int PageSize, bool IsFromApprove = false);
        /// <summary>
        /// 获取当前报表的导出excel对象
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        List<ExpenseAccountExcelDTO> GetExcelListByDto(ExpenseAccountSM sm, AccountVM UserInfo);

        Task<string> GetNewPublicId();

        bool IsCreater(string Ids, string userName);

        int UpdataApproveStatus(int Id, int ApproveStatus, string Message, string UserName);
        /// <summary>
        /// 添加变更记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ApproveStatus"></param>
        /// <param name="Message"></param>
        /// <param name="UserName"></param>
        void AddApproveHistory(int Id, int ApproveStatus, string Message, string UserName);

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
