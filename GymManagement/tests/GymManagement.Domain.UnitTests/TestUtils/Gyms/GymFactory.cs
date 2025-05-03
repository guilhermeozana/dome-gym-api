using GymManagement.Domain.GymAggregate;
using GymManagement.Domain.UnitTests.TestConstants;

namespace GymManagement.Domain.UnitTests.TestUtils.Gyms;

public static class GymFactory
{
    public static GymManagement CreateGym(
        string name = Constants.GymManagement.Name,
        int maxRooms = Constants.Subscriptions.MaxRoomsFreeTier,
        Guid? id = null)
    {
        return new GymManagement(
            name,
            maxRooms,
            subscriptionId: Constants.Subscriptions.Id,
            id: id ?? Constants.GymManagement.Id);
    }
}