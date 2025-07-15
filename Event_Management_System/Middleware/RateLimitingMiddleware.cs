using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class RateLimitingMiddleware
{
    private static int _counter = 0;
    private static DateTime _lastRequestDate = DateTime.Now;
    private readonly RequestDelegate _next;

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

public async Task InvokeAsync(HttpContext context)
{
    if (DateTime.Now.Subtract(_lastRequestDate).TotalSeconds > 10)
    {
        _counter = 1;
        _lastRequestDate = DateTime.Now;
        await _next(context);
    }
    else
    {
        if (_counter >= 5)
        {
            _lastRequestDate = DateTime.Now;
            await context.Response.WriteAsync("Rate limit exceeded.");
        }
        else
        {
            _counter++;
            _lastRequestDate = DateTime.Now;
            await _next(context);
        }
    }
}
}
