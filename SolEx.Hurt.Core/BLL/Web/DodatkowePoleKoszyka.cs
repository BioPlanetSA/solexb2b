using SolEx.Hurt.Model.Enums;
using System;
using System.Text;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class DodatkowePoleKoszyka
    {
        private string _wartosc;
        public string WybraneWartosciString { get { return _wartosc; } set { _wartosc = value; } }
        public TypDodatkowegoPolaKoszykowego TypPola { get; set; }
        public int IdModulu { get; set; }
        public string Symbol { get; set; }
        public string Opis { get; set; }
        public string[] WartosciMozliwe { get; set; }
        public string Komunikat { get; set; }

        public string[] WybraneWartosci
        {
            get
            {
                if (!string.IsNullOrEmpty(_wartosc))
                {
                    return _wartosc.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                }
                return new string[0];
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in value)
                    {
                        sb.Append(s);
                        sb.Append(", ");
                    }
                    _wartosc = sb.ToString(0, sb.Length - 2);
                }
                else
                {
                    _wartosc = null;
                }
            }
        }

        public bool Wymagane { get; set; }
        public bool MultiWybor { get; set; }
    }
}