using DomeGym.Domain.Common;
using DomeGym.Domain.Common.Entities;
using DomeGym.Domain.SessionAggregate;

using ErrorOr;

namespace DomeGym.Domain.RoomAggregate;

public class Room : AggregateRoot
{
    private readonly Dictionary<DateOnly, List<Guid>> _sessionIdsByDate = new();
    private readonly int _maxDailySessions;
    private readonly Schedule _schedule = Schedule.Empty();

    public string Name { get; } = null!;

    public Guid GymId { get; }

    public IReadOnlyList<Guid> SessionIds => _sessionIdsByDate.Values
        .SelectMany(sessionIds => sessionIds)
        .ToList()
        .AsReadOnly();

    public Room(
        string name,
        int maxDailySessions,
        Guid gymId,
        Schedule? schedule = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        _maxDailySessions = maxDailySessions;
        GymId = gymId;
        _schedule = schedule ?? Schedule.Empty();
    }

    public ErrorOr<Success> ScheduleSession(Session session)
    {
        if (SessionIds.Any(id => id == session.Id))
        {
            return Error.Conflict(description: "Session already exists in room");
        }

        if (!_sessionIdsByDate.ContainsKey(session.Date))
        {
            _sessionIdsByDate[session.Date] = new();
        }

        var dailySessions = _sessionIdsByDate[session.Date];

        if (dailySessions.Count >= _maxDailySessions)
        {
            return RoomErrors.CannotHaveMoreSessionThanSubscriptionAllows;
        }

        var addEventResult = _schedule.BookTimeSlot(session.Date, session.Time);

        if (addEventResult.IsError && addEventResult.FirstError.Type == ErrorType.Conflict)
        {
            return RoomErrors.CannotHaveTwoOrMoreOverlappingSessions;
        }

        dailySessions.Add(session.Id);

        return Result.Success;
    }

    public bool HasSession(Guid sessionId)
    {
        return SessionIds.Contains(sessionId);
    }

    private Room() { }
}
