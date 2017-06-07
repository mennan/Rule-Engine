using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RuleEngine.Web.Base;

namespace RuleEngine.Web.Helper
{
    public class ProxyMiddleware
    {
        private const string GetMethodKey = "GET";
        private const string ContentTypeValue = "application/json";
        private const string AcceptValue = "application/json";
        private const string ProxyUrlRegex = @"\/[Pp]roxy\/";
        private const string ProxyUrl = "proxy";
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public ProxyMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestPath = context.Request.Path;
            var requestMessage = new HttpRequestMessage();

            if (requestPath.HasValue)
            {
                var path = requestPath.Value.ToLower();

                if (!path.Contains(ProxyUrl))
                {
                    await _next(context);
                }
                else
                {
                    var url = Regex.Replace(path, ProxyUrlRegex, "");

                    try
                    {
                        using (var http = new HttpClient())
                        {
                            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptValue));

                            requestMessage.RequestUri = new Uri(string.Format("{0}{1}{2}", _appSettings.ApiUrl, url, context.Request.QueryString));
                            requestMessage.Method = new HttpMethod(context.Request.Method);

                            string content;
                            using (var sr = new StreamReader(context.Request.Body))
                                content = sr.ReadToEnd();

                            requestMessage.Content = context.Request.Method == GetMethodKey
                                ? null
                                : new StringContent(content, Encoding.UTF8, ContentTypeValue);

                            var response = await http.SendAsync(requestMessage);

                            context.Response.StatusCode = (int)response.StatusCode;
                            await response.Content.CopyToAsync(context.Response.Body);
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var innerException = ex.InnerException != null
                            ? $"{ex.InnerException.Message}\n{ex.InnerException.StackTrace}"
                            : "";

                        if (ex.InnerException?.InnerException != null)
                            innerException += $"\n{ex.InnerException.InnerException.Message}\n{ex.InnerException.InnerException.StackTrace}";

                        await context.Response.WriteAsync($"{ex.Message}\n{ex.StackTrace}\n{innerException}");
                    }
                }
            }
        }
    }

    public static class ProxyHandlerExtensions
    {
        public static IApplicationBuilder UseProxyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProxyMiddleware>();
        }
    }
}
