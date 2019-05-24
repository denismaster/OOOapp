using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data.Entities;

namespace OOOVote.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Organization> Organizations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrganizationUser>()
                .HasKey(t => new { t.OrganizationId, t.UserId });

            modelBuilder.Entity<OrganizationUser>()
                .Property(e => e.OrganizationRole)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrganizationRole)Enum.Parse(typeof(OrganizationRole), v)
                );

            modelBuilder.Entity<OrganizationUser>()
                .HasOne(pt => pt.Organization)
                .WithMany(p => p.Users)
                .HasForeignKey(pt => pt.OrganizationId);

            modelBuilder.Entity<OrganizationUser>()
                .HasOne(pt => pt.User)
                .WithMany(t => t.Organizations)
                .HasForeignKey(pt => pt.UserId);
        }
    }
}
