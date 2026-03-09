using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface ISyncProvider
    {
        void UstawParametryLaczenia(IConfigSynchro config);
        string SourceCS { get;set; }

        List<Produkt> PobierzProdukty(  out List<Model.Tlumaczenie> slownikiTlumaczen,out List<JednostkaProduktu> jednostki,HashSet<string> magazyny  );
        List<ProduktCecha> PobierzCechyProduktow_Polaczenia( int[] atrybutydlaktorychpobieramycechy);

        List<KategoriaKlienta> PobierzKategorieKlientow();
        List<KlientKategoriaKlienta> PobierzKategorieKlientowPolaczenia();

        Dictionary<long, Klient> PobierzKlientow(List<Klient> klienciNaPlatformie, out Dictionary<Adres, KlientAdres> adresy);
        List<Cecha> PobierzCechyIAtrybuty(out List<Atrybut> atrybuty, int[] atrybutydlaktorychpobieramycechy);

        List<Model.ZamowienieSynchronizacja> ImportZamowien(List<Model.ZamowienieSynchronizacja> zamowienia, Dictionary<long,Klient> wszyscy);
        List<StatusZamowienia> PobierzStatusyDokumentow();
        Dictionary<long, decimal> PobierzStanyDlaMagazynu(string mag);
        string przesunMazazyn(List<ZamowienieProdukt> suBraki, string mag, string docelowy, string nr, string uwagi="");

        bool CleanUp();

        List<CenaPoziomu> PobierzPoziomyCenoweProduktow();

        List<PoziomCenowy> PobierzDostepnePoziomyCen();

        List<Rabat> PobierzRabaty(Dictionary<long, Klient> dlaKogoLiczyc, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, 
            List<Cecha> cechy, List<ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie,
            IDictionary<int, KategoriaKlienta> kategorieKlientow, IDictionary<long, KlientKategoriaKlienta>klienciKategorie);

        List<Kraje> PobierzKraje();

        List<Region> PobierzRegiony();

        List<Magazyn> PobierzMagazynyErp();

        Dictionary<long, Waluta> PobierzDostepneWaluty();
    }

}
