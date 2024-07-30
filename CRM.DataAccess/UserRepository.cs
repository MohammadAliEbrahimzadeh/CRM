﻿using CRM.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace CRM.DataAccess;

public class UserRepository
{
    private readonly CRMContext _context;

    public UserRepository(CRMContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsByUsernameAsync(string username, CancellationToken cancellationToken) =>
        await _context.Users!.AnyAsync(x => x.Username == username, cancellationToken);

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        await _context.Users!.AnyAsync(x => x.Email == email, cancellationToken);
}
