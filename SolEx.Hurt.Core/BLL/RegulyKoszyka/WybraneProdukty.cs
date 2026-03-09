using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class WybraneProdukty : RegulaKoszyka, IRegulaPozycji, IRegulaCalegoKoszyka, ITestowalna, IWarunekRegulyPozycjiDokumentu
    {
        public override string Opis
        {
            get { return "Czy jest określony produkt (jakikolwiek z listy podanych). Opcjonalnie można podać minimalną wymaganą ilość"; }
        }

        [FriendlyName("Czy produkt jest/ nie jest określony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RelacjaJestNieJest Relacja { get; set; }

        [FriendlyName("Produkty")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> IdProduktow { get; set; }

        [FriendlyName("Wymagana minimalna ilość każdego produktu w jednostce podstawowej")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int WymaganaIlosci { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (WymaganaIlosci < 0)
            {
                listaBledow.Add("Wymagana minimalna ilość jest mniejsza od 0");
            }
            return listaBledow;
        }

        public WybraneProdukty()
        {
            WymaganaIlosci = 0;
            Relacja = RelacjaJestNieJest.Jest;
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Wynik(pozycja.ProduktId, pozycja.IloscWJednostcePodstawowej);
        }

        private bool Wynik(long produkt, decimal ilosc)
        {
            bool wynik = false;
            if (WymaganaIlosci > 0 && ilosc < WymaganaIlosci)
            {
                return false;
            }

            if (IdProduktow.Count() != 0)
            {
                if (IdProduktow.Contains(produkt.ToString(CultureInfo.InvariantCulture)))
                {
                    wynik = true;
                }
            }

            return Relacja == RelacjaJestNieJest.NieJest ? !wynik : wynik;
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Any(x=>PozycjaSpelniaRegule(x,koszyk));
        }

        public bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument)
        {
            
                return Wynik(pozycja.ProduktId, pozycja.PozycjaDokumentuIlosc);
           
            return false;
        }
    }
}