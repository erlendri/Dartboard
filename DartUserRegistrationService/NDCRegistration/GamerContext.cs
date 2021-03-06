﻿using Dart.Messaging.Models;
using Microsoft.EntityFrameworkCore;

namespace NDCRegistration
{
    public class GamerContext : DbContext
    {
        public GamerContext(DbContextOptions options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}