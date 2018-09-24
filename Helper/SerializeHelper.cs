using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace ChatBot.Helper
{
    public class SerializeHelper
    {
        public static T DeserializeFromXML<T>(string xml, XmlAttributeOverrides overrides)
        {
            using (System.IO.StringReader sr = new StringReader(xml))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T), overrides);
                return (T)x.Deserialize(sr);
            }
        }

        public static T DeserializeFromXML<T>(Stream xml)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
            return (T)x.Deserialize(xml);
        }


        /// <summary>
        /// This methods creates xml for this control or those inheriting it.
        /// </summary>
        /// <param name="type">the type of the control you want to serialize</param>
        /// <returns>The XML Text</returns>
        public static string SerializeToXML(object obj, Type type)
        {
            var result = new StringBuilder();

            using (System.IO.StringWriter sw = new StringWriter(result))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(type);
                x.Serialize(sw, obj);
            }

            return result.ToString();
        }
    }
}