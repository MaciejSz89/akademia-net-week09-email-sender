namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202302162154 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            AddForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            AddForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages", "Id", cascadeDelete: true);
        }
    }
}
