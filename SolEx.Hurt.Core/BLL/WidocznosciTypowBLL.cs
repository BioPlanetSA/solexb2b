using System;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;

namespace SolEx.Hurt.Core.BLL
{
    public class WidocznosciTypowBLL : LogikaBiznesBaza, IWidocznosciTypowBLL
    {
        public WidocznosciTypowBLL(ISolexBllCalosc calosc)
            : base(calosc)
        {
        }

        /// <summary>
        /// metoda NIE powinna być uruchamiana samodzielnie - obiekty automatycznie sa sprawdzane przy pobiernaiu danych
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="obiekt"></param>
        /// <returns></returns>
        public bool KlientMaDostepDoObiektu(IKlient klient, IObiektWidocznyDlaOkreslonychGrupKlientow obiekt)
        {
            return KlientMaDostepDoObiektuWylicz(klient, obiekt.Widocznosc);
        }

        public bool KlientMaDostepDoObiektuWylicz(IKlient klient, WidocznosciTypow wiodcznosc)
        {
            if (klient.Dostep == AccesLevel.Niezalogowani)
            {
                throw new Exception("Błąd - nie wolno sprawdzać widoczności dla klienta nie zalogowanego");
            }

            if (klient.CzyAdministrator)
            {
                throw new Exception("Błąd - nie wolno sprawdzać widoczności dla administratora");
            }

            if (wiodcznosc == null)
            {
                return true;
            }
            bool pasujeKlient = KlientSpelniaWarunek(klient, wiodcznosc);
            if (wiodcznosc.Kierunek == WidocznoscTypoKierunek.BrakDostepuDlaKlientowZKategorii)
            {
                return !pasujeKlient;
            }
            return pasujeKlient;
        }

        private bool KlientSpelniaWarunek(IKlient klient, WidocznosciTypow wiodcznosc)
        {
            if (wiodcznosc == null)
            {
                return true;//brak wpisu czyli widzimy
            }
            bool spelniaarunekWszystkie = WarunekWszystkie(klient, wiodcznosc.KategoriaKlientaIdWszystkie);
            bool spelniaarunekKtorekolwiek = WarunekKtorekolwiek(klient, wiodcznosc.KategoriaKlientaIdKtorakolwiek);
            return spelniaarunekWszystkie || spelniaarunekKtorekolwiek;
        }

        public IList<IKlient> PobierzKlientowSprelniajacychWarunkiSzablonu(WidocznosciTypow widocznosc)
        {
            List<IKlient> wynik = new List<IKlient>();
            var klienci = Calosc.DostepDane.Pobierz<Klient>(null, x => x.Aktywny);
            foreach (var k in klienci)
            {
                if (KlientSpelniaWarunek(k, widocznosc))
                {
                    wynik.Add(k);
                }
            }
            return wynik;
        }

        private bool WarunekWszystkie(IKlient klient, int[] kategorie)
        {
            if (kategorie == null)
            {
                return true;
            }
            foreach (int k in kategorie)
            {
                if (!klient.Kategorie.Contains(k))
                {
                    return false;
                }
            }
            return true;
        }

        private bool WarunekKtorekolwiek(IKlient klient, int[] kategorie)
        {
            if (kategorie == null)
            {
                return true;
            }
            foreach (int k in kategorie)
            {
                if (klient.Kategorie.Contains(k))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Pobiera listę zdefiniowanych szablonów widoczności
        /// </summary>
        /// <returns></returns>
        public IList<WidocznosciTypow> PobierzSzablony()
        {
            return Calosc.DostepDane.Pobierz<WidocznosciTypow>(null, x => x.Nazwa != null && x.ObiektId == 0);
        }
    }
}