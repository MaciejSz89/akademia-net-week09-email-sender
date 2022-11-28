namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211241600 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailMessages", "Content", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailMessages", "Content", c => c.String());
        }
    }
}
