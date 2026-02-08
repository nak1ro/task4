using System.Diagnostics;

namespace Backend.Api.Middleware;

public class ResendLoggingHandler : DelegatingHandler
{
    private readonly ILogger<ResendLoggingHandler> _logger;

    public ResendLoggingHandler(ILogger<ResendLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        _logger.LogInformation($"[Resend-Req-{id}] Starting request to {request.Method} {request.RequestUri}");
        
        // Log Headers (be careful with Auth)
        foreach (var header in request.Headers)
        {
            if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation($"[Resend-Req-{id}] Header {header.Key}: ***MASKED*** (Length: {header.Value.FirstOrDefault()?.Length ?? 0})");
            }
            else
            {
                _logger.LogInformation($"[Resend-Req-{id}] Header {header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        if (request.Content != null)
        {
             _logger.LogInformation($"[Resend-Req-{id}] Content-Type: {request.Content.Headers.ContentType}");
             var content = await request.Content.ReadAsStringAsync(cancellationToken);
             _logger.LogInformation($"[Resend-Req-{id}] Content: {content}");
        }

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            sw.Stop();

            _logger.LogInformation($"[Resend-Res-{id}] Completed in {sw.ElapsedMilliseconds}ms with status {response.StatusCode}");
            
            if (response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation($"[Resend-Res-{id}] Response Body: {content}");
            }

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, $"[Resend-Err-{id}] Request failed after {sw.ElapsedMilliseconds}ms");
            throw;
        }
    }
}
