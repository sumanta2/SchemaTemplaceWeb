using SchemaTemplateLib;
using SchemaTemplateLib.Implementation;
using SchemaTemplateLib.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataProcessing, DataProcessing>();
builder.Services.AddScoped<IExposeMethods, ExposeMethods>();

// Redirect stdout to stderr so the MCP stdio transport only receives JSON on stdout
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Suppress hosting startup/shutdown messages and redirect all logs to stderr
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace; // everything goes to stderr
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
