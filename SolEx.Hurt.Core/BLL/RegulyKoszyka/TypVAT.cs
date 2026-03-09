using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Typ Vatu", FriendlyOpis = "Warunek na podstawie stawki VAT")]
    public class TypVAT : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public enum TypVatu
        {
            [FriendlyName("Unijny (EU)")]
            Unijny = 1,

            [FriendlyName("Eksportowy")]
            Eksportowy = 2,

            [FriendlyName("Krajowy")]
            Krajowy = 3
        }

        [FriendlyName("Typ Vatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<TypVatu> ListaTypow{get; set;}

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            if (ListaTypow == null || !ListaTypow.Any())
            {
                return true;
            }
            IKlient klient = koszyk.Klient;

            if (klient.KlientEu && !klient.Eksport && ListaTypow.Contains(TypVatu.Unijny))
                return true;

            if (!klient.KlientEu && klient.Eksport && ListaTypow.Contains(TypVatu.Eksportowy))
                return true;

            if (!klient.KlientEu && !klient.Eksport && ListaTypow.Contains(TypVatu.Krajowy))
                return true;
            

            return false;
        }
    }
}