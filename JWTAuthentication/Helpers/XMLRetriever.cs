using System.Xml.Linq;

namespace JWTAuthentication.Helpers
{
    public class XMLRetriever
    {
        private XElement root;
        public XMLRetriever(XElement _root)
        {
            root = _root;
        }

        public string GetParameter(string param)
        {
            String value = null;
            XElement demiChild = null;
            try
            {
                string[] tokens = param.Split('.');
                for (int i = 0; i < 100; i++)
                {
                    XElement child;
                    if (i == 0)
                    {
                        child = root.Element(tokens[i]);
                    }
                    else
                    {
                        child = demiChild.Element(tokens[i]);
                    }
                    if (i == tokens.Length - 1)
                    {
                        value = child.Value;
                        break;
                    }
                    demiChild = child;
                }
                return value;
            }
            catch
            {
                return null;
            };
        }
    }
}
