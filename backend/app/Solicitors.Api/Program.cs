using Solicitors.Api.Endpoints;
using Solicitors.CacheBuild;
using Solicitors.Core;
using Solicitors.Data;
using Solicitors.HtmlParsing;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("/data/apiSettings.json", optional: true);

builder.Services.AddOpenApi();
builder.Services.AddCore();
builder.Services.AddData(builder.Configuration);
builder.Services.AddCacheBuild();
builder.Services.Configure<ImportConfiguration>(builder.Configuration.GetSection("Imports"));
builder.Services.AddHtmlParsing();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapSolicitors();
app.MapCities();
app.MapRatingsProviders();
app.UseCors();

using (var scope = app.Services.CreateScope())
{
    await scope.UseCacheBuildAsync(CancellationToken.None);
}

app.Run();