using JWTLogin.Entity.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTLogin.DAL.Contexts
{
    public class JwtLoginDbContext : IdentityDbContext<AppUser>
    {
        public JwtLoginDbContext(DbContextOptions<JwtLoginDbContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\MSSQLLocalDB;Database=JwtLoginDb;");
        }

        public DbSet<Token> Tokens{ get; set; }
    }
}
