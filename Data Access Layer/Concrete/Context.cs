using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Concrete
{
    public class Context : IdentityDbContext<AppUser, AppRole, int>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.AppUser)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<EventParticipant>()
            .HasKey(ep => new { ep.UserID, ep.EventId });

            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.AppUser)
                .WithMany(u => u.EventParticipants)
                .HasForeignKey(ep => ep.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.EventParticipants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCompanyEvent>()
            .HasKey(tce => new { tce.EventId, tce.CompanyId });

            modelBuilder.Entity<TicketCompanyEvent>()
                .HasOne(tce => tce.Event)
                .WithMany(e => e.TicketCompanyEvents)
                .HasForeignKey(tce => tce.EventId);

            modelBuilder.Entity<TicketCompanyEvent>()
                .HasOne(tce => tce.TicketCompany)
                .WithMany(tc => tc.TicketCompanyEvents)
                .HasForeignKey(tce => tce.CompanyId);
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }
        public DbSet<TicketCompany> TicketCompanies { get; set; }
        public DbSet<TicketCompanyEvent> TicketCompanyEvents { get; set; }
    }
}
