using KimmelTemplate.Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KimmelTemplate.Infrastructure.DataModel.Mappings
{
    internal class TodoConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.ToTable(nameof(Todo));
            builder.HasKey(l => l.Id);
        }
    }
}
