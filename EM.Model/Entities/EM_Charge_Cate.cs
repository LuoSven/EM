//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EM.Model.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class EM_Charge_Cate
    {
        public int Id { get; set; }
        public string CateName { get; set; }
        public int CateType { get; set; }
        public Nullable<int> ParentId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
    }
}
