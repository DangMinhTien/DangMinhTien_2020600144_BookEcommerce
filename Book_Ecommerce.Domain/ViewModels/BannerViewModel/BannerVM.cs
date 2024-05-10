using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.BannerViewModel
{
    public class BannerVM
    {
        public string BannerId { get; set; } = null!;
        public string BannerCode { set; get; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string ImageName { get; set; } = null!;
        public string UrlImage { get; set; } = null!;
    }
}
