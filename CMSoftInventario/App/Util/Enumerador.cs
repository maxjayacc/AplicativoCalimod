namespace CMSoftInventario.App.Util
{
    public static class Enumerador
    {
        public enum eConsulta : short
        {
            Item = 1,
            Kit = 2
        }

        public enum eLectura : short
        {
            Ubicacion = 1,
            Item = 2,
            Kit = 3
        }
    }
}