using ErrorOr;

namespace DomeGym.Domain.GymAggregate;

public static class GymErrors
{
    public static readonly Error CannotHaveMoreRoomsThanSubscriptionAllows = Error.Validation(
        "Room.CannotHaveMoreRoomsThanSubscriptionAllows",
        "A gym cannot have more rooms than the subscription allows");
}