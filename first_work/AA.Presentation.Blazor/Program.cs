using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AA.Presentation.Blazor;
using AA.Methods;
using AA.Methods.ValueObjects;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

builder.Services.AddSingleton<DUSolverOptions>(sp =>
{
    return new DUSolverOptions
    {
        Eps = 10e-3,
        N = 10,
        P = 1,
        T = 1.0,
        T0 = 0.0,
        U0 = 3.0
    };
});

builder.Services.AddTransient<IDUSolver, DUSolver>(p =>
{
    var duOptions = p.GetService<DUSolverOptions>();
    var duSolver = new DUSolver(duOptions);

    var equation =
                (double t, double u) => u * Math.Cos(t) - Math.Sin(2 * t);
    var derivative = (double t, double u) => Math.Cos(t);

    duSolver.SetEquation(equation);
    duSolver.SetDerivative(derivative);
    return duSolver;
});

builder.Services.AddSingleton<IInaccuracyFinder, InaccuracyFinder>(sp =>
{
    var duSolver = sp.GetRequiredService<IDUSolver>();
    var duOptions = sp.GetRequiredService<DUSolverOptions>();

    var inaccuracyFinder = new InaccuracyFinder(duSolver, duOptions);

    inaccuracyFinder.SetExactSolution((t) => Math.Exp(Math.Sin(t)) + 2 * Math.Sin(t) + 2);
    return inaccuracyFinder;
});

await builder.Build().RunAsync();
