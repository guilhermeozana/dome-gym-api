using ErrorOr;

using SessionReservation.Core.Common;
using SessionReservation.Domain.Common.Entities;
using SessionReservation.Domain.Common.ValueObjects;
using SessionReservation.Domain.SessionAggregate;

using Throw;

namespace SessionReservation.Domain.ParticipantAggregate;

public class Participant : AggregateRoot
{
    private readonly Schedule _schedule = Schedule.Empty();
    private readonly List<Guid> _sessionIds = new();
    public Guid UserId { get; }

    public IReadOnlyList<Guid> SessionIds => _sessionIds;

    public Participant(
        Guid userId,
        Schedule? schedule = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        _schedule = schedule ?? Schedule.Empty();
    }

    public bool IsTimeSlotFree(DateOnly date, TimeRange time)
    {
        return _schedule.CanBookTimeSlot(date, time);
    }

    public ErrorOr<Success> AddToSchedule(Session session)
    {
        if (_sessionIds.Contains(session.Id))
        {
            return Error.Conflict(description: "Session already exists in participant's schedule");
        }

        var bookTimeSlotResult = _schedule.BookTimeSlot(
            session.Date,
            session.Time);

        if (bookTimeSlotResult.IsError)
        {
            return bookTimeSlotResult.FirstError.Type == ErrorType.Conflict
                ? ParticipantErrors.CannotHaveTwoOrMoreOverlappingSessions
                : bookTimeSlotResult.Errors;
        }

        _sessionIds.Add(session.Id);
        return Result.Success;
    }

    public bool HasReservationForSession(Guid sessionId)
    {
        return _sessionIds.Contains(sessionId);
    }

    public ErrorOr<Success> RemoveFromSchedule(Session session)
    {
        if (!_sessionIds.Contains(session.Id))
        {
            return Error.NotFound(description: "Session not found in participant's schedule");
        }

        var removeBookingResult = _schedule.RemoveBooking(
            session.Date,
            session.Time);

        if (removeBookingResult.IsError)
        {
            return removeBookingResult.Errors;
        }

        _sessionIds.Remove(session.Id);
        return Result.Success;
    }

    private Participant() { }
}