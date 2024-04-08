using Currency_Conversion_Business.Constants;
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
                try
                {
                    xmlDoc.Load(path);
                    XmlNodeList list = xmlDoc.GetElementsByTagName(AppConstant.CUBE);
                    int x = 0;
                    foreach (XmlNode nodes in list)
                    {
                        //Lis conatains all cube nodes, first two cube nodes need to ignore
                        if (x >= 2)
                        {
                            double result = double.Parse(nodes.Attributes[AppConstant.RATE].Value, System.Globalization.CultureInfo.InvariantCulture);

                            extractedReult.Add(nodes.Attributes[AppConstant.CURRENCY].Value, result);

                        }
                        x++;
                    }
                }
                catch(Exception ex)  
                { 
                    
                }
            }
            
            return extractedReult;
        }
    }
}
