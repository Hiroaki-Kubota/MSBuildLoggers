using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildLoggers {
    /// <summary>
    /// Logger to output minimum and output, a summary to the console.
    /// </summary>
    public class MinimalSummaryConsoleLogger : Logger {
        #region LoggerOption

        /// <summary>
        /// Options for <see cref="MinimalSummaryConsoleLogger"/>
        /// </summary>
        private class Options {
            /// <summary>
            /// If you want to show errors, set true. Otherwise false.
            /// </summary>
            public bool ShowErrors = true;

            /// <summary>
            /// If you want to show warnings, set true. Otherwise false.
            /// </summary>
            public bool ShowWarnings = true;

            /// <summary>
            /// String to be ignored at log outputs.
            /// </summary>
            public string TrimPath = string.Empty;
        }

        #endregion

        int warningCount = 0;
        int errorCount = 0;
        Options options = new Options();

        /// <summary>
        /// Subscrive events.
        /// </summary>
        /// <param name="eventSource"></param>
        public override void Initialize(IEventSource eventSource) {
            Console.OutputEncoding = new System.Text.UTF8Encoding();
            ParseParameters();
            eventSource.WarningRaised += eventSource_WarningRaised;
            eventSource.ErrorRaised += eventSource_ErrorRaised;
            eventSource.BuildFinished += eventSource_BuildFinished;

        }

        void ParseParameters() {
            if (Parameters == null) {
                return;
            }
            string[] parameters = Parameters.Split(';');

            foreach (string parameter in parameters) {
                if (parameter.ToUpper().StartsWith("TRIMPATH")) {
                    int index = parameter.IndexOf('=');
                    this.options.TrimPath = parameter.Substring(index + 1).Trim('"').Trim();
                }
                else if (parameter.ToUpper().StartsWith("ENCODING")) {
                    int index = parameter.IndexOf('=');
                    String webname = parameter.Substring(index + 1).Trim('"').Trim();
                    Console.OutputEncoding = System.Text.Encoding.GetEncoding(webname);
                }
                else if (parameter.ToUpper() == "HIDEERRORS") {
                    this.options.ShowErrors = false;
                }
                else if (parameter.ToUpper() == "HIDEWARNINGS") {
                    this.options.ShowWarnings = false;
                }
                else {
                    throw new ArgumentException(Resources.UnrecognizedParameter, parameter);
                }
            }
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e) {
            warningCount++;
            if (this.options.ShowWarnings == true) {
                printHeaderOnlyFirstTime();
                Console.WriteLine(Resources.WARNING_MESSAGE.Replace(@"\t", "\t"),
                                  trimPath(e.ProjectFile),
                                  trimPath(e.File),
                                  e.LineNumber, e.ColumnNumber, e.Message);
            }
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e) {
            errorCount++;
            if (this.options.ShowErrors == true) {
                printHeaderOnlyFirstTime();
                Console.WriteLine(Resources.ERROR_MESSAGE.Replace(@"\t", "\t"),
                                  trimPath(e.ProjectFile),
                                  trimPath(e.File),
                                  e.LineNumber, e.ColumnNumber, e.Message);
            }
        }

        void eventSource_BuildFinished(object sender, BuildFinishedEventArgs e) {
            Console.WriteLine("");
            Console.WriteLine(Resources.Completed, errorCount, warningCount);
            Console.WriteLine("");
        }

        void printHeaderOnlyFirstTime() {
            if ((this.options.ShowErrors && this.errorCount == 1) ||
                (this.options.ShowWarnings && this.warningCount == 1)) {
                Console.WriteLine(Resources.HEADER.Replace(@"\t", "\t"));
            }
        }

        string trimPath(string path) {
            if (string.IsNullOrWhiteSpace(this.options.TrimPath)) {
                return path;
            }
            else {
                return path.Replace(this.options.TrimPath, string.Empty);
            }
        }
    }
}
