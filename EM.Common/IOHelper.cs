using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Logger;

namespace EMTop.Common
{
    public class IOHelper
    {
        public static void CopyDir(string srcPath, string aimPath)
        {

            if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                aimPath += Path.DirectorySeparatorChar;
            }
            if (!Directory.Exists(aimPath))
            {
                Directory.CreateDirectory(aimPath);
            }
            string[] fileList = Directory.GetFileSystemEntries(srcPath);
            foreach (string file in fileList)
            {
                if (Directory.Exists(file))
                    CopyDir(file, aimPath + Path.GetFileName(file));
                else
                    System.IO.File.Copy(file, aimPath + Path.GetFileName(file), true);
            }

        }
    }
}
