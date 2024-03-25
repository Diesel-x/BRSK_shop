using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop_API.Models;

namespace Shop_API.Data
{
    public class DBContext : DbContext
    {
        public DBContext (DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<Shop_API.Models.Product> Product { get; set; } = default!;
        public DbSet<Shop_API.Models.User> User { get; set; } = default!;
        public DbSet<Shop_API.Models.Order> Order { get; set; } = default!;
        public DbSet<Shop_API.Models.Role> Role { get; set; } = default!;

    }
}
