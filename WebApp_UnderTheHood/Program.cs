using Microsoft.AspNetCore.Authorization;
using WebApp_UnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();

//Configuring  Authentication middleware to accept our custom Authentication method
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";

    //If we do not follow the convension of the login page (located in Account and named Login '/Account/Login')
    //we need to specify the page Path as below.

    //options.LoginPath = "/Account/LoginPage";


    //In order to show to the  user he is not authorized to be in a page we offer him feedback by redirecting him to a special access denied page

    options.AccessDeniedPath = "/Account/AccessDenied";

    //Configure the lifetime of a cookie
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});

builder.Services.AddAuthorization(options =>
{
    //Specify a policy
    options.AddPolicy("MustBelongToHRDepartment", policy =>
    {
        policy.RequireClaim("Department", "HR");
    });
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireClaim("Admin");
    });

    //combine multiple Requirements in a policy
    options.AddPolicy("ManagerOfHumanResource", policy =>
    {
        policy.RequireClaim("Manager").RequireClaim("Department", "HR").Requirements.Add(new HRManagerProbationRequirement(1));

    });

});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

builder.Services.AddHttpClient("OurWebApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7179/");
});



builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


//Attach Authentication middleware to the pipeline
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
