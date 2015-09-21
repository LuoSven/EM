using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Common
{
    public class ResourceHelper
    {
#if DEBUG
        public static string TopU = AppDomain.CurrentDomain.BaseDirectory + "..\\EM.TopU";
#else
        public static string TopU=AppDomain.CurrentDomain.BaseDirectory+"..\\TopU";
#endif

//#if DEBUG
//        public static string TopUDomain = "http://localhost:2062";
//#else
//        public static string TopUDomain="http://www.zheyibu.com";
//#endif


//#if DEBUG
//        public static string CorporationDomain = "http://localhost:4182";
//#else
//        public static string CorporationDomain="http://enterprise.zheyibu.com";
//#endif

    }
}
