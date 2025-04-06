using FlowerSys.Frontend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);




// servicios de autenticación


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";  // Redirige al login si no está autenticado
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


// Evitar el almacenamiento en caché de las respuestas en el navegador
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});


app.UseRouting();

// Middleware para habilitar sesiones antes de la autenticación y autorización.
app.UseSession();  // Asegura de que las sesiones están habilitadas
app.UseAuthentication();  // Middleware de autenticación
app.UseAuthorization();  // Middleware de autorización





app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
