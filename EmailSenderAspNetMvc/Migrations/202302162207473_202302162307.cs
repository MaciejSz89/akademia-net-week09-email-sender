namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202302162307 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailAttachments", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailMessages", "EmailConfigurationId", "dbo.EmailConfigurations");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailAttachments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            DropIndex("dbo.EmailAttachments", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailAttachments", new[] { "UserId" });
            DropIndex("dbo.EmailMessages", new[] { "EmailConfigurationId" });
            DropIndex("dbo.EmailMessages", new[] { "UserId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "UserId" });
            DropTable("dbo.EmailAttachments");
            DropTable("dbo.EmailMessages");
            DropTable("dbo.EmailMessageReceivers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EmailMessageReceivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailMessageId = c.Int(nullable: false),
                        EmailAddressId = c.Int(nullable: false),
                        EmailMessageReceiverType = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Content = c.String(nullable: false),
                        IsBodyHtml = c.Boolean(nullable: false),
                        SaveDate = c.DateTime(),
                        EmailConfigurationId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileStream = c.Binary(),
                        EmailMessageId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.EmailMessageReceivers", "UserId");
            CreateIndex("dbo.EmailMessageReceivers", "EmailAddressId");
            CreateIndex("dbo.EmailMessageReceivers", "EmailMessageId");
            CreateIndex("dbo.EmailMessages", "UserId");
            CreateIndex("dbo.EmailMessages", "EmailConfigurationId");
            CreateIndex("dbo.EmailAttachments", "UserId");
            CreateIndex("dbo.EmailAttachments", "EmailMessageId");
            AddForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses", "Id");
            AddForeignKey("dbo.EmailMessages", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.EmailMessageReceivers", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.EmailAttachments", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.EmailMessageReceivers", "EmailMessageId", "dbo.EmailMessages", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmailMessages", "EmailConfigurationId", "dbo.EmailConfigurations", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmailAttachments", "EmailMessageId", "dbo.EmailMessages", "Id", cascadeDelete: true);
        }
    }
}
