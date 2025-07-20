using System;
using System.Threading.Tasks;
using TallerStock.Models;
using TallerStock.Services;
using Microsoft.Maui.Controls;

namespace TallerStock.Pages
{
    public partial class ArticuloDetailPage : ContentPage
    {
        private readonly ArticuloService _articuloService = new();
        private Articulo _currentArticulo;

        public ArticuloDetailPage()
        {
            InitializeComponent();
        }

        public ArticuloDetailPage(Articulo articulo) : this()
        {
            _currentArticulo = articulo;
            LoadArticuloDetails();
        }

        private void LoadArticuloDetails()
        {
            if (_currentArticulo != null)
            {
                Title = "Editar Artículo";
                NombreEntry.Text = _currentArticulo.Nombre;
                StockEntry.Text = _currentArticulo.Stock.ToString();
                CategoriaEntry.Text = _currentArticulo.Categoria;

                EliminarButton.IsVisible = true;
            }
            else
            {
                Title = "Nuevo Artículo";
                EliminarButton.IsVisible = false;
            }
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(StockEntry.Text) ||
                string.IsNullOrWhiteSpace(CategoriaEntry.Text))
            {
                await DisplayAlert("Error", "Por favor, completá todos los campos.", "OK");
                return;
            }

            if (!int.TryParse(StockEntry.Text, out int stock) || stock < 0)
            {
                await DisplayAlert("Error", "El stock debe ser un número válido no negativo.", "OK");
                return;
            }

            if (_currentArticulo == null)
            {
                var existente = await _articuloService.GetArticuloPorNombreAsync(NombreEntry.Text);

                if (existente != null)
                {
                    existente.Stock += stock;
                    var success = await _articuloService.UpdateArticuloAsync(existente);
                    if (success)
                    {
                        await DisplayAlert("Actualizado", $"Se aumentó el stock del artículo '{existente.Nombre}'.", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo actualizar el stock del artículo existente.", "OK");
                    }
                }
                else
                {
                    var newArticulo = new Articulo
                    {
                        Nombre = NombreEntry.Text,
                        Stock = stock,
                        Categoria = CategoriaEntry.Text
                    };

                    var success = await _articuloService.AddArticuloAsync(newArticulo);
                    if (success)
                    {
                        await DisplayAlert("Éxito", "Artículo agregado correctamente.", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo agregar el artículo.", "OK");
                    }
                }
            }
            else
            {
                _currentArticulo.Nombre = NombreEntry.Text;
                _currentArticulo.Stock = stock;
                _currentArticulo.Categoria = CategoriaEntry.Text;

                var success = await _articuloService.UpdateArticuloAsync(_currentArticulo);
                if (success)
                {
                    await DisplayAlert("Éxito", "Artículo actualizado correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar el artículo.", "OK");
                }
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmar eliminación",
                $"¿Querés eliminar el artículo '{_currentArticulo?.Nombre}'?",
                "Sí", "No");

            if (confirm && _currentArticulo != null)
            {
                bool success = await _articuloService.DeleteArticuloAsync(_currentArticulo.Id);
                if (success)
                {
                    await DisplayAlert("Éxito", "Artículo eliminado correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar el artículo.", "OK");
                }
            }
        }

        private async void OnAumentarStockClicked(object sender, EventArgs e)
        {
            if (_currentArticulo == null)
                return;

            var movimiento = new MovimientoStock
            {
                ArticuloId = _currentArticulo.Id,
                Cantidad = 1,
                TipoMovimiento = "Ajuste",
                Comentario = "Incremento rápido desde detalle"
            };

            bool success = await _articuloService.CrearMovimientoAsync(movimiento);
            if (success)
            {
                await RefrescarStock();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo incrementar el stock.", "OK");
            }
        }

        private async void OnDisminuirStockClicked(object sender, EventArgs e)
        {
            if (_currentArticulo == null || _currentArticulo.Stock <= 0)
                return;

            var movimiento = new MovimientoStock
            {
                ArticuloId = _currentArticulo.Id,
                Cantidad = -1,
                TipoMovimiento = "Ajuste",
                Comentario = "Disminución rápida desde detalle"
            };

            bool success = await _articuloService.CrearMovimientoAsync(movimiento);
            if (success)
            {
                await RefrescarStock();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo disminuir el stock.", "OK");
            }
        }

        private async Task RefrescarStock()
        {
            var articulos = await _articuloService.GetArticulosAsync();
            _currentArticulo = articulos.Find(a => a.Id == _currentArticulo.Id);

            if (_currentArticulo != null)
            {
                StockEntry.Text = _currentArticulo.Stock.ToString();
            }
        }
    }
}
