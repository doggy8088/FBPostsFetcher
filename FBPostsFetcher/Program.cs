using Facebook;
using FBPostsFetcher.Models;
using FBPostsFetcher.Properties;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
                SaveToDB(fb, db);
            }
        }

        static int i = 0;

        private static void SaveToDB(FacebookClient fb, FBPostsFetcherContext db, string after = "")
        {
            Console.WriteLine($"SaveToDB after {after}");

            // https://developers.facebook.com/docs/graph-api/reference/v3.3/page/feed
            var feed = (Facebook.JsonObject)fb.Get(
                $"{Settings.Default.FBID}/feed", new
                {
                    fields = new[] {
                        "id", "message", "created_time", "permalink_url",
                        "from",
                        "link",
                        "attachments{media,type,subattachments}"
                    },
                    limit = 100,
                    after = after
                });

            var data = feed["data"] as JsonArray;

            foreach (JsonObject item in data)
            {
                var post = db.Posts.Create();

                post.id = item["id"].ToString();
                post.created_time = item["created_time"].ToString();

                if (item.Keys.Contains("permalink_url"))
                {
                    post.permalink_url = item["permalink_url"].ToString();
                }

                if (item.Keys.Contains("message"))
                {
                    post.message = item["message"].ToString();
                }

                // Deprecated for Page posts for v3.3+.
                // Use attachments{url_unshimmed} instead.
                if (item.Keys.Contains("link"))
                {
                    post.link = item["link"].ToString();
                }

                bool saving = false;
                if (db.Posts.Find(item["id"].ToString()) == null &&
                    item.Keys.Contains("from"))
                {
                    var from = item["from"] as JsonObject;

                    // 僅將 "粉絲專頁" 的文章收錄到資料庫中
                    if (from["id"].ToString() == Settings.Default.FBID)
                    {
                        db.Posts.Add(post);
                        saving = true;
                    }
                }

                Console.WriteLine($"{++i}\t{post.created_time} {post.permalink_url}\t{saving}");

                #region Debug statements
                //msg_count++;
                //Console.WriteLine("--- " + msg_count);
                //Console.WriteLine("ＩＤ: " + item.id);
                //Console.WriteLine("訊息: " + item.message);
                //Console.WriteLine("時間: " + item.created_time);

                //if (IsPropertyExist(item, "link"))
                //{
                //    Console.WriteLine("連結: " + item.link);
                //}

                //if (IsPropertyExist(item, "name"))
                //{
                //    Console.WriteLine("名稱: " + item.name);
                //}
                //if (IsPropertyExist(item, "description"))
                //{
                //    Console.WriteLine("描述: " + item.description);
                //}
                //if (IsPropertyExist(item, "caption"))
                //{
                //    Console.WriteLine("標題: " + item.caption);
                //}

                //var i = 0;
                //if (IsPropertyExist(item, "data"))
                //{
                //    foreach (var att in item.attachments.data)
                //    {
                //        i++;
                //        if (att.type == "share")
                //        {
                //            Console.WriteLine($"附件: {i}\t {att.type}");
                //        }
                //    }
                //}

                //Console.WriteLine(); 
                #endregion
            }

            db.SaveChanges();

            if (feed.Keys.Contains("paging"))
            {
                var paging = feed["paging"] as JsonObject;
                if (paging.Keys.Contains("cursors"))
                {
                    var cursors = paging["cursors"] as JsonObject;
                    if (cursors.Keys.Contains("after"))
                    {
                        SaveToDB(fb, db, cursors["after"].ToString());
                    }
                }
            }
        }
    }
}
