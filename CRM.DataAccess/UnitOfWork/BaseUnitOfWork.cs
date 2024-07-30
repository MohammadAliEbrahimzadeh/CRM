using CRM.DataAccess.Context;

namespace CRM.DataAccess.UnitOfWork;

public class BaseUnitOfWork : IUnitOfWork
{
    private readonly CRMContext _context;

    public BaseUnitOfWork(CRMContext context)
    {
        _context = context;
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public int Commit()
    {
        throw new NotImplementedException();
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
