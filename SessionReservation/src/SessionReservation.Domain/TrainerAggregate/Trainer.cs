using ErrorOr;

using SessionReservation.Core.Common;
using SessionReservation.Domain.Common.Entities;
using SessionReservation.Domain.Common.ValueObjects;
using SessionReservation.Domain.SessionAggregate;

using Throw;

namespace SessionReservation.Domain.TrainerAggregate;

public class Trainer : AggregateRoot
{
    private readonly List<Guid> _sessionIds = new();
    private readonly Schedule _schedule = Schedule.Empty();

    public Guid UserId { get; }

    public Trainer(
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

    public ErrorOr<Success> AddSessionToSchedule(Session session)
    {
        if (_sessionIds.Contains(session.Id))
        {
            return Error.Conflict(description: "Session already exists in trainer's schedule");
        }

        var bookTimeSlotResult = _schedule.BookTimeSlot(session.Date, session.Time);

        if (bookTimeSlotResult.IsError && bookTimeSlotResult.FirstError.Type == ErrorType.Conflict)
        {
            return TrainerErrors.CannotHaveTwoOrMoreOverlappingSessions;
        }

        _sessionIds.Add(session.Id);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveFromSchedule(Session session)
    {
        if (!_sessionIds.Contains(session.Id))
        {
            return Error.NotFound("Session not found in trainer's schedule");
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

    private Trainer() { }
}