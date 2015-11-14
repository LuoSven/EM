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
            .ForMember(dest => dest.ApproveStatusName, source => source.ResolveUsing<ApproveStatusResolver>().FromMember(o => o.ApproveStatus));
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

        }
    }
}

