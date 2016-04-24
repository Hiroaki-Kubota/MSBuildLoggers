using System;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildLoggers {
    /// <summary>
    /// MSBuild のビルド出力のうち、エラー・警告のみをTSV形式で出力するコンソールロガーです。
    /// </summary>
    public class TSVConsoleLogger : Logger {
        #region LoggerOption

        /// <summary>
        /// <see cref="TSVConsoleLogger"/> のオプション
        /// </summary>
        class Options {
            /// <summary>
            /// エラーを出力する場合は true 。それ以外は false 。
            /// </summary>
            public bool ShowErrors = true;

            /// <summary>
            /// 警告を出力する場合は true 。それ以外は false 。
            /// </summary>
            public bool ShowWarnings = true;

            /// <summary>
            /// エラー・警告の出力から取り除く文字列を指定します。
            /// プロジェクトファイルが含まれるディレクトリのパスを指定すると、エラーが発生したファイル名の表示を簡略化することができます。
            /// </summary>
            public string TrimPath = string.Empty;

            /// <summary>
            /// 出力のエンコーディング
            /// </summary>
            private Encoding outputEncoding = new UTF8Encoding();

            public Encoding OutputEncoding
            {
                get { return outputEncoding; }
                set { outputEncoding = value; }
            }
        }

        #endregion

        internal int warningCount = 0;
        internal int errorCount = 0;
        Options options = new Options();

        /// <summary>
        /// Subscrive events.
        /// </summary>
        /// <param name="eventSource"></param>
        public override void Initialize(IEventSource eventSource) {
            ParseParameters();
            Console.OutputEncoding = options.OutputEncoding;
            eventSource.WarningRaised += eventSource_WarningRaised;
            eventSource.ErrorRaised += eventSource_ErrorRaised;
            eventSource.BuildFinished += eventSource_BuildFinished;
        }

        /// <summary>
        /// パラメータの解釈を行って <see cref="options"/> に反映します。
        /// </summary>
        void ParseParameters() {
            if (Parameters == null) return;

            string[] parameters = Parameters.Split(';');

            foreach (string parameter in parameters) {
                if (parameter.ToUpper().StartsWith("TRIMPATH")) {
                    int index = parameter.IndexOf('=');
                    options.TrimPath = parameter.Substring(index + 1).Trim('"').Trim();
                }
                else if (parameter.ToUpper().StartsWith("ENCODING")) {
                    int index = parameter.IndexOf('=');
                    string webname = parameter.Substring(index + 1).Trim('"').Trim();
                    options.OutputEncoding = Encoding.GetEncoding(webname);
                }
                else if (parameter.ToUpper() == "HIDEERRORS") {
                    options.ShowErrors = false;
                }
                else if (parameter.ToUpper() == "HIDEWARNINGS") {
                    options.ShowWarnings = false;
                }
                else {
                    throw new ArgumentException(Resources.UnrecognizedParameter, parameter);
                }
            }
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e) {
            warningCount++;
            if (options.ShowWarnings == true) {
                OutputWarning(sender, e);
            }
        }

        public override string FormatWarningEvent(BuildWarningEventArgs args) {
            return (string.Format(Resources.WARNING_MESSAGE.Replace(@"\t", "\t"),
                                  trimPath(args.ProjectFile),
                                  trimPath(args.File),
                                  args.LineNumber, args.ColumnNumber, args.Message));
        }

        internal virtual void OutputWarning(object sender, BuildWarningEventArgs e) {
            if (GetHeaderOnlyFirstTyme() != null) { Console.WriteLine(GetHeaderOnlyFirstTyme()); }
            Console.WriteLine(FormatWarningEvent(e));
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e) {
            errorCount++;
            if (options.ShowErrors == true) {
                OutputError(sender, e);
            }
        }

        public override string FormatErrorEvent(BuildErrorEventArgs args) {
            return (string.Format(Resources.ERROR_MESSAGE.Replace(@"\t", "\t"),
                                  trimPath(args.ProjectFile),
                                  trimPath(args.File),
                                  args.LineNumber, args.ColumnNumber, args.Message));
        }

        internal virtual void OutputError(object sender, BuildErrorEventArgs e) {
            if (GetHeaderOnlyFirstTyme() != null) { Console.WriteLine(GetHeaderOnlyFirstTyme()); }
            Console.WriteLine(FormatErrorEvent(e));
        }

        void eventSource_BuildFinished(object sender, BuildFinishedEventArgs e) {
            OutputBuildFinished(sender, e);
        }

        internal virtual string FormatBuildFinishedEvent(BuildFinishedEventArgs args) {
            return "\r\n" + string.Format(Resources.Completed, errorCount, warningCount) + "\r\n";
        }

        internal virtual void OutputBuildFinished(object sender, BuildFinishedEventArgs e) {
            Console.WriteLine(FormatBuildFinishedEvent(e));
        }

        internal string GetHeaderOnlyFirstTyme() {
            if ((options.ShowErrors && errorCount == 1) ||
               (options.ShowWarnings && warningCount == 1)) {
                return (string.Format(Resources.HEADER.Replace(@"\t", "\t")));
            }
            return null;
        }

        string trimPath(string path) {
            if (string.IsNullOrWhiteSpace(options.TrimPath)) {
                return path;
            }
            else {
                return path.Replace(options.TrimPath, string.Empty);
            }
        }
    }
}
