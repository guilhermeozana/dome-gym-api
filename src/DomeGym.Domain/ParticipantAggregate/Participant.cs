using DomeGym.Domain.Common;
using DomeGym.Domain.Common.Entities;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;

namespace DomeGym.Domain.ParticipantAggregate
{
    public class Participant : AggregateRoot
    {
        private readonly Schedule _schedule = Schedule.Empty();

        private readonly Guid _userId;
        private readonly List<Guid> _sessionIds = new(); 

        public Participant(Guid userId, Guid? id = null) : base(id ?? Guid.NewGuid())
        {
            _userId = userId;
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
    }
}