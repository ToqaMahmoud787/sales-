using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using AxiomSysPortal.Models;
using Microsoft.AspNetCore.Authentication;

namespace AxiomSysPortal.Services;

public sealed class ApiSessionExpiredException : Exception { }

public sealed class DeliveryTermsApiClient(HttpClient client, IHttpContextAccessor accessor)
{
    private const string Prefix = "api/Sales/SaDeliveryTerm/";
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        using var response = await client.PostAsJsonAsync("api/auth/login", new { username, password });
        if (response.StatusCode == HttpStatusCode.Unauthorized) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public Task<ApiResponse> ListAsync() => SendAsync(HttpMethod.Get, "GetSaDeliveryTerms");
    public Task<ApiResponse> GetAsync(int code) => SendAsync(HttpMethod.Get,
        "GetSaDeliveryTerm?code=" + code.ToString(System.Globalization.CultureInfo.InvariantCulture));
    public Task<ApiResponse> AddAsync(DeliveryTerm model) => SendAsync(HttpMethod.Post, "AddSaDeliveryTerm", model);
    public Task<ApiResponse> EditAsync(DeliveryTerm model) => SendAsync(HttpMethod.Post, "EditSaDeliveryTerm", model);
    public Task<ApiResponse> DeleteAsync(int code) => SendAsync(HttpMethod.Post, "DeleteSaDeliveryTerm", new { code });

    private async Task<ApiResponse> SendAsync(HttpMethod method, string endpoint, object? body = null)
    {
        var context = accessor.HttpContext ?? throw new InvalidOperationException();
        var token = await context.GetTokenAsync("access_token");
        if (string.IsNullOrWhiteSpace(token)) throw new ApiSessionExpiredException();
        using var request = new HttpRequestMessage(method, Prefix + endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body != null) request.Content = JsonContent.Create(body);
        using var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new ApiSessionExpiredException();
        if (response.StatusCode == HttpStatusCode.Forbidden) throw new HttpRequestException("Company access denied.");
        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound
            && response.StatusCode != HttpStatusCode.InternalServerError)
            response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse>()
            ?? throw new JsonException("Empty API response.");
    }
}
