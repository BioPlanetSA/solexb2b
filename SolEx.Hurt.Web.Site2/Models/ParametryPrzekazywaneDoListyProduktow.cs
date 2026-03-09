using System.Collections.Generic;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryPrzekazywaneDoListyProduktow
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        public ParametryPrzekazywaneDoListyProduktow()
        {
            filtry = null;
        }

        private bool? _brakKryteriow = null;

        /// <summary>
        /// Pomocnicze szybkie sprawznie czy sa jakies warunki
        /// </summary>
        public bool BrakKryteriowWyboru
        {
            get
            {
                if (_brakKryteriow == null)
                {
                    _brakKryteriow = !this.kategoria.HasValue && this.filtry == null && string.IsNullOrEmpty(this.szukane);
                }
                return _brakKryteriow.Value;
            }
        }

        public long? kategoria { get; set; }

        public Dictionary<int, HashSet<long>> filtry { get; set; }

        public string szukane { get; set; }
        public string szukanaWewnetrzne { get; set; }
        public int strona { get; set; }

        /// <summary>
        /// pole uzupelniane w binderze autoamtycznie
        /// </summary>
        public KategorieBLL KategoriaObiekt { get; set; }

        public int idKontrolki
        {
            get { return _kontrolka; }
            set
            {
                _kontrolka = value;
                if (_kontrolka != 0)
                {
                    this.Zaladujkontrolke(Calosc.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(idKontrolki).Kontrolka());
                }
            }
        }

        /// <summary>
        /// metoda do ladosania kontrokli
        /// </summary>
        /// <param name="bazowaKontrolka"></param>
        public void Zaladujkontrolke(IKontrolkaTresciBaza bazowaKontrolka)
        {
            KontrolkaProduktow = bazowaKontrolka as ListaProduktowBaza;
            KontrolkaProduktowJakoListaProduktow = bazowaKontrolka as ListaProduktow;
            KontrolkaProduktowJakoLosowaListaProduktow = bazowaKontrolka as LosowaListaProduktowWybraneIdProduktow;
            _kontrolka = bazowaKontrolka.Id;
        }

        private int _kontrolka;

        public ListaProduktowBaza KontrolkaProduktow { get; private set; }

        public ListaProduktow KontrolkaProduktowJakoListaProduktow { get; private set; }

        public LosowaListaProduktowWybraneIdProduktow KontrolkaProduktowJakoLosowaListaProduktow { get; private set; }

        private Dictionary<long,string> _kluczDoCachuFiltrow;

        /// <summary>
        /// Wyliczmy klucz do cachu na podstawie wybranych filtrów, stałych filtrów klienta, szukanych fraz i czy klient ma ofertę indywidualizowaną. 
        /// </summary>
        /// <param name="klient">Klient dla którego pobierane będą stałe filtry</param>
        /// <returns>Wyliczony klucz w postaci string </returns>
        public string KluczDoCachuFiltrow(IKlient klient)
        {
            string wynik;
            if (_kluczDoCachuFiltrow != null &&  _kluczDoCachuFiltrow.TryGetValue(klient.Id, out wynik))
            {
                return wynik;
            }

            _kluczDoCachuFiltrow = _kluczDoCachuFiltrow = new Dictionary<long, string>();
            //wyliczanie klucza do cache
            if (  (this.filtry != null && this.filtry.ContainsKey(Calosc.Konfiguracja.CechaUlubione.AtrybutId.Value)) || Calosc.ProfilKlienta.CzyWStalychFiltrachSaUlubioneWybrane(klient))
            {
                wynik = string.Empty;
            }
            else
            {
                string keyIdCechFiltry = "";
                if (this.filtry != null)
                {
                    List<long> idCech = new List<long>();
                    foreach (var value in this.filtry)
                    {
                        idCech.AddRange(value.Value);
                    }
                    idCech = new List<long>(idCech);
                    idCech.Sort();
                    keyIdCechFiltry = idCech.Join(",");
                }
                string keyIdCechStale = Calosc.ProfilKlienta.PobierzStaleFiltryString(klient);
                wynik = $"Filtry_{keyIdCechFiltry}_{keyIdCechStale}_{kategoria}_{szukane}_{klient.JezykId}";

                if (klient.OfertaIndywidualizowana)
                {
                    wynik += $"_{klient.Id}";
                }
            }
            _kluczDoCachuFiltrow.Add(klient.Id, wynik);
            return wynik;
        }
    }
}