using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using System.Xml;

namespace EM.Utils
{
    public class ObjectStrConvert
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(Object obj)
        {
            IFormatter ifor = new BinaryFormatter();
            string str = "";
            using (MemoryStream sm = new MemoryStream())
            {
                ifor.Serialize(sm, obj);                        //序列化
                sm.Seek(0, SeekOrigin.Begin);

                byte[] bytes = new byte[sm.Length];
                bytes = sm.ToArray();
                str = Convert.ToBase64String(bytes);
                str = HttpContext.Current.Server.UrlEncode(str);//编码
            }
            return str;
        }

        #region 反序列化字符串为对象
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <returns></returns>
        public static object DeserializeObject(string str)
        {
            str = HttpContext.Current.Server.UrlDecode(str);    //解码
            byte[] bt = Convert.FromBase64String(str);
            Stream smNew = new MemoryStream(bt);
            IFormatter fmNew = new BinaryFormatter();
            object obj = fmNew.Deserialize(smNew);              //反序列化
            return obj;
        }
        #endregion


        /**/
        /// <summary>
        /// 对象序列化，得到Base64字符串
        /// </summary>
        /// <param name="Obj">待序列化的对象</param>
        /// <returns></returns>
        public static string ObjectToBase64Str(Object Obj)
        {
            return Convert.ToBase64String(ObjToBytes(Obj));


        }

        /**/
        /// <summary>
        /// Base64字符串反序列化，得到对象
        /// </summary>
        /// <param name="ScrStr">待反序列化的字符串</param>
        /// <param name="ObjType">对象类型</param>
        /// <returns></returns>
        public static Object Base64StrToObject(string base64Str, Type ObjType)
        {
            if (base64Str != null)
                return BytesToObj(Convert.FromBase64String(base64Str), ObjType);
            return null;
        }

        /**/
        /// <summary>
        /// 对象序列化，得到字符串
        /// </summary>
        /// <param name="Obj">待序列化的对象</param>
        /// <returns></returns>
        public static string ObjectToString(Object Obj)
        {
            return Encoding.Default.GetString(ObjToBytes(Obj));
        }

        /**/
        /// <summary>
        /// 字符串反序列化，得到对象
        /// </summary>
        /// <param name="ScrStr">待反序列化的字符串</param>
        /// <param name="ObjType">对象类型</param>
        /// <returns></returns>
        public static Object StringToObject(string ScrStr, Type ObjType)
        {
            return BytesToObj(Encoding.Default.GetBytes(ScrStr), ObjType);
        }

        /**/
        /// <summary>
        /// 将对象序列化为字节数组
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        private static byte[] ObjToBytes(Object Obj)
        {
            XmlSerializer ser = new XmlSerializer(Obj.GetType());
            MemoryStream mem = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mem, Encoding.Default);
            ser.Serialize(writer, Obj);
            writer.Close();

            return mem.ToArray();
        }

        /**/
        /// <summary>
        /// 从字节数组反序列化得到对象
        /// </summary>
        /// <param name="ObjBytes"></param>
        /// <returns></returns>
        private static Object BytesToObj(byte[] ObjBytes, Type ObjType)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(ObjType);
                MemoryStream mem = new MemoryStream(ObjBytes);
                StreamReader sr = new StreamReader(mem, Encoding.Default);
                return ser.Deserialize(sr);
            }
            catch
            {
                return null;
            }
        }
    }
}
