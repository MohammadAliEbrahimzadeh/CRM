using CRM.DataAccess.Context;

namespace CRM.DataAccess.UnitOfWork;

public class UnitOfWork : BaseUnitOfWork
{
    private readonly CRMContext _context;
    private UserRepository? _userRepository;

    public UnitOfWork(CRMContext context) : base(context)
    {
        _context = context;
    }

    public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);
}
