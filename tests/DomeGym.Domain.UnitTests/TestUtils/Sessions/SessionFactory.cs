using DomeGym.Domain.UnitTests.TestConstants;

namespace DomeGym.Domain.UnitTests.TestUtils.Sessions;

public static class SessionFactory
{
    public static Session CreateSession(
        DateOnly? date = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        int maxParticipants = Constants.Session.MaxParticipants, 
        Guid? id = null)
    {
        return new Session(
            date ?? Constants.Session.Date,
            startTime ?? Constants.Session.StartTime,
            endTime ?? Constants.Session.EndTime,
            maxParticipants, 
            trainerId: Constants.Trainer.Id,
            id: id ?? Constants.Session.Id);
    }
}