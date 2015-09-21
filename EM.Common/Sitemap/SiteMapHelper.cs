using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace EM.Common
{
    public static class SiteMapHelper
    {
        /// <summary>
        /// 创建SiteMap索引文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="sitemapList"></param>
        public static void CreateSiteMapIndex(string path, string fileName, IList<OurSiteMap> siteMapList)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            XmlTextWriter xmlWriter = new XmlTextWriter(path + @"/" + fileName, Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteWhitespace("\n");
            xmlWriter.WriteStartElement("sitemapindex");
            foreach (OurSiteMap sitemap in siteMapList)
            {
                xmlWriter.WriteWhitespace("\n  ");
                xmlWriter.WriteStartElement("sitemap");
                xmlWriter.WriteWhitespace("\n    ");

                xmlWriter.WriteStartElement("loc");
                xmlWriter.WriteValue(sitemap.Loc);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteWhitespace("\n    ");

                xmlWriter.WriteStartElement("lastmod");
                xmlWriter.WriteValue(GetLastModString(sitemap.LastMod));
                xmlWriter.WriteEndElement();
                xmlWriter.WriteWhitespace("\n  ");

                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteWhitespace("\n");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteWhitespace("\n");
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        /// <summary>
        /// 创建SiteMap文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="urlList">地址列表</param>
        public static void CreateSiteMap(string path, string fileName, IList<OurUrl> urlList)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            XmlTextWriter xmlWriter = new XmlTextWriter(path + @"/" + fileName, Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteWhitespace("\n");
            xmlWriter.WriteStartElement("urlset");
            foreach (OurUrl url in urlList)
            {
                xmlWriter.WriteWhitespace("\n  ");
                xmlWriter.WriteStartElement("url");
                xmlWriter.WriteWhitespace("\n    ");

                xmlWriter.WriteStartElement("loc");
                xmlWriter.WriteValue(url.Loc);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteWhitespace("\n    ");

                xmlWriter.WriteStartElement("lastmod");
                xmlWriter.WriteValue(GetLastModString(url.LastMod));
                xmlWriter.WriteEndElement();
                xmlWriter.WriteWhitespace("\n    ");

                xmlWriter.WriteStartElement("changefreq");
                xmlWriter.WriteValue(url.ChangeFreq.ToString().ToLower());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteWhitespace("\n    ");

                xmlWriter.WriteStartElement("priority");
                xmlWriter.WriteValue(url.Priority);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteWhitespace("\n  ");

                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteWhitespace("\n");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteWhitespace("\n");
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        /// <summary>
        /// SiteMap生成时间
        /// </summary>
        /// <param name="lastMod"></param>
        /// <returns></returns>
        private static string GetLastModString(DateTime lastMod)
        {
            return lastMod.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// SiteMap日志
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="Content"></param>
        public static void SiteMapLogger(string logPath,DateTime LogTime,string Content)
        {
            logPath = System.IO.Path.Combine(logPath, LogTime.ToShortDateString());
            if (!System.IO.Directory.Exists(logPath))
            {
                System.IO.Directory.CreateDirectory(logPath);
            }
            logPath = System.IO.Path.Combine(logPath, LogTime.ToShortTimeString().Replace(':', '_') + ".txt");

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(logPath, true))
            {
                file.WriteLine(Content);
            }
        }

        /// <summary>
        /// 压缩文件为gz
        /// </summary>
        /// <param name="fileToCompress">要压缩的文件</param>
        public static void Compress(FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (System.IO.Compression.GZipStream compressionStream = new System.IO.Compression.GZipStream(compressedFileStream, System.IO.Compression.CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
        }
    }
}
