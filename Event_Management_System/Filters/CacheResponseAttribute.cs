using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace EventManagementSystem.API.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheResponseAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSeconds;

        public CacheResponseAttribute(int durationInSeconds = 300) // Default to 5 minutes
        {
            _durationInSeconds = durationInSeconds;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get services
            var cacheService = context.HttpContext.RequestServices.GetService<IMemoryCache>();
            var logger = context.HttpContext.RequestServices.GetService<ILogger<CacheResponseAttribute>>();

            if (cacheService == null)
            {
                logger?.LogWarning("IMemoryCache service not found. Caching skipped.");
                await next();
                return;
            }

            // Generate cache key
            var cacheKey = GenerateCacheKey(context.HttpContext);

            // Try to get cached response
            if (cacheService.TryGetValue(cacheKey, out string cachedResponse))
            {
                logger?.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            logger?.LogInformation("Cache miss for key: {CacheKey}. Executing action and caching result.", cacheKey);

            // Proceed with action execution
            var executedContext = await next();

            // Cache the response if it's successful
            if (executedContext.Result is OkObjectResult okResult)
            {
                var response = JsonSerializer.Serialize(okResult.Value);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_durationInSeconds)
                };
                cacheService.Set(cacheKey, response, cacheEntryOptions);
                logger?.LogInformation("Response cached for key: {CacheKey} with duration: {Duration} seconds", cacheKey, _durationInSeconds);
            }
            else
            {
                logger?.LogWarning("Response not cached for key: {CacheKey}. Action did not return OkObjectResult.", cacheKey);
            }
        }

        private string GenerateCacheKey(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var path = context.Request.Path.ToString();
            var queryString = context.Request.QueryString.ToString();

            return $"cache_{userId}_{path}_{queryString}";
        }
    }
}