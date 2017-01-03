using AutoMapper;
using EM.Common;
using EM.Data.Infrastructure;
using EM.Data.Repositories;
using EM.Model.DTOs;
using EM.Model.Entities;
using EM.Model.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EM.Web.Core
{

    public class ExpenseAccountApproveHistoryResolver : ValueResolver<int,List< ExpenseAccountApproveHistoryListDTO>>
    {
        protected override List<ExpenseAccountApproveHistoryListDTO> ResolveCore(int source)
        {
            IExpenseAccountApproveHistoryRepo expenseAccountApproveHistoryRepo = new ExpenseAccountApproveHistoryRepo(new DatabaseFactory());
            return expenseAccountApproveHistoryRepo.GetListStringByECId(source);
        }
    }
    public class ExpenseAccountDetailsResolver : ValueResolver<ExpenseAccountListDTO, List<ExpenseAccountDetailListDTO>>
    {
        protected override List<ExpenseAccountDetailListDTO> ResolveCore(ExpenseAccountListDTO source)
        {
            IExpenseAccountDetailRepo expenseAccountDetailRepo = new ExpenseAccountDetailRepo(new DatabaseFactory());
            if (source.SearchCompanyId.HasValue)
            {
                return expenseAccountDetailRepo.GetListDtoByExpenseAccountId(source.Id, source.SearchCompanyId.Value.ToString(), ViewHelp.GetUserName());
            }
            else
            {  
              return expenseAccountDetailRepo.GetListDtoByExpenseAccountId(source.Id, ViewHelp.GetDetailCompanyIds(),ViewHelp.GetUserName());
            }
        }
    }
    public class AccountStatusResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            return Enum.ToObject(typeof(AccountStatus), source).GetEnumDescription();
        }
    }
    public class ApproveStatusResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            return Enum.ToObject(typeof(ExpenseAccountApproveStatus), source).GetEnumDescription();
        }
    }
    public class SeasonTypeResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            return Enum.ToObject(typeof(SeasonTypeEnum), source).GetEnumDescription();
        }
    }
    public class RoleTypeResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            return Enum.ToObject(typeof(RoleType), source).GetEnumDescription();
        }
    }

    public class ActionNameResolver : ValueResolver<UserRoleProgramDTO, string>
    {
        protected override string ResolveCore(UserRoleProgramDTO source)
        {
            var RightTypeInt = Convert.ToInt32(source.RightType);
            var ActionTypeName = Enum.ToObject(typeof(RightType), RightTypeInt).GetEnumDescription();
            var result = source.ActionDescription + "(" + ActionTypeName + ")";
            return result;
        }
    }

    public class CateNameResolver : ValueResolver<int,string >
    {
        protected override string ResolveCore(int source)
        {
            IChargeCateRepo changeCateRepo = new ChargeCateRepo(new DatabaseFactory());
            return changeCateRepo.GetCateName(source);
        }
    }
    public class CompanyNameResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
            return companyRepo.GetCompanyName(source);
        }
    }

    public class CompanysNameResolver : ValueResolver<string, string>
    {
        protected override string ResolveCore(string source)
        {
            ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
            return companyRepo.GetCompanysName(source,",");
        }
    }

    public class CompanyCateLimitResolver : ValueResolver<EM_ExpenseAccount_Detail, string>
    {
        protected override string ResolveCore(EM_ExpenseAccount_Detail source)
        {
            var companyLimitRepo = new CompanyLimitRepo(new DatabaseFactory());
            var limit=   companyLimitRepo.GetCompanyLimit(source.CompanyId, source.CateId,source.OccurDate.Year);
            var result = string.Format("{0}({1}/{2})", limit.CateName, limit.TotalCost, limit.TotalLimit);
            return result;
        }
    }

    public class EANumberResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            var expenseAccountRepo = new ExpenseAccountRepo(new DatabaseFactory());
            var EANumber = expenseAccountRepo.GetEANumber(source);
            return EANumber;
        }
    }

    public class UserNameResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            var userAccountRepo = new UserAccountRepo(new DatabaseFactory());
            var UserName = userAccountRepo.GetNameAndRole(source);
            return UserName;
        }
    }

    public class MessageTypeResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            return Enum.ToObject(typeof(MessageType), source).GetEnumDescription();
        }
    }



}
