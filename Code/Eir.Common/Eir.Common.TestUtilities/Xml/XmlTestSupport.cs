using System.IO;
using System.Text;
using System.Xml;
using Eir.Common.Test.Extensions;

namespace Eir.Common.Test.Xml
{
    public static class XmlTestSupport
    {
        public static string GetComparableXml(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Crazy dance to remove whitespace and BOM and output saying "UTF-8" in the XML header...

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = false,
                NewLineChars = string.Empty,
                Encoding = Encoding.UTF8
            };

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    using (XmlWriter xwWriter = XmlWriter.Create(writer, settings))
                    {
                        xmlDoc.Save(xwWriter);
                    }
                }

                return Encoding.UTF8.GetString(Encoding.UTF8.RemoveBom(memoryStream.ToArray()));
            }
        }
    }
}