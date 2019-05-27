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
        public DbSet<InvitationCode> InvitationCodes { get; set; }

        public DbSet<Voting> Votings { get; set; }

        public DbSet<VotingOption> VotingOptions { get; set; }
        public DbSet<VotingMessage> VotingMessages { get; set; }
        public DbSet<UserVotingDecision> VotingDecisions { get; set; }

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

            modelBuilder.Entity<InvitationCode>()
                .Property(e => e.InitialRole)
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

            modelBuilder.Entity<Organization>()
                .Property(b => b.ShareСapital)
                .HasDefaultValue(10_000);

            modelBuilder.Entity<Organization>()
                .Property(b => b.RulesUrl)
                .IsRequired(false);

            modelBuilder.Entity<Organization>()
                .Property(e => e.RuleType)
                .HasConversion(
                    v => v.ToString(),
                    v => (RuleType)Enum.Parse(typeof(RuleType), v)
                );
        }
    }
}
