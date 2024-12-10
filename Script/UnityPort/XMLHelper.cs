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
        /// ��һ���������л�ΪXML�ַ���
        /// </summary>
        /// <param name="o">Ҫ���л��Ķ���</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>���л�������XML�ַ���</returns>
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
        /// ��һ������XML���л��ķ�ʽд�뵽һ���ļ�
        /// </summary>
        /// <param name="o">Ҫ���л��Ķ���</param>
        /// <param name="path">�����ļ�·��</param>
        /// <param name="encoding">���뷽ʽ</param>
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
        /// ��XML�ַ����з����л�����
        /// </summary>
        /// <typeparam name="T">�����������</typeparam>
        /// <param name="s">���������XML�ַ���</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>�����л��õ��Ķ���</returns>
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
        /// ����һ���ļ�������XML�ķ�ʽ�����л�����
        /// </summary>
        /// <typeparam name="T">�����������</typeparam>
        /// <param name="path">�ļ�·��</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>�����л��õ��Ķ���</returns>
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