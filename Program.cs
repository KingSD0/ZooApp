using Microsoft.EntityFrameworkCore;
using ZooApp.Data; // Zorg dat dit overeenkomt met jouw mapstructuur

var builder = WebApplication.CreateBuilder(args);

// Voeg EF Core databasecontext toe met LocalDB 
builder.Services.AddDbContext<ZooContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Dierentuin3;Trusted_Connection=True;MultipleActiveResultSets=true"));

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
