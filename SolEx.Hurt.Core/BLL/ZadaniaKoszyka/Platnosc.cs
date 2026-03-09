using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Płatność", FriendlyOpis = "Sposób płatności")]
    public class Platnosc : ZadanieCalegoKoszyka, ISposobPlatnosci
    {
        [FriendlyName("Gdzie wyświetlać komunikat")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PokazywanieKomunikatu KomunikatPozycja { get; set; }

        [FriendlyName("Komunikat")]
        [Niewymagane]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Komunikat { get; set; }

        public IKlienciDostep klienciDostep = SolexBllCalosc.PobierzInstancje.Klienci;

        public override List<Type> WykluczoneWarunki
        {
            get
            {
                List<Type> wynik = new List<Type> {typeof(SposobPlatnosci), typeof(SposobDostawy)};
                return wynik;
            }
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            IKlient k = koszyk.Klient;
            if (string.IsNullOrEmpty(GrupaKategorii))
            {
                return true;
            }
            var kategoria = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.KategoriaKlienta>(null, x => Sql.In(x.Id,k.Kategorie) && x.Grupa == GrupaKategorii).FirstOrDefault();
            if (kategoria == null)
            {
                return false;
            }
            bool wynik = int.TryParse(kategoria.Nazwa, out _termin);
            if (!wynik)
            {
                int a = kategoria.Nazwa.IndexOf(':');
                if (a > 0)
                {
                    string term = kategoria.Nazwa.Substring(a + 1).Trim();
                    return int.TryParse(term, out _termin);
                }
            }
            return true;
        }

        [FriendlyName("Nazwa")]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public new string Nazwa { get; set; }

        private int _termin;

        [FriendlyName("Termin")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Termin
        {
            get { return _termin; }
            set { _termin = value; }
        }

        [FriendlyName("Grupa z której pobierać termin płatności")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikGrupyKategoriiKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string GrupaKategorii { get; set; }

        public string PobierzOpis(IKoszykiBLL koszyk)
        {
            return Nazwa + (Termin > 0 ? $" ({Termin})" : "");
        }
    }
}