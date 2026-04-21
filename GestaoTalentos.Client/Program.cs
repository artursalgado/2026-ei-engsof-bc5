using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestaoTalentos.Client;
using GestaoTalentos.Client.Services;

using Microsoft.AspNetCore.Components.Authorization;
using GestaoTalentos.Client.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "UserManager", "Admin"));
    options.AddPolicy("UserManagerPolicy", policy => policy.RequireRole("UserManager", "Admin"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<CustomHttpHandler>();

builder.Services.AddHttpClient("Api", client => 
    client.BaseAddress = new Uri("http://localhost:5193"))
    .AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddScoped<ISkillService, SkillService>();

await builder.Build().RunAsync();