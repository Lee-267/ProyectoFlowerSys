﻿using FlowerSys.Frontend.Models;
using FlowerSys.Frontend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;


namespace FlowerSys.Frontend.Controllers
{
    
    public class ProductosController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7202/");/*url de la api*/
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var service = new AuthResponse();

            var response = await _httpClient.GetAsync("/api/Productos");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var productos = JsonConvert.DeserializeObject<IEnumerable<Productos>>(content);
                return View("Index", productos);
            }

            return View(new List<Productos>());
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Productos producto)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Productos/", content);

                if (response.IsSuccessStatusCode)
                {
                    HistorialAccion historialAccion = new()
                    {
                        Accion = $"Se insertó un nuevo producto: {producto.Nombre}",
                        FechaAccion = DateTime.Now
                    };
                    var jsonHistorial = JsonConvert.SerializeObject(historialAccion);
                    var contentHistorial = new StringContent(jsonHistorial, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync("/api/HistorialAccion/", contentHistorial);
                    TempData["AlertMessage"] = "Producto creado exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear el producto!!!";
                }


            }

            return View(producto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _httpClient.GetFromJsonAsync<Productos>($"/api/Productos/{id}");
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Productos producto)
        {
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/Productos/{id}", content);

                if (response.IsSuccessStatusCode)
                {

                    HistorialAccion historialAccion = new()
                    {
                        Accion = $"Se Actualizó la información del siguiente producto: {producto.Nombre}",
                        FechaAccion = DateTime.Now
                    };
                    var jsonHistorial = JsonConvert.SerializeObject(historialAccion);
                    var contentHistorial = new StringContent(jsonHistorial, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync("/api/HistorialAccion/", contentHistorial);
                    TempData["AlertMessage"] = "Producto actualizado exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["ErrorMessage"] = "Error al actualizar producto!!";
                }
            }

            return View(producto);
        }

        public async Task<IActionResult> Entrada(int id)
        {
            var producto = await _httpClient.GetFromJsonAsync<Productos>($"/api/Productos/{id}");
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Entrada(int id, Productos producto)
        {
            if (ModelState.IsValid)
            {

                producto.Stock += producto.Cantidad;
                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/Productos/{id}", content);


                if (response.IsSuccessStatusCode)
                {
                    Operacion operacion = new()
                    {
                        Tipo = "Entrada",
                        Fecha = DateTime.Now,
                        Producto = producto.Nombre,
                        Cantidad = producto.Cantidad,
                        Stock = producto.Stock,
                    };
                    var jsonOperacion = JsonConvert.SerializeObject(operacion);
                    var contentOperacion = new StringContent(jsonOperacion, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync("/api/Operaciones/", contentOperacion);
                    TempData["AlertMessage"] = "Entrada agregada exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al agregar entrada!!!";
                }
            }

            return View(producto);
        }

        public async Task<IActionResult> Salida(int id)
        {
            var producto = await _httpClient.GetFromJsonAsync<Productos>($"/api/Productos/{id}");
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Salida(int id, Productos producto)
        {
            if (ModelState.IsValid)
            {

                producto.Stock -= producto.Cantidad;
                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/Productos/{id}", content);


                if (response.IsSuccessStatusCode)
                {
                    Operacion operacion = new()
                    {
                        Tipo = "Salida",
                        Fecha = DateTime.Now,
                        Producto = producto.Nombre,
                        Cantidad = producto.Cantidad,
                        Stock = producto.Stock,
                    };
                    var jsonOperacion = JsonConvert.SerializeObject(operacion);
                    var contentOperacion = new StringContent(jsonOperacion, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync("/api/Operaciones/", contentOperacion);
                    TempData["AlertMessage"] = "Salida agregada exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al agregar la Salida de inventario!!!";
                }
            }

            return View(producto);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Productos/{id}");

            if (response.IsSuccessStatusCode)
            {
                HistorialAccion historialAccion = new()
                {
                    Accion = "Se eliminó un producto existente en el inventario",
                    FechaAccion = DateTime.Now
                };
                var jsonHistorial = JsonConvert.SerializeObject(historialAccion);
                var contentHistorial = new StringContent(jsonHistorial, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("/api/HistorialAccion/", contentHistorial);
                TempData["AlertMessage"] = "Producto eliminado exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Error al eliminar el producto.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {

            var response = await _httpClient.GetAsync($"/api/Productos/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var producto = await response.Content.ReadFromJsonAsync<Productos>();
            return View(producto);
        }

        public async Task<IActionResult> ProductosProximosAVencer()
        {
            var response = await _httpClient.GetAsync("/api/Productos/vencimiento-proximo");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<dynamic>(content);

                var productos = JsonConvert.DeserializeObject<IEnumerable<Productos>>(resultado.productos.ToString());

                return View("ProductosProximosAVencer", productos);
            }

            TempData["ErrorMessage"] = "Error al obtener productos próximos a vencer";
            return View(new List<Productos>());
        }

        // Acción para obtener los productos con bajo stock
        public async Task<IActionResult> ProductosBajoStock()
        {
            var response = await _httpClient.GetAsync("/api/Productos/bajo-stock");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<dynamic>(content);

                var productos = resultado.productos.ToObject<List<Productos>>();

                return View("ProductosBajoStock", productos);
            }

            TempData["ErrorMessage"] = "Error al obtener productos con bajo stock";
            return View(new List<Productos>());
        }
    }
}
