using DomeGym.Domain.Common;
using DomeGym.Domain.Common.Interfaces;
using DomeGym.Domain.Common.ValueObjects;
using DomeGym.Domain.ParticipantAggregate;

using ErrorOr;

namespace DomeGym.Domain.SessionAggregate;

public class Session : AggregateRoot
{
    private readonly List<Reservation> _reservations = new();
    private readonly List<SessionCategory> _categories = new();

    public int NumParticipants => _reservations.Count;

    public DateOnly Date { get; }

    public TimeRange Time { get; } = null!;

    public string Name { get; } = null!;

    public string Description { get; } = null!;

    public int MaxParticipants { get; }

    public Guid RoomId { get; }

    public IReadOnlyList<SessionCategory> Categories => _categories;

    public Guid TrainerId { get; }

    public Session(
        string name,
        string description,
        int maxParticipants,
        Guid roomId,
        Guid trainerId,
        DateOnly date,
        TimeRange time,
        List<SessionCategory> categories,
        Guid? id = null)
            : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Description = description;
        MaxParticipants = maxParticipants;
        RoomId = roomId;
        TrainerId = trainerId;
        Date = date;
        Time = time;
        _categories = categories;
    }

    public ErrorOr<Success> CancelReservation(Participant participant, IDateTimeProvider dateTimeProvider)
    {
        var reservation = _reservations.Find(reservation => reservation.ParticipantId == participant.Id);
        if (reservation is null)
        {
            return Error.NotFound(description: "Reservation not found");
        }

        if (IsPastSession(dateTimeProvider.UtcNow))
        {
            return SessionErrors.CannotCancelPastSession;
        }

        if (IsTooCloseToSession(dateTimeProvider.UtcNow))
        {
            return SessionErrors.CannotCancelReservationTooCloseToSession;
        }

        _reservations.Remove(reservation);

        return Result.Success;
    }

    public ErrorOr<Success> ReserveSpot(Participant participant)
    {
        if (_reservations.Count >= MaxParticipants)
        {
            return SessionErrors.CannotHaveMoreReservationsThanParticipants;
        }

        if (_reservations.Any(reservation => reservation.ParticipantId == participant.Id))
        {
            return Error.Conflict(description: "Participants cannot reserve twice to the same session");
        }

        var reservation = new Reservation(participant.Id);
        _reservations.Add(reservation);

        return Result.Success;
    }

    public bool HasReservationForParticipant(Guid participantId)
    {
        return _reservations.Any(reservation => reservation.ParticipantId == participantId);
    }

    public bool IsBetweenDates(DateTime startDateTime, DateTime endDateTime)
    {
        var sessionDateTime = Date.ToDateTime(Time.Start);

        return sessionDateTime >= startDateTime && sessionDateTime <= endDateTime;
    }

    public void Cancel()
    {
    }

    public List<Guid> GetParticipantIds()
    {
        return _reservations.ConvertAll(reservation => reservation.ParticipantId);
    }

    private bool IsPastSession(DateTime utcNow)
    {
        return (Date.ToDateTime(Time.End) - utcNow).TotalHours < 0;
    }

    private bool IsTooCloseToSession(DateTime utcNow)
    {
        const int MinHours = 24;

        return (Date.ToDateTime(Time.Start) - utcNow).TotalHours < MinHours;
    }

    private Session() { }
}