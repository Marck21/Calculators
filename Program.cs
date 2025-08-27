using Blazored.LocalStorage;
using Calculators;
using Calculators.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ICompoundInterestService, CompoundInterestService>();

builder.Services.AddScoped( sp =>
    new HttpClient { BaseAddress = new Uri( "https://marck21.github.io/Calculators/" ) }
);

await builder.Build().RunAsync();
