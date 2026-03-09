using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ModulLojalnosciowy : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        public enum CzyKlientZostalPolecony
        {
            ZostalPolecony, NieZostalPolecony
        }

        public ModulLojalnosciowy()
        {
            CzyZostalPolecony = CzyKlientZostalPolecony.ZostalPolecony;
        }

        public override string Opis
        {
            get { return "Klient został polecony przez kogoś innego (moduł lojalnościowy)"; }
        }

        [FriendlyName("Czy klient został polecony lub nie został polecony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public CzyKlientZostalPolecony CzyZostalPolecony { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        private bool Regula(IKoszykiBLL koszyk)
        {
            bool jestRabat = !string.IsNullOrEmpty(koszyk.Klient.OsobaPolecajaca);

            switch (CzyZostalPolecony)
            {
                case CzyKlientZostalPolecony.ZostalPolecony:
                    return jestRabat;

                case CzyKlientZostalPolecony.NieZostalPolecony:
                    return !jestRabat;
            }

            return false;
        }
    }
}