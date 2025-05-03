using ErrorOr;

using MediatR;

using SessionReservation.Domain.SessionAggregate;

namespace SessionReservation.Application.Participants.Queries.ListParticipantSessions;

public record ListParticipantSessionsQuery(
        Guid ParticipantId,
        DateTime? StartDateTime = null,
        DateTime? EndDateTime = null) : IRequest<ErrorOr<List<Session>>>;