using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Autofac;
using DepthViewerServer.Contracts;
using Microsoft.Owin;
using Owin;
using Parse;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Logging;
using Hangfire.Logging.LogProviders;
using Hangfire.SqlServer;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

[assembly: OwinStartup(typeof(DepthViewerServer.Startup))]

namespace DepthViewerServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Use Attribute Routing
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.MapHttpAttributeRoutes();
            app.UseWebApi(httpConfiguration);

            // Init IoC
            IoC.Initialize();

            // Init Parse
            InitParse();

            // Init Hangfire
            InitHangfire(app);
        }

        private void InitHangfire(IAppBuilder app)
        {
            var hangfireConfig = IoC.Container.Resolve<IHangfireConfig>();
            GlobalConfiguration.Configuration.UseSqlServerStorage(
                hangfireConfig.SqlServerConnectionString,
                new SqlServerStorageOptions()
                {
                    PrepareSchemaIfNecessary = true,
                    QueuePollInterval = TimeSpan.FromSeconds(1)
                });
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });

            LogProvider.SetCurrentLogProvider(new ElmahLogProvider(LogLevel.Trace));

            var options = new BackgroundJobServerOptions
            {
                WorkerCount = Environment.ProcessorCount * 5
            };
            app.UseHangfireServer(options);

        }

        private void InitParse()
        {
            var parseConfig = IoC.Container.Resolve<IParseConfig>();
            ParseClient.Initialize(parseConfig.AppId, parseConfig.NetKey);
        }

        public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
        {
            public bool Authorize(IDictionary<string, object> owinEnvironment)
            {
                // In case you need an OWIN context, use the next line,
                // `OwinContext` class is the part of the `Microsoft.Owin` package.
                var context = new OwinContext(owinEnvironment);

                if (context.Request.Cookies["isA"] != null && context.Request.Cookies["isA"] == "true")
                    return true;

                var user = context.Request.Query["u"];
                var password = context.Request.Query["p"];

                var allowedUserName = WebConfigurationManager.AppSettings["hangfireUsername"];
                var allowedUserPassword = WebConfigurationManager.AppSettings["hangfireUserPassword"];

                if (!(user != null & password != null)
                    || !user.Equals(allowedUserName)
                    || !password.Equals(allowedUserPassword))
                {
                    return false;
                }

                context.Response.Cookies.Append("isA", "true", new CookieOptions()
                {
                    Expires = DateTime.Now.AddHours(1)
                });
                return true;
            }
        }
    }
}
