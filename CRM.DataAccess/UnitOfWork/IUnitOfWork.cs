
namespace CRM.DataAccess.UnitOfWork;

public interface IUnitOfWork
{
    int Commit();

    Task<int> CommitAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task AddAsync<T>(T entity, CancellationToken cancellationTokenn) where T : class;
}
