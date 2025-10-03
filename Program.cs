using WeatherGuardiansAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add controllers for attribute-routed APIs (e.g., BlazeController)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Register application services
builder.Services.AddScoped<IBlazeService, BlazeService>();
builder.Services.AddScoped<IGaleService, GaleService>();
builder.Services.AddScoped<WeatherGuardiansAPI.Services.IDrizzleService, WeatherGuardiansAPI.Services.DrizzleService>();
builder.Services.AddScoped<WeatherGuardiansAPI.Services.IHazeService, WeatherGuardiansAPI.Services.HazeService>();
builder.Services.AddScoped<WeatherGuardiansAPI.Services.IHealthService, WeatherGuardiansAPI.Services.HealthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Map attribute-routed controllers
app.MapControllers();

app.Run();
