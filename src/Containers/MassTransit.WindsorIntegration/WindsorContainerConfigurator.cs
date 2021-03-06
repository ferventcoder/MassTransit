using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MassTransit.Configuration;
using MassTransit.Saga;
using MassTransit.Saga.Pipeline;
using MassTransit.Serialization;
using MassTransit.Services.Subscriptions.Client;
using MassTransit.Services.Subscriptions.Server;
using Microsoft.Practices.ServiceLocation;

namespace MassTransit.WindsorIntegration
{
	public static class WindsorContainerConfigurator
	{
		public static IObjectBuilder InitializeContainer(IWindsorContainer container)
		{
			var wob = new WindsorObjectBuilder(container.Kernel);
			ServiceLocator.SetLocatorProvider(() => wob);

			container.Kernel.AddComponentInstance("kernel", typeof(IKernel), container.Kernel);
			container.Kernel.AddComponentInstance("objectBuilder", typeof(IObjectBuilder), wob);

			container.Register(
				Component.For<IObjectBuilder>()
					.ImplementedBy<WindsorObjectBuilder>()
					.LifeStyle.Singleton,

				// The subscription client
				Component.For<SubscriptionClient>()
					.ImplementedBy<SubscriptionClient>()
					.LifeStyle.Transient,

				// Message Serializers
				Component.For<BinaryMessageSerializer>()
					.ImplementedBy<BinaryMessageSerializer>()
					.LifeStyle.Singleton,
				Component.For<DotNotXmlMessageSerializer>()
					.ImplementedBy<DotNotXmlMessageSerializer>()
					.LifeStyle.Singleton,
				Component.For<XmlMessageSerializer>()
					.ImplementedBy<XmlMessageSerializer>()
					.LifeStyle.Singleton
				);

			ServiceBusConfigurator.Defaults(x => x.SetObjectBuilder(wob));

			return wob;
		}

		public static void RegisterInMemorySubscriptionRepository(IWindsorContainer container)
		{
			container.Kernel.Register(
				Component.For<ISubscriptionRepository>()
					.ImplementedBy<InMemorySubscriptionRepository>()
					.LifeStyle.Singleton
				);
		}

		public static void RegisterInMemorySagaRepository(IWindsorContainer container)
		{
			container.Kernel.Register(
				Component.For(typeof(ISagaRepository<>))
					.ImplementedBy(typeof(InMemorySagaRepository<>))
					.LifeStyle.Singleton
				);
		}
	}
}
