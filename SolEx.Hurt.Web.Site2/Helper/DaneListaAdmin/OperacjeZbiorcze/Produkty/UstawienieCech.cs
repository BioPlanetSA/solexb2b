using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.OperacjeZbiorcze.Produkty
{
    /// <summary>
    /// Klasa bazowa dla modułów operacji zbiorczych
    /// </summary>
    public class UstawianieCech : OperacjaZbiorczaBaza
    {
        public override string PokazywanaNazwa
        {
            get { return "Ustaw cechę"; }
        }

        public override Type OblugiwanyTyp()
        {
            return typeof(ProduktBazowy);
        }

        [FriendlyName("Cechy do ustawienia")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdmin(true, true, true, true)]
        public int[] WybraneCechy { get; set; }

        [FriendlyName("Pole słownikowe")]
        [SyncSlownikNaPodstawieInnegoTypu("SolEx.Hurt.Model.produkty, SolEx.Hurt.Model")]
        public int[] PoleSlownikowe { get; set; }

        [FriendlyName("Pole słownikowe enum")]
        public RoleType[] PoleSlownikoweEnum { get; set; }

        [FriendlyName("Pole słownikowe enum pojedyńcze pole")]
        public RoleType PoleSlownikoweEnumPojedynczePole { get; set; }

        public override void Wykonaj(string[] klucze)
        {
        }
    }
}