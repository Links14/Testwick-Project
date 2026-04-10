using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Testwick.Data;
using Testwick.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Swagger is OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // adds OpenAPI

builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizResultsService, QuizResultsService>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // options.UseSqlServer(connectionString); // Depreciated
    options.UseSqlite(connectionString, o =>
    {
        o.CommandTimeout(10); // seconds
    });
});

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // utilizes OpenAPI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                message = contextFeature.Error.Message
            });
        }
    });
});

//app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAll"); // CORS
app.UseAuthorization();
app.MapControllers();

// ensure safe entry
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    int retries = 5;

    for (int i = 0; i < retries; i++)
    {
        try
        {
            using var retryScope = app.Services.CreateScope();
            var db = retryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            Console.WriteLine("Database migration successful.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration attempt {i + 1} failed: {ex.Message}");
            if (i == retries - 1) Console.WriteLine("Migration failed after retries. Continuing startup...");
            else Thread.Sleep(1000 * (i + 1)); // backoff
        }
    }
}

app.Run();
