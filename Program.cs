using SaleCheck.DataAccess;
using SaleCheck.DataAccess.Interfaces;
using SaleCheck.Model.Utility;
using SaleCheck.Repositories;
using SaleCheck.Repositories.Interfaces;
using SaleCheck.Tests.SaleCheck.Tests.Model.Utility;
using SaleCheck.Utility;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

// Register MongoDbContext as Singleton
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddSingleton<ScheduledTaskService>();

// Register WebsiteRepository as Scoped
builder.Services.AddScoped<IWebsiteRepository, WebsiteRepository>();
builder.Services.AddScoped<DataFactory>();
builder.Services.AddScoped<ProductAnalysisService>();
builder.Services.AddScoped<WebsiteSummaryService>();
builder.Services.AddHostedService<ScheduledTaskService>();
builder.Services.AddScoped<PriceCleanupService>();

// Add ScheduledTaskService as both a singleton and hosted service

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

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
