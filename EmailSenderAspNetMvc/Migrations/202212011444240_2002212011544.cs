namespace EmailSenderAspNetMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2002212011544 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailMessages", "SaveDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailMessages", "SaveDate");
        }
    }
}
