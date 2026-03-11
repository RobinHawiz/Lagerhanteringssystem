using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
        .UseSeeding((context, _) =>
        {
            if (context is ApplicationDbContext db)
            {
                // Employees
                if (!db.Employees.Any())
                {
                    db.Employees.AddRange(
                        new Employee { FirstName = "Anna", LastName = "Svensson", Email = "anna.svensson@company.se", PhoneNumber = "0701234567" },
                        new Employee { FirstName = "Erik", LastName = "Nilsson", Email = "erik.nilsson@company.se", PhoneNumber = "0707654321" }
                    );
                    db.SaveChanges();
                }

                // Categories
                if (!db.Categories.Any())
                {
                    db.Categories.AddRange(
                        new Category { Name = "Torrvaror" },
                        new Category { Name = "Dryck" },
                        new Category { Name = "Hygien" }
                    );
                    db.SaveChanges();
                }

                // Items
                if (!db.Items.Any())
                {
                    var torrvarorId = db.Categories.Single(c => c.Name == "Torrvaror").Id;
                    var dryckId = db.Categories.Single(c => c.Name == "Dryck").Id;
                    var hygienId = db.Categories.Single(c => c.Name == "Hygien").Id;

                    db.Items.AddRange(
                        new Item
                        {
                            Name = "Pasta",
                            Description = "Spaghetti 500g",
                            Price = 19.90m,
                            Amount = 120,
                            CategoryId = torrvarorId
                        },
                        new Item
                        {
                            Name = "Kaffe",
                            Description = "Mörkrost 450g",
                            Price = 59.90m,
                            Amount = 40,
                            CategoryId = torrvarorId
                        },
                        new Item
                        {
                            Name = "Läsk",
                            Description = "Cola 1.5L",
                            Price = 24.90m,
                            Amount = 75,
                            CategoryId = dryckId
                        },
                        new Item
                        {
                            Name = "Tvål",
                            Description = "Flytande tvål 500ml",
                            Price = 29.90m,
                            Amount = 30,
                            CategoryId = hygienId
                        }
                    );

                    db.SaveChanges();
                }
            }
        })
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    await DbAccountSeeder.Seed(app.Services);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
