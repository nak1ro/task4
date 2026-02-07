using System.Security.Claims;
using Backend.Api.Data.Repositories;
using Backend.Api.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Backend.Api.Middleware;

public class UserStatusMiddleware
{
    private readonly RequestDelegate _next;

    public UserStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var user = await userRepository.GetByIdAsync(userId);

                // Check if user exists and isn't blocked
                if (user == null || user.Status == UserStatus.Blocked)
                {
                    // Log out the user
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    // Return 401 Unauthorized
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { message = "User is blocked or deleted." });
                    return;
                }
            }
        }

        await _next(context);
    }
}
