namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202302192116 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EmailFolderMessages", newName: "EmailMessages");
            DropForeignKey("dbo.EmailFolderAttachments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            DropForeignKey("dbo.EmailFolderMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            DropForeignKey("dbo.EmailFolderMessageReceivers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolderMessageReceivers", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            DropIndex("dbo.EmailFolderAttachments", new[] { "EmailFolderMessageId" });
            DropIndex("dbo.EmailFolderAttachments", new[] { "UserId" });
            DropIndex("dbo.EmailFolderMessageReceivers", new[] { "EmailFolderMessageId" });
            DropIndex("dbo.EmailFolderMessageReceivers", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailFolderMessageReceivers", new[] { "UserId" });
            RenameColumn(table: "dbo.EmailMessageFolderPairs", name: "EmailFolderMessageId", newName: "EmailMessageId");
            RenameIndex(table: "dbo.EmailMessageFolderPairs", name: "IX_EmailFolderMessageId", newName: "IX_EmailMessageId");
            CreateTable(
                "dbo.EmailAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileStream = c.Binary(),
                        EmailMessageId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId)
                .Index(t => t.EmailMessageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmailMessageReceivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailMessageId = c.Int(nullable: false),
                        EmailAddressId = c.Int(nullable: false),
                        EmailMessageReceiverType = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailAddresses", t => t.EmailAddressId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId)
                .Index(t => t.EmailMessageId)
                .Index(t => t.EmailAddressId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.EmailMessageFolderPairs", "ImapMessageUid", c => c.Long(nullable: false));
            DropColumn("dbo.EmailMessageFolderPairs", "ImapFolderMessageUid");
            DropTable("dbo.EmailFolderAttachments");
            DropTable("dbo.EmailFolderMessageReceivers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EmailFolderMessageReceivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailFolderMessageId = c.Int(nullable: false),
                        EmailAddressId = c.Int(nullable: false),
                        EmailMessageReceiverType = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailFolderAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileStream = c.Binary(),
                        EmailFolderMessageId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.EmailMessageFolderPairs", "ImapFolderMessageUid", c => c.Long(nullable: false));
            DropForeignKey("dbo.EmailMessageReceivers", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailMessageReceivers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            DropForeignKey("dbo.EmailAttachments", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailAttachments", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.EmailMessageReceivers", new[] { "UserId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailAttachments", new[] { "UserId" });
            DropIndex("dbo.EmailAttachments", new[] { "EmailMessageId" });
            DropColumn("dbo.EmailMessageFolderPairs", "ImapMessageUid");
            DropTable("dbo.EmailMessageReceivers");
            DropTable("dbo.EmailAttachments");
            RenameIndex(table: "dbo.EmailMessageFolderPairs", name: "IX_EmailMessageId", newName: "IX_EmailFolderMessageId");
            RenameColumn(table: "dbo.EmailMessageFolderPairs", name: "EmailMessageId", newName: "EmailFolderMessageId");
            CreateIndex("dbo.EmailFolderMessageReceivers", "UserId");
            CreateIndex("dbo.EmailFolderMessageReceivers", "EmailAddressId");
            CreateIndex("dbo.EmailFolderMessageReceivers", "EmailFolderMessageId");
            CreateIndex("dbo.EmailFolderAttachments", "UserId");
            CreateIndex("dbo.EmailFolderAttachments", "EmailFolderMessageId");
            AddForeignKey("dbo.EmailFolderMessageReceivers", "EmailFolderMessageId", "dbo.EmailFolderMessages", "Id");
            AddForeignKey("dbo.EmailFolderMessageReceivers", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.EmailFolderMessageReceivers", "EmailAddressId", "dbo.EmailAddresses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages", "Id");
            AddForeignKey("dbo.EmailFolderAttachments", "UserId", "dbo.AspNetUsers", "Id");
            RenameTable(name: "dbo.EmailMessages", newName: "EmailFolderMessages");
        }
    }
}
