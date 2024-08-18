using CRM.Common.Extensions;
using CRM.DataAccess.Context;
using CRM.Models.Models;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CRM.GraphQL.Queries;

public class UserQueries
{
    private static readonly List<int> allowedPageSizes = new List<int>() { 5, 10, 25, 100 };

    [HotChocolate.Authorization.Authorize]
    [UseDbContext(typeof(CRMContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers
        ([Service(ServiceKind.Default)] CRMContext context,
        [Service(ServiceKind.Default)] IHttpContextAccessor httpContextAccessor,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        if (page < 1)
            throw new BadHttpRequestException("Page number must be greater than zero.");

        if (!allowedPageSizes.Contains(pageSize))
            throw new BadHttpRequestException($"Page size should be one of these values : {string.Join(",", allowedPageSizes)}");

        var userId = Convert.ToInt32(httpContextAccessor.HttpContext!.GetUserId());

        return context.Users!.Skip((page - 1) * pageSize).Take(pageSize);
    }

}
