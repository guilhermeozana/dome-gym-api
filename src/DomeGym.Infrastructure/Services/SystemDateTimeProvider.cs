using DomeGym.Domain.Common.Interfaces;

namespace DomeGym.Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
