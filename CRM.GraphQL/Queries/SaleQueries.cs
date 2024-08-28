using CRM.Common.Extensions;
using CRM.DataAccess.Context;
using CRM.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace CRM.GraphQL.Queries;

public class SaleQueries
{

    [Authorize]
    [UseDbContext(typeof(CRMContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Sale> GetSales
        ([Service(ServiceKind.Default)] CRMContext context,
        [Service(ServiceKind.Default)] IHttpContextAccessor httpContextAccessor,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 2)
    {
        if (page < 1)
            throw new BadHttpRequestException("Page number must be greater than zero.");

        if (!PaginationConstants.AllowedPageSizes.Contains(pageSize))
            throw new BadHttpRequestException($"Page size should be one of these values : " +
                $"{string.Join(",", PaginationConstants.AllowedPageSizes)}");

        var userId = Convert.ToInt32(httpContextAccessor.HttpContext!.GetUserId());

        return context.Sales!.Where(x => x.UserId == userId)!.Skip((page - 1) * pageSize).Take(pageSize);
    }

}
