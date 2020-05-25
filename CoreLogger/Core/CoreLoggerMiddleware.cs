using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreLogger
{
    public class CoreLoggerMiddleware
    {
        readonly RequestDelegate _next;
        readonly ICoreLogger _logger;

        public CoreLoggerMiddleware(RequestDelegate next, ICoreLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (_logger.Configuration.MinLevel == LogLevel.Trace &&
                    (context.Response.StatusCode != (int)HttpStatusCode.NotFound && context.Response.StatusCode != (int)HttpStatusCode.GatewayTimeout))
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append($"REQUEST:");
                    sb.Append($" TRACEIDENTIFIER:{context.TraceIdentifier}");
                    sb.Append($" - HOST:{context.Request.Host}");
                    sb.Append($" - HTTPS:{context.Request.IsHttps}");
                    sb.Append($" - PATH:{context.Request.Path}");
                    sb.Append($" - QUERYSTRING:{context.Request.QueryString}");
                    sb.Append($" - METHOD:{context.Request.Method}");
                    sb.Append($" - CONTENT-TYPE:{context.Request.ContentType}");
                    sb.Append($" - RESPONSE STATUSCODE:{context.Response.StatusCode}");
                    await _logger.LogTrace(message: sb.ToString());
                }

                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound || context.Response.StatusCode == (int)HttpStatusCode.GatewayTimeout)
                {
                    if (_logger.Configuration.MinLevel == LogLevel.Warning)
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.Append($"REQUEST:");
                        sb.Append($" TRACEIDENTIFIER:{context.TraceIdentifier}");
                        sb.Append($" - HOST:{context.Request.Host}");
                        sb.Append($" - HTTPS:{context.Request.IsHttps}");
                        sb.Append($" - PATH:{context.Request.Path}");
                        sb.Append($" - QUERYSTRING:{context.Request.QueryString}");
                        sb.Append($" - METHOD:{context.Request.Method}");
                        sb.Append($" - CONTENT-TYPE:{context.Request.ContentType}");
                        sb.Append($" - RESPONSE STATUSCODE:{context.Response.StatusCode}");
                        await _logger.LogWarning(message: sb.ToString());
                    }

                    if (_logger.Configuration.ErrorPages?.Count() > 0)
                        if (_logger.Configuration.ErrorPages.Select(t => t.Key).Contains(context.Response.StatusCode))
                            context.Response.Redirect(_logger.Configuration.ErrorPages.FirstOrDefault(t => t.Key == context.Response.StatusCode).Value);
                    if (!string.IsNullOrWhiteSpace(_logger.Configuration.ErrorPage))
                        context.Response.Redirect(_logger.Configuration.ErrorPage);
                }
            }
            catch (Exception e)
            {
                var fulldata = JsonConvert.SerializeObject(e, Formatting.Indented);
                await _logger.LogError(e.Message, fulldata);
                if (_logger.Configuration.ErrorPages?.Count() > 0)
                    if (_logger.Configuration.ErrorPages.Select(t => t.Key).Contains(context.Response.StatusCode))
                        context.Response.Redirect(_logger.Configuration.ErrorPages.FirstOrDefault(t => t.Key == context.Response.StatusCode).Value);
                if (!string.IsNullOrWhiteSpace(_logger.Configuration.ErrorPage))
                    context.Response.Redirect(_logger.Configuration.ErrorPage);
            }
        }
    }
}
