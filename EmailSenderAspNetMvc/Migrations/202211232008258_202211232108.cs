namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211232108 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfigurations", "EmailAddressId", c => c.Int(nullable: false));
            CreateIndex("dbo.EmailConfigurations", "EmailAddressId");
            AddForeignKey("dbo.EmailConfigurations", "EmailAddressId", "dbo.EmailAddresses", "Id");
            DropColumn("dbo.EmailConfigurations", "EmailAddress");
            DropColumn("dbo.EmailConfigurations", "DisplayName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailConfigurations", "DisplayName", c => c.String());
            AddColumn("dbo.EmailConfigurations", "EmailAddress", c => c.String(nullable: false));
            DropForeignKey("dbo.EmailConfigurations", "EmailAddressId", "dbo.EmailAddresses");
            DropIndex("dbo.EmailConfigurations", new[] { "EmailAddressId" });
            DropColumn("dbo.EmailConfigurations", "EmailAddressId");
        }
    }
}
