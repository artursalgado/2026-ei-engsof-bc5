using System;
using Microsoft.JSInterop;

namespace GestaoTalentos.Client.Services;

public class LocalStorageService(IJSRuntime js)
{
    public ValueTask SetItemAsync(string key, string value) =>
        js.InvokeVoidAsync("localStorage.setItem", key, value);

    public ValueTask<string?> GetItemAsync(string key) =>
        js.InvokeAsync<string?>("localStorage.getItem", key);

    public ValueTask RemoveItemAsync(string key) =>
        js.InvokeVoidAsync("localStorage.removeItem", key);
}