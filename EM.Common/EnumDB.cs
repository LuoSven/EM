using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EM.Common
{
 
    /// <summary>
    /// 学校类别
    /// </summary>
    public enum SchoolType
    {
        [Description("综合")]
        A = 1,
        [Description("理工")]
        B,
        [Description("财经")]
        C,
        [Description("师范")]
        D,
        [Description("语言")]
        E,
        [Description("政法")]
        F,
        [Description("民族")]
        G,
        [Description("农林")]
        H,
        [Description("医药")]
        I,
        [Description("艺术")]
        J,
        [Description("体育")]
        K,
        [Description("军事")]
        L
    }

    #region 系统（账号，权限）
    /// <summary>
    /// 账号状态枚举
    /// </summary>
    public enum AccountStatus
    {
        [Description("账号可以登陆")]
        Allow = 1,
        [Description("禁止登陆")]
        Deny,
    }

    /// <summary>
    /// 权限类型枚举
    /// </summary>
    public enum RightType
    {
        [Description("查询类型action")]
        View = 1,
        [Description("增删改类型action")]
        Form,
    }

    /// <summary>
    /// 权限所属系统枚举
    /// </summary>
    public enum SystemType
    {
        [Description("浙江报销系统")]
        ZJ = 1,
        [Description("易捷报销系统")]
        YJ,
    }

    #endregion






}
