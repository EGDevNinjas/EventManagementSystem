using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuditAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
        var logger = loggerFactory?.CreateLogger("Audit");

        var actionName = context.ActionDescriptor.DisplayName;
        var user = context.HttpContext.User.Identity?.Name ?? "Anonymous";
        var timeBefore = DateTime.UtcNow;

        logger?.LogInformation("🔍 [Audit] BEFORE executing {Action} by {User} at {Time}", actionName, user, timeBefore);

        var resultContext = await next(); // تنفيذ الأكشن

        var timeAfter = DateTime.UtcNow;
        logger?.LogInformation("✅ [Audit] AFTER executing {Action} by {User} at {Time}", actionName, user, timeAfter);
    }
}
