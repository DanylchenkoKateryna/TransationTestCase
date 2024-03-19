using Microsoft.EntityFrameworkCore;
using TransactionTestCase.Contracts;
using TransactionTestCase.Data;
using TransactionTestCase.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICSVService, CSVService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<ITimeZoneService, TimeZoneService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddDbContext<TransactionContex>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
