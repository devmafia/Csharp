using System.ComponentModel.DataAnnotations.Schema;

namespace RedMango_API.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }
        [NotMapped]
        public int WayForPayPaymentIntentId { get; set; }
        [NotMapped]
        public string ClientSecret { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public int ShoppingCartId { get; set; }
    }
}
