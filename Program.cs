// OpenAPI/Swagger not configured to avoid missing package errors

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddEndpointsApiExplorer();

// Add controllers and configure enums to be serialized as strings
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Dependency Injection for new services
builder.Services.AddScoped<WeatherGuardiansAPI.Services.IDrizzleService, WeatherGuardiansAPI.Services.DrizzleService>();
builder.Services.AddScoped<WeatherGuardiansAPI.Services.IHazeService, WeatherGuardiansAPI.Services.HazeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

// Map attribute-routed controllers
app.MapControllers();

app.Run();
