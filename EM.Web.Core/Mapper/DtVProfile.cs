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


        }
    }
}

