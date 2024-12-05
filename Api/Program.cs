using Business.Services;
using Business.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();
builder.Services.AddSingleton<ChangeFeedService>();

// Register controllers
builder.Services.AddControllers();


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Initialize CosmosDbService
var cosmosDbService = app.Services.GetRequiredService<ICosmosDbService>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseRouting();


app.MapControllers();

app.Run();