namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202212022210 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailAttachments", "FileStream", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailAttachments", "FileStream");
        }
    }
}
