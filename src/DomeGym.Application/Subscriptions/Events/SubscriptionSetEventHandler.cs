using DomeGym.Application.Common.Interfaces;
using DomeGym.Domain.AdminAggregate.Events;

using MediatR;

namespace DomeGym.Application.Subscriptions.Events;

public class  SubscriptionSetEventHandler : INotificationHandler<SubscriptionSetEvent>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public SubscriptionSetEventHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task Handle(SubscriptionSetEvent notification, CancellationToken cancellationToken)
    {
        await _subscriptionsRepository.AddSubscriptionAsync(notification.Subscription);
    }
}
