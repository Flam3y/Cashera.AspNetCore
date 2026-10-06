# Cashera.AspNetCore

ASP.NET Core support for [Cashera](https://cashera.cash) webhooks.

```csharp
builder.Services.AddSingleton<ICasheraClient>(new CasheraClient("pk_...", "sk_..."));
builder.Services.AddControllers();
builder.Services.ConfigureCasheraMvc();

[HttpPost("cashera/webhook")]
public IActionResult Webhook([FromBody] Update update, [FromServices] ICasheraClient client)
{
    if (!Request.IsValidCasheraWebhook(client))
        return Unauthorized();

    if (update.Type == UpdateType.TransactionStatusUpdated && update.Transaction!.Status == TransactionStatus.Paid)
    {
        // Check amount and currency, then fulfill the order (idempotently by uuid + status)
    }

    return Ok();
}
```
