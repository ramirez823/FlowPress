using FlowPress.Data;
using FlowPress.Repositories;
using FlowPress.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlowPress.Services;
using FlowPress.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = true; })
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// Registro de servicios
//recuperacion de contrasenia por email
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSender, IdentityEmailSender>();
//-------------------------------------------------------------
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<ISecretService, SecretService>();

// Registro de repositorios
builder.Services.AddScoped<ISourceRepository, SourceRepository>();
builder.Services.AddScoped<ISourceItemRepository, SourceItemRepository>();
builder.Services.AddScoped<ISecretRepository, SecretRepository>(); 


// Registro de News Adapters y Aggregator
builder.Services.AddHttpClient<FlowPress.Services.News.GNewsAdapter>();
builder.Services.AddHttpClient<FlowPress.Services.News.NewsApiAdapter>();
builder.Services.AddHttpClient<FlowPress.Services.News.NewsDataAdapter>();
builder.Services.AddHttpClient<FlowPress.Services.News.MediaStackAdapter>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        // MediaStack plan gratuito no soporta HTTPS
        AllowAutoRedirect = false
    });

builder.Services.AddScoped<INewsAdapter, FlowPress.Services.News.GNewsAdapter>();

builder.Services.AddHttpClient<FlowPress.Services.News.NewsApiAdapter>(client => //como jode 
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("FlowPress/1.0");
});
builder.Services.AddScoped<INewsAdapter, FlowPress.Services.News.NewsDataAdapter>();
builder.Services.AddScoped<INewsAdapter, FlowPress.Services.News.MediaStackAdapter>();
builder.Services.AddScoped<INewsAggregatorService, FlowPress.Services.News.NewsAggregatorService>();


// Registrar el seed de admin como hosted service
builder.Services.AddHostedService<AdminSeedService>();

builder.Services.AddHostedService<NewsSourcesSeedService>();



builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
