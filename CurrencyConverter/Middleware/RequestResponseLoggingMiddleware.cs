using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Middleware
{
    public class RequestResponseLoggingMiddleware
    {//
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.ForContext<RequestResponseLoggingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();


            // Enable buffering to read the request body multiple times

            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

            context.Request.Body.Seek(0, SeekOrigin.Begin); // Rewind the body stream
            var userId = context.User?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            // Capture the response body
            var originalResponseBody = context.Response.Body;
            var queryString = context.Request.QueryString.ToString();
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    context.Response.Body = memoryStream;

                    // Call the next middleware in the pipeline
                    await _next(context);

                    // Read the response body
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();


                    // Log request and response bodies to Serilog
                    _logger.Information("Client Id: {UserId}, Method :{HttpMethod}, Endpoint : {Endpoint}, " +
                        "Response Body: {ResponseBody}", userId,  context.Request.Method,
                        context.Request.Path,responseBody);

                    // Copy the response back to the original response stream

                }

                catch (Exception ex)
                {
                    _logger.Information("Client Id: {UserId}, Method :{HttpMethod}, Endpoint : {Endpoint}, " +
                        "Response Body: {ResponseBody}", userId, context.Request.Method,
                            context.Request.Path, ex.Message);
                    // return;

                }
                finally
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalResponseBody);
                }
                // Continue the request processing (don't block if logging fails)
            }
        }




        private string FormatHeaders(IHeaderDictionary headers)
        {
            StringBuilder headerString = new StringBuilder();
            foreach (var header in headers)
            {
                headerString.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            return headerString.ToString();
        }

    }
}