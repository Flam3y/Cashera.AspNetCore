using Cashera.Serialization;
using Cashera.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

namespace Cashera.AspNetCore;

public static class Extensions
{
    private const string ApiKeyHeader = "X-Api-Key";
    private const string SecretHeader = "X-Secret";

    public static IServiceCollection ConfigureCasheraMvc(this IServiceCollection services)
        => services.Configure<MvcOptions>(options =>
        {
            options.InputFormatters.Insert(0, _inputFormatter);
        });

    public static bool IsValidCasheraWebhook(this HttpRequest request, ICasheraClient apiClient)
        => apiClient.ValidateWebhook(request.Headers[ApiKeyHeader], request.Headers[SecretHeader]);

    private static readonly CasheraInputFormatter _inputFormatter = new();

    private sealed class CasheraInputFormatter : TextInputFormatter
    {
        public CasheraInputFormatter()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Add("application/json");
        }

        protected override bool CanReadType(Type type) => type == typeof(Update);

        public sealed override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            object? model = await JsonSerializer.DeserializeAsync(context.HttpContext.Request.Body, context.ModelType, JsonApi.Options, context.HttpContext.RequestAborted);
            return await InputFormatterResult.SuccessAsync(model);
        }
    }
}
