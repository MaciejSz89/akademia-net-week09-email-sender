namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211191331 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EmailConfigurations", "EnableSmtp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailConfigurations", "EnableSmtp", c => c.Boolean(nullable: false));
        }
    }
}
