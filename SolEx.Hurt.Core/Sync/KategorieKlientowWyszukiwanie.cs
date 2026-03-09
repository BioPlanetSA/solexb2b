using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Core.Sync
{
    public class KategorieKlientowWyszukiwanie:BLL.BllBaza<KategorieKlientowWyszukiwanie>
    {
        public IConfigBLL Konfiguracja = null;

        //public virtual bool CzyKlientMaCeche(IKlienci klient, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, string grupa)
        //{
        //    //string separatorGrupy = SyncManager.PobierzInstancje.Konfiguracja.SeparatorGrupKlientow;
        //    //string cechaAuto = SyncManager.PobierzInstancje.Konfiguracja.CechaAuto;

        //    return PobierzKategorieKlienta(klient, kategorie, laczniki, grupa) != null;
        //}

        /// <summary>
        /// Pobiera kategorie klienta wg grupy - wykorzystywane w modułach dodatkowych
        /// </summary>
        /// <param name="klient">Obiekt klienta dla którego są pobierane kategorie</param>
        /// <param name="kategorieklientow">Lista wszystkich kategorii z B2B lub lista kategorii klienta jeśli parametr laczniki = null</param>
        /// <param name="laczniki">Łączniki do kategorii klientów z B2B. Null jeśli parametr kategorieklientow ma kategorie danego klienta a nie wszystkie z B2B</param>
        /// <param name="grupa">Szukana grupa</param>
        /// <param name="cechaAuto"></param>
        /// <param name="dokladneSzukanie">Czy kategoria ma być dokłanie wpisana, czy zaczynać się od</param>
        /// <param name="separator"></param>
        /// <returns></returns>
        //public KategoriaKlienta PobierzKategorieKlienta(IKlienci klient, IList<KategoriaKlienta> kategorieklientow, List<KlientKategoriaKlienta> laczniki, string grupa, bool dokladneSzukanie = false)
        //{
        //    List<KategoriaKlienta> listakategorii = PobierzWszystkieKategorieKlienta(klient, kategorieklientow, laczniki, grupa, dokladneSzukanie);

        //    if (listakategorii != null)
        //    {
        //        return listakategorii.FirstOrDefault();
        //    }

        //    return null;
        //}

        /// <summary>
        /// metoda filtruje kategorie klientow w oparciu o ciag wejściowy - ktory moze byc kawalkiem grupy lub pełnej cechy (z grupą)
        /// </summary>
        /// <param name="kategorieklientow"></param>
        /// <param name="ciagGrupaLubCecha"></param>
        /// <returns></returns>
        public Dictionary<int, KategoriaKlienta> FiltrujKategorieWgGrupyLubCechy(IList<KategoriaKlienta> kategorieklientow, string ciagGrupaLubCecha, bool dokladneSzukanie = false)
        {
            List<KategoriaKlienta> wynik;
            List<KategoriaKlienta> kategorieKlientowNieNullowaGrupa = kategorieklientow.Where(x => !string.IsNullOrEmpty(x.Grupa)).ToList();

            string[] ciag = ciagGrupaLubCecha.Trim().Split(Konfiguracja.SeparatorGrupKlientow, StringSplitOptions.RemoveEmptyEntries);

            if (ciag.Length > 2)
            {
                throw new Exception($"Błąd wyszukiwania w kategoriach klientów wg. frazy: {ciagGrupaLubCecha}. Fraza zawiera w sobie więcej niż jedno wystąpienie syperatorów kategorii (wg. ustawienia): {Konfiguracja.SeparatorGrupKlientow.ToCsv()}");
            }

            //jesli jest tylko jedna fraza - czyli nie bylo splitowania, to znaczy ze szukamy TYLKO poczatku grupy
            if (ciag.Length == 1)
            {
                if (dokladneSzukanie)
                {
                    wynik = kategorieKlientowNieNullowaGrupa.Where(x => x.Grupa.Equals(ciag[0], StringComparison.InvariantCultureIgnoreCase)).AsParallel().ToList();
                }
                else
                {
                    wynik = kategorieKlientowNieNullowaGrupa.Where(x => x.Grupa.StartsWith(ciag[0], StringComparison.InvariantCultureIgnoreCase)).AsParallel().ToList();
                }
            }
            else
            {
                if (dokladneSzukanie)
                {
                    wynik = kategorieKlientowNieNullowaGrupa.Where(x => x.Grupa.Equals(ciag[0], StringComparison.InvariantCultureIgnoreCase) && x.Nazwa.Equals(ciag[1], StringComparison.InvariantCultureIgnoreCase)).AsParallel().ToList();
                }
                else
                {
                    wynik = kategorieKlientowNieNullowaGrupa.Where(x => x.Grupa.Equals(ciag[0], StringComparison.InvariantCultureIgnoreCase) && x.Nazwa.StartsWith(ciag[1], StringComparison.InvariantCultureIgnoreCase)).AsParallel().ToList();
                }
            }

            if (Log.IsDebugEnabled && wynik.IsEmpty())
            {
                Log.Debug($"Filtracja kategorii klientów dała brak wyników. Szukanie wg. parametrów: ilość{ciag.Length} wartości: [{ciag.Join(",")}], separatory: [{Konfiguracja.SeparatorGrupKlientow.Join(",")}], dokladne szukanie: {dokladneSzukanie}, wszystkich kategorii nie nullowych: {kategorieKlientowNieNullowaGrupa.Count}");
            }

            return wynik.ToDictionary(x => x.Id, x => x);
        }


    }
}
