using DomeGym.Application.Common.Interfaces;
using DomeGym.Domain.TrainerAggregate;

using ErrorOr;

using MediatR;

namespace DomeGym.Application.Gyms.Commands.AddTrainer;

public class AddTrainerCommandHandler : IRequestHandler<AddTrainerCommand, ErrorOr<Success>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ITrainersRepository _trainersRepository;

    public AddTrainerCommandHandler(ITrainersRepository trainersRepository, ISubscriptionsRepository subscriptionsRepository, IGymsRepository gymsRepository)
    {
        _trainersRepository = trainersRepository;
        _subscriptionsRepository = subscriptionsRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(AddTrainerCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId);

        if (subscription is null)
        {
            return Error.NotFound(description: "Subscription not found");
        }

        if (!subscription.HasGym(command.GymId))
        {
            return Error.NotFound(description: "Gym not found");
        }

        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return Error.NotFound(description: "Gym not found");
        }

        if (gym.HasTrainer(command.TrainerId))
        {
            return Error.Conflict("Trainer already in gym");
        }

        Trainer? trainer = await _trainersRepository.GetByIdAsync(command.TrainerId);

        if (trainer is null)
        {
            return Error.NotFound(description: "Trainer not found");
        }

        gym.AddTrainer(trainer);

        await _gymsRepository.UpdateAsync(gym);

        return Result.Success;
    }
}
