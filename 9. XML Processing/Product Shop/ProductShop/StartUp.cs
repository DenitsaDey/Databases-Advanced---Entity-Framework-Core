using ProductShop.Data;
using System.IO;
using XMLHelper;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            var usersXml = File.ReadAllText("../../../Datasets/users.xml");

            var result = ImportUsers(context, usersXml);
        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var userResult = XMLConverter.Deserializer.
        }
    }
}