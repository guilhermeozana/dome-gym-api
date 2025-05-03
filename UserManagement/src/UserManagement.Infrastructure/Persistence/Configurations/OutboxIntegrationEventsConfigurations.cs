using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserManagement.Infrastructure.IntegrationEvents;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public class OutboxIntegrationEventConfigurations : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder
            .Property<int>("Id")
            .ValueGeneratedOnAdd();

        builder.HasKey("Id");

        builder.Property(o => o.EventName);

        builder.Property(o => o.EventContent);
    }
}
