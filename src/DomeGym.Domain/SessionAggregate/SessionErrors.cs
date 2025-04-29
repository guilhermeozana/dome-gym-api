using ErrorOr;

namespace DomeGym.Domain.SessionAggregate;

public static class SessionErrors
{
    public static readonly Error CannotCancelPastSession = Error.Validation(
      "Session.CannotCancelPastSession",
      "A participant cannot cancel a reservation for a session that has completed");

    public readonly static Error CannotHaveMoreReservationsThanParticipants = Error.Validation(
        code: "Session.CannotHaveMoreReservationsThanParticipants",
        description: "Cannot have more reservations than participants");

    public readonly static Error CannotCancelReservationTooCloseToSession = Error.Validation(
        code: "Session.CannotCancelReservationTooCloseToSession",
        description: "Cannot cancel reservation too close to session start time");
}