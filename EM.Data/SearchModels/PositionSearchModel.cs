using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;

namespace Topuc22Top.Data.SearchModels
{
    public class PositionSearchModel
    {
        private int _currentstatus = (int)PositionStatus.Publish;

        public int enterpriseId { get; set; }
        //public string keyWord { get; set; }
        public string func { get; set; }
        public string city { get; set; }
        public string posName { get; set; }
        public string posType { get; set; }
        public int currentStatus
        {
            get { return this._currentstatus; }
            set { this._currentstatus = value; }
        }
        public bool HasValue
        {
            get
            {
                return !string.IsNullOrEmpty(func) || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(posName) || !string.IsNullOrEmpty(posType);
            }
        }
    }
}
