using DomeGym.Domain.Common.ValueObjects;
using DomeGym.Domain.SessionAggregate;

namespace DomeGym.Domain.UnitTests.TestUtils.Sessions;

public static class SessionFactory
{
    public static Session CreateSession(
        DateOnly? date = null,
        TimeRange? time = null,
        int maxParticipants = Constants.Session.MaxParticipants,
        Guid? id = null)
    {
        return new Session(
            date ?? Constants.Session.Date,
            time ?? Constants.Session.Time,
            maxParticipants,
            trainerId: Constants.Trainer.Id,
            id: id ?? Constants.Session.Id);
    }
}