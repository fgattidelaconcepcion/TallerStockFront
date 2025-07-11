using System;

namespace TallerStock.Models
{
    public class MovimientoStock
    {
        public int Id { get; set; }
        public int ArticuloId { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public string? Comentario { get; set; }
    }
}
