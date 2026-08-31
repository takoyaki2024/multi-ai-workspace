# Multi AI Workspace

ChatGPT・Gemini・ClaudeのWeb版を、1つのWindowsデスクトップ画面に横並びで表示する無料のWPFアプリです。有料APIやクラウドサービスは使わず、各サービスの通常のWebサイトをWebView2で開きます。

## 対応AI

- ChatGPT — `https://chatgpt.com/`
- Gemini — `https://gemini.google.com/`
- Claude — `https://claude.ai/`

## 初回セットアップ

1. 初回セットアップでビルドするため、Windows 10/11に [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) と [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) が入っていることを確認します。通常のWindows 10/11にはWebView2が導入済みです。
2. リポジトリ直下の `install-desktop-launcher.bat` をダブルクリックします。
3. ビルドが終わると、デスクトップに「Multi AI Workspace」ショートカットが作成されます。

以降はショートカットをダブルクリックするだけで起動できます。アプリ本体は `%LocalAppData%\MultiAiWorkspace\App` に配置されます。更新後は同じbatをもう一度実行してください。

## ログイン

初回起動時に3つのペインそれぞれで通常どおりログインしてください。アプリはパスワードを保存しません。WebView2のセッションは次の独立フォルダーに保存されるため、サービス間でCookieは共有されず、次回起動時も可能な限りログイン状態が維持されます。

`%LocalAppData%\MultiAiWorkspace\WebViewProfiles\chatgpt`、`gemini`、`claude`

CAPTCHA、追加認証、サービス側の利用制限は回避しません。表示された場合は各Webページ上で完了してください。

## 使い方

上部の共通入力欄にプロンプトを書きます。

- **全部へ貼付**: 3サイト本体の入力欄へ文章を入れます。自動送信しません。内容を確認してから送信できます。
- **全部へ送信**: 各サイト本体に現在入っている文章を送信します。1サイトが失敗しても残りは実行されます。
- **個別の貼付**: そのAIだけへ共通入力を入れます。自動送信しません。
- **個別の送信**: そのAIのWeb入力欄に現在ある文章を送信します。
- **コピー**: そのAIの最新アシスタント回答をWindowsクリップボードへコピーします。
- **境界線のドラッグ**: ペイン幅を変更できます。「均等に戻す」で3等分へ戻ります。
- **F11 / 全画面**: 全画面表示を切り替えます。

操作結果やエラーは各ペイン上部に表示されます。Webサイト側で文章を生成中の場合、送信ボタンが無効なことがあります。

## DOM変更について

Webサイトの画面構造（DOM）は各社が予告なく変更します。本アプリは `textarea`、`contenteditable`、`role=textbox` やサイト固有属性など複数候補を順に試しますが、変更直後は貼付・送信・コピーが一時的に動かない場合があります。Webサイトの閲覧・手入力はそのまま利用できます。

修正する場合は `src/MultiAiWorkspace/Providers` 内の該当Providerにある `InputSelectors`、`SendSelectors`、`ResponseSelectors` を、ブラウザー開発者ツールで確認した現在の要素に合わせて更新します。サイト固有DOMはMainWindowには置いていません。

## 開発とテスト

```powershell
dotnet restore MultiAiWorkspace.sln
dotnet build MultiAiWorkspace.sln --configuration Release --no-restore
dotnet test MultiAiWorkspace.sln --configuration Release --no-build
```

テストはURL判定、Provider選択、設定、3ペイン定義、処理失敗時の障害分離を対象にしています。ライブサイトのDOM操作成功をCIで保証するテストは、サイト変更やログイン状態に左右されるため意図的に含めていません。GitHub ActionsはWindows runnerでrestore・build・testを実行します。

## プライバシーとデータ

入力内容は選択したWebサービスのページへ直接投入されます。本アプリ独自のサーバー、DB、APIキー、認証情報保存はありません。各サービスの利用規約とプライバシーポリシーに従って利用してください。
