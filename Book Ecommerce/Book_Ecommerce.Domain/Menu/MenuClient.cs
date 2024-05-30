using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.Menu
{
    public class MenuClient
    {
        public static string Home => "Home";

        public static string Product => "Product";

        public static string MyOrder => "MyOrder";

        public static string Profile => "Profile";

        public static string Register => "Register";

        public static string Login => "Login";

        public static string HomeNavClass(string viewName) => PageNavClass(viewName, Home);

        public static string ProductNavClass(string viewName) => PageNavClass(viewName, Product);

        public static string MyOrderNavClass(string viewName) => PageNavClass(viewName, MyOrder);

        public static string ProfileNavClass(string viewName) => PageNavClass(viewName, Profile);

        public static string RegisterNavClass(string viewName) => PageNavClass(viewName, Register);

        public static string LoginNavClass(string viewName) => PageNavClass(viewName, Login);

        private static string PageNavClass(string viewName = "", string page = "")
        {
            return viewName == page ? "active" : "";
        }
    }
}
