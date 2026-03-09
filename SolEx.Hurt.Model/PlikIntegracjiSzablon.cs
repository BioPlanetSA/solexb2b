using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;
using System;
using ServiceStack.Common;

namespace SolEx.Hurt.Core.BLL
{
        public class PlikIntegracjiSzablon
        {
            public TypDanychIntegracja typDanych { get; set; }
            public string Format { get; set; }
            public List<int> Wersja { get; set; }
            public string Szablon { get; set; }

            /// <summary>
            /// ścieżka do pliku na dysku z szablonem
            /// </summary>
            public string SciezkaDoSzablonu { get; set; }

            public string PobierzWidokPliku(int wersja)
            {
                return this.SciezkaDoSzablonu + wersja;
            }

            public int IdSzablonu { get; set; }

            public string SzablonLadnaNazwa
            {
                get { return this.Szablon.Replace("-", " "); }
            }
            public static string GenerujNazwePliku_przyPobieraniu(PlikIntegracjiSzablon szablon, int wersja, string jezykSymbol)
            {
                return $"{szablon.typDanych}_{szablon.Format}_{wersja}_{DateTime.Now.ToString().Replace(".", "-").Replace(" ", "_")}_{jezykSymbol}.{szablon.Format}".ToLower();
            }

            public static string GenerujNazwePliku_plikDoDrukowaniaKatalogu(PlikIntegracjiSzablon szablon, int wersja)
            {
                return $"daneTestoweKatalogu-SolexB2B.{wersja}.{szablon.IdSzablonu}.{DateTime.Now.DayOfYear}.{szablon.Format}".ToLower();
            }
    }
    
}