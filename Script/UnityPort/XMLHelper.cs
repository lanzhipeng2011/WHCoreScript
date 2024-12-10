using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Module.Log;

namespace UnityPort
{
    public static class XmlHelper
    {
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o, namespaces);
                writer.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializeInternal(stream, o, encoding);

                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (System.Exception e)
            {
                LogModule.ErrorLog("Serialize object to string error " + " e: " + e.ToString());
                return "";
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path");

                using (FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializeInternal(file, o, encoding);
                    file.Close();
                }
            }
            catch (System.Exception e)
            {
                LogModule.ErrorLog("Serialize object to file error: " + path + " e: " + e.ToString());
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                    throw new ArgumentNullException("s");
                if (encoding == null)
                    throw new ArgumentNullException("encoding");

                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
                {
                    using (StreamReader sr = new StreamReader(ms, encoding))
                    {
                        return (T)mySerializer.Deserialize(sr);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogModule.ErrorLog("Deserialize object from string error " + " e: " + e.ToString());
                return default(T);
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    return default(T);
                }
                
                if (encoding == null)
                    throw new ArgumentNullException("encoding");

                StreamReader objReader = new StreamReader(path, encoding);
                string xml = objReader.ReadToEnd();
                objReader.Close();

                return XmlDeserialize<T>(xml, encoding);
            }
            catch (System.Exception e)
            {
                LogModule.ErrorLog("Deserialize object from file error: " + path + " e: " + e.ToString());
                return default(T);
            }
        }
    }



}