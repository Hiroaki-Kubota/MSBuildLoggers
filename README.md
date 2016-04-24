# MSBuildLoggers

プログラム練習を兼ねたMSBuild用のロガーです。
JenkinsでビルドプロジェクトへこのDLLを配備するには、
Custom Tools Plugin (https://wiki.jenkins-ci.org/display/JENKINS/Custom+Tools+Plugin) を使用すると簡単です。

## TSVConsoleLogger

このロガーはエラー・警告の出力とサマリーの出力を行います。
各項目をタブ区切りで出力しているのでExcelへコピペしてフィルタできます。
また、既定のエンコードをUTF-8としているため、JenkinsのMSBuild Puginを使用しても日本語が文字化けしません。

### 使い方

```
msbuild /noconsolelogger /logger:TSVConsoleLogger,MSBuildLoggers.dll
```

のようにして既定のコンソールロガーを置き換えて使用します。
`MSBuildLoggers.dll`はフルパスでしていするか、MSBuildLoggers.dllをパスの通った場所に置いておきます。
ロガーのパラメータは、それぞれセミコロンで区切って指定します。

### パラメータ

<dl>
<dt>ENCODING</dt>
<dd>出力のエンコードを指定します。既定ではUTF-8です。ENCODING=SJISのように指定します。</dd>
<dt>HIDEWARNINGS</dt>
<dd>指定すると、警告を出力しません。</dd>
<dt>HIDEERRORS</dt>
<dd>指定すると、エラーを出力しません。</dd>
<dt>TRIMPATH</dt>
<dd>
エラー・警告が発生したプロジェクトファイルおよびソースファイルのパスから指定された文字列を取り除きます。
Jenkinsからビルドする際に、ワークスペースのパスを指定しておくとファイル名が簡略化されて見やすいです。
</dd>
</dl>

```パラメータの指定例
msbuild /noconsolelogger /logger:TSVConsoleLogger,MSBuildLoggers.dll;Encoding=SJIS;HideWarnings;TrimPath="C:\MySource\MyProject"
```

### Example for jenkins

```MSBuild タスクの引数設定例
/noconsolelogger /logger:TSVConsoleLogger,%MSBuildLoggers_HOME%\MSBuildLoggers.dll;TrimPath="%WORKSPACE%\\"
```

## TSVFileLogger

TSVConsoleLoggerのファイル出力版です。

### パラメータ

TSVConsoleLogger のパラメータがすべて使用できます。
加えて、下記２つのパラメータが使用できます。
<dl>
<dt>LOGFILE</dt>
<dd>ビルド ログの書き込み先のログファイルへのパス。既定では"msbuild.log"です。</dd>
<dt>APPEND</dt>
<dd>指定すると、ログファイルに追記します。既定ではログファイルに追加されません。</dd>
</dl>