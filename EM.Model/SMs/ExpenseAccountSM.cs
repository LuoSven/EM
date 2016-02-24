using EM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
     public   class ExpenseAccountSM
    {
        public string EANumber { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public int? CateId { get; set; }
        public int DateType { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public string Creater { get; set; }
        public string Modifier { get; set; }
        public string CompanyIds { get; set; }
        public int? ApproveStatus { get; set; }
         /// <summary>
         /// 根据传入对象获取对应sql
         /// </summary>
        public string SearchSql
        {
            get
            {
                var dateSql = " and ( b.{0} is null or  b.{0} >= @SDate and b.{0} <=@EDate)";
                var sql = "";
                if (CompanyId.HasValue)
                    sql += " (b.CompanyId  is null or and b.CompanyId = @CompanyId )";
                if (!string.IsNullOrEmpty(Creater))
                    sql += " and a.Creater like '%'+@Creater+'%' ";
                if (!string.IsNullOrEmpty(EANumber))
                    sql += " and a.EANumber like '%'+@EANumber+'%' ";
                if (!string.IsNullOrEmpty(Name))
                    sql += " and a.Name like '%'+@Name+'%' ";
                if (CateId.HasValue)
                    sql += " and  ( b.CateId is null or b.CateId In (select * from [dbo].[FC_GetChildCateIds](@CateId))) ";
                if (ApproveStatus.HasValue)
                    sql += " and a.ApproveStatus = @ApproveStatus ";
                SDate = SDate.HasValue ? SDate : DateTime.Now.AddYears(-10);
                EDate = EDate.HasValue ? EDate : DateTime.Now.AddYears(10);
                var dateType = (ExpenseAccountDateType)Enum.ToObject(typeof(ExpenseAccountDateType), DateType);
                switch (dateType)
                {
                    case ExpenseAccountDateType.CreateDate:
                        sql += string.Format(dateSql, "CreateDate");
                        break;
                    case ExpenseAccountDateType.OccurDate:
                        sql += string.Format(dateSql, "OccurDate");
                        break;
                    case ExpenseAccountDateType.ApplyDate:
                        sql += string.Format(dateSql, "ApplyDate");
                        break;
                    case ExpenseAccountDateType.ModifyDate:
                        sql += string.Format(dateSql, "ModifyDate");
                        break;
                }
                return sql;
            }
        }

    }
}
