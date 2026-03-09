using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    /// <summary>
    /// obiekt produktu klienta
    /// </summary>
    public class ProduktKlientZaPunktyLubGratis : ProduktKlienta
    {
        private IFlatCenyBLL _cena;
        public override IFlatCenyBLL FlatCeny
        {
            get { return _cena; }
        }
        public ProduktKlientZaPunktyLubGratis(ProduktBazowy bazowy, IFlatCenyBLL cena, IKlient klient):base(bazowy,klient)
        {
            _cena = cena;
        }
    }
}