using System;
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
                TamanoEntry.Text = _currentArticulo.Tamano;
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
            if (!int.TryParse(StockEntry.Text, out int stock))
            {
                await DisplayAlert("Error", "Stock inválido", "OK");
                return;
            }

            var articulo = new Articulo
            {
                Nombre = NombreEntry.Text?.Trim(),
                Stock = stock,
                Categoria = CategoriaEntry.Text?.Trim(),
                Tamano = TamanoEntry.Text?.Trim()
            };

            var success = await _articuloService.AddArticuloAsync(articulo);

            if (success)
            {
                await DisplayAlert("Éxito", "Artículo guardado correctamente", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar el artículo", "OK");
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmar eliminación",
                                             $"Eliminar '{_currentArticulo?.Nombre}'?",
                                             "Sí", "No");
            if (confirm)
            {
                if (await _articuloService.DeleteArticuloAsync(_currentArticulo.Id))
                {
                    await DisplayAlert("Éxito", "Artículo eliminado.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar.", "OK");
                }
            }
        }

    }
}
