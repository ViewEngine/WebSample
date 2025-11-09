using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

const string API_BASE_URL = "https://www.viewengine.io";

// Proxy endpoint to call ViewEngine API
app.MapPost("/api/retrieve", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var request = JsonSerializer.Deserialize<RetrieveRequest>(body, options);

        if (request == null || string.IsNullOrEmpty(request.ApiKey))
        {
            return Results.BadRequest(new { error = "API key is required" });
        }

        var httpClient = httpClientFactory.CreateClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE_URL}/v1/mcp/retrieve");
        httpRequest.Headers.Add("X-API-Key", request.ApiKey);
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(new { url = request.Url, forceRefresh = request.ForceRefresh, mode = request.Mode }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return Results.Json(JsonSerializer.Deserialize<object>(responseContent), statusCode: (int)response.StatusCode);
        }

        return Results.Content(responseContent, "application/json");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/status/{requestId}", async (Guid requestId, string apiKey, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var httpClient = httpClientFactory.CreateClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{API_BASE_URL}/v1/mcp/retrieve/{requestId}");
        httpRequest.Headers.Add("X-API-Key", apiKey);

        var response = await httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return Results.Json(JsonSerializer.Deserialize<object>(responseContent), statusCode: (int)response.StatusCode);
        }

        return Results.Content(responseContent, "application/json");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/content/{requestId}", async (Guid requestId, string apiKey, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var httpClient = httpClientFactory.CreateClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{API_BASE_URL}/v1/mcp/retrieve/{requestId}/content");
        httpRequest.Headers.Add("X-API-Key", apiKey);

        var response = await httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return Results.Json(JsonSerializer.Deserialize<object>(responseContent), statusCode: (int)response.StatusCode);
        }

        return Results.Content(responseContent, "application/json");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

record RetrieveRequest(string ApiKey, string Url, bool ForceRefresh, string? Mode);
