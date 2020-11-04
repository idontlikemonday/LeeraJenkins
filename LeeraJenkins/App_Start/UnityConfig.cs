using System;
using Unity;
using LeeraJenkins.Code;
using LeeraJenkins.Configuration;

namespace LeeraJenkins
{
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterBot(container);
              UnityConfiguration.RegisterTypes(container);
              return container;
          });

        public static IUnityContainer Container => container.Value;

        private static void RegisterBot(IUnityContainer container)
        {
            container.RegisterType<IBot, Bot>();
        }
    }
}