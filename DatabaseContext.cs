using NonCreative.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace NonCreative
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        //Use threadlocal because DatabaseContext isn't thread safe
        //Plus, we know that a single object won't be on 2 threads simultaneously
        //so we won't have even more errors
        private static ThreadLocal<DatabaseContext> _ctx =
            new ThreadLocal<DatabaseContext>(() =>
            {
                var result = new DatabaseContext();
                result.Configuration.AutoDetectChangesEnabled = false;
                return result;
            });

        public static DatabaseContext Shared
        {
            get
            {
                if (_ctx.Value.Disposed)
                    _ctx.Value = new DatabaseContext();
                return _ctx.Value;
            }
        }

        public DatabaseContext()
            : base("DatabaseConnection")
        {
        }
        public System.Data.Entity.DbSet<NonCreative.Models.WallPost> WallPosts { get; set; }

        public System.Data.Entity.DbSet<NonCreative.Models.WallModel> WallModels { get; set; }

        public System.Data.Entity.DbSet<NonCreative.Models.FileUploadModel> Files { get; set; }
        public System.Data.Entity.DbSet<NonCreative.Models.UserWallReference> WallReferences { get; set; }
        public System.Data.Entity.DbSet<NonCreative.Models.UserPostReference> PostReferences { get; set; }
        public System.Data.Entity.DbSet<NonCreative.Models.WallUserReferenceModel> ModeratorReferences { get; set; }

        /// <summary>
        /// Releases context after request
        /// </summary>
        public static void Release()
        {
            Shared.Dispose(true);
        }

        public bool Disposed { get; private set; }
        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}