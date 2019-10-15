namespace CMSoftInventario.App.Model
{
    public class ItemReconteo
    {
        public int IdAlmacen { get; set; } = 0;
        public int IdItem { get; set; } = 0;
        public int IdInventario { get; set; } = 0;
        public string Descripcion { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
    }
}