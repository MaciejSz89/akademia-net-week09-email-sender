namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2002211302154 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailConfigurations", "EmailAddress_Id", "dbo.EmailAddresses");
            DropIndex("dbo.EmailConfigurations", new[] { "EmailAddress_Id" });
            DropColumn("dbo.EmailConfigurations", "EmailAddress_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailConfigurations", "EmailAddress_Id", c => c.Int());
            CreateIndex("dbo.EmailConfigurations", "EmailAddress_Id");
            AddForeignKey("dbo.EmailConfigurations", "EmailAddress_Id", "dbo.EmailAddresses", "Id");
        }
    }
}
