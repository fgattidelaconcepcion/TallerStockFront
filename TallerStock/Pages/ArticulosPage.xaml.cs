using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerStock.Models;
using TallerStock.Services;
using Microsoft.Maui.Controls;

namespace TallerStock.Pages
{
    public partial class ArticulosPage : ContentPage
    {
        private readonly ArticuloService _articuloService = new();
        private List<Articulo> _allArticulos = new();

        public ArticulosPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadArticulosAsync();
        }

        private async Task LoadArticulosAsync()
        {
            _allArticulos = await _articuloService.GetArticulosAsync();
            ArticulosListView.ItemsSource = _allArticulos;
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = e.NewTextValue?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(filtro))
            {
                ArticulosListView.ItemsSource = _allArticulos;
            }
            else
            {
                var filtrados = _allArticulos
                    .Where(a => a.Nombre.ToLower().Contains(filtro) || a.Categoria.ToLower().Contains(filtro))
                    .ToList();

                ArticulosListView.ItemsSource = filtrados;
            }
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
                        await LoadArticulosAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo eliminar el artículo.", "OK");
                    }
                }
            }
        }

        }
    }

