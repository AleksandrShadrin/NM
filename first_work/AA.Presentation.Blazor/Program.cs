using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AA.Presentation.Blazor;
using AA.Methods;
using AA.Methods.ValueObjects;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddTransient<IDUSolver, DUSolver>(p =>
{
    var duOptions = new DUSolverOptions
    {
        Eps = 10e-3,
        N = 10,
        P = 1,
        T = 1.0,
        T0 = 0.0,
        U0 = 3.0
    };
    var duSolver = new DUSolver(duOptions);

    var equation =
                (double t, double u) => u * Math.Cos(t) - Math.Sin(2 * t);

    duSolver.SetEquation(equation);
    return duSolver;
});

await builder.Build().RunAsync();
