namespace FBPostsFetcher.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class FBPostsFetcherContext : DbContext
    {
        // 您的內容已設定為使用應用程式組態檔 (App.config 或 Web.config)
        // 中的 'FBPostsFetcher' 連接字串。根據預設，這個連接字串的目標是
        // 您的 LocalDb 執行個體上的 'FBPostsFetcher.Models.FBPostsFetcher' 資料庫。
        // 
        // 如果您的目標是其他資料庫和 (或) 提供者，請修改
        // 應用程式組態檔中的 'FBPostsFetcher' 連接字串。
        public FBPostsFetcherContext()
            : base("name=FBPostsFetcher")
        {
        }

        // 針對您要包含在模型中的每種實體類型新增 DbSet。如需有關設定和使用
        // Code First 模型的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=390109。

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<From> Froms { get; set; }
    }

    public class Post
    {
        public Post()
        {
            this.attachments = new List<Attachment>();
        }

        [Key]
        // Disable Auto-increment Feature
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string id { get; set; }
        public string fb_id { get; set; }
        public string raw_json { get; set; }
        public From from { get; set; }
        public string message { get; set; }
        public string created_time { get; set; }
        public string permalink_url { get; set; }

        public string link { get; set; }

        public List<Attachment> attachments { get; set; }
        //public string name { get; set; }
        //public string description { get; set; }
        //public string caption { get; set; }
    }

    public class From
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Attachment
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }
}