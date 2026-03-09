namespace SolEx.Hurt.Core.ModelBLL
{
    public class BrakIdentyfikatoraException : System.Exception
    {
        public string OczekiwanyKlucz { get; set; }
        public BrakIdentyfikatoraException(string Message, string oczekiwanyKlucz)
            : base(Message)
        {
            OczekiwanyKlucz = oczekiwanyKlucz;
        }
    }
}