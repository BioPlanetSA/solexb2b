namespace SolEx.Hurt.Core.ModelBLL
{
    public class OpisProduktuGratisowego
    {

        public OpisProduktuGratisowego() { }
        public OpisProduktuGratisowego(long produkt,IFlatCenyBLL cena)
        {
            IdProduktu = produkt;
            Cena = cena;
        }
        public long IdProduktu { get; set; }
        public IFlatCenyBLL Cena { get; set; }
    }
}
