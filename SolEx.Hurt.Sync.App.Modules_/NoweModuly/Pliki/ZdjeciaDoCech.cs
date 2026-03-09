using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Helpers;

using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    [FriendlyName("Zdjecia do cech", FriendlyOpis = "Moduł daje możliwość przypisania zdjęć z podanego katalogu do cechy produktu. Należy sprawdzić czy \"KtorePolaSynchronizowacCechy\" ma wyłączona cechę synchronizacji, ponadto należy określić w jaki sposób mapowana ma być cecha")]
    public class ZdjeciaDoCech : SyncModul, IModulPliki,ITestowalna, IModulCechyIAtrybuty
    {
        public ZdjeciaDoCech()
        {
            Metka = "<img src=\"/zasoby/import/{0}?preset=metkaCechy\" alt=\"{1}\"/>";
            KomuPrzypisacZdjecie=KomuPrzypisywac.PierwzemuZnalezionemu;
            SposobDopasowania = SposobDopasowania.JedenDoJednego;
            
        }

        //public override string Opis
        //{
        //    get
        //    {
        //        return "Moduł ZdjeciaDoCech daje możliwość przypisania zdjęć z podanego katalogu do cechy produktu. Należy sprawdzić czy \"KtorePolaSynchronizowacCechy\" ma wyłączona cechę synchronizacji, ponadto należy określić w jaki sposób mapowana ma być cecha";
        //    }
        //}
        public override string uwagi
        {
            get { return "Zdjęcia do cech. Trzeba włączyć KtorePolaSynchronizowacCechy"; }
        }

        [FriendlyName("Pole wg ktorego mapujemy cechy")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Cecha))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Sposób dopasowania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobDopasowania SposobDopasowania { get; set; }


        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        
        [FriendlyName("Jakie pole cechy wypełniać")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KtorePoleCechyWypelniac JakiePole { get; set; }
        
        [FriendlyName("Format metki, za {0} sciezka do zdjecia, za {1} nazwa cechy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Metka { get; set; }
        
        [FriendlyName("Komu przypisywac zdjecie pasujące zdjecie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KomuPrzypisywac KomuPrzypisacZdjecie{ get; set; }

        public void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            LogiFormatki.PobierzInstancje.LogujInfo("Rozpoczynam działanie modułu: {0} ", this.Nazwa);
            int iloscZmian = PlikiHelper.PrzetworzPlikiDlaTypu(Sciezka, plikiLokalne, Pole, WypelnijCeche, cechyB2B,true,KomuPrzypisacZdjecie,SposobDopasowania);
            LogiFormatki.PobierzInstancje.LogujInfo("Dopasowano {0} zdjęć do cech", iloscZmian);
        }

        private bool WypelnijCeche(object c,Plik p)
        {
            Cecha zmienianaCecha = (Cecha) c;
            switch (JakiePole)
            {
                    case KtorePoleCechyWypelniac.Metka:
                    zmienianaCecha.MetkaOpis = string.Format(Metka, p.Nazwa[0] + "/" + p.Nazwa, zmienianaCecha.Nazwa);
                    break;
                    case KtorePoleCechyWypelniac.MetkaKatalog:
                    zmienianaCecha.MetkaKatalog = string.Format(Metka, p.Nazwa[0] + "/" + p.Nazwa, zmienianaCecha.Nazwa);
                    break;
                    case KtorePoleCechyWypelniac.ObrazekID:
                    zmienianaCecha.ObrazekId = p.Id;
                    break;
            }
            LogiFormatki.PobierzInstancje.LogujDebug("Id pliku: {0}, id cechy:{1}",p.Id, zmienianaCecha.Id);
            return true;
        }


        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            CechyBll c = new CechyBll(null);
            bool czySynch = false;
            //switch (JakiePole)
            //{
            //    case KtorePoleCechyWypelniac.Metka:
            //        czySynch = SyncManager.PobierzInstancje.Konfiguracja.CzyPoleJestSynchronizowane(typeof(CechyBll), "metka_opis");
            //        break;
            //    case KtorePoleCechyWypelniac.MetkaKatalog:
            //        czySynch = SyncManager.PobierzInstancje.Konfiguracja.CzyPoleJestSynchronizowane(typeof(CechyBll), "metka_katalog");
            //        break;
            //    case KtorePoleCechyWypelniac.ObrazekID:
            //        czySynch = SyncManager.PobierzInstancje.Konfiguracja.CzyPoleJestSynchronizowane(typeof(CechyBll), "obrazek_id");
            //        break;
            //}

            if(czySynch)
            {
                listaBledow.Add(string.Format("Pole {0} - do którego jest wpisana scieżka do zdjęcia powinien być wyłączony z synchronizacji", JakiePole));
            }
            return listaBledow;
        }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            var pliki = ApiWywolanie.PlikNaB2BPobierz();
            List<ProduktPlik> plikiLokalnePowiazania= new List<ProduktPlik>();
            List<KategoriaProduktu> kategorieB2B = new List<KategoriaProduktu>();
            List<Klient> klienciB2B = new List<Klient>();
            Przetworz(produktyNaB2B, ref plikiLokalnePowiazania, ref pliki, null, ref cechy, ref kategorieB2B, ref klienciB2B);
        }
    }
}
