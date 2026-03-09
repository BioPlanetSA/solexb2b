namespace System
{
    public static class DecimalExtension
    {
        public static string DoRabatString(this decimal wartosc)
        {
            if (wartosc < 0)
            {
                return "";
            }
            return wartosc.ToString("0.##") + "%";
        }
    }
}
