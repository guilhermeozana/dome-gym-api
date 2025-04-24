namespace DomeGym.Domain;

public class Session
{
    private readonly Guid _id;
    private readonly Guid _trainerId;
    private readonly List<Guid> _participantIds = new();
    private readonly DateOnly _date;
    private readonly TimeOnly _startTime;
    private readonly TimeOnly _endTime;
    private readonly int _maxParticipants;

    public Session(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        int maxParticipants,
        Guid trainerId,
        Guid? id = null)
    {
        _date = date;
        _startTime = startTime;
        _endTime = endTime;
        _maxParticipants = maxParticipants;
        _trainerId = trainerId;
        _id = id ?? Guid.NewGuid();
    }

    public void ReserveSpot(Participant participant)
    {
        if (_participantIds.Count >= _maxParticipants)
        {
            throw new Exception("Cannot have more reservations than max participants");
        }
        _participantIds.Add(participant.Id);
    }

    public void CancelReservation(Participant participant, IDateTimeProvider dateTimeProvider)
    {
        if (IsTooCloseToSession(dateTimeProvider.UtcNow))
        {
            throw new Exception("Cannot cancel reservation because time is too close to a session");
        }

        if (!_participantIds.Remove(participant.Id))
        {
            throw new Exception("Reservation not found");
        }
    }

    private bool IsTooCloseToSession(DateTime utcNow)
    {
        const int MinHours = 24;

        return (_date.ToDateTime(_startTime) - utcNow).TotalHours < MinHours;
    }
}