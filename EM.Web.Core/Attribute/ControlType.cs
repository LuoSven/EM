
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Security.Principal;
using EM.Common;
using EM.Web.Core.Base;
using System.Reflection; 
namespace EM.Web.Core
{
    /// <summary>
    /// 表示control的系统类型
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class)]

    public class ControlType : System.Attribute
    {
        public ControlType(SystemType ControlType)
        {
            this.SystemType = ControlType;
        }
        public SystemType SystemType { get; set; }


    }
}