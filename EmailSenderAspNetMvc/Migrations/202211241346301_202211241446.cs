namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211241446 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailMessages", "Content", c => c.String());
            DropColumn("dbo.EmailMessages", "Body");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailMessages", "Body", c => c.String());
            DropColumn("dbo.EmailMessages", "Content");
        }
    }
}
