using Microsoft.Extensions.DependencyInjection;

namespace DigitalWorldOnline.Game
{
    public static class SingletonResolver
    {
        public static IServiceProvider Services { get; set; }

        public static T GetService<T>() where T : notnull
        {
            return Services.GetRequiredService<T>();
        }
        
        // Function to retrieve all registered services
        public static List<Type> GetAllRegisteredServices()
        {
            var serviceDescriptors = Services.GetService<IServiceCollection>();
            if (serviceDescriptors == null)
                throw new InvalidOperationException("ServiceCollection is not available in the current context.");

            return serviceDescriptors.Select(descriptor => descriptor.ServiceType).ToList();
        }
    }
}