using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//Register custom database context obj
builder.Services.AddDbContext<ApplicationDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure the Stripe env to a StripeSettings object, basically injecting the values in
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//Register default use for ASP.NET Core Identity and also adding db context for it
//and also handles roles when handling Identity, different type of roles and identity obj
//is handled in the register page
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();


//configure what is in the authentication cookie for the application
//since the default redirect path is wrong
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//Register the db handler interface and class for db operations, this is only valid for the
//duration of the request, so the next request uses another instance of unitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Assign the Stripe SecretKey value in env(appsettings) to StripeConfiguration API key
// so that the nuget package can connect to the stripe backend service
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

//Telling the app to use authentication state, the auth should always be before authorization
app.UseAuthentication();
app.UseAuthorization();

//Routing for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

//Routing for razor pages, used by ASP.net core Identity
app.MapRazorPages();

app.Run();
