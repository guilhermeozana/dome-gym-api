using DomeGym.Domain.RoomAggregate;

using ErrorOr;

using MediatR;

namespace DomeGym.Application.Rooms.Queries.ListRooms;

public record ListRoomsQuery(Guid GymId) : IRequest<ErrorOr<List<Room>>>;