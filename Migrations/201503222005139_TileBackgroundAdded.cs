namespace NonCreative.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TileBackgroundAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WallModels", "TileBackground", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WallModels", "TileBackground");
        }
    }
}
