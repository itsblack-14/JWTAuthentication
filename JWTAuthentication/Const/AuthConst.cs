using JWTAuthentication.Helpers;

namespace JWTAuthentication.Const
{
    public class AuthConst
    {
        public static readonly string PARAM_APPLICATION = "Auth";

        public static int BUYER_USERTYPE_ID;
        public static int SELLER_USERTYPE_ID;

        public static void LoadConfigData()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\appsetting.config");
            var xml = System.Xml.Linq.XElement.Load(filePath);
            var xmlRetriever = new XMLRetriever(xml);

            BUYER_USERTYPE_ID = int.Parse(xmlRetriever.GetParameter(PARAM_APPLICATION+ ".userTypeBuyer"));
            SELLER_USERTYPE_ID = int.Parse(xmlRetriever.GetParameter(PARAM_APPLICATION+ ".userTypeSeller"));
        }
    }
}
