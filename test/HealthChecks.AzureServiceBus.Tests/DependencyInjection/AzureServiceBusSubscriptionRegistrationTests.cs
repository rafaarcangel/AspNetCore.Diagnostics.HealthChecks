using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace HealthChecks.AzureServiceBus.Tests
{
    public class azure_service_bus_subscription_registration_should
    {
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddAzureServiceBusSubscription("cnn", "topicName", "subscriptionName");

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("azuresubscription");
            check.GetType().Should().Be(typeof(AzureServiceBusSubscriptionHealthCheck));
        }

        [Fact]
        public void add_health_check_using_factories_when_properly_configured()
        {
            var services = new ServiceCollection();
            bool connectionStringFactoryCalled = false, topicNameFactoryCalled = false, subscriptionNameFactoryCalled = false;
            services.AddHealthChecks()
                .AddAzureServiceBusSubscription(_ =>
                    {
                        connectionStringFactoryCalled = true;
                        return "cnn";
                    },
                    _ =>
                    {
                        topicNameFactoryCalled = true;
                        return "topicName";
                    },
                    _ =>
                    {
                        subscriptionNameFactoryCalled = true;
                        return "subscriptionName";
                    });

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("azuresubscription");
            check.GetType().Should().Be(typeof(AzureServiceBusSubscriptionHealthCheck));
            connectionStringFactoryCalled.Should().BeTrue();
            topicNameFactoryCalled.Should().BeTrue();
            subscriptionNameFactoryCalled.Should().BeTrue();
        }

        [Fact]
        public void add_named_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddAzureServiceBusSubscription("cnn", "topic", "subscriptionName",
                name: "azuresubscriptioncheck");

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("azuresubscriptioncheck");
            check.GetType().Should().Be(typeof(AzureServiceBusSubscriptionHealthCheck));
        }

        [Fact]
        public void add_named_health_check_using_factories_when_properly_configured()
        {
            var services = new ServiceCollection();
            bool connectionStringFactoryCalled = false, topicNameFactoryCalled = false, subscriptionNameFactoryCalled = false;
            services.AddHealthChecks()
                .AddAzureServiceBusSubscription(_ =>
                    {
                        connectionStringFactoryCalled = true;
                        return "cnn";
                    },
                    _ =>
                    {
                        topicNameFactoryCalled = true;
                        return "topicName";
                    },
                    _ =>
                    {
                        subscriptionNameFactoryCalled = true;
                        return "subscriptionName";
                    },
                    "azuresubscriptioncheck");

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("azuresubscriptioncheck");
            check.GetType().Should().Be(typeof(AzureServiceBusSubscriptionHealthCheck));
            connectionStringFactoryCalled.Should().BeTrue();
            topicNameFactoryCalled.Should().BeTrue();
            subscriptionNameFactoryCalled.Should().BeTrue();
        }

        [Fact]
        public void fail_when_no_health_check_configuration_provided()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddAzureServiceBusSubscription(string.Empty, string.Empty, string.Empty);

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();

            Assert.Throws<ArgumentNullException>(() => registration.Factory(serviceProvider));
        }
    }
}
