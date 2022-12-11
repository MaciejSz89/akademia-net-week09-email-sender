namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202212042349 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailAddresses", "IsDefined", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailAddresses", "IsDefined");
        }
    }
}
