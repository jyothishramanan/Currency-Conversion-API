using System.Xml;

namespace Currency_Conversion_Business.Helper
{
    public static class Parser
    {
        public static Dictionary<string, double> ParseXML(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            Dictionary<string, double> extractedReult = new Dictionary<string, double>();

            if (File.Exists(path))
            {

                xmlDoc.Load(path);
                XmlNodeList list = xmlDoc.GetElementsByTagName("Cube");
                int x = 0;
                foreach (XmlNode nodes in list)
                {
                    if (x >= 2)
                    {
                        double result = double.Parse(nodes.Attributes["rate"].Value, System.Globalization.CultureInfo.InvariantCulture);

                        extractedReult.Add(nodes.Attributes["currency"].Value, result);

                    }
                    x++;
                }
            }
            
            return extractedReult;
        }
    }
}
