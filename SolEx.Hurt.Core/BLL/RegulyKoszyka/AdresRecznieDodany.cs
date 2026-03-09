using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Adres ręcznie dodanym.", FriendlyOpis = "Sprawdza czy adres wybrany w koszyku jest adresem ręcznie dodanym.")]
    public class AdresRecznieDodany : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return koszyk.Adres != null && koszyk.Adres.RecznieDodany();
        }
    }
}