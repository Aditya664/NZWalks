﻿using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Data
{
    public class NZWalksDbContext : DbContext
    {
        public NZWalksDbContext(DbContextOptions options):base(options) 
        {
            
        }

        public DbSet<Region> Region { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; } 
        public DbSet<Walk> Walks { get; set; }
    }
}