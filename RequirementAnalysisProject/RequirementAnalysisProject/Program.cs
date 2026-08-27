using Microsoft.EntityFrameworkCore;
using RequirementAnalysisProject.Data;
using RequirementAnalysisProject.Repositories;
using RequirementAnalysisProject.Repositories.Interfaces;
using RequirementAnalysisProject.Services;
using RequirementAnalysisProject.Services.AI;


var builder = WebApplication.CreateBuilder(args);

// ── Controllers & Swagger ──────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── PostgreSQL Database ────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// ── HttpClient ─────────────────────────────────────────────────
builder.Services.AddHttpClient();

// ── Repositories ───────────────────────────────────────────────
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IAnalysisResultRepository, AnalysisResultRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IConsolidatedResultRepository, ConsolidatedResultRepository>();

// ── Services ───────────────────────────────────────────────────
builder.Services.AddScoped<TranscriptionService>();  
builder.Services.AddScoped<GeminiClientService>();
builder.Services.AddScoped<AnalysisOrchestrator>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

// ── CORS for React ─────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ── Auto migrate on startup ────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();