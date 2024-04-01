using System.ComponentModel.DataAnnotations;

namespace Shop_API.Models
{
    public class OrderBody
    {
        [Key]
        public int Id { get; set; }
        public int orderID { get; set; }
        public Order? order { get; set; }
        public int productID { get; set; }
        public Product? product { get; set; }
        public int productCount { get; set; }
    }
}
