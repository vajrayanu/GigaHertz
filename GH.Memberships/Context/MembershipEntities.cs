using System.Data.Entity;
using GH.Memberships.Model;

namespace GH.Memberships.Context
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; } 
    }
}
