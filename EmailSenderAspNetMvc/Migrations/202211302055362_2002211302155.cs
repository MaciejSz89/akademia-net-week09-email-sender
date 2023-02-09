namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2002211302155 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            AddForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses");
            AddForeignKey("dbo.EmailMessageReceivers", "EmailAddressId", "dbo.EmailAddresses", "Id", cascadeDelete: true);
        }
    }
}
