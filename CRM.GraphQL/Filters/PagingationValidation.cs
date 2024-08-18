using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.Web.Filters;

public class PagingationValidation : ActionFilterAttribute
{
    private static readonly List<int> allowedPageSizes = new List<int>() { 5, 10, 25, 100 };

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var pageSizeVal = context.HttpContext.Request.Query.Keys.FirstOrDefault(x => x.Equals("PageSize"));

        if (pageSizeVal is null)
        {
            throw new BadHttpRequestException($"Page size should be one of these values : {string.Join(",", allowedPageSizes)}");
        }

        if (!allowedPageSizes.Contains(Convert.ToInt32(pageSizeVal)))
        {
            throw new BadHttpRequestException($"Page size should be one of these values : {string.Join(",", allowedPageSizes)}");
        }
    }
}
