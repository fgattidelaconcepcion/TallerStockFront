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
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(StockEntry.Text) ||
                string.IsNullOrWhiteSpace(CategoriaEntry.Text) ||
                string.IsNullOrWhiteSpace(TamanoEntry.Text))
            {
                await DisplayAlert("Error", "Complete todos los campos.", "OK");
                return;
            }

            if (!int.TryParse(StockEntry.Text, out int stock) || stock < 0)
            {
                await DisplayAlert("Error", "Stock inválido.", "OK");
                return;
            }

            if (_currentArticulo == null)
            {
                var existente = await _articuloService.GetArticuloPorNombreAsync(NombreEntry.Text);
                if (existente != null)
                {
                    existente.Stock += stock;
                    existente.Categoria = CategoriaEntry.Text;
                    existente.Tamano = TamanoEntry.Text;
                    if (await _articuloService.UpdateArticuloAsync(existente))
                    {
                        await DisplayAlert("Actualizado", "Se actualizó el stock y datos.", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                }
                else
                {
                    _currentArticulo = new Articulo
                    {
                        Nombre = NombreEntry.Text,
                        Stock = stock,
                        Categoria = CategoriaEntry.Text,
                        Tamano = TamanoEntry.Text
                    };
                    if (await _articuloService.AddArticuloAsync(_currentArticulo))
                    {
                        await DisplayAlert("Éxito", "Artículo agregado correctamente.", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                }
                await DisplayAlert("Error", "No se pudo guardar el artículo.", "OK");
            }
            else
            {
                _currentArticulo.Nombre = NombreEntry.Text;
                _currentArticulo.Stock = stock;
                _currentArticulo.Categoria = CategoriaEntry.Text;
                _currentArticulo.Tamano = TamanoEntry.Text;

                if (await _articuloService.UpdateArticuloAsync(_currentArticulo))
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
