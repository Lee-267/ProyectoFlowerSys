using FlowerSys.Frontend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);




// servicios de autenticaci�n


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";  // Redirige al login si no est� autenticado
        options.LogoutPath = "/Auth/Logout";  // Redirige al logout
        options.AccessDeniedPath = "/Home/AccessDenied";  // Redirige si no tiene acceso
    });






// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession();  // Habilitar sesiones
builder.Services.AddHttpContextAccessor();  // Acceso al HttpContext
builder.Services.AddHttpClient<AuthService>();



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


// Evitar el almacenamiento en cach� de las respuestas en el navegador
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});


app.UseRouting();

// Middleware para habilitar sesiones antes de la autenticaci�n y autorizaci�n.
app.UseSession();  // Asegura de que las sesiones est�n habilitadas
app.UseAuthentication();  // Middleware de autenticaci�n
app.UseAuthorization();  // Middleware de autorizaci�n





app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
