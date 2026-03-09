namespace System
{
    public static class DateTimeExtension
    {
        public static bool JestWPrzedziale(this DateTime data,DateTime poczatek,DateTime koniec)
        {
            return data >= poczatek && data <= koniec;

        }
  
    }

}
