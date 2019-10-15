namespace CMSoftInventario.App.Model
{
    public class Parametro
    {
        public int IdInventario { get; set; } = 0;
        public string AlmacenDescripcion { get; set; } = string.Empty;
        public string CompaniaDescripcion { get; set; } = string.Empty;
        public string NombreInventario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int IdTipo { get; set; } = 0;
        public int IdUbicacion { get; set; } = 0;
        public int IdAsignacionUbicacion { get; set; } = 0;
        public string EstadoUbicacion { get; set; } = string.Empty;
        public string NombreUbicacion { get; set; } = string.Empty;
        public string UltimaLectura { get; set; } = string.Empty;
        public string Reconteo { get; set; } = string.Empty;
        public string DesReconteo { get; set; } = string.Empty;
        public int CantidadReconteo { get; set; } = 0;
        public int SelectedPosition { get; set; } = -1;
    }
}