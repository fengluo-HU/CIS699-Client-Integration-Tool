using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Web;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ClientIntegrator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Initialize(args);
                return 0;
            }
            finally
            {
                //give nlog async some time to sync
                Task.Delay(1000 * 5).Wait();
            }
        }


        public static IHost Initialize(string[] args, bool run = true)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true)
               .AddJsonFile($"appsettings.{environment}.json", optional: true)
               .AddEnvironmentVariables()
               .Build() as IConfiguration;

            var loggingConfig = config.GetSection("Logging");

            var nLogConfiguration = new XmlLoggingConfiguration("nlog.config");

            if (!bool.Parse(loggingConfig["EnableConsoleLog"]))
            {
                nLogConfiguration.RemoveTarget("console");
            }

            if (!bool.Parse(loggingConfig["EnableFileLog"]))
            {
                nLogConfiguration.RemoveTarget("allfile");
            }

            if (!bool.Parse(loggingConfig["EnableGrayLog"]))
            {
                nLogConfiguration.RemoveTarget("GelfUdp");
            }

            var logger = NLogBuilder.ConfigureNLog(nLogConfiguration).GetCurrentClassLogger();

            try
            {
                logger.Info("Application started..");

                var host = BuildWebHost(args, config);
                if (run)
                {
                    host.Run();
                }
                return host;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHost BuildWebHost(string[] args, IConfiguration config)
        {
            var host = Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder
                     .UseStartup<Startup>()
                     .UseConfiguration(config);
                 })
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();
                     logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                 })
                 .UseNLog()
                 .Build();

            return host;
        }
    }
}
