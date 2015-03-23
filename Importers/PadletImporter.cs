using CsvHelper.Configuration;
using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonCreative.Importers
{
    public static class PadletImporter
    {
        public static IEnumerable<WallPost> ImportFromCsv(string csv)
        {
            try
            {
                return Parse(csv);
            }
            catch (Exception e)
            {
                return Enumerable.Empty<WallPost>();
            }
        }
        /// <summary>
        /// Importer for padlet posts
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static IEnumerable<WallPost> Parse(string csv)
        {
            var posts = new List<WallPost>();
            var reader = new System.IO.StringReader(csv);
            var csvData = new CsvHelper.CsvReader(reader);
            while (csvData.Read())
            {
                try
                {
                    var subject = Helpers.TextSanitizer.MakeSafe(csvData.CurrentRecord[0], false);
                    var body = Helpers.TextSanitizer.MakeSafe(csvData.CurrentRecord[1]);
                    var attachment = Helpers.TextSanitizer.MakeSafe(csvData.CurrentRecord[2]);
                    var author = Helpers.TextSanitizer.MakeSafe(csvData.CurrentRecord[3], false);
                    var created = DateTime.Parse(csvData.CurrentRecord[4].Replace("UTC", "Z"));
                    var updated = DateTime.Parse(csvData.CurrentRecord[5].Replace("UTC", "Z"));

                    var post = DatabaseContext.Shared.WallPosts.Create();
                    post.Header = subject;
                    post.Content = body;
                    post.Attachment = attachment;
                    post.CreatorName = author;
                    post.XPosition = 0;
                    post.YPosition = 0;
                    post.CreationTime = created;
                    post.UpdateTime = updated;
                    post.SortOrder = 0;
                    post.PostId = 0;
                    post.CreatorPublic = Guid.NewGuid().ToString();
                    post.CreatorPrivate = Guid.NewGuid().ToString();

                    posts.Add(post);
                }
                catch (Exception e)
                {
                    //Swallow read exception but not parser level errors
                }
            }
            return posts;
        }
    }
}