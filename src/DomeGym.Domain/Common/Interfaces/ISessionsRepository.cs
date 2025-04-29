using DomeGym.Domain.SessionAggregate;

namespace DomeGym.Domain.Common.Interfaces;

public interface ISessionsRepository
{
    Task AddSessionAsync(Session session);
    
    Task UpdateSessionAsync(Session session);
} 