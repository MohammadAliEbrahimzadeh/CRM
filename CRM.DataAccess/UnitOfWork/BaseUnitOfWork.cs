using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.DataAccess.UnitOfWork;

public class BaseUnitOfWork : IUnitOfWork
{
    public Task AddAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public int Commit()
    {
        throw new NotImplementedException();
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
