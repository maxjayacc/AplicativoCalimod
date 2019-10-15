namespace CMSoftInventario.App.Model
{
    public class Equipo
    {
        public int IdEquipo { get; set; } = 0;
        public int FlagLectura { get; set; } = 0;
        public string IpMiddleWare { get; set; } = string.Empty;
        public string IpServicio { get; set; } = string.Empty;
        public string PathMiddleWare { get; set; } = string.Empty;
        public string PathServicio { get; set; } = string.Empty;
        public int PuertoMiddleWare { get; set; } = 0;
        public int PuertoServicio { get; set; } = 0;
    }
}