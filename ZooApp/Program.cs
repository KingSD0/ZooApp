using Microsoft.EntityFrameworkCore;
using System.Globalization; // Nodig voor cultuurinstellingen
using ZooApp.Data; // Zorg dat dit overeenkomt met jouw mapstructuur

var builder = WebApplication.CreateBuilder(args);

// Stel standaardcultuur in op Nederlands (ondersteunt o.a. komma's als decimaalteken)
var cultureInfo = new CultureInfo("nl-NL");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Voeg EF Core databasecontext toe met LocalDB 
builder.Services.AddDbContext<ZooContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Dierentuin03;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Voeg controllers en views toe
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Route-configuratie voor MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Animals}/{action=Index}/{id?}");

// Seed de database met testdata bij startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ZooContext>();
    DbInitializer.Seed(context);
}


app.Run();
