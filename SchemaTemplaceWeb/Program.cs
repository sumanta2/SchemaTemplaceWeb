using SchemaTemplateLib;
using SchemaTemplateLib.Implementation;
using SchemaTemplateLib.Interfaces;
using SchemaTemplaceWeb.Models;

var builder = WebApplication.CreateBuilder(args);

var mcpTransportOptions = builder.Configuration.GetSection("McpTransport").Get<McpTransportOptions>() ?? new McpTransportOptions();

if (!mcpTransportOptions.EnableStdio && !mcpTransportOptions.EnableHttp)
{
    throw new InvalidOperationException("At least one MCP transport must be enabled. Set McpTransport:EnableStdio or McpTransport:EnableHttp to true.");
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataProcessing, DataProcessing>();
builder.Services.AddScoped<IExposeMethods, ExposeMethods>();

if (mcpTransportOptions.EnableStdio)
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole(options =>
    {
        options.LogToStandardErrorThreshold = LogLevel.Trace;
    });
}

var mcpServerBuilder = builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly();

if (mcpTransportOptions.EnableStdio)
{
    mcpServerBuilder.WithStdioServerTransport();
}

if (mcpTransportOptions.EnableHttp)
{
    mcpServerBuilder.WithHttpTransport(options =>
    {
        options.Stateless = mcpTransportOptions.Stateless;
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

if (mcpTransportOptions.EnableHttp)
{
    app.MapMcp(mcpTransportOptions.Path);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

