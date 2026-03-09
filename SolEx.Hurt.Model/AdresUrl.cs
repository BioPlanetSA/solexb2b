using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    public class AdresUrl 
    {
        public AdresUrl(string url):base()
        {
            Url = url;
        }
        public AdresUrl()
        {
            Tryb = TrybOtwierania.ObecneOkno;
        }
        public string Url { get; set; }
        public TrybOtwierania Tryb { get; set; }
        public override string ToString()
        {
            if (Tryb == TrybOtwierania.Modal)
            {
                return Url+"/m";
            }
            if (Tryb == TrybOtwierania.NoweOkno)
            {
                return Url + "/blank";
            }
            return Url;
        }
        public static implicit operator AdresUrl(string url)
        {
            return new AdresUrl(url);
        }
      
    }
}

