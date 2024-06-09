using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Messsages")]
    public class Messsages
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string MessageId { get; set; } = null!;
        [Column(TypeName = "nvarchar(4000)")]
        public string Content { get; set; } = null!;
        public DateTime SendDate { get; set; }
        [Column(TypeName = "char(36)")]
        public string SendBy { get; set; } = null!;
        [Column(TypeName = "char(36)")]
        public string CustomerId { get; set; } = null!;
        [Column(TypeName = "char(36)")]
        public string EmployeeId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public Employee Employee { get; set; } = null!;

    }
}
