using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace SolEx.Hurt.Core
{
    public class CustomPatternLayout : PatternLayout
    {
        public CustomPatternLayout()
        {
            AddConverter("stack", typeof(StackTraceConverter));
        }
    }

    public class StackTraceConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var stack = new StackTrace();

            var frames = stack.GetFrames();
            for (var i = 0; i < frames.Length; i++)
            {
                var frame = frames[i];

                // if the stack frame corresponds to still being inside the log4net assembly, skip it.
                if (frame.GetMethod().DeclaringType != null && frame.GetMethod().DeclaringType.Assembly != typeof(LogManager).Assembly)
                {
                    writer.WriteLine("{0}.{1} line {2}",
                        frame.GetMethod().DeclaringType.FullName,
                        frame.GetMethod().Name,
                        frame.GetFileLineNumber());
                }
            }
        }
    }
}