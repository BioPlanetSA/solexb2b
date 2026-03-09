using System;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class RodzinyOryginalyIKopie : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty, Model.Interfaces.SyncModuly.IModulCechyIAtrybuty
    {
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;
        public RodzinyOryginalyIKopie()
        {
            NazwaDlaOryginalu = "Oryginał";
            NazwaDlaKopii = "Kopia";
            NazwaAtrybutu = "kopiaczyoryginal";
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public override string Opis
        {
            get { return "Moduł, który automatycznie tworzy cechę kopiaczyoryginal:Oryginał dla produktów w rodzinie których nazwa jest taka sama jak nazwa rodziny i cechę kopiaczyoryginal:Kopia dla produktów w rodzinie których nazwa jest inna niż nazwa rodziny"; }
        }

        [FriendlyName("Nazwa cechy dla oryginału produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaDlaOryginalu { get; set; }

        [FriendlyName("Nazwa cechy dla kopii produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaDlaKopii { get; set; }

        [FriendlyName("Nazwa atrybutu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaAtrybutu { get; set; }

        public Atrybut WygenerujAtrybut(string nazwaAtrybutu)
        {
            Atrybut nowyAtrybut = new Atrybut();
            nowyAtrybut.Nazwa = nazwaAtrybutu;
            nowyAtrybut.Widoczny = true;
            nowyAtrybut.ProviderWyswietlania = Config.TypDomyslnyFiltru;
            nowyAtrybut.Id = nowyAtrybut.WygenerujIDObiektuSHA(1);
            return nowyAtrybut;
        }

        public Cecha WygenerujCeche(string nazwa, Atrybut atrybut)
        {
            Cecha nowaCecha = new Cecha();
            nowaCecha.Nazwa = nazwa;
            nowaCecha.AtrybutId = atrybut.Id;
            nowaCecha.Symbol = string.Format("{0}:{1}", atrybut.Nazwa, nazwa).ToLower();
            nowaCecha.Widoczna = true;
            nowaCecha.Id = nowaCecha.WygenerujIDObiektuSHA(1);

            return nowaCecha;
        }

        public ProduktCecha WygenerujLacznikCech(Cecha cecha, Produkt produkt)
        {
            ProduktCecha lacznik = new ProduktCecha();
            lacznik.CechaId = cecha.Id;
            lacznik.ProduktId = produkt.Id;
            return lacznik;
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Atrybut atrybut = WygenerujAtrybut(NazwaAtrybutu);
            Cecha cechaOryginal = WygenerujCeche(NazwaDlaOryginalu, atrybut);
            Cecha cechaKopia = WygenerujCeche(NazwaDlaKopii, atrybut);

            foreach (Produkt produkt in listaWejsciowa)
            {
                if (!string.IsNullOrEmpty(produkt.Rodzina))
                {
                    if (produkt.Nazwa == produkt.Rodzina)
                    {
                        ProduktCecha lacznikOryginalu = WygenerujLacznikCech(cechaOryginal, produkt);
                        if (!lacznikiCech.ContainsKey(lacznikOryginalu.Id))
                            lacznikiCech.Add(lacznikOryginalu.Id, lacznikOryginalu);
                    }
                    else
                    {
                        ProduktCecha lacznikKopii = WygenerujLacznikCech(cechaKopia, produkt);
                        if (!lacznikiCech.ContainsKey(lacznikKopii.Id))
                            lacznikiCech.Add(lacznikKopii.Id, lacznikKopii);
                    }
                }
            }
        }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Atrybut atrybut = WygenerujAtrybut(NazwaAtrybutu);
            Atrybut nowyAtrybut = atrybuty.FirstOrDefault(a => a.Nazwa == atrybut.Nazwa);
            if (nowyAtrybut == null)
            {
                atrybuty.Add(atrybut);
            }
            else nowyAtrybut.KopiujPola(atrybut);


            Cecha cechaOryginal = WygenerujCeche(NazwaDlaOryginalu, atrybut);
            Cecha cechaKopia = WygenerujCeche(NazwaDlaKopii, atrybut);

            Cecha aktualnaCechaOryginal = cechy.FirstOrDefault(a => a.Symbol == cechaOryginal.Symbol);
            if (aktualnaCechaOryginal == null)
            {
                cechy.Add(cechaOryginal);
            }
            else aktualnaCechaOryginal.KopiujPola(cechaOryginal);

            Cecha aktualnaCechaKopia = cechy.FirstOrDefault(a => a.Symbol == cechaKopia.Symbol);
            if (aktualnaCechaKopia == null)
            {
                cechy.Add(cechaKopia);
            }
            else aktualnaCechaKopia.KopiujPola(cechaKopia);
        }
    }
}
