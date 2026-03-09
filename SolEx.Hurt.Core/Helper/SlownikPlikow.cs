namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPlikow: SlownikPlikowBaza
    {
        protected override string Sciezka
        {
            get { return "\\pliki"; }
        }

    }
}