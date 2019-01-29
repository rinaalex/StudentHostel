namespace StudentHostelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "GroupName", c => c.String(maxLength: 10));
            DropColumn("dbo.Groups", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "Name", c => c.String(maxLength: 10));
            DropColumn("dbo.Groups", "GroupName");
        }
    }
}
