namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211182357 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfigurations", "DisplayName", c => c.String());
            AlterColumn("dbo.EmailConfigurations", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailConfigurations", "Name", c => c.String());
            DropColumn("dbo.EmailConfigurations", "DisplayName");
        }
    }
}
