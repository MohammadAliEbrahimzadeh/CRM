using CRM.DataAccess.Context;
using CRM.Models.Models;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.GraphQL.Queries;

public class UserQueries
{
    //[HotChocolate.Authorization.Authorize]
    [UseDbContext(typeof(CRMContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers
        ([Service(ServiceKind.Default)] CRMContext context, [FromQuery] int page = 1, [FromQuery] int pageSize = 2) =>
         context.Users!.Skip((page - 1) * pageSize).Take(pageSize);
}
