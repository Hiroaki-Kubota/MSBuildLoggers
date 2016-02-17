using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildLoggers
{
    /// <summary>
    /// Logger to output minimum and output, a summary to the console.
    /// </summary>
    public class MinimalSummaryConsoleLogger : Logger
    {
        private class Options
        {
            public bool ShowErrors = true;
            public bool ShowWarnings = true;
            public string TrimPath = string.Empty;
        }

        int warningCount = 0;
        int errorCount = 0;
        Options options = new Options();

        const string HEADER = "Warning/Error\tProject\tFile\t(Line,Column)\tMessage";
        const string WARNING_MESSAGE = "WARNING\t{0}\t{1}\t({2}, {3})\t{4}";
        const string ERROR_MESSAGE = "ERROR\t{0}\t{1}\t({2}, {3})\t{4}";

        public override void Initialize(IEventSource eventSource)
        {
            Console.OutputEncoding = new System.Text.UTF8Encoding();
            ParseParameters();
            eventSource.WarningRaised += eventSource_WarningRaised;
            eventSource.ErrorRaised += eventSource_ErrorRaised;
            eventSource.BuildFinished += eventSource_BuildFinished;
            Console.WriteLine(HEADER);
        }

        void ParseParameters()
        {
            if (Parameters == null)
            {
                return;
            }
            string[] parameters = Parameters.Split(';');

            foreach (string parameter in parameters)
            {
                if (parameter.ToUpper().StartsWith("TRIMPATH"))
                {
                    int index = parameter.IndexOf('=');
                    this.options.TrimPath = parameter.Substring(index + 1).Trim('"').Trim();
                }
                else if (parameter.ToUpper().StartsWith("ENCODING"))
                {
                    int index = parameter.IndexOf('=');
                    String webname = parameter.Substring(index + 1).Trim('"').Trim();
                    Console.OutputEncoding = System.Text.Encoding.GetEncoding(webname);
                }
                else if (parameter.ToUpper() == "HIDEERRORS")
                {
                    this.options.ShowErrors = false;
                }
                else if (parameter.ToUpper() == "HIDEWARNINGS")
                {
                    this.options.ShowWarnings = false;
                }
                else
                {
                    throw new ArgumentException("Unrecognized parameter : ", parameter);
                }
            }
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            warningCount++;
            if (this.options.ShowWarnings == true)
            {
                Console.WriteLine(WARNING_MESSAGE,
                                  trimPath(e.ProjectFile),
                                  trimPath(e.File),
                                  e.LineNumber, e.ColumnNumber, e.Message);
            }
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            errorCount++;
            if (this.options.ShowErrors == true)
            {
                Console.WriteLine(ERROR_MESSAGE,
                                  trimPath(e.ProjectFile),
                                  trimPath(e.File),
                                  e.LineNumber, e.ColumnNumber, e.Message);
            }
        }

        void eventSource_BuildFinished(object sender, BuildFinishedEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Processing completed: " + errorCount + " error(s) | " + warningCount + " warning(s)");
            Console.WriteLine("");
        }

        string trimPath(string path)
        {
            if (string.IsNullOrWhiteSpace(this.options.TrimPath))
            {
                return path;
            }
            else
            {
                return path.Replace(this.options.TrimPath, string.Empty);
            }
        }
    }
}
