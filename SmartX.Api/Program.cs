var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<SmartX.Api.Services.RealtimeEngine>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
SmartX.Api.Services.RealtimeBridge.OnReading = app.Services.GetRequiredService<SmartX.Api.Services.RealtimeEngine>().Accept;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/api/dashboard", () =>
{
    return Results.Ok(new
    {
        message = "SmartX API is running"
    });
});

app.Run();
