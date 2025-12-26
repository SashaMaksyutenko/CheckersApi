using CheckersApi.Engine;
using CheckersApi.Validation;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// 🔧 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 🔧 Engine
builder.Services.AddSingleton<IEngineAdapter>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var dbPath = cfg["Engine:Databases"];
    var useInit = bool.TryParse(cfg["Engine:UseInit"], out var flag) && flag;

    return new KingsRowAdapter(dbPath, useInit);
});

var app = builder.Build();

// 🔧 Middleware
app.UseRouting();
app.UseCors();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkers API v1");
        c.RoutePrefix = "swagger"; // Swagger доступний на /swagger/index.html
    });
}

app.MapControllers();
app.Run();