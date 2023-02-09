namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202211242256 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailAddresses", "Address", c => c.String(nullable: false));
            DropColumn("dbo.EmailAddresses", "Addresss");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailAddresses", "Addresss", c => c.String(nullable: false));
            DropColumn("dbo.EmailAddresses", "Address");
        }
    }
}
