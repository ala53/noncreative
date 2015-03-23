namespace NonCreative.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileUploadModels",
                c => new
                    {
                        FileId = c.Long(nullable: false, identity: true),
                        Filename = c.String(),
                        WallId = c.String(),
                        Wall_WallUrl = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FileId)
                .ForeignKey("dbo.WallModels", t => t.Wall_WallUrl)
                .Index(t => t.Wall_WallUrl);
            
            CreateTable(
                "dbo.WallModels",
                c => new
                    {
                        WallUrl = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false),
                        Subtitle = c.String(nullable: false),
                        OwnerPrivate = c.String(nullable: false),
                        OwnerPublic = c.String(nullable: false),
                        OwnerName = c.String(nullable: false),
                        Password = c.String(),
                        WallMode = c.Int(nullable: false),
                        UnauthorizedUserPermissionLevel = c.Int(nullable: false),
                        BackgroundUrl = c.String(),
                    })
                .PrimaryKey(t => t.WallUrl);
            
            CreateTable(
                "dbo.WallUserReferenceModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PermissionLevel = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                        WallModel_WallUrl = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .ForeignKey("dbo.WallModels", t => t.WallModel_WallUrl)
                .Index(t => t.User_Id)
                .Index(t => t.WallModel_WallUrl);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(nullable: false),
                        TokenKeyPublic = c.String(),
                        TokenKeyPrivate = c.String(),
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
                "dbo.UserWallReferences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WallId = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WallModels", t => t.WallId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.WallId)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
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
                "dbo.UserPostReferences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PostId = c.Long(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WallPosts", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.PostId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.WallPosts",
                c => new
                    {
                        PostId = c.Long(nullable: false, identity: true),
                        SortOrder = c.Long(nullable: false),
                        XPosition = c.Int(nullable: false),
                        YPosition = c.Int(nullable: false),
                        CreatorName = c.String(nullable: false),
                        Header = c.String(),
                        Content = c.String(),
                        Attachment = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        CreatorPrivate = c.String(nullable: false),
                        CreatorPublic = c.String(nullable: false),
                        WallId = c.String(),
                        Wall_WallUrl = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PostId)
                .ForeignKey("dbo.WallModels", t => t.Wall_WallUrl)
                .Index(t => t.Wall_WallUrl);
            
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
            DropForeignKey("dbo.FileUploadModels", "Wall_WallUrl", "dbo.WallModels");
            DropForeignKey("dbo.WallUserReferenceModels", "WallModel_WallUrl", "dbo.WallModels");
            DropForeignKey("dbo.WallUserReferenceModels", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserWallReferences", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPostReferences", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPostReferences", "PostId", "dbo.WallPosts");
            DropForeignKey("dbo.WallPosts", "Wall_WallUrl", "dbo.WallModels");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserWallReferences", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserWallReferences", "WallId", "dbo.WallModels");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.WallPosts", new[] { "Wall_WallUrl" });
            DropIndex("dbo.UserPostReferences", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserPostReferences", new[] { "PostId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.UserWallReferences", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.UserWallReferences", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserWallReferences", new[] { "WallId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.WallUserReferenceModels", new[] { "WallModel_WallUrl" });
            DropIndex("dbo.WallUserReferenceModels", new[] { "User_Id" });
            DropIndex("dbo.FileUploadModels", new[] { "Wall_WallUrl" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.WallPosts");
            DropTable("dbo.UserPostReferences");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.UserWallReferences");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.WallUserReferenceModels");
            DropTable("dbo.WallModels");
            DropTable("dbo.FileUploadModels");
        }
    }
}
