namespace CMSoftInventario.App.Model
{
    public class Item
    {
        public int idUsuario { get; set; } = 0;
        public int idEquipo { get; set; } = 0;
        public int idInventario { get; set; } = 0;
        public int idUbicacion { get; set; } = 0;
        public int idAsignacionUbicacion { get; set; } = 0;
        public string CodBarraItemUnitario { get; set; } = string.Empty;
        public string Es_Automatico { get; set; } = string.Empty;
        public decimal Cantidad_Tomada { get; set; } = 0;
    }
}