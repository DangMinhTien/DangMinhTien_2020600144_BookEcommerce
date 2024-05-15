using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.Menu
{
    public class MenuManage
    {
        public static string Home => "Home";

        public static string Product => "Product";

        public static string Category => "Category";

        public static string Author => "Author";

        public static string Brand => "Brand";

        public static string Banner => "Banner";

        public static string Order => "Order";

        public static string Customer => "Customer";

        public static string Employee => "Employee";

        public static string Statistical => "Statistical";

        public static string HomeNavClass(string viewName) => PageNavClass(viewName, Home);

        public static string ProductNavClass(string viewName) => PageNavClass(viewName, Product);

        public static string CategoryNavClass(string viewName) => PageNavClass(viewName, Category);

        public static string AuthorNavClass(string viewName) => PageNavClass(viewName, Author);

        public static string BrandNavClass(string viewName) => PageNavClass(viewName, Brand);

        public static string BannerNavClass(string viewName) => PageNavClass(viewName, Banner);
        public static string OrderNavClass(string viewName) => PageNavClass(viewName, Order);

        public static string CustomerNavClass(string viewName) => PageNavClass(viewName, Customer);

        public static string EmployeeNavClass(string viewName) => PageNavClass(viewName, Employee);

        public static string StatisticalNavClass(string viewName) => PageNavClass(viewName, Statistical);

        private static string PageNavClass(string viewName = "", string page = "")
        {
            return viewName == page ? "active" : "";
        }
    }
}
