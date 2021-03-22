using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        }

        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<ShortUrlHost> ShortUrlHosts { get; set; }
        public DbSet<ShortUrlActivity> ShortUrlActivity { get; set; }
        public DbSet<AlbaAccount> AlbaAccounts { get; set; }
        public DbSet<AlbaUser> AlbaUsers { get; set; }
        public DbSet<TerritoryUser> TerritoryUser { get; set; }
        public DbSet<TerritoryUserAlbaAccountLink> TerritoryUserAlbaAccountLink { get; set; }
    }
}
