using KimmelTemplate.ApplicationServices.Boundaries;
using KimmelTemplate.Domain;
using KimmelTemplate.Infrastructure.DataModel.Context;
using KimmelTemplate.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KimmelTemplate.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<ITodosRepository, TodosRepository>();

            return serviceCollection.AddPersistenceModule(configuration);
        }

        public static IServiceCollection AddPersistenceModule(
           this IServiceCollection serviceCollection,
           IConfiguration configuration)
        {
            return serviceCollection
                .AddDbContext<TodosContext>((options) =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DB"));
                });
        }
    }
}
