using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ProduktMojKatalog : RegulaKoszyka, IRegulaPozycji, IWarunekRegulyPozycjiDokumentu
    {
        public override string Opis
        {
            get { return "Czy określony pracownika. edytuje koszyk"; }
        }

        [FriendlyName("Czy produkt jest/ nie jest w moim katalogu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RelacjaJestNieJest Relacja { get; set; }

        public bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument)
        {
            return Test(pozycja.ProduktId, dokument.DokumentPlatnikId);
        }

        public bool Test(long? produkt, long klient)
        {
            bool jest = false;
            if (produkt.HasValue)
            {
                var k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(klient);
                jest = k.MojKatalog.Contains(produkt.Value);
            }
            return Relacja == RelacjaJestNieJest.Jest ? jest : !jest;
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Test(pozycja.ProduktId, koszyk.KlientId);
        }
    }
}