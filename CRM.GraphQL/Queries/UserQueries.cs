using CRM.DataAccess.Context;
using CRM.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRM.GraphQL.Queries;

public class UserQueries
{
    [UseDbContext(typeof(CRMContext))]
    [UseProjection]
    [UseFiltering]
    public IQueryable<User> GetUsers
        ([Service(ServiceKind.Default)] CRMContext context, [FromQuery] int page = 1, [FromQuery] int pageSize = 2) =>
         context.Users!.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
}
