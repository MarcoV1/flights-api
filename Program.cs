using angular_asp.Data;
using angular_asp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// add db context

// in memory
builder.Services.AddDbContext<Entities>(options => 
    options.UseInMemoryDatabase(databaseName: "Flights" ),
    ServiceLifetime.Singleton
    );

// uncomment if you want to use sql
/*builder.Services.AddDbContext<Entities>(options =>
    options.UseSqlServer(
            builder.Configuration.GetConnectionString("Flights")
        ));*/


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen(c =>
{
    c.DescribeAllParametersInCamelCase();
});

builder.Services.AddScoped<Entities>();

var app = builder.Build();

var entities = app.Services.CreateScope().ServiceProvider.GetService<Entities>();

entities.Database.EnsureCreated();

var random = new Random();

Flight[] flightsToSeed = new Flight[]
{
            new ( Guid.NewGuid(),
                "American Airlines",
                random.Next(90, 5000).ToString(),
                new TimePlace("LA", DateTime.Now.AddHours(random.Next(1,3))),
                new TimePlace("New York", DateTime.Now.AddHours(random.Next(1,3))),
                random.Next(10, 853)),
             new ( Guid.NewGuid(),
                "British Airlines",
                random.Next(90, 5000).ToString(),
                new TimePlace("London", DateTime.Now.AddHours(random.Next(1,3))),
                new TimePlace("Instanbul", DateTime.Now.AddHours(random.Next(1,3))),
                random.Next(10, 853))
};
entities.Flights.AddRange(flightsToSeed);

entities.SaveChanges();

app.UseCors(builder => builder.WithOrigins("*")
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseSwagger().UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
