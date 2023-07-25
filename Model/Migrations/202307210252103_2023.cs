namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2023 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkFlow_Instance", "OutNum", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkFlow_Instance", "OutNum", c => c.Int());
        }
    }
}
