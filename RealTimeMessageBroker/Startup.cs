//|---------------------------------------------------------------|
//|                     REAL TIME MESSAGE BROKER                  |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                     REAL TIME MESSAGE BROKER                  |
//|---------------------------------------------------------------|
using System;
using System.Web.Http;
using Owin;

using Ninject;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin;
using Microsoft.Owin.Cors;

using RealTimeServiceBroker.SignalRHubs;
using RealTimeServiceBroker.Interfaces;
using RealTimeServiceBroker.MessageBroadCaster;
using RealTimeServiceBroker.DependencyContainer;


[assembly: OwinStartup(typeof(RealTimeServiceBroker.Startup))]
namespace RealTimeServiceBroker
{
    /// <summary>
    /// OWIN startup class
    /// </summary>
    public class Startup
    {
        #region Public Methods 

        /// <summary>
        /// Configuration value
        /// </summary>
        /// <param name="app">IAppBuilder value</param>
        public void Configuration(IAppBuilder app)
        {
            var kernel = new StandardKernel();

            // SignalR Hub DP resolver
            var signalRDependencyResolver = new NInjectSignalRDependencyResolver(kernel);

            // Register hub connection context
            kernel.Bind(typeof(IHubConnectionContext<dynamic>)).
                  ToMethod(context =>
                  signalRDependencyResolver.Resolve<IConnectionManager>().
                  GetHubContext<BroadCastHub>().Clients).
                  WhenInjectedInto<IBroadCast>();

            // Register message broadcaster class
            kernel.Bind<IBroadCast>().
                ToConstant<BroadCaster>(new BroadCaster());

            // IBroadcast DP resolver
            GlobalConfiguration.Configuration.DependencyResolver = new NInjectDependencyResolver(kernel);
          
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null; // Unlimited incoming message size

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(new HubConfiguration()
                {
                    EnableDetailedErrors = true,
                    Resolver = signalRDependencyResolver,
                });
            });
        }

        #endregion

    }
}
