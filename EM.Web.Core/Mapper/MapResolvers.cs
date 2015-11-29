using AutoMapper;
using EM.Common;
using EM.Data.Infrastructure;
using EM.Data.Repositories;
using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EM.Web.Core
{

    public class ExpenseAccountDetailsResolver : ValueResolver<int, List<ExpenseAccountDetailListDTO>>
    {
        protected override List<ExpenseAccountDetailListDTO> ResolveCore(int source)
        {
            IExpenseAccountDetailRepo expenseAccountDetailRepo = new ExpenseAccountDetailRepo(new DatabaseFactory());
            return expenseAccountDetailRepo.GetListByExpenseAccountId(source);
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
}
