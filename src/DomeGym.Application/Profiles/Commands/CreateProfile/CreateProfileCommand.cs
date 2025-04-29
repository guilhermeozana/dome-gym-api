using DomeGym.Domain.Profiles;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(ProfileType ProfileType, Guid UserId)
    : IRequest<ErrorOr<Guid>>;
