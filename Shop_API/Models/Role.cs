using System.ComponentModel.DataAnnotations;

namespace Shop_API.Models
{
    public class Role
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
    }
}
