using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class CompanyAlbumResult
    {
        public int AlbumId { get; set; }
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public int? CoverId { get; set; }
        public int PhotoCount { get; set; }
        public string CoverPath { get; set; }
    }
}
