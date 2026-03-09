using System;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Pozostawianie w koszyku pozycji ponad stan", FriendlyOpis = "Pozostawianie w koszyku pozycji ponad stan")]
    public class PozostawWKoszykuPozycjePonadStan : PrzekroczoneStanyBaza, IZadaniePoFinalizacji
    {
        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            Dictionary<IKoszykPozycja, decimal> braki = PobierzBraki(koszyk);
            if (braki == null || !braki.Any()) return true;
            if (koszyk.PobierzPozycje.All(x => x.StanKoszyk == StanKoszyk.Niedostepy)) return false;
            KoszykBll nowy = TworzNowyKoszyk(braki);
            PoprawPozycjeKoszyka(koszyk, braki);
            SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk((KoszykBll)koszyk);
            SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(nowy);
            return true;
        }

        public Dictionary<IKoszykPozycja, decimal> PobierzBraki(IKoszykiBLL koszyk)
        {
            Dictionary<IKoszykPozycja, decimal> braki = null;
            base.Wykonaj(koszyk, ref braki);
            return braki;
        }

        public void PoprawPozycjeKoszyka(IKoszykiBLL koszyk, Dictionary<IKoszykPozycja, decimal> braki)
        {
            if (braki == null || !braki.Any()) return;
            foreach (KoszykPozycje pozycje in koszyk.PobierzPozycje)
            {
                if (braki.ContainsKey(pozycje))
                {
                    pozycje.Ilosc -= braki[pozycje];
                }
            }
        }

        public KoszykBll TworzNowyKoszyk(Dictionary<IKoszykPozycja, decimal> braki)
        {
            if (braki == null || !braki.Any()) return null;

            IKlient klient = braki.First().Key.Klient;
            string nazwa = string.Format("Braki z zamówienia z dnia {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm"));

            KoszykBll nowy = new KoszykBll
            {
                Klient = klient,
                Nazwa = nazwa,
                DataModyfikacji = DateTime.Now,
                Typ = TypKoszyka.Koszyk,
                Aktywny = true,
                KlientId = klient.Id,
            };
            foreach (var pozycja in braki)
            {
                KoszykPozycje poz = new KoszykPozycje(pozycja.Key);
                poz.Ilosc = pozycja.Value;
                poz.DataDodania = DateTime.Now;
                poz.Id = 0;
                poz.Klient = pozycja.Key.Klient;
                nowy.DodajPozycjeDoKoszyka(poz);
            }
            return nowy;
        }
       
    }
}