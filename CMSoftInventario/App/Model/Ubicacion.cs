namespace CMSoftInventario.App.Model
{
    public class Ubicacion
    {
        public int IdInventario { get; set; } = 0;
        public int IdUbicacion { get; set; } = 0;
        public int IdUsuario { get; set; } = 0;
        public int IdAsignacionUbicacion { get; set; } = 0;
        public string EstadoCodigo { get; set; } = string.Empty;
        public string EstadoDescripcion { get; set; } = string.Empty;
        public string NombreUbicacion { get; set; } = string.Empty;
        public int Reconteo { get; set; } = 0;
    }
}