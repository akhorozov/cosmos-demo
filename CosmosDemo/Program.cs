using CosmosDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TelemetryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/meter/telemetry",
    async (TelemetryRepository repo, Telemetry item) =>
    {
        return Results.Ok(await repo.Create(item));
    });

app.MapGet("/meter/telemetry/{id}",
    async (TelemetryRepository repo, string id) =>
    {
        return Results.Ok(await repo.GetById(id));
    });

app.MapGet("/meter/monitor/{id}",
    async (TelemetryRepository repo, string id) =>
    {
        return Results.Ok(await repo.GetAll(id));
    });

app.MapPut("/meter/telemetry",
    async (TelemetryRepository repo, Telemetry item) =>
    {
        await repo.Update(item);
        return Results.NoContent();
    });

app.MapDelete("/meter/telemetry/{id}",
    async (TelemetryRepository repo, string id) =>
    {
        var item = await repo.GetById(id);
        if (item is not null)
        {
            await repo.Delete(item);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    });

app.Run();

