using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class DostawaBaza : ZadanieCalegoKoszyka, ISposobDostawy, ITestowalna
    {
        [FriendlyName("Gdzie wyświetlać komunikat")]
        public PokazywanieKomunikatu KomunikatPozycja { get; set; }

        [FriendlyName("Komunikat")]
        [Niewymagane]
        [Lokalizowane]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Komunikat { get; set; }

        public string PobierzOpis(IKoszykiBLL koszyk)
        {
            string opis = $"{(string.IsNullOrEmpty(OpisDostawy) ? ProduktDostawy.Kod : OpisDostawy)}";
            var cena = WyliczCene(koszyk);
            if (PokazujCeneZerowa ||cena!=0)
            {
                opis += $" ({cena:0.00} {koszyk.WalutaKoszyka().WalutaB2b})";
            }
            return opis;
        }

        public override List<Type> WykluczoneWarunki
        {
            get
            {
                List<Type> wynik = new List<Type> {typeof(SposobDostawy)};
                return wynik;
            }
        }

        [Obsolete("Pole wycofane.  Uzywać pola produkt")]
        [FriendlyName("Symbol produktu")]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, false, false)]
        public string SymbolProduktu { get; set; }

        /// <summary>
        /// Propertis określający czy był błąd podczas pobierania sposoby dostawy wykorzystywany np przy pobieraniu ceny wysyłki z Upsa
        /// </summary>
        [FriendlyName("Czy podczas pobierania sposobu dostawy wystapił błąd")]
        [Niewymagane]
        public bool CzyWystapilBlad { get; set; }

        [FriendlyName("Produkt")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public int IdProduktu { get; set; }

        [FriendlyName("Opis dostawy")]
        [Niewymagane]
        [Lokalizowane]
        [WidoczneListaAdmin(false, false, true, false)]
        public string OpisDostawy { get; set; }

        public virtual List<string> TestPoprawnosci()
        {
            return new List<string>();
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            var wynik = ProduktDostawy != null;
            return wynik;
        }

        public abstract decimal WyliczCene(IKoszykiBLL koszyk);

        [FriendlyName("Czy cena dostawy ma być pokazana w opisie gdy jej wartość to 0?")]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool PokazujCeneZerowa { get; set; }

        public ProduktBazowy ProduktDostawy
        {
            get
            {
                ProduktBazowy p = null;
                if (!string.IsNullOrEmpty(SymbolProduktu))
                {
                    p = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(SymbolProduktu,SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                }
                else if (IdProduktu != 0)
                {
                    p = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(IdProduktu, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                }

                if (p != null && p.Widoczny)
                {
                    return p;
                }
                return null;
            }
        }
    }
}