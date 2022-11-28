namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211182313 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfigurations", "Email", c => c.String(nullable: false));
            DropColumn("dbo.EmailConfigurations", "UserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailConfigurations", "UserName", c => c.String(nullable: false));
            DropColumn("dbo.EmailConfigurations", "Email");
        }
    }
}
