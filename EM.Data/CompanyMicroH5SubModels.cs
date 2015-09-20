using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Data
{
    /// <summary>
    /// 公司微主页H5 背景音乐
    /// </summary>
    public class CompanyMicroH5Music
    {
        public int MusicId { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }

    /// <summary>
    /// 公司微主页H5 背景风格（颜色）
    /// </summary>
    public class CompanyMicroH5BGColor
    {
        public int BGId { get; set; }
        public string Name { get; set; }
        public string RGB { get; set; }
    }

}
