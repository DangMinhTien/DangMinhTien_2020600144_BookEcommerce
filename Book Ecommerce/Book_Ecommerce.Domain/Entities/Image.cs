﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Images")]
    public class Image
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string ImageId { get; set; } = null!;
        [Column(TypeName = "nvarchar(250)")]
        public string ImageName { get; set; } = null!;
        [Column(TypeName = "nvarchar(250)")]
        public string Url { get; set; } = null!;
        [Column(TypeName = "char(36)")] 
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
