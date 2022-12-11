namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2002212022117 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailAttachments", "FileName", c => c.String());
            DropColumn("dbo.EmailAttachments", "Attachment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailAttachments", "Attachment", c => c.Binary());
            DropColumn("dbo.EmailAttachments", "FileName");
        }
    }
}
