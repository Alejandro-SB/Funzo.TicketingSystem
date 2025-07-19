using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace TicketingSystem.Tests;

internal static class HttpClientExtensions
{
    public static JsonSerializerOptions Options => Program.AddConverters(new JsonSerializerOptions(JsonSerializerDefaults.Web));

    public static Task<T?> FromJson<T>(this HttpContent content)
    {
        return content.ReadFromJsonAsync<T>(options: Options);
    }
}
