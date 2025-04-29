using DomeGym.Domain.SessionAggregate;

namespace DomeGym.Application.Common.Interfaces;

public interface ISessionsRepository
{
    Task AddSessionAsync(Session session);
    
    Task UpdateSessionAsync(Session session);
}