using System.ComponentModel.DataAnnotations;

namespace Shop_API.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Login { get; set; }
    }

}