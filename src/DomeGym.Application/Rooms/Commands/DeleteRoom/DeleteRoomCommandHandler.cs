using DomeGym.Application.Common.Interfaces;

using ErrorOr;

using MediatR;

namespace DomeGym.Application.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, ErrorOr<Deleted>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly IRoomsRepository _roomsRepository;

    public DeleteRoomCommandHandler(IRoomsRepository roomsRepository, IGymsRepository gymsRepository)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteRoomCommand command, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return Error.NotFound(description: "Gym not found");
        }

        if (!gym.HasRoom(command.RoomId))
        {
            return Error.NotFound(description: "Room not found");
        }

        var room = await _roomsRepository.GetByIdAsync(command.RoomId);

        if (room is null)
        {
            return Error.NotFound(description: "Room not found");
        }

        var removeGymResult = gym.RemoveRoom(room);

        if (removeGymResult.IsError)
        {
            return removeGymResult.Errors;
        }

        await _gymsRepository.UpdateAsync(gym);

        return Result.Deleted;
    }
}
