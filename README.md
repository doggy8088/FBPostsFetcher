# FBPostsFetcher

FBPostsFetcher 可用來取得特定個人與粉絲專頁所有貼文。

## 使用方式

1. 取得原始碼

    ```sh
    git clone https://github.com/doggy8088/FBPostsFetcher.git
    ```

2. 使用 Visual Studio 2017+ 開啟專案

3. 調整應用程式設定參數 ( `app.config` )

    - `connectionStrings`
      - 因為採用 Entity Framework Code First 開發，只要填寫連接字串後，資料庫與表格會自動建立完成。
        - 建議設定: `data source=(LocalDb)\MSSQLLocalDB;initial catalog=FBPostsFetcher;integrated security=True;MultipleActiveResultSets=True;App=FBPostsFetcher`
    - `userSettings`
      - 請透過 [圖形 API 測試工具](https://developers.facebook.com/tools/explorer/) 取得**存取權杖**(`FBAccessToken`)
        - `me/feed?pretty=0&fields=id,from,name,message,story,created_time,link,description,caption,attachments&limit=100`
        - `119279178101235/feed?pretty=0&fields=id,from,name,message,story,created_time,link,description,caption,attachments&limit=100`
      - 請透過 [Find your Facebook ID](https://findmyfbid.com/) 取得**用戶編號**(`FBID`)
      - 設定 `ContinueLastError`  為 `True` 可以讓錯誤發生時，不會從頭開始抓取訊息！
        - 當發生錯誤時，會自動建立 `exception_point.txt` 中斷點檔案，保存發生錯誤時的 URL 為何。
        - 當所有資料都抓取回來後，中斷點檔案 `exception_point.txt` 將會被刪除。

## 相關連結

- [Will 保哥的技術交流中心](https://www.facebook.com/will.fans)
- [所有應用程式 - Facebook for Developers](https://developers.facebook.com/apps/)
- [使用 API - 行銷 API](https://developers.facebook.com/docs/marketing-api/using-the-api)
  - [最佳作法 - 行銷 API](https://developers.facebook.com/docs/marketing-api/best-practices)
- [Facebook api: (#4) Application request limit reached](https://stackoverflow.com/questions/14092989/facebook-api-4-application-request-limit-reached)
- [Graph API - Page Feed](https://developers.facebook.com/docs/graph-api/reference/v3.3/page/feed)
- [Graph API - {user-id} Feed](https://developers.facebook.com/docs/graph-api/reference/v3.3/user/feed)
- [Facebook SDK for .NET](https://github.com/facebook-csharp-sdk/facebook-csharp-sdk)
  - [Community Contents](https://github.com/facebook-csharp-sdk/facebook-csharp-sdk/wiki/Community-Contents)
  - [Docs](https://github.com/facebook-csharp-sdk/facebook-csharp-sdk.github.com/tree/master/docs)
  - [FB C# SDK - 粉絲專頁](https://www.facebook.com/csharpsdk)
  - [FB C# SDK - Twitter](https://twitter.com/csharpsdk)
- [CODE-查詢特定網址的臉書按讚數-黑暗執行緒](https://blog.darkthread.net/blog/get-fb-likes-count/)