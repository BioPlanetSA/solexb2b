namespace SolEx.Hurt.Core.Helper
{
    public class SlownikUkladowDlaBloga : SlownikWidokowBaza
    {
        protected override string SciezkaWidokow
        {
            get { return "\\Blog\\UkladyListy"; }
        }
    }
}