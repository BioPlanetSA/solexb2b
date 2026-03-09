using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.OperacjeZbiorcze;
using System;
using System.Collections.Generic;
using System.Reflection;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class DaneLista
    {
        private readonly IList<OpisPolaObiektuBaza> _kolumny;
        private readonly IList<DaneObiekt> _obiektyDoPokazania;
        private int _nrStrony, _rozmiarStrony;
        private readonly long _lacznie;
        private readonly Type _typ;
        private readonly string[] _szukanie;
        private readonly string _sortowanie;
        private readonly KolejnoscSortowania _kierunek;
        //private readonly IKlient _klient;
        private readonly IList<OperacjaZbiorczaBaza> _modul;
        private string _akcja = "Lista";
        private string _akcjaDodaj = "Dodaj";
        /// <summary>
        /// Tworzy nową instancję obiektu DaneLista
        /// </summary>
        /// <param name="typ">Typ danych</param>
        /// <param name="kolumnyWidoczne">Lista widocznych kolumn</param>
        /// <param name="obiekty">Obiekty do pokazania</param>
        /// <param name="numerStrony">Aktualny numer strony</param>
        /// <param name="rozmiarStrony">Aktualnyt rozmiar strony</param>
        /// <param name="lacznie">Lacznie obiektów</param>
        /// <param name="szukanie">Szukane frazy wg kolumn, tablica ma mieć tyle elementów ile widocznych kolumn</param>
        /// <param name="sortowanie">nazwa pola wg którego aktualnie posortowane są obiekty</param>
        /// <param name="kierunek">Kierunek sortowania</param>
        /// <param name="modul">Dostepne operacje zbiorcze</param>
        /// <param name="zadajacy">Klient</param>
        public DaneLista(Type typ, IList<OpisPolaObiektuBaza> kolumnyWidoczne, IList<DaneObiekt> obiekty, int numerStrony, int rozmiarStrony, long lacznie, string[] szukanie, string sortowanie, KolejnoscSortowania kierunek, IKlient zadajacy, IList<OperacjaZbiorczaBaza> modul, bool czyMoznaDodawacNowe, List<Komunikat> komunikaty)
        {
            _kolumny = kolumnyWidoczne;
            _obiektyDoPokazania = obiekty;
            NrStrony = numerStrony;
            RozmiarStrony = rozmiarStrony;
            _lacznie = lacznie;
            _typ = typ;
            _szukanie = szukanie;
            _sortowanie = sortowanie;
            _kierunek = kierunek;
            _modul = modul;
            MoznaDodawacNowe = czyMoznaDodawacNowe;
            Komunikaty = komunikaty;
            //    _klient = zadajacy;
        }

        public bool MoznaDodawacNowe { get; private set; }

        public IList<string> Szukanie
        {
            get { return _szukanie; }
        }

        public IList<OpisPolaObiektuBaza> KolumnyWidoczne
        {
            get { return _kolumny; }
        }

        public IEnumerable<DaneObiekt> ObiektyDoPokazania
        {
            get { return _obiektyDoPokazania; }
        }

        public int NrStrony
        {
            get { return _nrStrony; }
            set
            {
                if (value <= 0)
                {
                    throw new InvalidOperationException("Numer strony musi być większy od 0");
                }
                _nrStrony = value;
            }
        }

        public int RozmiarStrony
        {
            get { return _rozmiarStrony; }
            set
            {
                if (value <= 0)
                {
                    throw new InvalidOperationException("Rozmiar strony musi być większy od 0");
                }
                _rozmiarStrony = value;
            }
        }

        public long Lacznie
        {
            get { return _lacznie; }
        }

        public Type Typ
        {
            get { return _typ; }
        }

        public string Sortowanie
        {
            get { return _sortowanie; }
        }

        public KolejnoscSortowania Kierunek
        {
            get { return _kierunek; }
        }

        public IList<OperacjaZbiorczaBaza> ModulyZbiorcze
        {
            get { return _modul; }
        }

        public string Akcja
        {
            get { return _akcja; }
            set { _akcja = value; }
        }

        public string AkcjaDodaj
        {
            get { return _akcjaDodaj; }
            set { _akcjaDodaj = value; }
        }
        

        public string NazwaObiektu
        {
            get
            {
               FriendlyNameAttribute opisy = _typ.GetCustomAttribute<FriendlyNameAttribute>();
               return (opisy != null) ? opisy.FriendlyName : _typ.Name;
            }
        }

        public string OpisObiektu
        {
            get
            {
                FriendlyNameAttribute opisy = _typ.GetCustomAttribute<FriendlyNameAttribute>();
                return (opisy != null) ? opisy.FriendlyOpis : "";
            }
        }

        public List<Komunikat> Komunikaty { get; set; }
    }
}