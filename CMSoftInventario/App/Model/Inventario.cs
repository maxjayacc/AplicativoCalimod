namespace CMSoftInventario.App.Model
{
    public class Inventario
    {
        public int IdInventario { get; set; } = 0;
        public string AlmacenDescripcion { get; set; } = string.Empty;
        public string CompaniaDescripcion { get; set; } = string.Empty;
        public bool Es_Item_Lote { get; set; } = false;
        public string EstadoCodigo { get; set; } = string.Empty;
        public string EstadoDescripcion { get; set; } = string.Empty;
        public int IdLecturaTipo { get; set; } = 0;
        public int IdTipo { get; set; } = 0;
        public string LecturaTipo { get; set; } = string.Empty;
        public string NombreInventario { get; set; } = string.Empty;
    }
}