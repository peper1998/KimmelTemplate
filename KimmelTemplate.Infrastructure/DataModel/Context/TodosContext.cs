using KimmelTemplate.Domain.Todos;
using KimmelTemplate.Infrastructure.DataModel.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KimmelTemplate.Infrastructure.DataModel.Context
{
    public class TodosContext : DbContext
    {
        public TodosContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TodoConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Todos"));
            }
        }
    }
}
