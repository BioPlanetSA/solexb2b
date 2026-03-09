namespace SolEx.Hurt.Core.Helper
{
    public class SlownikSzablonowRaportowPdf : SlownikWidokowBaza
    {
        protected override string SciezkaWidokow
        {
            get
            {
                return "\\Integracja\\RaportySzabonyPDF";
            }
        }
    }
}