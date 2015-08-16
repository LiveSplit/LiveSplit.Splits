using System;
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
            return new ColumnData(element["Name"].InnerText,
                (ColumnType)Enum.Parse(typeof(ColumnType), element["Type"].InnerText),
                element["Comparison"].InnerText,
                element["TimingMethod"].InnerText);
        }

        public int CreateElement(XmlDocument document, XmlElement element)
        {
            return SettingsHelper.CreateSetting(document, element, "Version", "1.5") ^
            SettingsHelper.CreateSetting(document, element, "Name", Name) ^
            SettingsHelper.CreateSetting(document, element, "Type", Type) ^
            SettingsHelper.CreateSetting(document, element, "Comparison", Comparison) ^
            SettingsHelper.CreateSetting(document, element, "TimingMethod", TimingMethod);
        }
    }
}
