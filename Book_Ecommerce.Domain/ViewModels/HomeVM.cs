using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Author> Authors { get; set; } = null!;
        public IEnumerable<Brand> Brands { get; set; } = null!;
        public IEnumerable<ProductVM> NewProducts { get; set; } = null!;
        public IEnumerable<ProductVM> Products { get; set; } = null!;
    }
}
