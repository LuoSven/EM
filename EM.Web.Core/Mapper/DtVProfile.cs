using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;
using EM.Model.DTOs;
using EM.Model.VMs;

namespace EM.Web.Core
{
    public class DtVProfile : Profile
    {

      protected override void Configure()
    {
            Mapper.CreateMap<AccountDetailDTO, AccountDetailVM>()
            .ForMember(dest => dest.StatusName, source => source.ResolveUsing<AccountStatusResolver>().FromMember(o=>o.Status));
        }
    }
}

