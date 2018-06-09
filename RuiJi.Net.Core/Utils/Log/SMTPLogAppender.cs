using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace RuiJi.Net.Core.Utils.Log
{
    public class SMTPLogAppender : LogAppenderBase
    {
        public SMTPLogAppender(string to, string from, string password, string subject, string host) : base()
        {
            To = to;
            From = from;
            Password = password;
            Subject = subject;
            Host = host;
        }

        public SMTPLogAppender(LayoutModel layout, string to, string from, string password, string subject, string host) : base(layout)
        {
            To = to;
            From = from;
            Password = password;
            Subject = subject;
            Host = host;
        }

        public SMTPLogAppender(List<Level> levels, string to, string from, string password, string subject, string host) : base(levels)
        {
            To = to;
            From = from;
            Password = password;
            Subject = subject;
            Host = host;
        }

        public SMTPLogAppender(List<Level> levels, LayoutModel layout, string to, string from, string password, string subject, string host) : base(levels, layout)
        {
            To = to;
            From = from;
            Password = password;
            Subject = subject;
            Host = host;
        }

        public string To { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Host { get; set; }

        public override List<IAppender> GetAppender()
        {
            var appender = new SmtpAppender();
            appender.Authentication = SmtpAppender.SmtpAuthentication.Basic;
            appender.Name = "SmtpAppender";

            appender.To = To;
            appender.From = From;
            appender.Username = From;
            appender.Password = Password;
            appender.Subject = Subject;
            appender.SmtpHost = Host;

            appender.Lossy = true;

            var level = Levels == null || Levels.Count == 0 ? log4net.Core.Level.Fatal : Levels[0];
            appender.Threshold = level;
            appender.Evaluator = new log4net.Core.LevelEvaluator(level);

            appender.Layout = GetLayout(Layout.Container, Layout.Header, Layout.Footer);
            appender.ActivateOptions();
            return new List<IAppender>() { appender };
        }
    }
}
