using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedMango_API.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        public int MenuItemId { get; set; } 
        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }
        [Required]
        public double Quantity { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
