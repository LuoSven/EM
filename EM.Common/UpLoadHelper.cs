using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;

namespace EMTop.Common
{
    public class UpLoadHelper
    {
        /// <summary>
        /// 文件归属
        /// </summary>
        public enum FileOriginal
        {
            Grade = 1//成绩单
        }

        /// <summary>
        /// 上传大小限制
        /// </summary>
        /// <param name="p_FO"></param>
        /// <returns></returns>
        public static int LengthLimit(FileOriginal p_FO)
        {
            int maxLength = 0;
            switch (p_FO)
            {
                case FileOriginal.Grade:
                    maxLength = 500;
                    break;
                default:
                    break;
            }
            return maxLength * 1024;
        }
        /// <summary>
        /// 上传文件(失败则返回Guid.Empty)
        /// </summary>
        /// <param name="p_FU"></param>
        /// <param name="p_UserId"></param>
        /// <param name="p_FO"></param>
        /// <param name="p_Msg"></param>
        /// <returns></returns>
        public static bool IsUpLoadFileSuccessed(FileUpload p_FU, ref string p_Msg)
        {
            int maxLength = LengthLimit(FileOriginal.Grade);
            //获取由客户端指定的上传文件的访问 
            HttpPostedFile upFile = p_FU.PostedFile;
            //得到上传文件的长度 
            int upFileLength = upFile.ContentLength;

            if (upFileLength > maxLength)
            {
                p_Msg = string.Format("上传文件不能大于{0}KB", maxLength / 1024);
                return false;
            }
            return true;
        }
    }
}
