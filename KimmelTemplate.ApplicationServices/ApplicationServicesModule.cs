using Microsoft.Extensions.DependencyInjection;


namespace KimmelTemplate.ApplicationServices
{
    public static class ApplicationServicesModule
    {
        public static IServiceCollection AddApplicationServicesModule(
            this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(ApplicationServicesModule).Assembly));
        }
    }
}
