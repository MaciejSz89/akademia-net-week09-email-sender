using System.Data.Entity;
using EmailSenderAspNetMvc.Models.Domains;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmailSenderAspNetMvc.Models
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<EmailConfiguration> EmailConfigurations { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<EmailMessageReceiver> EmailMessageReceivers { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.EmailAddresses)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.EmailAttachments)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.EmailConfigurations)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.EmailMessages)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.EmailMessageReceivers)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EmailAddress>()
                .HasMany(x => x.EmailConfigurations)
                .WithRequired(x => x.EmailAddress)
                .HasForeignKey(x => x.EmailAddressId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EmailAddress>()
               .HasMany(x => x.EmailMessageReceivers)
               .WithRequired(x => x.EmailAddress)
               .HasForeignKey(x => x.EmailAddressId)
               .WillCascadeOnDelete(false);


            base.OnModelCreating(modelBuilder);
        }
    }
}