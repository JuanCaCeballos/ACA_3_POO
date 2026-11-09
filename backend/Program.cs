using EntradasEspectaculo.Endpoints;
using EntradasEspectaculo.Servicios;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "../frontend"
});

builder.Services.AddSingleton<TaquillaServicio>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapTaquillaEndpoints();

app.Run();
