using System.IO;
using System.Xml;

namespace UnityNuGetManager
{
    public class NamespaceAgnosticXmlReader : XmlTextReader
    {
        public override string NamespaceURI => string.Empty;

        public NamespaceAgnosticXmlReader(Stream stream) : base(stream)
        {
        }
    }
}