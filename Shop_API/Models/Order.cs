using System.ComponentModel.DataAnnotations;

namespace Shop_API.Models
{
    public class Order
    {
        [Key] public int Id { get; set; }
        public int UserID { get; set; }
        public int SumCost { get; set; }
    }
}
