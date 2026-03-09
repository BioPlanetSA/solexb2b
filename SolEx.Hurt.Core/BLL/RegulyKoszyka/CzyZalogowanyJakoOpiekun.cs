using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class CzyZalogowanyJakoOpiekun : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public CzyZalogowanyJakoOpiekun()
        {
        }

        public override string Opis
        {
            get { return "Czy na koncie klienta jest zalogowany opiekun/administrator"; }
        }

        [FriendlyName("Czy klient posiada którąkolwiek z kategorii")]
        [WidoczneListaAdmin(true,true,true,false)]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        public List<int> KategorieKlienta { get; set; }
        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            var klientEdujacy = koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta;
            if (klientEdujacy == null)
            {
                return false;
            }

            if (KategorieKlienta==null || !KategorieKlienta.Any() || klientEdujacy.Role.Contains(RoleType.Administrator) || klientEdujacy.Kategorie.Any(x => KategorieKlienta.Contains(x)))
            {
                return true;
            }


            return false;
        }
    }
}