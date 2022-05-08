using System.Net;
using System.Text.Json.Serialization;
using Application;
using Application.Exceptions;
using Domain.PlantAggregate;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews(configure =>
{
});
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.RegisterApplication(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(config =>
{
    config.InvalidModelStateResponseFactory = ctx =>
    {
        //var json = System.Text.Json.JsonSerializer.Serialize();
        throw new MyApplicationException("Invalid Model State", null) {  Payload = ctx.ModelState.Values.SelectMany(p => p.Errors) };
    };
});

//builder.Services.AddDbContext<ApplicationDbContext>(options => options
//    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()), ServiceLifetime.Scoped, ServiceLifetime.Scoped
//);
builder.Services.AddDbContext<ApplicationDbContext>(options => options
        .UseSqlite($"Data Source=plant.db")
);

// builder.Services.AddHostedService<WateringService>();

var app = builder.Build();

app.UseExceptionHandler(a => a.Run(async context =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature?.Error;
    if (exception is AggregateException aggEx)
        exception = aggEx.InnerExceptions.First();
    
    switch (exception)
    {
        case FluentValidation.ValidationException validationEx:
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new { validationEx.Message, validationEx.Errors });

            break;
        }
        case MyApplicationException appEx:
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new { appEx.Message, appEx.Payload });

            break;
        }
        default:
            await context.Response.WriteAsJsonAsync(new { error = exceptionHandlerPathFeature?.Error?.Message });
            break;
    }
}));

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

app.MapFallbackToFile("index.html"); ;

Seed(app.Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>());

app.Run();

static void Seed(ApplicationDbContext context)
{
    // dotnet ef migrations Add InitialCreate --project ../Persistence/Persistence.csproj
    // dotnet ef migrations remove  --project ../Persistence/Persistence.csproj
    if (context.Database.EnsureCreated() == false)
    {
        // Database already exists, don't try to seed it
        return;
    }
    
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Angel Wing Begonia", Location = "Living Room", LastWateredAt = null, ImageUrl = "https://www.houseplantsexpert.com/assets/images/angel_wing_begonia.jpg", IsWatering = true});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Barberton Daisy", Location = "Front Door", LastWateredAt = DateTimeOffset.UtcNow, ImageUrl = "https://www.gardeningknowhow.com/wp-content/uploads/2012/03/gerbera.jpg"});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Beach Spider Lily", Location = "Yard", LastWateredAt = DateTime.UtcNow.AddHours(-2), ImageUrl = "https://www.gardenia.net/storage/app/public/uploads/images/detail/2CUAR7OwcmqF87elKFxxyck0qSAt30tIaQdLAofX.jpeg"});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Belladonna Lily", Location = "Kitchen", LastWateredAt = DateTime.UtcNow.AddHours(-7), ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/e/ea/Amaryllis_belladonna.jpg"});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Bird Of Paradise", Location = "Bedroom", LastWateredAt = null, ImageUrl = "https://www.ftd.com/blog/wp-content/uploads/2016/08/hero-birdofparadise-720x480.jpg"});
        
    context.SaveChanges();
}