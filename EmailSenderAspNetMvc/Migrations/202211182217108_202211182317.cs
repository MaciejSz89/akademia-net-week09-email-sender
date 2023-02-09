namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211182317 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfigurations", "Name", c => c.String());
            DropColumn("dbo.EmailConfigurations", "DisplayUserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailConfigurations", "DisplayUserName", c => c.String());
            DropColumn("dbo.EmailConfigurations", "Name");
        }
    }
}
