global using Blazored.LocalStorage;
global using Calculators;
global using Calculators.Services;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
global using MudBlazor.Services;
global using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ICompoundInterestService, CompoundInterestService>();
builder.Services.AddScoped<IMortgageService, MortgageService>();

builder.Services.AddScoped( sp =>
    new HttpClient { BaseAddress = new Uri( "https://marck21.github.io/Calculators/" ) }
);

await builder.Build().RunAsync();
