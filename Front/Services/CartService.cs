using Shop_API.Models;
using System.Collections.ObjectModel;

namespace Front.Services
{
    public class CartService
    {

    
        public List<Product> CartItems { get; set; } = new List<Product>();
    }
}
