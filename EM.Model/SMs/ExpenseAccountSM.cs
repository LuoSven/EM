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
        public string KeyWord { get; set; }
        public int? CateId { get; set; }
        public int DateType { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public string Creater { get; set; }
        public string Modifier { get; set; }
        public int? ApproveStatus { get; set; }

        public int? IsNotAccount { get; set; }
        public int? IsPublic { get; set; }
         /// <summary>
         /// 根据传入对象获取对应sql
         /// </summary>
        public string SearchSql
        {
            get
            {
                var dateSqla = " and ( a.{0} >= @SDate and a.{0} <=@EDate)";
                var dateSqlb = " and ( b.{0} >= @SDate and b.{0} <=@EDate)";
                var sql = "";
                if (CompanyId.HasValue)
                    sql += "and  b.CompanyId = @CompanyId ";
                if (!string.IsNullOrEmpty(Creater))
                    sql += " and a.Creater like '%'+@Creater+'%' ";
                if (!string.IsNullOrEmpty(EANumber))
                    sql += " and a.EANumber like '%'+@EANumber+'%' ";
                if (!string.IsNullOrEmpty(KeyWord))
                    sql += " and (  a.Name like N'%'+@KeyWord+'%' or b.Remark like N'%'+@KeyWord+'%' ) ";
                if (CateId.HasValue)
                    sql += " and  b.CateId In (select * from [dbo].[FC_GetChildCateIds](@CateId)) ";
                if (ApproveStatus.HasValue)
                    sql += " and a.ApproveStatus = @ApproveStatus ";
                if (IsNotAccount.HasValue)
                {
                    if (IsNotAccount.Value == 0)
                        sql += " and ( a.IsNotAccount = 0 or a.IsNotAccount is null ) ";
                    else
                        sql += " and a.IsNotAccount = @IsNotAccount ";
                }
                if (IsPublic.HasValue)
                    sql += " and a.IsPublic = @IsPublic ";

                SDate = SDate.HasValue ? SDate : DateTime.Now.AddYears(-10);
                EDate = EDate.HasValue ? EDate : DateTime.Now.AddYears(10);

                if (EDate.HasValue)
                {
                    EDate=new DateTime(EDate.Value.Year, EDate.Value.Month, EDate.Value.Day, 23, 59, 59);
                }
                var dateType = (ExpenseAccountDateType)Enum.ToObject(typeof(ExpenseAccountDateType), DateType);
                switch (dateType)
                {
                    case ExpenseAccountDateType.CreateDate:
                        sql += string.Format(dateSqla, "CreateDate");
                        break;
                    case ExpenseAccountDateType.OccurDate:
                        sql += string.Format(dateSqlb, "OccurDate");
                        break;
                    case ExpenseAccountDateType.ApplyDate:
                        sql += string.Format(dateSqla, "ApplyDate");
                        break;
                    case ExpenseAccountDateType.ModifyDate:
                        sql += string.Format(dateSqla, "ModifyDate");
                        break;
                    case ExpenseAccountDateType.ApproveDate:
                        sql += string.Format(dateSqla, "ApproveDate");
                        break;
                }
                return sql;
            }
        }

    }
}
