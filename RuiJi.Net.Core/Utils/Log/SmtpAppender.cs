using log4net;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    public class SmtpAppender : AppenderBase
    {
        public string To { get; set; }

        public string From { get; set; }

        public string Password { get; set; }

        public string Subject { get; set; }

        public string Host { get; set; }

        public SmtpAppender(string to, string from, string password, string subject, string host)
        {
            To = to;
            From = from;
            Password = password;
            Subject = subject;
            Host = host;
        }

        public override void Configure(string key, ILoggerRepository repository)
        {
            var appender = new log4net.Appender.SmtpAppender();
            appender.Authentication = log4net.Appender.SmtpAppender.SmtpAuthentication.Basic;
            appender.Name = "SmtpAppender";

            appender.To = To;
            appender.From = From;
            appender.Username = From;
            appender.Password = Password;
            appender.Subject = Subject;
            appender.SmtpHost = Host;

            appender.Lossy = true;
            appender.Threshold = Levels[0];
            appender.Evaluator = new log4net.Core.LevelEvaluator(Levels[0]);

            var layout = new PatternLayout(Pattern);
            layout.ActivateOptions();
            appender.Layout = layout;

            appender.ActivateOptions();

            BasicConfigurator.Configure(repository, appender);
        }
    }
}
