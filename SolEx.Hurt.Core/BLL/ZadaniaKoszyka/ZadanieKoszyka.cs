using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Linq;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class ZadanieKoszyka : ModulPosiadajacyWarunki, IPoleJezyk
    {
        protected ZadanieKoszyka()
        {
            KomunikatPozycja = PokazywanieKomunikatu.NaGorze;
        }

        public event DodanieKomunikatuEventHandler DodajWiadomosc;

        [FriendlyName("Gdzie wyświetlać komunikat")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PokazywanieKomunikatu KomunikatPozycja { get; set; }

        [FriendlyName("Komunikat")]
        [Niewymagane]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Komunikat { get; set; }


        public string Grupa
        {
            get
            {
                var nazwa = typeof(IGrupaZadania).Name;
                string grupaNazwa = "";
                var interfejsy = GetType().GetInterfaces();
                foreach (Type i in interfejsy)
                {
                    if (i.Name != nazwa)
                    {
                        var x = i.GetInterfaces().FirstOrDefault(p => p.Name == nazwa);
                        if (x != null)
                        {
                            grupaNazwa += $"{i.Name}; ";
                        }
                    }
                }
                return grupaNazwa;
            }
        }

        protected void WyslijWiadomosc(string wiadomosc, KomunikatRodzaj komunikatRodzaj)
        {
            if (DodajWiadomosc != null)
            {
                DodajWiadomosc(this, new DodanieKomunikatuEventArgs(wiadomosc, komunikatRodzaj, $"{GetType().Name}Id{Id}", KomunikatPozycja));
            }
        }
        [Ignore]
        public int JezykId { get; set; }
    }

    public enum PozycjaWKoszyku
    {
        ZPrawej,
        ZLewej,
        Centralnie,

    }
}