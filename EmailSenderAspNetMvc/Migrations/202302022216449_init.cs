namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(nullable: false),
                        DisplayName = c.String(),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IsDefined = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmailConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SmtpHost = c.String(nullable: false),
                        SmtpPort = c.Int(nullable: false),
                        ImapHost = c.String(nullable: false),
                        ImapPort = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        EmailAddressId = c.Int(nullable: false),
                        Password = c.String(nullable: false),
                        PasswordHash = c.Binary(),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.EmailAddresses", t => t.EmailAddressId)
                .Index(t => t.EmailAddressId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmailFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailConfigurationId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        FullName = c.String(nullable: false),
                        ParentFolderName = c.String(),
                        ParentFolderId = c.Int(),
                        Attributes = c.Int(nullable: false),
                        UserId = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailFolders", t => t.ParentFolderId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.EmailConfigurations", t => t.EmailConfigurationId, cascadeDelete: true)
                .Index(t => t.EmailConfigurationId)
                .Index(t => t.ParentFolderId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.EmailMessageFolderPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailFolderId = c.Int(nullable: false),
                        EmailFolderMessageId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ImapFolderMessageUid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.EmailFolderMessages", t => t.EmailFolderMessageId)
                .ForeignKey("dbo.EmailFolders", t => t.EmailFolderId)
                .Index(t => t.EmailFolderId)
                .Index(t => t.EmailFolderMessageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmailFolderMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImapMessageId = c.String(nullable: false),
                        Subject = c.String(),
                        Body = c.String(),
                        IsBodyHtml = c.Boolean(nullable: false),
                        FromId = c.Int(nullable: false),
                        EmailConfigurationId = c.Int(nullable: false),
                        Flags = c.Int(),
                        Date = c.DateTimeOffset(nullable: false, precision: 7),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailConfigurations", t => t.EmailConfigurationId, cascadeDelete: true)
                .ForeignKey("dbo.EmailAddresses", t => t.FromId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.FromId)
                .Index(t => t.EmailConfigurationId)
                .Index(t => t.UserId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailFolderMessages", t => t.EmailFolderMessageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.EmailFolderMessageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.EmailMessageId)
                .Index(t => t.UserId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailConfigurations", t => t.EmailConfigurationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.EmailConfigurationId)
                .Index(t => t.UserId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.EmailAddresses", t => t.EmailAddressId)
                .Index(t => t.EmailMessageId)
                .Index(t => t.EmailAddressId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailAddresses", t => t.EmailAddressId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.EmailFolderMessages", t => t.EmailFolderMessageId)
                .Index(t => t.EmailFolderMessageId)
                .Index(t => t.EmailAddressId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            DropForeignKey("dbo.EmailConfigurations", "EmailAddressId", "dbo.EmailAddresses");
            DropForeignKey("dbo.EmailFolders", "EmailConfigurationId", "dbo.EmailConfigurations");
            DropForeignKey("dbo.EmailMessageFolderPairs", "EmailFolderId", "dbo.EmailFolders");
            DropForeignKey("dbo.EmailFolderMessages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolderMessages", "FromId", "dbo.EmailAddresses");
            DropForeignKey("dbo.EmailMessageFolderPairs", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            DropForeignKey("dbo.EmailFolderMessageReceivers", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            DropForeignKey("dbo.EmailFolderMessageReceivers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolderMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageFolderPairs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolders", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolderAttachments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailConfigurations", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailAttachments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailMessages", "EmailConfigurationId", "dbo.EmailConfigurations");
            DropForeignKey("dbo.EmailAttachments", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailAddresses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailFolderAttachments", "EmailFolderMessageId", "dbo.EmailFolderMessages");
            DropForeignKey("dbo.EmailFolderMessages", "EmailConfigurationId", "dbo.EmailConfigurations");
            DropForeignKey("dbo.EmailFolders", "ParentFolderId", "dbo.EmailFolders");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.EmailFolderMessageReceivers", new[] { "UserId" });
            DropIndex("dbo.EmailFolderMessageReceivers", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailFolderMessageReceivers", new[] { "EmailFolderMessageId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "UserId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailMessages", new[] { "UserId" });
            DropIndex("dbo.EmailMessages", new[] { "EmailConfigurationId" });
            DropIndex("dbo.EmailAttachments", new[] { "UserId" });
            DropIndex("dbo.EmailAttachments", new[] { "EmailMessageId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.EmailFolderAttachments", new[] { "UserId" });
            DropIndex("dbo.EmailFolderAttachments", new[] { "EmailFolderMessageId" });
            DropIndex("dbo.EmailFolderMessages", new[] { "UserId" });
            DropIndex("dbo.EmailFolderMessages", new[] { "EmailConfigurationId" });
            DropIndex("dbo.EmailFolderMessages", new[] { "FromId" });
            DropIndex("dbo.EmailMessageFolderPairs", new[] { "UserId" });
            DropIndex("dbo.EmailMessageFolderPairs", new[] { "EmailFolderMessageId" });
            DropIndex("dbo.EmailMessageFolderPairs", new[] { "EmailFolderId" });
            DropIndex("dbo.EmailFolders", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.EmailFolders", new[] { "ParentFolderId" });
            DropIndex("dbo.EmailFolders", new[] { "EmailConfigurationId" });
            DropIndex("dbo.EmailConfigurations", new[] { "UserId" });
            DropIndex("dbo.EmailConfigurations", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailAddresses", new[] { "UserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.EmailFolderMessageReceivers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.EmailMessageReceivers");
            DropTable("dbo.EmailMessages");
            DropTable("dbo.EmailAttachments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.EmailFolderAttachments");
            DropTable("dbo.EmailFolderMessages");
            DropTable("dbo.EmailMessageFolderPairs");
            DropTable("dbo.EmailFolders");
            DropTable("dbo.EmailConfigurations");
            DropTable("dbo.EmailAddresses");
        }
    }
}
