using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public enum KierunekOpiekun
    {
        JestOpiekun, BrakOpiekuna
    }

    public class Opiekun : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        public override string Opis
        {
            get { return "Regula opiekuna"; }
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        [FriendlyName("Czy ma/nie ma opiekuna")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KierunekOpiekun Relacja { get; set; }

        [FriendlyName("Opiekun")]
        [PobieranieSlownika(typeof(SlownikNieKlientow))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? OpiekunId { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        private bool Regula(IKoszykiBLL koszyk)
        {
            switch (Relacja)
            {
                case KierunekOpiekun.BrakOpiekuna:
                    if (!OpiekunId.HasValue)
                    {
                        return koszyk.Klient.Opiekun==null || koszyk.Klient.Opiekun.Id == 0;
                    }
                    return koszyk.Klient.Opiekun==null || koszyk.Klient.Opiekun.Id != OpiekunId;

                case KierunekOpiekun.JestOpiekun:
                    if (!OpiekunId.HasValue)
                    {
                        return koszyk.Klient.Opiekun != null && koszyk.Klient.Opiekun.Id != 0;
                    }
                    return koszyk.Klient.Opiekun != null && koszyk.Klient.Opiekun.Id == OpiekunId;
            }
            throw new InvalidOperationException("Nie znana opcja");
        }
    }
}