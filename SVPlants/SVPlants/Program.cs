using System.Net;
using Application;
using Application.Exceptions;
using Domain.PlantAggregate;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

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

var app = builder.Build();

app.UseExceptionHandler(a => a.Run(async context =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

    switch (exceptionHandlerPathFeature?.Error)
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

app.Run();


static void UpdateDatabase(ApplicationDbContext context)
{
    context.Database.Migrate();

    Seed(context);
}

static void Seed(ApplicationDbContext context)
{
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Angel Wing Begonia", Location = "Living Room", Status = PlantStatus.Normal, LastWateredAt = null});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Barberton Daisy", Location = "Front Door", Status = PlantStatus.Normal, LastWateredAt = null});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Beach Spider Lily", Location = "Yard", Status = PlantStatus.Normal, LastWateredAt = null});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Belladonna Lily", Location = "Kitchen", Status = PlantStatus.Normal, LastWateredAt = null});
    context.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Bird Of Paradise", Location = "Bedroom", Status = PlantStatus.Normal, LastWateredAt = null});
        
    context.SaveChanges();
}