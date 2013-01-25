using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

//#if WINDOWS
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
//#endif

namespace Gnomic.Util
{
    public static class XmlSerializerUtil
    {
        public static void Save<T>(string relativeFileName, T obj)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.NewLineChars = "\n";

            using (Stream strm = TitleContainer.OpenStream(relativeFileName))
            {
                using (XmlWriter outXml = XmlWriter.Create(strm, xmlSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(outXml, obj);
                }
            }
        }

        public static T Load<T>(string relativeFileName) where T : class
        {
            using (Stream strm = TitleContainer.OpenStream(relativeFileName))
            {
                using (XmlReader inXml = XmlReader.Create(strm))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(inXml);
                }
            }
        }

        public static T Load<T>(Stream strm) where T : class
        {
            using (XmlReader inXml = XmlReader.Create(strm))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(inXml);
            }
        }

#if WINDOWS
        public static void SaveAbsolute<T>(string fileName, T obj)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.NewLineChars = "\n";
            using (XmlWriter outXml = XmlWriter.Create(fileName, xmlSettings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(outXml, obj);
            }
        }

        public static T LoadAbsolute<T>(string fileName) where T : class
        {
            using (XmlReader inXml = XmlReader.Create(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(inXml);
            }
        }
#endif

        //#if WINDOWS
        //        public static T Clone<T>(T toClone) where T : class
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            using (XmlWriter outXml = XmlWriter.Create(sb))
        //            {
        //                 IntermediateSerializer.Serialize(outXml, toClone, null);
        //            }

        //            using (XmlReader inXml = XmlReader.Create(new StringReader(sb.ToString())))
        //            {
        //                 return (T)IntermediateSerializer.Deserialize<T>(inXml, null);
        //            }
        //        }
        //#else
        //        public static T Clone<T>(T toClone) where T : class
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            XmlSerializer serializer = new XmlSerializer(typeof(T));
        //            using (XmlWriter outXml = XmlWriter.Create(sb))
        //            {
        //                serializer.Serialize(outXml, obj);
        //            }

        //            using (XmlReader inXml = XmlReader.Create(new StringReader(sb.ToString())))
        //            {
        //                return (T)serializer.Deserialize(inXml);
        //            }
        //        }
        //#endif


    }
}
