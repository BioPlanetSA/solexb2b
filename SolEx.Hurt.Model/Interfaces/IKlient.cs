using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IKlient: IKlienci
    {     
        [Ignore]
        IObrazek Obrazek { get; }
        [Ignore]
        int[] Kategorie { get; set; }

        [Ignore]
        IKlient KlientNadrzedny { get; }

        [Ignore]
        HashSet<long> MojKatalog { get; }

        [Ignore]
        IList<IAdres> Adresy { get; }

        [Ignore]
        IAdres DomyslnyAdres { get; }


        [Ignore]
        IKlient Opiekun { get; }


        [Ignore]
        IKlient Przedstawiciel { get; }

        [Ignore]
        IKlient DrugiOpiekun { get; }

        [Ignore]
        long OddzialDoJakiegoNalezyKlient { get; }

        [Ignore]
        [FriendlyName("Oddzia³")]
        string OddzialDoJakiegoNalezyKlientNazwa { get; }

        [Ignore]
        List<Magazyn> DostepneMagazynyDlaKlienta { get; set; }


        //WartoscLiczbowa KredytPozostaly { get; }


        /// <summary>
        /// jeœli to oddzia³, to ma swoj katalog  z zasobami. jesli nie bedzie to oddzial to wywali blad
        /// </summary>
        string PobierzKatalogZasobowOddzialu();

        /// <summary>
        /// IKlient pierowtny- IKlient nadrzêdny który nie ma ju¿ ojca, lub jeœli IKlient nie ma ojca to ten sam klient
        /// </summary>
        IKlient KlientPodstawowy();
        List<IKlient> Subkonta();
        List<IKlient> WszystkieKontaPodrzedne();
        HashSet<IKlient> WszystkieKontaPowiazane();
        bool CzyKlientPosiadaLimity();
        List<string> JakieElementyMenu { get; set; }
        HashSet<long> IdUlubionych { get; set; }
        HashSet<long> IdInfoODostepnosci { get; set; }
        [Ignore]
        HashSet<Komunikaty> Komunikaty { get; set; }
    }
}