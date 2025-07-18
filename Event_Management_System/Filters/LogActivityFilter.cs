using Microsoft.AspNetCore.Mvc.Filters;

namespace EventManagementSystem.API.Filters
{
    public class LogActivityFilter : IAsyncActionFilter, IOrderedFilter
    {
        public int Order => 0; // Set the order of execution for this filter
        private readonly ILogger<LogActivityFilter> _logger;
        public LogActivityFilter(ILogger<LogActivityFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Action executing: {ActionName} at {Time} and the {ControllerName}", 
                context.ActionDescriptor.DisplayName, DateTime.UtcNow, context.Controller.GetType().Name);
        await next();
            _logger.LogInformation("Action executed: {ActionName} at {Time} and the {ControllerName}", 
                context.ActionDescriptor.DisplayName, DateTime.UtcNow, context.Controller.GetType().Name);
        }
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException(nameof(context));
        //    }
        //    // Log the action execution start details
        //    _logger.LogInformation("Action executing: {ActionName} at {Time} and {ControllerName}", context.ActionDescriptor.DisplayName, DateTime.UtcNow, context.Controller.GetType().Name);
        //}
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException(nameof(context));
        //    }
        //    // Log the action execution details and the controller name
        //    _logger.LogInformation(
        //        "Action executed: {ActionName} at {Time} and {ControllerName}",
        //        context.ActionDescriptor.DisplayName,
        //        DateTime.UtcNow,
        //        context.Controller.GetType().Name
        //    );
        //}
    }
}
