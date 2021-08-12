using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TerritoryTools.Entities;
using TerritoryTools.Web.MainSite.Areas.UrlShortener.Models;

namespace TerritoryTools.Web.Data
{
    public class MainDbContext : IdentityDbContext<IdentityUser>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            
            builder.Entity<TerritoryUserAlbaAccountLink>()
               .HasOne(uaa => uaa.TerritoryUser)
               .WithMany(u => u.AlbaAccountLinks)
               .HasForeignKey(uaa => uaa.TerritoryUserId)
               .HasConstraintName("ForeignKey_TerritoryUser_AlbaAccount_Link");

            builder.Entity<TerritoryUserAlbaAccountLink>()
                .HasOne(uaa => uaa.AlbaAccount)
                .WithMany(a => a.TerritoryUserLinks)
                .HasForeignKey(uaa => uaa.AlbaAccountId)
                .HasConstraintName("ForeignKey_AlbaAccount_TerritoryUser_Link");

            builder.Entity<AlbaUser>()
                .HasOne(u => u.Account)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AccountId)
                .HasConstraintName("ForeignKey_User_Account");

            builder.Entity<AlbaAccount>();

            builder.Entity<AlbaUser>();

            builder.Entity<ShortUrl>()
             .HasOne(u => u.Host)
             .WithMany(a => a.Urls)
             .HasForeignKey(u => u.HostId)
             .HasConstraintName("ForeignKey_ShortUrl_Host");

            builder.Entity<TerritoryUser>()
                .HasData(new TerritoryUser
                {  
                    Id = new Guid("0714316c-8a94-438d-9f76-4c4c9b77ef89"),
                    Email = "admin@territorytools.org", 
                    GivenName = "Admin", 
                    Role = "Administrator",
                    Created = new DateTime(2021,8,1)
                });

            builder.Entity<AlbaAccount>()
                .HasData(new AlbaAccount
                {
                     Id = new Guid("90BC0598-1907-4384-8D8D-E5D336C769C3"),
                     AccountName = "account-name",
                     HostName = "host-name",
                     IdInAlba = 1,
                     LongName = "Long Name for Alba Account", 
                     Created = new DateTime(2021,8,1)
                });

            builder.Entity<TerritoryUserAlbaAccountLink>()
                .HasData(new TerritoryUserAlbaAccountLink
                {
                    TerritoryUserAlbaAccountLinkId = 1,
                    AlbaAccountId = new Guid("90BC0598-1907-4384-8D8D-E5D336C769C3"),
                    TerritoryUserId = new Guid("0714316c-8a94-438d-9f76-4c4c9b77ef89")
                });

        }

        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<ShortUrlHost> ShortUrlHosts { get; set; }
        public DbSet<ShortUrlActivity> ShortUrlActivity { get; set; }
        public DbSet<AlbaAccount> AlbaAccounts { get; set; }
        public DbSet<AlbaUser> AlbaUsers { get; set; }
        public DbSet<TerritoryUser> TerritoryUser { get; set; }
        public DbSet<TerritoryUserAlbaAccountLink> TerritoryUserAlbaAccountLink { get; set; }
        public DbSet<TerritoryAssignment> TerritoryAssignments { get; set; }
    }
}
