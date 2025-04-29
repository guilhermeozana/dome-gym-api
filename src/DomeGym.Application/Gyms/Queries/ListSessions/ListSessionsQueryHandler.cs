using DomeGym.Application.Common.Interfaces;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Queries.ListSessions;

public class ListSessionsQueryHandler : IRequestHandler<ListSessionsQuery, ErrorOr<List<Session>>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsQueryHandler(IGymsRepository gymsRepository, ISubscriptionsRepository subscriptionsRepository, ISessionsRepository sessionsRepository)
    {
        _gymsRepository = gymsRepository;
        _subscriptionsRepository = subscriptionsRepository;
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<List<Session>>> Handle(ListSessionsQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(query.SubscriptionId);

        if (subscription is null)
        {
            return Error.NotFound(description: "Subscription not found");
        }

        if (!subscription.HasGym(query.GymId))
        {
            return Error.NotFound(description: "Gym not found");
        }

        if (!await _gymsRepository.ExistsAsync(query.GymId))
        {
            return Error.NotFound(description: "Gym not found");
        }

        return await _sessionsRepository.ListByGymIdAsync(query.GymId, query.StartDateTime, query.EndDateTime, query.Categories);
    }
}