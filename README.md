# MSBuildLoggers

プログラム練習を兼ねたMSBuild用のロガーです。
JenkinsでビルドプロジェクトへこのDLLを配備するには、
Custom Tools Plugin (https://wiki.jenkins-ci.org/display/JENKINS/Custom+Tools+Plugin) を使用すると簡単です。

## MinimalSummaryConsoleLogger

このロガーはエラー・警告の出力とサマリーの出力を行います。
各項目をタブ区切りで出力しているのでExcelへコピペしてフィルタできます。
また、既定のエンコードをUTF-8としているため、JenkinsのMSBuild Puginを使用しても日本語が文字化けしません。

### Example for jenkins

MSBuild タスクの引数に次のように記述します:
/noconsolelogger /logger:MinimalSummaryConsoleLogger,%MSBuildLoggers_HOME%\MSBuildLoggers.dll;TrimPath="%WORKSPACE%\\"