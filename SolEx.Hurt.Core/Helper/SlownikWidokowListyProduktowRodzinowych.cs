
namespace SolEx.Hurt.Core.Helper
{
    public class SlownikWidokowListyProduktowRodzinowych : SlownikWidokowBaza
    {
        protected override string SciezkaWidokow
        {
            get
            {
                return "\\Produkty\\WidokiRodzinowe\\Lista";
            }
        }
    }
}

               