using System.Globalization;
using System.Reflection; 
using System.Text.RegularExpressions;
using CsvHelper;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;using System.Threading.Tasks;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ImportEksportXMLCSV
{
    public class ImportCSVImpelProdukty : Model.SyncModul, SolEx.Hurt.Model.Interfaces.SyncModuly.IModulEksportImportXMLCSV
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ImportCSVImpelProdukty()
        {
            Sciezka = "";
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Ścieżka do pliku CSV z towarami")]
        public string Sciezka { get; set; }

        public override string Opis
        {
            get { return "Importuje towary z pliku CSV Impela."; }
        }

        #region kolumny produktu
        private int kKod = 0;
        private int kNazwa = 1;
        private int kJednostka = 3;
        private int kProducent = 5;
        private int kKodProducenta = 6;
        #endregion

        #region ceny
        private int kVAT = 15;
        private int kCena = 14;
        #endregion

        private int kKategoria = 11;



        public void Przetworz()
        {
            log.Debug("uruchamianie modułu ImportCSVImpelProdukty");
            if (!File.Exists(Sciezka))
            {
                log.Error("Nie znaleziono pliku CSV w podanej ścieżce");
                return;
            }
            CsvReader r = new CsvReader(new StreamReader(Sciezka));
            r.Configuration.Delimiter = ";";
            r.Configuration.Encoding = Encoding.UTF8;

            List<produkty> produkty = new List<produkty>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ceny_poziomy> ceny = new List<ceny_poziomy>();

            List<poziomy_cen> poziomycen = new List<poziomy_cen>();

            
            poziomy_cen pc = new poziomy_cen();
            r.Read();

            string poziomCeny = r.GetField(kCena);
            pc.nazwa = poziomCeny;
            pc.waluta = "PLN";
            pc.id = poziomCeny.WygenerujIDObiektu(true);
            poziomycen.Add(pc);

            APIWywolania.AktualizujPoziomyCen(poziomycen);

            while (r.Read())
            {
                string kod = r.GetField(kKod);
                string nazwa = r.GetField(kNazwa);
                string jednostka = r.GetField(kJednostka);
                string producent = r.GetField(kProducent);
                string svat = r.GetField(kVAT).Replace(",", ".");
                decimal vat = 0;
                decimal.TryParse(svat.Replace("%", "").Trim(),
                                          System.Globalization.NumberStyles.Float,
                                          System.Globalization.CultureInfo.InvariantCulture, out vat);

               // string kodproducenta = r.GetField(kKodProducenta);

                produkty nowyProdukt = new produkty();
                nowyProdukt.kod = kod;
                nowyProdukt.nazwa = nazwa;
              //  nowyProdukt.marka_nazwa = producent;
                nowyProdukt.vat = vat;
                nowyProdukt.produkt_id = nowyProdukt.WygenerujIDObiektu(true);

                if(produkty.All(a => a.produkt_id != nowyProdukt.produkt_id))
                    produkty.Add(nowyProdukt);

                JednostkaProduktu pj = new JednostkaProduktu();
                pj.JednostkaId = jednostka.WygenerujIDObiektu();
                pj.ProduktId = nowyProdukt.produkt_id;
                pj.Lacznik.PrzelicznikIlosc = 1;
                pj.Nazwa = jednostka;
                jednostki.Add(pj);

                ceny_poziomy cena = new ceny_poziomy();
                string scenabrutto = r.GetField(kCena).Replace(",", ".");
                if (!string.IsNullOrEmpty(scenabrutto))
                {
                    decimal cenabrutto = decimal.Parse(scenabrutto, NumberStyles.AllowDecimalPoint);
                    // r.GetField<decimal>(kCena);

                    cena.poziom_id = pc.id;
                    cena.netto = cenabrutto;
                    cena.brutto = cenabrutto*(1 + (vat/100));
                    cena.produkt_id = nowyProdukt.produkt_id;

                    ceny.Add(cena);
                }
            }

            APIWywolania.AktualizujProdukty(produkty);
            APIWywolania.AktualizujPoziomyCenProduktow(ceny);

            var jednostkiCriteria = new JednostkiSearchCriteria();jednostkiCriteria.AddtionalSQL = " JednostkaId>0";
            //List<Jednostki> jednostkiB2B = APIWywolania.PobierzJednostki(jednostkiCriteria);
            //var produktyJEdnostkiCriteria = new JednostkiProduktyCriteria();

            //List<Jednostki> doAkualizacji = new List<Jednostki>();
            //List<ProduktyJednostki> jednostkiLacznikiDoAktualizacji = new List<ProduktyJednostki>();

            //produktyJEdnostkiCriteria.AddtionalSQL = " JednostkaId>0 and ProduktID>0";
          //  List<ProduktyJednostki> produktyJednoskiB2B =
          //      APIWywolania.PobierzProduktyJednostki(produktyJEdnostkiCriteria);
          //  foreach (JednostkaProduktu j in jednostki)
          //  {
          //      var tmp = new Jednostki(j);
          //      var jednostka = jednostkiB2B.FirstOrDefault(p => p.JednostkaId == tmp.JednostkaId);
          //      if (jednostka == null)
          //      {
          //          doAkualizacji.Add(tmp);
          //      }
          //      else if (jednostka != null)
          //      {
          //          tmp.Calkowitoliczowa = jednostka.Calkowitoliczowa;
          //          if (!SyncTools.PorownajObiekty(jednostka, tmp))
          //          {
          //              doAkualizacji.Add(tmp);
          //          }
          //      }
          //      ProduktyJednostki lacznikTmp = j.Lacznik;

          //      var lacznik =
          //          produktyJednoskiB2B.FirstOrDefault(
          //              p => p.JednostkaId == lacznikTmp.JednostkaId && p.ProduktId == lacznikTmp.ProduktId);
          //      if (lacznik == null)
          //      {
          //          jednostkiLacznikiDoAktualizacji.Add(lacznikTmp);
          //      }
          //      else if (lacznik != null)
          //      {
          //          if (!SyncTools.PorownajObiekty(lacznik, lacznikTmp))
          //          {
          //              jednostkiLacznikiDoAktualizacji.Add(lacznikTmp);
          //          }
          //      }

          //  }
          //  var jednostkiPojedyczne = new List<Jednostki>();
          //  foreach (var jednostki1 in doAkualizacji)
          //  {
          //      if (!jednostkiPojedyczne.Any(p => p.JednostkaId == jednostki1.JednostkaId))
          //      {
          //          jednostkiPojedyczne.Add(jednostki1);
          //      }
          //  }
          //  APIWywolania.AktualizujJednostki(jednostkiPojedyczne);
          //  APIWywolania.AktualizujProduktyJednostkiJednostki(jednostkiLacznikiDoAktualizacji);
          //  var jednostkiDoUsuniecia = new List<int>();
          //  var jednostkiLacznikiDoUsuniecia = new List<ProduktyJednostki>();
          //  foreach (var c in jednostkiB2B)
          //  {
          //      if (!jednostki.Any(p => p.JednostkaId == c.JednostkaId))
          //      {
          //          jednostkiDoUsuniecia.Add(c.JednostkaId);
          //      }
          //  }
          // // APIWywolania.UsunJednostki(jednostkiDoUsuniecia);
          //  foreach (var pj in produktyJednoskiB2B)
          //  {
          //      if (!jednostki.Any(p => p.JednostkaId == pj.JednostkaId && p.ProduktId == pj.ProduktId))
          //      {
          //          jednostkiLacznikiDoUsuniecia.Add(pj);
          //      }
          //  }
          ////  APIWywolania.UsunJednostkiLaczniki(jednostkiLacznikiDoUsuniecia);
            log.Debug("zakończenie pracy modułu ImportCSVImpelProdukty");
        }
    }
}
