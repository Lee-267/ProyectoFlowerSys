using FlowerSys.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FlowerSys.Frontend.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        public UsuarioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7202/"); /* URL de la API */
        }

        // MÉTODO PRIVADO PARA VALIDAR SESIÓN
        private bool ValidarSesion()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        // LISTAR USUARIOS
        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Auth");

            var response = await _httpClient.GetAsync("/api/Usuarios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<UsuarioDto>>(content);
                return View("ListaUsuarios", usuarios);
            }

            ViewBag.Error = "Error al obtener la lista de usuarios.";
            return View(new List<UsuarioDto>());
        }

        // OBTENER USUARIO PARA EDICIÓN
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Auth");

            var response = await _httpClient.GetAsync($"/api/Usuarios/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<UsuarioDto>(content);
                return View("Editar", usuario);
            }

            TempData["ErrorMessage"] = "No se encontró el usuario.";
            return RedirectToAction("ListaUsuarios");
        }


        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioDto model)
        {
            if (!ValidarSesion())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                return View(model); // Mantiene al usuario en la vista de edición
            }

            if (!string.IsNullOrEmpty(model.Password) && model.Password != model.ConfirmarPassword)
            {
                ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                return View(model); // Se mantiene en la vista
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria.");
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Usuarios/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Usuario actualizado correctamente.";
                return RedirectToAction("ListaUsuarios");
            }

            ViewBag.ErrorMessage = "Error al actualizar el usuario.";
            return View(model);
        }


        //REGISTRAR NUEVO USUARIO
        [HttpGet]
        public IActionResult Crear()
        {
            return View("Crear");
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioDto model)
        {
            if (!ModelState.IsValid)
            {
                return View("Crear", model);
            }

            // Validar si el correo ya está registrado
            var existeUsuario = await VerificarEmailExiste(model.Email);
            if (existeUsuario)
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
                return View("Crear", model); // Asegurar que la vista recibe el modelo con el error
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Usuarios", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario registrado correctamente.";
                return RedirectToAction("ListaUsuarios");
            }

            ViewBag.Error = "Error al registrar el usuario.";
            return View("Crear", model);
        }
        // MÉTODO PARA VALIDAR SI EL EMAIL YA ESTÁ REGISTRADO
        private async Task<bool> VerificarEmailExiste(string email)
        {
            var response = await _httpClient.GetAsync($"/api/Usuarios/ExisteEmail?email={email}");
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadAsStringAsync();
            return bool.Parse(result); // La API debe devolver `true` si el correo existe
        }
        // DESACTIVAR USUARIO
        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Auth");

            var response = await _httpClient.PatchAsync($"/api/Usuarios/DesactivarUsuario/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario desactivado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al desactivar el usuario.";
            }

            return RedirectToAction("ListaUsuarios");
        }

        // ACTIVAR USUARIO
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Auth");

            var response = await _httpClient.PatchAsync($"/api/Usuarios/ActivarUsuario/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario activado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al activar el usuario.";
            }

            return RedirectToAction("ListaUsuarios");
        }
    }
}
