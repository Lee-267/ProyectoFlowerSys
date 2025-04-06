using FlowerSys.Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using FlowerSys.Frontend.Models;
using Microsoft.CodeAnalysis.Scripting;

namespace FlowerSys.Frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;


        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

  
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = await _authService.LoginAsync(model);
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Credenciales incorrectas.";  // Usamos TempData para el mensaje de error
                return RedirectToAction("Login");  // Redirigimos al mismo formulario para mostrar el mensaje
            }
            // Guardar el token en la sesión y en una cookie segura
            HttpContext.Session.SetString("Token", token);
            Response.Cookies.Append("Token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = System.DateTime.UtcNow.AddHours(1) // Expira en 1 hora
            });
            HttpContext.Session.SetString("Token", token);
            TempData["WelcomeMessage"] = "¡Bienvenido a FlowerSys!";
            return RedirectToAction("Index", "Home");
        }



        // Acción para cerrar sesión
        [HttpPost]
        public IActionResult Logout()
        {
            // Eliminar la sesión
            HttpContext.Session.Clear();

            // Eliminar las cookies de autenticación
            Response.Cookies.Delete("Token");
            Response.Cookies.Delete(".AspNetCore.Session");

            // Devolver un JSON indicando éxito
            return Json(new { success = true, redirectUrl = Url.Action("Login", "Auth") });
        }


    }
}



