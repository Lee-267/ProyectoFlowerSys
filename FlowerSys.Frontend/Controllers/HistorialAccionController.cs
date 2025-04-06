using FlowerSys.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlowerSys.Frontend.Controllers
{
    public class HistorialAccionController : Controller
    {
        private readonly ILogger<HistorialAccionController> _logger;
        private readonly HttpClient _httpClient;

        public HistorialAccionController(ILogger<HistorialAccionController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7202/");

        }

        public async Task<IActionResult> Index()
        {

            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _httpClient.GetAsync("/api/HistorialAccion");
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var HistorialAcciones = JsonConvert.DeserializeObject<IEnumerable<HistorialAccion>>(content);
                return View("Index", HistorialAcciones);
            }

            return View(new List<HistorialAccion>());
        }


        public async Task <IActionResult>DeleteAllHistorial()
        {
            var response = await _httpClient.DeleteAsync($"/api/HistorialAccion/deleteall");

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertMessage"] = "Se ha vaciado tu historial.Ten un buen día! ";
                return RedirectToAction("Index","HistorialAccion");
            }

            return View(new List<HistorialAccion>());
        }
        
     

    }
}
