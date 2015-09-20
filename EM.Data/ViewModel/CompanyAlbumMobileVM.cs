using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ViewModel
{
    public class CompanyAlbumMobileVM
    {

        public CompanyDetailsMobileVM CompanyDetails { get; set; }

        public Dictionary<string, List<Photo>> AlbumGroup { get; set; }
    }
}
