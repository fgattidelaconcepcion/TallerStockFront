using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TallerStock.Models;

namespace TallerStock.Services
{
    public class ArticuloService
    {
        private readonly HttpClient _httpClient;

        public ArticuloService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7054/") // Cambia la URL a tu API real
            };
        }

        public async Task<List<Articulo>> GetArticulosAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = await _httpClient.GetFromJsonAsync<List<Articulo>>("api/articulos", options);
                return response ?? new List<Articulo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener artículos: {ex.Message}");
                return new List<Articulo>();
            }
        }

        public async Task<bool> AddArticuloAsync(Articulo articulo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/articulos", articulo);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar artículo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateArticuloAsync(Articulo articulo)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/articulos/{articulo.Id}", articulo);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar artículo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteArticuloAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/articulos/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar artículo: {ex.Message}");
                return false;
            }
        }

        public async Task<List<MovimientoStock>> GetMovimientosPorArticuloAsync(int articuloId)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = await _httpClient.GetFromJsonAsync<List<MovimientoStock>>($"api/movimientosstock/{articuloId}", options);
                return response ?? new List<MovimientoStock>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener movimientos: {ex.Message}");
                return new List<MovimientoStock>();
            }
        }

        public async Task<bool> CrearMovimientoAsync(MovimientoStock movimiento)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/movimientosstock", movimiento);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear movimiento: {ex.Message}");
                return false;
            }
        }

        public async Task<Articulo?> GetArticuloPorNombreAsync(string nombre)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Articulo>>($"api/articulos?nombre={Uri.EscapeDataString(nombre)}");
                return response?.FirstOrDefault(a => a.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar artículo por nombre: {ex.Message}");
                return null;
            }
        }
    }
}
