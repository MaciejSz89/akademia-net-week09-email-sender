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
                        Addresss = c.String(nullable: false),
                        DisplayName = c.String(),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
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
                        Attachment = c.Binary(),
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
                        Body = c.String(),
                        IsBodyHtml = c.Boolean(nullable: false),
                        EmailConfigurationId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailConfigurations", t => t.EmailConfigurationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.EmailConfigurationId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmailConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Host = c.String(nullable: false),
                        Port = c.Int(nullable: false),
                        UserName = c.String(nullable: false),
                        DisplayUserName = c.String(),
                        Password = c.String(nullable: false),
                        EnableSmtp = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
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
                .ForeignKey("dbo.EmailAddresses", t => t.EmailAddressId, cascadeDelete: true)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
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
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailConfigurations", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailAttachments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            DropForeignKey("dbo.EmailMessages", "EmailConfigurationId", "dbo.EmailConfigurations");
            DropForeignKey("dbo.EmailAttachments", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailAddresses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "UserId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailAddressId" });
            DropIndex("dbo.EmailMessageReceivers", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailConfigurations", new[] { "UserId" });
            DropIndex("dbo.EmailMessages", new[] { "UserId" });
            DropIndex("dbo.EmailMessages", new[] { "EmailConfigurationId" });
            DropIndex("dbo.EmailAttachments", new[] { "UserId" });
            DropIndex("dbo.EmailAttachments", new[] { "EmailMessageId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.EmailAddresses", new[] { "UserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.EmailMessageReceivers");
            DropTable("dbo.EmailConfigurations");
            DropTable("dbo.EmailMessages");
            DropTable("dbo.EmailAttachments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.EmailAddresses");
        }
    }
}
