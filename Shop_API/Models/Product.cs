using System.ComponentModel.DataAnnotations;

namespace Shop_API.Models
{
    public class Product
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Count { get; set; }
        public List<Order> Orders { get; set; }
    }
}
