using System.Text;
using FlowerSys.Frontend.Models;
using System.Text.Json;

namespace FlowerSys.Frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7202/api/Auth/Login";
        private readonly string _apiUrlRegister = "https://localhost:7202/api/Auth/Register";  // API para registrar usuario
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // Lógica para login
        public async Task<string?> LoginAsync(LoginDto login)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(login, options); // Serializa el objeto correctamente

            Console.WriteLine($"Enviando JSON: {json}");  // Muestra los datos en la consola

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error en la respuesta: {errorMessage}");  // Muestra la respuesta en consola
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<AuthResponse>(responseBody, options);
            return token?.Token;
        }

        // Lógica para registrar un usuario
        public async Task<string?> RegisterAsync(UsuarioDto register)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(register, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST a la API
                var response = await _httpClient.PostAsync(_apiUrlRegister, content);

                if (!response.IsSuccessStatusCode)
                {
                    // Si la respuesta no es exitosa, leer el cuerpo de la respuesta
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en el registro: {responseBody}");  // Log en consola
                    return responseBody; // Retorna el mensaje de error recibido desde la API
                }

                // Si el registro fue exitoso, retornar null (sin errores)
                return null;
            }
            catch (Exception ex)
            {
                // Si ocurre una excepción durante la solicitud HTTP, loguearla
                Console.WriteLine($"Excepción en el registro: {ex.Message}");
                return "Ocurrió un error al procesar la solicitud de registro. Por favor, intente más tarde."; // Mensaje de error general
            }
        }
    }
}
