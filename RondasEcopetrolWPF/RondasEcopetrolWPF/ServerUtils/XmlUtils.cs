namespace RondasEcopetrolWPF.ServerUtils
{
    using System.Xml;
    public class XmlUtils
    {
        public static void readEndElement(XmlReader reader, string tag)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name.Equals(tag))
                    {
                        break;
                    }
                }
            }
        }
        public static bool readNextElement(XmlReader reader)
        {
            bool result;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
        public static bool readNextTextContent(XmlReader reader)
        {
            bool result;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
        public static string readNextTextContent(XmlReader reader, string tag)
        {
            bool flag = false;
            string text = null;
            string result;
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
                switch (nodeType)
                {
                    case XmlNodeType.Element:
                        if (!reader.Name.Equals(tag))
                        {
                            continue;
                        }
                        if (!reader.IsEmptyElement)
                        {
                            flag = true;
                            continue;
                        }
                        result = "";
                        break;
                    case XmlNodeType.Attribute:
                        continue;
                    case XmlNodeType.Text:
                        if (flag)
                        {
                            text = reader.Value;
                        }
                        continue;
                    default:
                        if (nodeType != XmlNodeType.EndElement)
                        {
                            continue;
                        }
                        if (!reader.Name.Equals(tag))
                        {
                            continue;
                        }
                        result = text;
                        break;
                }
                return result;
            }
            result = null;
            return result;
        }
    }
}
