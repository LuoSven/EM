using AutoMapper;
using EM.Common;
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
}
