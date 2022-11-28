namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211191304 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfigurations", "EmailAddress", c => c.String(nullable: false));
            DropColumn("dbo.EmailConfigurations", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailConfigurations", "Email", c => c.String(nullable: false));
            DropColumn("dbo.EmailConfigurations", "EmailAddress");
        }
    }
}
