using MaslomatAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaslomatAPI.Context
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        public UsersContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordEntity> Passwords { get; set; }
    }
}
