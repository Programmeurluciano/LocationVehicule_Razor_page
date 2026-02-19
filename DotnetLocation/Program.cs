using DotnetLocation.Data;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Identity ----------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Config cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";          // redirection si non connecté
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// ---------------- Elasticsearch ----------------
var esSettings = builder.Configuration.GetSection("Elasticsearch");

var settings = new ElasticsearchClientSettings(new Uri(esSettings["Uri"]))
    .Authentication(new BasicAuthentication(
        esSettings["Username"],
        esSettings["Password"]
    ))
    .EnableDebugMode();

var elasticClient = new ElasticsearchClient(settings);
builder.Services.AddSingleton(elasticClient);

// ---------------- Razor Pages ----------------
builder.Services.AddRazorPages(options =>
{
    // Protéger tout le site par défaut
    options.Conventions.AuthorizeFolder("/");

    // Autoriser login et access denied sans authentification
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");

    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage("/Error/404");

});

var app = builder.Build();

// ---------------- Middleware ----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseRouting();

// ⚠️ L’ordre est important
app.UseAuthentication(); // d’abord
app.UseAuthorization();  // ensuite


// Razor pages + assets
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

// ---------------- Seeder rôles et admin ----------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Création admin
    var admin = await userManager.FindByNameAsync("admin");
    if (admin == null)
    {
        admin = new IdentityUser { UserName = "admin", Email = "admin@test.com", EmailConfirmed = true };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.Run();
