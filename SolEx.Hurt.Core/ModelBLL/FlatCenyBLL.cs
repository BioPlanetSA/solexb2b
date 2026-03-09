using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{

    public class FlatCenyBLL:FlatCeny, IFlatCenyBLL
    {
        private IProduktBazowy _pb;
        private IKlient _klient;

        private IProduktBazowy ProduktBazowy
        {
            get
            {
                if (_pb == null)
                {
                    _pb = Bll.DostepDane.PobierzPojedynczy<ProduktBazowy>(ProduktId);
                }
                return _pb;
            }
        }

        public ISolexBllCalosc Bll = SolexBllCalosc.PobierzInstancje;
        
        [FriendlyName("cena netto po rabacie")]
        [RealSortColumnName("FlatCeny.cena_netto")]
        public new WartoscLiczbowa CenaNetto {
            get
            {
                if (Waluta == null)
                {
                    throw new Exception($"Brak waluty dla produktu: {ProduktBazowy.Nazwa}[{ProduktId}]");
                }
                return new WartoscLiczbowa(base.CenaNetto, Waluta.WalutaB2b);
            }
            set { base.CenaNetto = value.Wartosc; }
        }

        private WartoscLiczbowa WyliczCeneBrutto(decimal cenaNetto, long walutaId)
        {
            return new WartoscLiczbowa(Kwoty.WyliczBrutto(cenaNetto, ProduktBazowy.Vat, _klient),  SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut[walutaId].WalutaB2b);
        }

        [RealSortColumnName("FlatCeny.cena_brutto")]
        [FriendlyName("cena brutto po rabacie")]
        public virtual WartoscLiczbowa CenaBrutto
        {
            get { return this.WyliczCeneBrutto(base.CenaNetto, base.WalutaId); }
        }

        [FriendlyName("cena brutto po rabacie bez przeliczenia walutowego")]
        public virtual WartoscLiczbowa CenaBruttoPrzedPrzewalutowaniem
        {
            get
            {
                if (base.PrzeliczenieWaluty_Kurs == 0)
                {
                    throw new Exception("Nie można wyliczyć ceny brutto bo nie nastąpiło przewalutowanie");
                }
                return this.WyliczCeneBrutto(base.PrzeliczenieWaluty_CenaNettoBazowa, base.PrzeliczenieWaluty_WalutaIdBazowa);
            }
        }

        [RealSortColumnName("FlatCeny.cena_hurtowa_netto")]
        [FriendlyName("cena netto przed rabatem")]
        public new WartoscLiczbowa CenaHurtowaNetto
        {
            get
            {
                if (Waluta == null)
                {
                    throw new Exception(string.Format("Brak waluty dla produktu: {0}[{1}]", ProduktBazowy.Nazwa, ProduktId));
                }
                return new WartoscLiczbowa(base.CenaHurtowaNetto, Waluta.WalutaB2b);
            }
            set { base.CenaHurtowaNetto = value.Wartosc; }
        }

        [FriendlyName("cena brutto przed rabatem")]
        [RealSortColumnName("FlatCeny.cena_hurtowa_brutto")]
        public WartoscLiczbowa CenaHurtowaBrutto
        {
            get
            {
                if (Waluta == null)
                {
                    throw new Exception(string.Format("Brak waluty dla produktu: {0}[{1}]", ProduktBazowy.Nazwa, ProduktId));
                }
                return new WartoscLiczbowa(Kwoty.WyliczBrutto(base.CenaHurtowaNetto, ProduktBazowy.Vat, _klient), Waluta.WalutaB2b);
            }
        }

        [FriendlyName("zysk klienta netto")]
        [RealSortColumnName("FlatCeny.zysk_klienta_netto")]
        public WartoscLiczbowa ZyskKlientaNetto
        {
            get
            {
                return CenaHurtowaNetto - CenaNetto;
            }
        }


        [FriendlyName("cena detaliczna netto")]
        [RealSortColumnName("FlatCeny.cena_detaliczna_netto")]
        public virtual WartoscLiczbowa CenaDetalicznaNetto
        {
            get
            {
                if (!Bll.Konfiguracja.GetPriceLevelDetal.HasValue || ProduktBazowy.CenyPoziomy == null || !ProduktBazowy.CenyPoziomy.ContainsKey(Bll.Konfiguracja.GetPriceLevelDetal.Value))
                {
                    return 0;
                }
                CenaPoziomu poziom = ProduktBazowy.CenyPoziomy[Bll.Konfiguracja.GetPriceLevelDetal.Value];

                try
                {
                    return new WartoscLiczbowa(poziom.Netto, SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut[poziom.WalutaId.Value].WalutaB2b);
                } catch (Exception e)
                {
                    SolexBllCalosc.PobierzInstancje.Log.FatalFormat("Cena detaliczna nie ma w poziomie cenowym waluty, wiadomość: {0}", e.Message);
                    return 0;
                }
            }
            set { base.CenaNetto = value.Wartosc; }
        }

        [FriendlyName("cena detaliczna brutto")]
        [RealSortColumnName("FlatCeny.cena_detaliczna_brutto")]
        public WartoscLiczbowa CenaDetalicznaBrutto
        {
            get
            {
                return new WartoscLiczbowa(Kwoty.WyliczBrutto(this.CenaDetalicznaNetto.Wartosc, ProduktBazowy.Vat), this.CenaDetalicznaNetto.Waluta);
            }
        }

        public List<GradacjaWidok> GradacjaUzytaDoLiczeniaCeny_Poziomy { get; set; }
        public decimal GradacjaUzytaDoLiczeniaCeny_KupioneIlosci { get; set; }

        public FlatCenyBLL(IFlatCeny bazowy, IProduktBazowy pb, IKlient klient): base(bazowy)
        {
            if (bazowy.KlientId != klient.Id)
            {
                throw new ArgumentException("Id klienta jest rózne od id z ceny");
            }
            _klient = klient;
            _pb = pb;
        }

        [Ignore]
        public Waluta Waluta
        {
            get { return SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut[WalutaId]; }
        }

        public FlatCenyBLL(IProduktBazowy pb, IKlient klient) : base()
        {
            _pb = pb;
            _klient = klient;
            KlientId = klient.Id;
            ProduktId = pb.Id;
        }

        public WartoscLiczbowa CenaBruttoPrzedPromocja
        {
            get
            {
                if (Waluta == null)
                {
                    throw new Exception(string.Format("Brak waluty dla produktu: {0}[{1}]", ProduktBazowy.Nazwa, ProduktId));
                }
                if (CenaNettoPrzedPromocja == null) return null;
                return new WartoscLiczbowa(Kwoty.WyliczBrutto(CenaNettoPrzedPromocja, ProduktBazowy.Vat,_klient), Waluta.WalutaB2b);
            }
        }
    }
}
