namespace CMSoftInventario.App.Model
{
    public class Kit
    {
        public int idUsuario { get; set; } = 0;
        public int idEquipo { get; set; } = 0;
        public int idInventario { get; set; } = 0;
        public int idUbicacion { get; set; } = 0;
        public int idAsignacionUbicacion { get; set; } = 0;
        public string Codigo_Caja { get; set; } = string.Empty;
        public string Codigo_Subordinado { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Es_Automatico { get; set; } = string.Empty;
        public decimal Cantidad_Tomada { get; set; } = 0;
    }
}