using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class RabatZaZestaw : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        public override string Opis
        {
            get
            {
                return "Dodaje rabat procentowy na zestaw. Aby rabat był naliczony w koszyku muszą znaleść się wszystkie elementy zestawu";
            }
        }

        [FriendlyName("Symbole produktów tworzące zestawa, oddzielone ;")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SymboleProduktow { get; set; }

        [FriendlyName("Typ liczenia rabatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TrybLiczeniaRabatuWKoszyku TypLiczeniaRabatu { get; set; }

        [FriendlyName("Wartość rabatu procentowa")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Rabat { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.SpradzWartosc(Rabat, "Rabat");
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            List<KoszykPozycje> pozycje = koszyk.PobierzPozycje;
            //jak nie ma produktów zestawowwych - wychodzimy
            if (KodyProduktow.Count == 0)
            {
                return true;
            }
            _kody = null;
            int typ = Math.Abs((Rabat.ToString(CultureInfo.InvariantCulture) + SymboleProduktow).GetHashCode());
            foreach (KoszykPozycje pozycja in pozycje)
            {
                if (KodyProduktow.ContainsKey(pozycja.ProduktId))
                {
                    KodyProduktow[pozycja.ProduktId] += pozycja.IloscWJednostcePodstawowej;
                    if (pozycja.Hash == typ)
                    {
                        pozycja.Ilosc = 0;//zerujemy pozycje ręcznie dodane
                    }
                }
            }//wiemy ile jest każdego z interesujących nas produktów w koszyku
            decimal iloscZestawow = KodyProduktow.Values.Min();
            if (iloscZestawow > 0)
            {
                List<long> klucze = KodyProduktow.Keys.ToList();
                foreach (int sp in klucze)
                {
                    KoszykPozycje kp = pozycje.FirstOrDefault(x => x.ProduktId == sp && x.Hash == typ);
                    //      KoszykPozycja kpzwykla = koszyk.Pozycje.FirstOrDefault(x => x.produkt_id == sp && x.typ != typ);
                    if (kp == null)
                    {
                        kp = new KoszykPozycje();
                        kp.Hash = typ;
                        pozycje.Add(kp);
                    }
                    ProduktBazowy pb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(sp, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                    kp.Ilosc = iloscZestawow;
                    kp.Klient = koszyk.Klient;
                    ((IKoszykPozycja)kp).JednostkaId = pb.JednostkaPodstawowa.Id;
                    kp.KoszykId = koszyk.Id;
                    kp.ProduktId = sp;
                    kp.DataDodania = DateTime.Now;
                    kp.ZmienDodatkowyRabat(Rabat, Komunikat, TypLiczeniaRabatu);
                    KodyProduktow[sp] -= iloscZestawow;
                    //if (kpzwykla != null)
                    //{
                    //  //  KodyProduktow[sp] = 0;
                    ////    kpzwykla.ilosc -= iloscZestawow/kpzwykla.Jednostka.Przelicznik;
                    //}
                }
            }
            foreach (var sp in KodyProduktow)
            {
                KoszykPozycje kp = pozycje.FirstOrDefault(x => x.ProduktId == sp.Key && x.Hash != typ);

                if (kp == null)
                {
                    kp = new KoszykPozycje();
                    pozycje.Add(kp);
                }
                ProduktBazowy pb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(sp.Key, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                kp.Ilosc = sp.Value;
                kp.Klient = koszyk.Klient;
                kp.JednostkaId = pb.JednostkaPodstawowa.Id;
                kp.KoszykId = koszyk.Id;
                kp.ProduktId = sp.Key;
                kp.DataDodania = DateTime.Now;
            }
            return true;
        }

        private Dictionary<long, decimal> _kody;

        private Dictionary<long, decimal> KodyProduktow
        {
            get
            {
                if (_kody == null)
                {
                    _kody = new Dictionary<long, decimal>();
                    if (!string.IsNullOrEmpty(SymboleProduktow))
                    {
                        string[] kody = SymboleProduktow.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in kody)
                        {
                            ProduktBazowy pb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(s, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                            if (pb != null)
                            {
                                if (!_kody.ContainsKey(pb.Id))
                                {
                                    _kody.Add(pb.Id, 0);
                                }
                            }
                        }
                    }
                }
                return _kody;
            }
        }
    }
}