using Facebook;
using FBPostsFetcher.Models;
using FBPostsFetcher.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.IO;
using System.Threading;

namespace FBPostsFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var fb = new FacebookClient(Settings.Default.FBAccessToken);

            using (var db = new FBPostsFetcherContext())
            {
                var next = "";
                if (Settings.Default.ContinueLastError && File.Exists("exception_point.txt"))
                {
                    next = File.ReadAllText("exception_point.txt");
                }
                SaveToDB(fb, db, next);
                if (File.Exists("exception_point.txt"))
                {
                    File.Delete("exception_point.txt");
                }
            }
        }

        static int i = 0;

        private static void SaveToDB(FacebookClient fb, FBPostsFetcherContext db, string next = "")
        {
            Console.WriteLine($"SaveToDB (next = {next})");

            // https://developers.facebook.com/docs/graph-api/reference/v3.2/page/feed

            JsonObject feed;

            if (String.IsNullOrEmpty(next))
            {
                feed = (Facebook.JsonObject)fb.Get(
                   $"{Settings.Default.FBID}/feed", new
                   {
                       fields = new[] {
                        "id", "message", "created_time", "permalink_url", "link",
                        "from",
                        "attachments"
                       },
                       limit = 100
                   });
            }
            else
            {
                try
                {
                    feed = (Facebook.JsonObject)fb.Get(next);
                }
                catch (FacebookOAuthException ex)
                {
                    File.WriteAllText("exception_point.txt", next);
                    throw ex;
                }
            }

            var data = feed["data"] as JsonArray;

            if (data.Count == 0)
            {
                return;
            }
            else
            {
                Console.WriteLine($"data.Count = {data.Count}");
            }

            foreach (dynamic item in data)
            {
                var post = db.Posts.Create();

                post.fb_id = Settings.Default.FBID;
                post.raw_json = item.ToString();

                post.id = item["id"].ToString();
                post.created_time = item["created_time"].ToString();

                if (item.ContainsKey("permalink_url"))
                {
                    post.permalink_url = item["permalink_url"].ToString();
                }

                if (item.ContainsKey("message"))
                {
                    post.message = item["message"].ToString();
                }

                // Deprecated for Page posts for v3.3+.
                // Use attachments{url_unshimmed} instead.
                if (item.ContainsKey("link"))
                {
                    post.link = item["link"].ToString();
                }

                if (db.Posts.Find(item["id"].ToString()) == null &&
                    item.ContainsKey("from"))
                {
                    var from = item["from"] as JsonObject;

                    if (db.Froms.Find(from["id"].ToString()) == null)
                    {
                        var from_user = new From()
                        {
                            id = from["id"].ToString(),
                            name = from["name"].ToString()
                        };
                        post.from = from_user;
                    }
                    //post.from.id = from["id"].ToString();
                    //post.from.name = from["name"].ToString();

                    // 僅將 "粉絲專頁" 的文章收錄到資料庫中
                    //if (from["id"].ToString() == Settings.Default.FBID)
                    //{
                    //}
                    db.Posts.Add(post);
                }

                if (item.ContainsKey("attachments"))
                {
                    JsonArray attachments = item.attachments.data as JsonArray;

                    foreach (dynamic att in attachments)
                    {
                        post.attachments.Add(new Attachment()
                        {
                            type = att.type,
                            title = att.title,
                            url = att.url
                        });
                    }
                }


                Console.WriteLine($"{++i}\t{post.created_time} {post.permalink_url}");
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

            if (feed.ContainsKey("paging"))
            {
                var paging = feed["paging"] as JsonObject;
                if (paging.ContainsKey("next"))
                {
                    SaveToDB(fb, db, paging["next"].ToString());
                }
            }
        }
    }
}
