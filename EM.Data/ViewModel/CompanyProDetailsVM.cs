using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using Topuc22Top.Model.DTOs;
namespace Topuc22Top.Data.ViewModel
{
    public class CompanyProDetailsVM
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string IndustryName { get; set; }
        public string IndustryIDs { get; set; }
        public string ModeName { get; set; }
        public string ScaleName { get; set; }
        public string CityName { get; set; }
        public string CityIDs { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        public string Abbr { get; set; }
        public string CompanyDesc { get; set; }
        public string SourceName { get; set; }

        //视频
        public IList<EtpVideoDTO> Videos { get; set; }
        //相册
        public bool HasAlbum { get; set; }

        //产品
        public IList<TB_Enterprise_Business> Buessinesses { get; set; }
        //欢迎词
        public string Slogan { get; set; }
        //员工成长案例
        public IList<TB_Enterprise_StaffGrowth> Staffs { get; set; }
        //在招职位
        public IList<ZtcPositionInfoVM> Positions { get; set; }
        //对话HR
        public IList<TB_Enterprise_QA> QAs { get; set; }
        //联系我们
        public IList<TB_Enterprise_Address> Contacts { get; set; }



        public string ConsultantName { get; set; }
        public string ConsultantComment { get; set; }

        public CompanyLinkResult Links { get; set; }


        /// <summary>
        /// 是否为激活企业
        /// </summary>
        public bool HasAccount { get; set; }
        
        public int? ConsultantId { get; set; }

        //----Green 2014-7-15 判断是否上传Banner

        //----------2014-7-16 公司亮点
        public IList<string> EnterpriseTags { get; set; }
        //-------------------------------------


        //add 2015 01 15
        public IList<TB_Enterprise_News> News { get; set; }

        public TB_Enterprise_Display Display { get; set; }
    }
}
