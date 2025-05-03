using System.Reflection;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using SessionReservation.Core.Common;
using SessionReservation.Domain.Common;
using SessionReservation.Domain.ParticipantAggregate;
using SessionReservation.Domain.RoomAggregate;
using SessionReservation.Domain.SessionAggregate;
using SessionReservation.Domain.TrainerAggregate;
using SessionReservation.Infrastructure.Middleware;

namespace SessionReservation.Infrastructure.Persistence;

public class SessionReservationDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;

    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Trainer> Trainers { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;

    public SessionReservationDbContext(
        DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
           .Select(entry => entry.Entity.PopDomainEvents())
           .SelectMany(x => x)
           .ToList();

        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
            return await base.SaveChangesAsync(cancellationToken);
        }

        await PublishDomainEvents(domainEvents);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value) &&
            value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents
            : new();

        domainEvents.ForEach(domainEventsQueue.Enqueue);
        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
    }
}