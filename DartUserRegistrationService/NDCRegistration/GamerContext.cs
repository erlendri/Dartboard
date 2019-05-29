using Dart.Messaging.Models;
using Microsoft.EntityFrameworkCore;

namespace NDCRegistration
{
    public class GamerContext : DbContext
    {
        public GamerContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Gamer> Gamers { get; set; }
    }
}