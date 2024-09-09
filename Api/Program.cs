using Api.Business.Abstracts;
using Api.Business.Services;
using Api.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

using Api.Business.Hubs;
using Api.Business.Services;
using Api.DataAccess.Abstracts;
using Api.DataAccess.Concretes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddDbContext<DataContext>(opt =>
//{
//    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSignalR();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddSingleton<BroadcastService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();




builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

var broadcastService = app.Services.GetRequiredService<BroadcastService>();
_ = Task.Run(async () => await broadcastService.StartBroadcastAsync());


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(builder =>
{
    builder.WithOrigins("http://192.168.1.195:11000", "https://192.168.1.195:11000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseRouting();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<DrawingHub>("/Whiteboard");
    
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.MapControllers();

app.Run();
