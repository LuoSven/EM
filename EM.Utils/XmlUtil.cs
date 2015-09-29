using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EM.Utils
{
    public class XmlUtil
    {
        public static string Serializer<T>(T obj) where T : class
        {
            MemoryStream Stream = new MemoryStream();

            //创建序列化对象
            XmlSerializer xml = new XmlSerializer(obj.GetType());

            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }

            catch (InvalidOperationException)
            {
                return null;
            }

            Stream.Position = 0;

            StreamReader sr = new StreamReader(Stream);

            return sr.ReadToEnd();

        }

        public static T Deserialize<T>(string xml)
        {

            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(sr);
                }

            }

            catch
            {
                return default(T);
            }

        }

        public static void SerializerToXml<T>(T obj, string path) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            string folderpath = Path.GetDirectoryName(path);

            if (!Directory.Exists(folderpath))
                Directory.CreateDirectory(folderpath);



            XmlTextWriter xmlwriter = new XmlTextWriter(path, Encoding.UTF8);
            serializer.Serialize(xmlwriter, obj);
            xmlwriter.Close();
        }

        public static T DeserializeFromXml<T>(string path) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            try
            {
                using (XmlTextReader xmlreader = new XmlTextReader(path))
                {
                    return (T)serializer.Deserialize(xmlreader);
                }
            }
            catch
            {
                return null;
            }
        }


        //public static object Deserialize(Type type, string xml)
        //{

        //    try
        //    {
        //        using (StringReader sr = new StringReader(xml))
        //        {
        //            XmlSerializer xmldes = new XmlSerializer(type);
        //            return xmldes.Deserialize(sr);
        //        }

        //    }

        //    catch (Exception e)
        //    {
        //        return null;
        //    }

        //}

    }
}
