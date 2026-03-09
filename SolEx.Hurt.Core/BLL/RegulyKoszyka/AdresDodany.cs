using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Adres dodanym.", FriendlyOpis = "Sprawdza czy adres jest wybrany w koszyku.")]
    public class AdresDodany : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return koszyk.Adres != null;
        }
    }
}