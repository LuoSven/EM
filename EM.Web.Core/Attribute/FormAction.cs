
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Security.Principal;

namespace EM.Web.Core.Attribute
{
    /// <summary>
    /// 表示Action是动作，比如添加，删除，修改记录，提交表单的接口
    /// </summary>
    public class FormAction : System.Attribute
    {
    }
}