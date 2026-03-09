using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Czy pozycja ma wybrają odpowiednią jednostkę", FriendlyOpis = "Czy pozycja w koszyku jest w wybranej jednostce")]
    public class ProduktJednostka : RegulaKoszyka, IRegulaPozycji
    {
        [FriendlyName("Jednostka")]
        [PobieranieSlownika(typeof(SlownikJednostek))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int NazwaJednostki { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            var jednostka = SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.PobierzJednostki(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).First(x => x.Key == NazwaJednostki).Value.Nazwa;
            return pozycja.Jednostka().Nazwa.Equals(jednostka, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}