using ConwayGameOfLife.Core.Services;
using ConwayGameOfLife.Infrastructure.Data;
using ConwayGameOfLife.Infrastructure.Repositories;
using ConwayGameOfLife.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGameOfLifeService, HashlifeGameOfLifeService>();
builder.Services.AddScoped<BoardRepository>();
builder.Services.AddDbContext<GameOfLifeDbContext>(options => options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("ConwayGameOfLife")));
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameOfLifeDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseRouting();
app.MapControllers();
// app.UseCors("AllowAll");

app.Run();
