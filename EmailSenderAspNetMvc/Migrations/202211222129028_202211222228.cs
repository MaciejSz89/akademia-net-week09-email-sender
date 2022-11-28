namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211222228 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfigurations", "PasswordHash", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailConfigurations", "PasswordHash");
        }
    }
}
