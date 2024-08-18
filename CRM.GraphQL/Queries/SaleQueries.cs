using CRM.Common.Extensions;
using CRM.DataAccess.Context;
using CRM.Models.Models;
using CRM.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace CRM.GraphQL.Queries;

public class SaleQueries
{
    //private static readonly List<int> allowedPageSizes = new List<int>() { 5, 10, 25, 100 };

    [Authorize]
    [UseDbContext(typeof(CRMContext))]
    [PagingationValidation]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Sale> GetSales
        ([Service(ServiceKind.Default)] CRMContext context,
        [Service(ServiceKind.Default)] IHttpContextAccessor httpContextAccessor,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 2)
    {
        //if (page < 1)
        //    throw new BadHttpRequestException("Page number must be greater than zero.");

        //if (!allowedPageSizes.Contains(pageSize))
        //    throw new BadHttpRequestException($"Page size should be one of these values : {string.Join(",", allowedPageSizes)}");

        var userId = Convert.ToInt32(httpContextAccessor.HttpContext!.GetUserId());

        return context.Sales!.Where(x => x.UserId == userId)!.Skip((page - 1) * pageSize).Take(pageSize);
    }

}
