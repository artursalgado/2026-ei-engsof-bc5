using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace GestaoTalentos.Client.Auth;

public class CustomHttpHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public CustomHttpHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Obtain token from localStorage using IJSRuntime
        var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
