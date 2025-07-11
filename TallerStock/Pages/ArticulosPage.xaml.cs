using System;
using TallerStock.Models;
using TallerStock.Services;
using Microsoft.Maui.Controls;

namespace TallerStock.Pages
{
    public partial class ArticulosPage : ContentPage
    {
        private readonly ArticuloService _articuloService = new();

        public ArticulosPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadArticulos();
        }

        private async void LoadArticulos()
        {
            var articulos = await _articuloService.GetArticulosAsync();
            ArticulosListView.ItemsSource = articulos;
        }

        private async void OnAddArticuloClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArticuloDetailPage());
        }

        private async void OnEditarArticuloClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Articulo articulo)
            {
                await Navigation.PushAsync(new ArticuloDetailPage(articulo));
            }
        }

        private async void OnEliminarArticuloClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Articulo articulo)
            {
                bool confirm = await DisplayAlert("Confirmar eliminación",
                                                  $"¿Querés eliminar el artículo '{articulo.Nombre}'?",
                                                  "Sí", "No");
                if (confirm)
                {
                    bool success = await _articuloService.DeleteArticuloAsync(articulo.Id);
                    if (success)
                    {
                        await DisplayAlert("Éxito", "Artículo eliminado correctamente.", "OK");
                        LoadArticulos();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo eliminar el artículo.", "OK");
                    }
                }
            }
        }

        private async void OnDecreaseStockClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Articulo articulo)
            {
                if (articulo.Stock > 0)
                {
                    articulo.Stock--;
                    bool success = await _articuloService.UpdateArticuloAsync(articulo);
                    if (success)
                    {
                        LoadArticulos();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo actualizar el stock.", "OK");
                    }
                }
            }
        }

        private async void OnIncreaseStockClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Articulo articulo)
            {
                articulo.Stock++;
                bool success = await _articuloService.UpdateArticuloAsync(articulo);
                if (success)
                {
                    LoadArticulos();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar el stock.", "OK");
                }
            }
        }
    }
}
