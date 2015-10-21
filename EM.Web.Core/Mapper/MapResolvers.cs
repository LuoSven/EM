using AutoMapper;
using EM.Common;
using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Web.Core
{
    public class AccountStatusResolver : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            return Enum.ToObject(typeof(AccountStatus), source).GetEnumDescription();
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
}
