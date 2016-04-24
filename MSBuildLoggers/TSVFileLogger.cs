using System;
using Microsoft.Build.Framework;

namespace MSBuildLoggers {
    public class TSVFileLogger : TSVConsoleLogger {

        private class Opions {
            public string LogFile = "msbuild.log";
            public bool Apped = false;
        }

        private Opions options = new Opions();
        private System.IO.StreamWriter logStream;

        public override void Initialize(IEventSource eventSource) {
            base.Initialize(eventSource);
            ParseParameters();
            logStream = new System.IO.StreamWriter(options.LogFile, options.Apped);
        }

        void ParseParameters() {
            if (Parameters == null) return;

            string[] parameters = Parameters.Split(';');

            foreach (string parameter in parameters) {
                if (parameter.ToUpper().StartsWith("LOGFILE")) {
                    int index = parameter.IndexOf('=');
                    options.LogFile = parameter.Substring(index + 1).Trim('"').Trim();
                }
                else if (parameter.ToUpper() == "APPEND") {
                    options.Apped = true;
                }
                else {
                    throw new ArgumentException(Resources.UnrecognizedParameter, parameter);
                }
            }
        }

        public override void Shutdown() {
            base.Shutdown();
            if (logStream != null) {
                logStream.Dispose();
            }
        }

        internal override void OutputError(object sender, BuildErrorEventArgs e) {
            if (GetHeaderOnlyFirstTyme() != null) { logStream.WriteLine(GetHeaderOnlyFirstTyme()); }
            logStream.WriteLine(FormatErrorEvent(e));
        }

        internal override void OutputWarning(object sender, BuildWarningEventArgs e) {
            if (GetHeaderOnlyFirstTyme() != null) { logStream.WriteLine(GetHeaderOnlyFirstTyme()); }
            logStream.WriteLine(FormatWarningEvent(e));
        }

        internal override void OutputBuildFinished(object sender, BuildFinishedEventArgs e) {
            logStream.WriteLine(FormatBuildFinishedEvent(e));
        }
    }
}
