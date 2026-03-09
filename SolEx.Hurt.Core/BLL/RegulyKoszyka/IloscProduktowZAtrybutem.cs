using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class IloscProduktowZAtrybutem : IloscLacznaBaza
    {
        public enum IleCechAtrybutu
        {
            NieWszystkie = 1,
            Wszystkie = 10
        }

        public IloscProduktowZAtrybutem()
        {
            Ilosc = IleProduktow.NieWszystkie;
            IloscCech = IleCechAtrybutu.NieWszystkie;
        }

        public override string Opis
        {
            get { return "Czy określona liczba produktów ma określoną cechę"; }
        }

        [FriendlyName("Czy produkt musi posiadać (nie)wszystkie cechy z atrybutu aby warunek został spełniony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public IleCechAtrybutu IloscCech { get; set; }

        [FriendlyName("Atrybut")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        public int Atrybut { get; set; }

        public override List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();

            listaBledow = Przedzial.Sprawdzenie(Minimum, Maksimum);
            AtrybutBll atrybut = null;
            if (Atrybut == 0)
            {
                listaBledow.Add("Pola Atrybut jest puste");
            }
            if (Atrybut != 0)
            {
                atrybut = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<AtrybutBll>(Atrybut);
            }

            if (atrybut == null)
            {
                listaBledow.Add(string.Format("Brak Atrybutu o następującym id: {0}", Atrybut));
            }

            return listaBledow;
        }

        public override decimal Wylicz(IKoszykiBLL koszyk, out int ilosc)
        {
            decimal liczba = 0;
            ilosc = 0;
            if (Atrybut == 0) throw new InvalidOperationException("Pole Atrybut jest puste.");
            AtrybutBll atrybut = null;
            if (Atrybut != 0)
            {
                atrybut = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<AtrybutBll>(Atrybut);
            }

            if (atrybut != null)
            {
                List<CechyBll> wszystkieCechy = CechyAtrybuty.PobierzCecheDlaAtrybutu(atrybut.Id, Config.JezykIDPolski);
                int iloscCech = 0;

                foreach (var poz in koszyk.PobierzPozycje)
                {
                    foreach (CechyBll cechyBll in wszystkieCechy)
                    {
                        if (poz.Produkt.Cechy.Any(p => p.Key == cechyBll.Id))
                        {
                            iloscCech++;
                        }
                    }
                    if ((IloscCech == IleCechAtrybutu.Wszystkie && iloscCech == wszystkieCechy.Count) || (IloscCech == IleCechAtrybutu.NieWszystkie && iloscCech > 0))
                    {
                        liczba += poz.IloscWJednostcePodstawowej;
                        ilosc++;
                    }
                }
            }

            return liczba;
        }
    }
}