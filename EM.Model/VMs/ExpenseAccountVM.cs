using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
    public class ExpenseAccountListVM : ExpenseAccountListDTO
    {
        public string ApproveStatusName { get; set; }

        public List<ExpenseAccountDetailListDTO> List { get; set; }



        public List<ExpenseAccountApproveHistoryListDTO> ApproveList { get; set; } 
    }
}
