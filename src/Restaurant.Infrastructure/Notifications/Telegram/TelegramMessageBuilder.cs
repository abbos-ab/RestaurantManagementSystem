using Microsoft.AspNetCore.Http;
using System.Text;

namespace Restaurant.Infrastructure.Notifications.Telegram;

internal static class TelegramMessageBuilder
{
    public static string Build(Exception exception, HttpContext context)
    {
        var builder = new StringBuilder();

        builder.AppendLine("🚨 Restaurant API Exception");
        builder.AppendLine();

        builder.AppendLine($"🕒 Time:");
        builder.AppendLine(DateTime.UtcNow.ToString());
        builder.AppendLine();

        builder.AppendLine($"🌐 Method:");
        builder.AppendLine(context.Request.Method);
        builder.AppendLine();

        builder.AppendLine($"📍 Endpoint:");
        builder.AppendLine($"{context.Request.Path}{context.Request.QueryString}");
        builder.AppendLine();

        builder.AppendLine($"🌍 IP:");
        builder.AppendLine(context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

        builder.AppendLine();

        builder.AppendLine("❌ Exception:");
        builder.AppendLine(exception.GetType().Name);

        builder.AppendLine();

        builder.AppendLine("💬 Message:");
        builder.AppendLine(exception.Message);

        if (exception.InnerException != null)
        {
            builder.AppendLine();

            builder.AppendLine("🔥 Inner Exception:");
            builder.AppendLine(exception.InnerException.Message);
        }

        builder.AppendLine();

        builder.AppendLine("🔗 TraceId:");

        builder.AppendLine(context.TraceIdentifier);


        if (!string.IsNullOrWhiteSpace(exception.StackTrace))
        {
            builder.AppendLine();

            builder.AppendLine("📄 StackTrace:");

            var stack = exception.StackTrace.Length > 1500 ? exception.StackTrace[..1500] : exception.StackTrace;

            builder.AppendLine(stack);
        }

        return builder.ToString();
    }
}