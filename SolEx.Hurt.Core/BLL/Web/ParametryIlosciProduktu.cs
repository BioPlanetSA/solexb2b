using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class ParametryIlosciProduktu: KoszykPozycje
    {
        public DodawanieProduktuPrzyciski Przyciski { get; set; }
        public JednostkaProduktu[] WidoczneJednostki { get; set; }
        public bool PokazywacJednostki { get; set; }

        public ParametryIlosciProduktu()
        {
            TypPozycji = TypPozycjiKoszyka.Zwykly;
        }

        public decimal KrokDodawanie { get; set; }
        public bool Indywidualizowany { get; set; }

        private void WypelnijProdukt(IProduktKlienta produkt, bool odwrotnaKolejnoscJednostek)
        {
            this.Indywidualizowany = false;
            KrokDodawanie = produkt.WymaganeOz ? produkt.IloscWOpakowaniu : 1;
            ProduktId = produkt.Id;

            if (produkt.IndiwidualizacjeProduktu != null)
            {
                this.Indywidualizowany = true;
            }

            if (produkt.Jednostki == null)
            {
                throw new Exception($"Produkt id: {produkt.Id} nie posiada jednostek.");
            }

            List<JednostkaProduktu> aktywnejednostki = new List<JednostkaProduktu>();
            foreach (var j in produkt.Jednostki)
            {
                if (!j.Aktywna)
                {
                    continue;
                }
               
               aktywnejednostki.Add(j);
            }

            if (aktywnejednostki.IsEmpty())
            {
                string komunikat = string.Format("Brak aktywnych jednostek dla produktu id: {0}", produkt.Id);
                SolexBllCalosc.PobierzInstancje.Log.ErrorFormat(komunikat);
                throw new Exception(komunikat);
            }

            if (odwrotnaKolejnoscJednostek)
            {
                aktywnejednostki.Reverse();
            }
            //krok dla jednoski w tym miejscu juz jest niepotrzebny 
            //WidoczneJednostki = SolexBllCalosc.PobierzInstancje.Koszyk.UzupelnijJednostkiOKrokDodania(aktywnejednostki, produkt);
            WidoczneJednostki = aktywnejednostki.ToArray();
            if (JednostkaId.HasValue && WidoczneJednostki.All(x => x.Id != JednostkaId.Value))
            {
                JednostkaId = null;
            }

            if (!JednostkaId.HasValue)
            {
                if (odwrotnaKolejnoscJednostek)
                {
                    JednostkaId = WidoczneJednostki.First().Id;
                }
                else
                {
                    var jed = WidoczneJednostki.FirstOrDefault();
                    if (jed != null)
                    {
                        JednostkaId = jed.Id;
                    }
                    else
                    {
                        throw new Exception(string.Format("Produkt o id: {0}, symbolu: {1} nie posiada widocznej jednostki", produkt.Id, produkt.Kod));
                    }
                }
            }

            //BCH  - nie wiem czemu tego nie bylo wczesniej - na liscie uzupelnijmy produkt przez co nie musimy pobiearc go pozniej
            this._produkt = produkt;

        }

        public ParametryIlosciProduktu(IKoszykPozycja baza, bool odtwortnaKolejnoscJednostek, DodawanieProduktuPrzyciski przyciski, bool pokazwacJednostki):base(baza)
        {
            WypelnijProdukt(baza.Produkt, odtwortnaKolejnoscJednostek);
            Przyciski = przyciski;
            PokazywacJednostki = pokazwacJednostki;
            ProduktId = baza.ProduktId;
            Klient = baza.Klient;
        }

        public ParametryIlosciProduktu(IProduktKlienta produkt, bool odwrotnaKolejnoscJednostek, TypPozycjiKoszyka typ = TypPozycjiKoszyka.Zwykly, DodawanieProduktuPrzyciski przyciski = null, bool pokazjednostki = true)
        {
            WypelnijProdukt(produkt, odwrotnaKolejnoscJednostek);
            TypPozycji = typ;
            Przyciski = przyciski;
            PokazywacJednostki = pokazjednostki;
            ProduktBazowyId = ((ProduktBazowy) produkt).Id;
            ProduktId = ((ProduktBazowy)produkt).Id;
            Klient = produkt.Klient;
        }

        public bool PokazujListeJednostek { get; set; }

        public bool? DodawnieTekstowe { get; set; }
        public bool MoznaDodawacDoKoszyka { get; set; }
        public bool UkryjJednostki { get; set; }

        public string Info { get; set; }
    }
}