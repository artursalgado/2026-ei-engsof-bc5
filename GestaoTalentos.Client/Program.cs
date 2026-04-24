using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestaoTalentos.Client;
using GestaoTalentos.Client.Services;

using Microsoft.AspNetCore.Components.Authorization;
using GestaoTalentos.Client.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<CustomHttpHandler>();

builder.Services.AddHttpClient("Api", client => 
    client.BaseAddress = new Uri("http://localhost:5193"))
    .AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IPropostaService, PropostaService>();
await builder.Build().RunAsync();