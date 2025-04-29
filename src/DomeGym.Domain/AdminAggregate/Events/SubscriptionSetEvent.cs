using DomeGym.Domain.Common;
using DomeGym.Domain.SubscriptionAggregate;

namespace DomeGym.Domain.AdminAggregate.Events;

public record SubscriptionSetEvent(Admin Admin, Subscription Subscription) : IDomainEvent;