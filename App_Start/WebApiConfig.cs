using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;
using System.Web.Http;

namespace FtpFileCommunication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            SetNLogConfig();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static void SetNLogConfig()
        {
            var applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            var directoryPath = Path.Combine(
                Path.GetDirectoryName(applicationPath), "bin", "Logs");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var fileTarget = new FileTarget
            {
                FileName = "${basedir}/Logs/${logger}.log",
                Layout = "${date} --|-- ${message}",
                MaxArchiveDays = 30
            };
            var logConsole = new ConsoleTarget("logconsole");

            var rule1 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            var rule5 = new LoggingRule("*", LogLevel.Debug, logConsole);

            var config = new LoggingConfiguration();
            config.AddTarget("log1", fileTarget);
            config.LoggingRules.Add(rule1);
            config.LoggingRules.Add(rule5);

            LogManager.Configuration = config;
        }
    }
}
