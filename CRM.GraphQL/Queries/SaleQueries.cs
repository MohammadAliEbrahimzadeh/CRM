using CRM.Common.Extensions;
using CRM.DataAccess.Context;
using CRM.Models.Models;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.GraphQL.Queries;

public class SaleQueries
{
    [HotChocolate.Authorization.Authorize]
    [UseDbContext(typeof(CRMContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Sale> GetSales
        ([Service(ServiceKind.Default)] CRMContext context,
        [Service(ServiceKind.Default)] IHttpContextAccessor httpContextAccessor,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 2)
    {

        var userId = Convert.ToInt32(httpContextAccessor.HttpContext!.GetUserId());

        return context.Sales!.Where(x => x.UserId == userId)!.Skip((page - 1) * pageSize).Take(pageSize);
    }

}
