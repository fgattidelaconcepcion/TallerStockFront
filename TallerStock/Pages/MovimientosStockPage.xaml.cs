using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using TallerStock.Models;
using TallerStock.Services;

namespace TallerStock.Pages;

public partial class MovimientosStockPage : ContentPage
{
    private readonly ArticuloService _articuloService = new();
    private int _articuloId;

    public MovimientosStockPage(int articuloId)
    {
        InitializeComponent();
        _articuloId = articuloId;
        TipoMovimientoPicker.ItemsSource = new List<string> { "Compra", "Uso", "Ajuste" };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarMovimientos();
    }

    private async System.Threading.Tasks.Task CargarMovimientos()
    {
        var movimientos = await _articuloService.GetMovimientosPorArticuloAsync(_articuloId);
        MovimientosList.ItemsSource = movimientos;
    }

    private async void OnRegistrarMovimientoClicked(object sender, EventArgs e)
    {
        if (TipoMovimientoPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Seleccioná un tipo de movimiento.", "OK");
            return;
        }

        if (!int.TryParse(CantidadEntry.Text, out int cantidad) || cantidad == 0)
        {
            await DisplayAlert("Error", "Ingresá una cantidad válida distinta de cero.", "OK");
            return;
        }

        // Ajustamos la cantidad según el tipo de movimiento
        var tipoMovimiento = TipoMovimientoPicker.SelectedItem.ToString()!;
        if (tipoMovimiento == "Uso" && cantidad > 0)
        {
            cantidad = -cantidad; // egreso, stock baja
        }
        else if (tipoMovimiento != "Uso" && cantidad < 0)
        {
            cantidad = Math.Abs(cantidad); // ingreso, stock sube
        }

        var nuevoMovimiento = new MovimientoStock
        {
            ArticuloId = _articuloId,
            Cantidad = cantidad,
            TipoMovimiento = tipoMovimiento,
            Comentario = string.IsNullOrWhiteSpace(ComentarioEntry.Text) ? null : ComentarioEntry.Text
        };

        var success = await _articuloService.CrearMovimientoAsync(nuevoMovimiento);
        if (success)
        {
            await DisplayAlert("Éxito", "Movimiento registrado correctamente.", "OK");
            await CargarMovimientos();
            CantidadEntry.Text = string.Empty;
            ComentarioEntry.Text = string.Empty;
            TipoMovimientoPicker.SelectedIndex = -1;
        }
        else
        {
            await DisplayAlert("Error", "No se pudo registrar el movimiento.", "OK");
        }
    }
}
