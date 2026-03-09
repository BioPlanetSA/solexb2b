using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ProduktyZCechaPozycje : RegulaKoszyka, IRegulaPozycji, ITestowalna, IWarunekRegulyPozycjiDokumentu
    {
        public override string Opis
        {
            get { return "Czy dany produkt spełnia kryterium cechy"; }
        }

        [FriendlyName("Symbole cechy")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Cecha { get; set; }

        [FriendlyName("Cechy w produkcie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public CechyJakie Kierunek { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (Cecha.Any())
            {
                List<CechyBll> listaWszystkich = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Values.ToList();
                foreach (var symbol in Cecha)
                {
                    int idCechy;
                    CechyBll cecha = Int32.TryParse(symbol, out idCechy) ? SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski)[idCechy]
                        : SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(symbol, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                    if (cecha == null || !listaWszystkich.Contains(cecha))
                    {
                        listaBledow.Add(string.Format("Brak cechy o nazwie: {0}", symbol));
                    }
                }
            }
            return listaBledow;
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Wynik(pozycja.Produkt);
        }

        private bool Wynik(IProduktBazowy produkt)
        {
            //string []cechy = Cecha.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            bool wynik = false;
            foreach (var c in Cecha)
            {
                int idCechy;
                if (Int32.TryParse(c, out idCechy))
                {
                    if (produkt.Cechy.Any(a => a.Value.Id == idCechy))
                    {
                        wynik = true;
                    }
                }
                else
                {
                    if (produkt.Cechy.Any(a => a.Value.Symbol.Equals(c, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        wynik = true;
                    }
                }
            }
            return Kierunek == CechyJakie.Ktoraklwiek ? wynik : !wynik;
        }

        public bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument)
        {
            var produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(pozycja.ProduktId);
            if (produkt == null)
            {
                return false;
            }
            return Wynik(produkt);
        }
    }

    public enum CechyJakie
    {
        Ktoraklwiek, Zadna
    }
}