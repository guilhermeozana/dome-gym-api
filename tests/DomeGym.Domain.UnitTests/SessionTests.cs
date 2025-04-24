using DomeGym.Domain.UnitTests.TestConstants;
using DomeGym.Domain.UnitTests.TestUtils.Participants;
using DomeGym.Domain.UnitTests.TestUtils.Services;
using DomeGym.Domain.UnitTests.TestUtils.Sessions;
using FluentAssertions;

namespace DomeGym.Domain.UnitTests;

public class SessionTests
{
    [Fact]
    public void ReserveSpot_WhenNoMoreRoom_ShouldFailReservation()
    {
        // Arrange
        var session = SessionFactory.CreateSession(maxParticipants: 1);
        var participant1 = ParticipantFactory.CreateParticipant(id: Guid.NewGuid(), userId: Guid.NewGuid());
        var participant2 = ParticipantFactory.CreateParticipant(id: Guid.NewGuid());

        // Act
        session.ReserveSpot(participant1);
        var action = () => session.ReserveSpot(participant2);

        // Assert
        action.Should().ThrowExactly<Exception>();
        
    }

    [Fact]
    public void CancelReservation_WhenCancelationIsTooCloseToSession_ShouldFailCancelation()
    {
        // Arrange
        var session = SessionFactory.CreateSession(
            date: Constants.Session.Date,
            startTime: Constants.Session.StartTime,
            endTime: Constants.Session.EndTime);
        
        var participant = ParticipantFactory.CreateParticipant();
        
        session.ReserveSpot(participant);

        var cancellationDateTime = Constants.Session.Date.ToDateTime(TimeOnly.MinValue);
        // Act
        var action = () => session.CancelReservation(
            participant, 
            new TestDateTimeProvider(fixedDateTime: cancellationDateTime));

        // Assert
        action.Should().ThrowExactly<Exception>();
    }
}