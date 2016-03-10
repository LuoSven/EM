using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;
using EM.Model.DTOs;
using EM.Model.VMs;
using EM.Model.Entities;
using EM.Data.Infrastructure;

namespace EM.Web.Core
{
    public class DtVProfile : Profile
    {

      protected override void Configure()
    {
            Mapper.CreateMap<AccountDetailDTO, AccountDetailVM>()
            .ForMember(dest => dest.StatusName, source => source.ResolveUsing<AccountStatusResolver>().FromMember(o=>o.Status));


            Mapper.CreateMap<AccountDetailVM, EM_User_Account>();
            Mapper.CreateMap<UserRoleProgramDTO, UserRoleProgramVM>()
       .ForMember(dest => dest.ActionName, source => source.ResolveUsing<ActionNameResolver>());

            Mapper.CreateMap<ExpenseAccountListDTO, ExpenseAccountListVM>()
            .ForMember(dest => dest.ApproveStatusName, source => source.ResolveUsing<ApproveStatusResolver>().FromMember(o => o.ApproveStatus))
            .ForMember(dest => dest.List, source => source.ResolveUsing<ExpenseAccountDetailsResolver>())
            .ForMember(dest => dest.ApproveList, source => source.ResolveUsing<ExpenseAccountApproveHistoryResolver>().FromMember(o => o.Id));


            Mapper.CreateMap<CompanyLimitDTO, CompanyLimitVM>()
            .ForMember(dest => dest.SeasonTypeName, source => source.ResolveUsing<SeasonTypeResolver>().FromMember(o => o.SeasonType));

            Mapper.CreateMap<EM_ExpenseAccount, EM_ExpenseAccount>()
            .ForMember(o=>o.Creater,s=>s.Ignore())
            .ForMember(o => o.CreateDate, s => s.Ignore());

            Mapper.CreateMap<ExpenseAccountFileDTO, ExpenseAccountFileVM>();
            Mapper.CreateMap<CompanyPerformanceDTO, CompanyPerformanceVM>();
          
            Mapper.CreateMap<EM_Company_Performance, EM_Company_Performance>()
            .ForMember(o=>o.Creater,s=>s.Ignore())
            .ForMember(o => o.CreateDate, s => s.Ignore());

            Mapper.CreateMap<EM_ExpenseAccount_Detail, EM_ExpenseAccount_Detail>()
            .ForMember(o => o.Creater, s => s.Ignore())
            .ForMember(o => o.CreateTime, s => s.Ignore());


            Mapper.CreateMap<EM_Company_Limit, EM_Company_Limit>()
            .ForMember(o => o.Creater, s => s.Ignore())
            .ForMember(o => o.CreateDate, s => s.Ignore());

            Mapper.CreateMap<EM_User_Role, EM_User_Role>();

            Mapper.CreateMap<UserRoleListDTO, UserRoleListVM>()
            .ForMember(d => d.CompanyNames, source => source.ResolveUsing<CompanysNameResolver>().FromMember(s => s.CompanyIds))
            .ForMember(d => d.RoleTypeName, source => source.ResolveUsing<RoleTypeResolver>().FromMember(s => s.RoleType));

            Mapper.CreateMap<EM_ExpenseAccount_Detail, ExpenseAccountDetailListDTO>()
                .ForMember(d => d.CateName, source => source.ResolveUsing<CateNameResolver>().FromMember(s => s.CateId))
                .ForMember(d => d.LimitInfo, source => source.ResolveUsing<CompanyCateLimitResolver>())
                .ForMember(d => d.CompanyName, source => source.ResolveUsing<CompanyNameResolver>().FromMember(s => s.CompanyId));

            Mapper.CreateMap<EM_User_Account, EM_User_Account>()
            .ForMember(o => o.CreateTime, s => s.Ignore());

            Mapper.CreateMap<EM_Company, EM_Company>()
            .ForMember(o => o.Creater, s => s.Ignore())
            .ForMember(o => o.CreateDate, s => s.Ignore());


            Mapper.CreateMap<ExpenseAccountMonthCateDTO, ExpenseAccountMonthCateVM>();

            Mapper.CreateMap<ExpenseAccountExcelDTO, ExpenseAccountExcelVM>();

            Mapper.CreateMap<KeyValuePair<string, string>, KeyValueVM>()
            .ForMember(o => o.Key, s => s.MapFrom(sm => sm.Key))
            .ForMember(o => o.Value, s => s.MapFrom(sm => sm.Value))
            ;

            Mapper.CreateMap<EM_ExpenseAccount_Detail, CompanyLimitDetailDTO>()
                .ForMember(d => d.CateName, source => source.ResolveUsing<CateNameResolver>().FromMember(s => s.CateId))
                .ForMember(d => d.EANumber, source => source.ResolveUsing<EANumberResolver>().FromMember(s => s.ExpenseAccountId))
                .ForMember(d => d.CompanyName, source => source.ResolveUsing<CompanyNameResolver>().FromMember(s => s.CompanyId));

            //mirkmf110@163.com‖lsw13003213417‖180.155.79.192‖Windows 8‖Chrome41.0
            Mapper.CreateMap<UserLoginRecordDTO, UserLoginRecordVM>()
                .ForMember(o => o.LoginEmail, s => s.MapFrom(sm => string.IsNullOrEmpty(sm.LoginInfo) ? "" : sm.LoginInfo.Split('‖')[0]))
                .ForMember(o => o.Password, s => s.MapFrom(sm => string.IsNullOrEmpty(sm.LoginInfo) ? "" : sm.LoginInfo.Split('‖')[1]))
                .ForMember(o => o.LoginIp, s => s.MapFrom(sm => string.IsNullOrEmpty(sm.LoginInfo) ? "" : sm.LoginInfo.Split('‖')[2]))
                .ForMember(o => o.LoginSystem, s => s.MapFrom(sm => string.IsNullOrEmpty(sm.LoginInfo) ? "" : sm.LoginInfo.Split('‖')[3]))
                .ForMember(o => o.LoginBrower, s => s.MapFrom(sm => string.IsNullOrEmpty(sm.LoginInfo) ? "" : sm.LoginInfo.Split('‖')[4]));


            Mapper.CreateMap<SystemFeedbackDTO, SystemFeedbackVM>()
                .ForMember(o => o.IsReply, s => s.MapFrom(sm =>sm.ReplyDate.HasValue || !string.IsNullOrEmpty(sm.ReplyMessage)?true:false));

        }
    }
}

