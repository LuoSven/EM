using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class SimpleJobInfo
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public string City { get; set; }

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }

        public string SalaryRange
        {
            get
            {
                string Salary = "";

                if (SalaryMin == -1)
                    Salary = "面议";
                else if (SalaryMin == 0 && SalaryMax == 0)
                    Salary = " ";
                else if (SalaryMin == 0 && SalaryMax > 0)
                    Salary = SalaryMax.ToString() + "及以下";
                else if (SalaryMin > 0 && SalaryMax == 0)
                    Salary = SalaryMin.ToString() + "及以上";
                else
                    Salary = SalaryMin.ToString() + "-" + SalaryMax.ToString();

                if (Salary.Trim() == "")
                {
                    Salary = "面议";
                }

                return Salary;
            }
        }
    }

}
