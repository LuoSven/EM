using EM.Common;
using EM.Utils;
using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace EM.Model.DTOs
{

    public class ExpenseAccountApproveHistoryListDTO
    {
        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                var StatusType = (ExpenseAccountApproveStatus)Status;
                switch(StatusType)
                {
                    case ExpenseAccountApproveStatus.Created:
                        return "撤回";
                    case ExpenseAccountApproveStatus.FailApproved:
                        return "不通过";
                    case ExpenseAccountApproveStatus.PassApproved:
                        return "通过";
                    case ExpenseAccountApproveStatus.WaitingApprove:
                        return "提交";
                }
                return "";
            }
        }
        public string StatusClass
        {
            get
            {
                var StatusType = (ExpenseAccountApproveStatus)Status;
                switch (StatusType)
                {
                    case ExpenseAccountApproveStatus.Created:
                        return "pullback";
                    case ExpenseAccountApproveStatus.FailApproved:
                        return "fail";
                    case ExpenseAccountApproveStatus.PassApproved:
                        return "approved";
                    case ExpenseAccountApproveStatus.WaitingApprove:
                        return "submit";
                }
                return "";
            }
        }
        public string CreateDateName
        {
            get
            {
                return CreateDate.ToString("yyyy-MM-dd HH:mm");
            }
        }
        public System.DateTime CreateDate { get; set; }
        public string FailReason { get; set; }
        public string Note { get; set; }
        public string Creater { get; set; }

    }
}
