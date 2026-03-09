using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class RabatKwotowyNaPozycje : ZadaniePozycjiKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        public override string Opis
        {
            get { return "Rabat kwotowy na pozycji"; }
        }

        [FriendlyName("Typ liczenia rabatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TrybLiczeniaRabatuWKoszyku TypLiczeniaRabatu { get; set; }

        [FriendlyName("Wartość kwotowa rabatu do ceny netto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Rabat { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (Rabat == 0)
            {
                listaBledow.Add("Wartość rabatu jest równa 0");
            }
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            if (pozycja.Produkt.FlatCeny.CenaNetto > 0 && Rabat != 0)
            {
                decimal nowacena = pozycja.Produkt.FlatCeny.CenaNetto - Rabat;
                decimal nowy = Kwoty.WyliczRabatDlaUzyskaniaCeny(pozycja.Produkt.FlatCeny.CenaNetto, nowacena);
                pozycja.ZmienDodatkowyRabat(nowy, Komunikat, TypLiczeniaRabatu);
            }
            return true;
        }
    }
}