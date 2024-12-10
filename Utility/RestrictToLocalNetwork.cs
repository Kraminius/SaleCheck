using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RestrictToLocalNetworkAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var allowedIps = new List<string>
        {
            "127.0.0.1",
            "192.168.1.200"
        };

        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

        if (remoteIp == null || !allowedIps.Contains(remoteIp.ToString()))
        {
            context.Result = new ForbidResult();
        }

        base.OnActionExecuting(context);
    }
}