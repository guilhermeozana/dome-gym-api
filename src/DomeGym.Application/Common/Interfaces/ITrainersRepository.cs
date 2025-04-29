
using DomeGym.Application.Profiles.Common;
using DomeGym.Domain.TrainerAggregate;

namespace DomeGym.Application.Common.Interfaces;

public interface ITrainersRepository
{
    Task AddTrainerAsync(Trainer participant);
    Task<Trainer?> GetByIdAsync(Guid trainerId);
    Task<Profile?> GetProfileByUserIdAsync(Guid userId);
    Task UpdateAsync(Trainer trainer);
}