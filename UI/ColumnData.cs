using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LiveSplit.UI
{
    public class ColumnData
    {
        public String Name { get; set; }
        public ColumnType Type { get; set; }
        public String Comparison { get; set; }
        public String TimingMethod { get; set; }

        public ColumnData(String name, ColumnType type, String comparison, String method)
        {
            Name = name;
            Type = type;
            Comparison = comparison;
            TimingMethod = method;
        }

        public static ColumnData FromXml(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version;
            if (element["Version"] != null)
                version = Version.Parse(element["Version"].InnerText);
            else
                version = new Version(1, 0, 0, 0);
            return new ColumnData(element["Name"].InnerText,
                (ColumnType)Enum.Parse(typeof(ColumnType), element["Type"].InnerText),
                element["Comparison"].InnerText,
                element["TimingMethod"].InnerText);
        }

        public XmlNode ToXml(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            parent.AppendChild(ToElement(document, "Version", "1.5"));
            parent.AppendChild(ToElement(document, "Name", Name));
            parent.AppendChild(ToElement(document, "Type", Type));
            parent.AppendChild(ToElement(document, "Comparison", Comparison));
            parent.AppendChild(ToElement(document, "TimingMethod", TimingMethod));
            return parent;
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }
    }
}
