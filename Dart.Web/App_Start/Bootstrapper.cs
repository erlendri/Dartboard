using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity.Mvc;
using Dart.Web.Managers;
using Dart.Web.Hubs;
using Dart.Web.Interfaces;
using Dart.GameManager;
using System.Configuration;
using uPLibrary.Networking.M2Mqtt;

namespace Dart.Web.App_Start
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            GlobalHost.DependencyResolver = new SignalRUnityDependencyResolver(container);
            return container;
        }
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            var ip = ConfigurationManager.AppSettings["mqttBrokerIp"];
            var client = new MqttClient(ip);
          
            var listener = new DartboardListener(client);
            var publisher = new BeerPublisher(client);
            container.RegisterInstance<IDartboardListener>(listener);
            container.RegisterInstance<IBeerPublisher>(publisher);
            var gm = new GameManager.GameManager(listener, publisher);
            container.RegisterInstance<IGameManager>(gm);
            var sm = new StoreManager();
            container.RegisterInstance<IStoreManager>(sm);
            RegisterHubs(container);
            return container;
        }

        private static void RegisterHubs(UnityContainer container)
        {
            container.RegisterType<ScoreHub>(new InjectionFactory(CreateScoreHub));
        }

        private static object CreateScoreHub(IUnityContainer container)
        {
            var hub = new ScoreHub(container.Resolve<IStoreManager>(), container.Resolve<IGameManager>());
            return hub;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
        }
    }

    public class SignalRUnityDependencyResolver : DefaultDependencyResolver
    {
        private IUnityContainer _container;

        public SignalRUnityDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            if (_container.IsRegistered(serviceType)) return _container.Resolve(serviceType);
            return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            if (_container.IsRegistered(serviceType)) return _container.ResolveAll(serviceType);
            return base.GetServices(serviceType);
        }
    }
}