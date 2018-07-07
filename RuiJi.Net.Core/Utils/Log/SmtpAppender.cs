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
    /// <summary>
    /// smtp logger appender
    /// </summary>
    public class SmtpAppender : AppenderBase
    {
        /// <summary>
        /// send mail to
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// mail from
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// smtp username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// smtp password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// mail subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// smtp host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="to">mail send to</param>
        /// <param name="from">mail from</param>
        /// <param name="username">smtp username</param>
        /// <param name="password">smtp password</param>
        /// <param name="subject">mail subject</param>
        /// <param name="host">smtp host</param>
        public SmtpAppender(string to, string from, string username, string password, string subject, string host)
        {
            To = to;
            From = from;
            Username = username;
            Password = password;
            Subject = subject;
            Host = host;
        }

        /// <summary>
        /// configure method
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="repository">repository</param>
        public override void Configure(string key, ILoggerRepository repository)
        {
            var appender = new log4net.Appender.SmtpAppender();
            appender.Authentication = log4net.Appender.SmtpAppender.SmtpAuthentication.Basic;
            appender.Name = "SmtpAppender";

            appender.To = To;
            appender.From = From;
            appender.Username = Username;
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
