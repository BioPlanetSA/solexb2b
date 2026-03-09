using System;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model ustawienia konfiguracyjnego
    /// </summary>
    [Serializable]
    public class Ustawienie : ICloneable, IPolaIDentyfikujaceRecznieDodanyObiekt, IHasStringId, IStringIntern
    {
        public string Id
        {
            get
            {
                return Symbol.ToLower() + "_"+OddzialId;
            }
        }

        public string Symbol { get; set; }
        public string Opis { get; set; }
        public string Wartosc { get; set; }
        public string WartoscDlaNiezalogowanych { get; set; }
        public string Nazwa { get; set; }
        public TypUstawienia Typ { get; set; }

        [StringInternuj]
        public string Grupa { get; set; }
        public bool Widoczne { get; set; }

        [StringInternuj]
        public string Slownik { get; set; }

        public bool? NadpisywanyPracownik {get; set; }

        [UpdateColumnKey]
        public long? OddzialId {get; set; }
        public string OpisGrupy { get; set; }
        public bool Multiwartosc { get; set; }
        public string PoprzedniaWartosc	 { get; set; }
        public string PoprzedniaWartoscDlaNiezalogowanych { get; set; }

        public TypUstawieniaPodgrupa? Podgrupa { get; set; }
        public bool Dynamiczne { get; set; }
        public string PodGrupaTekstowa { get; set; }

        public string WartoscDomyslna { get; set; }
        public string WartoscDomyslnaDlaNiezalogowanych { get; set; }


        public Ustawienie( string symbol, string desc, string value, string group, string name, TypUstawienia type, bool visibleToUser,
            string slownik, bool? nadpisywanePrzezPracownika, long? oddzialId, string opisGrupy, bool multiValue, string wartoscDlaNiezalogowanych, 
            TypUstawieniaPodgrupa podgrupa,string poprzedniaWartosc,string poprzedniaWartoscPoprzeniaNiezalogowani,bool dynamiczne,string podgrupaTekstowa, string wartoscDomyslna, string wartoscDonyslnaDlaNiezalogowanych)
        {
            Symbol = symbol;
            Opis = desc;
            Wartosc = value;
            Grupa = group;
            Nazwa = name;
            Typ = type;
            Widoczne = visibleToUser;
            Slownik = slownik;
            NadpisywanyPracownik = nadpisywanePrzezPracownika;
            OddzialId = oddzialId;
            OpisGrupy = opisGrupy;
            Multiwartosc = multiValue;
            WartoscDlaNiezalogowanych = wartoscDlaNiezalogowanych;
            PoprzedniaWartosc = poprzedniaWartosc;
            PoprzedniaWartoscDlaNiezalogowanych = poprzedniaWartoscPoprzeniaNiezalogowani;
            Podgrupa = podgrupa ;
            Dynamiczne = dynamiczne;
            PodGrupaTekstowa = podgrupaTekstowa;
            WartoscDomyslna = wartoscDomyslna;
            WartoscDomyslnaDlaNiezalogowanych = wartoscDonyslnaDlaNiezalogowanych;
        }

        public Ustawienie(string symbol, string value, TypUstawienia type)
        {
            Symbol = symbol;
            Wartosc = value;
            Typ = type;
            Nazwa = symbol;
        }

        public Ustawienie(string symbol, string value, TypUstawienia type, int sellerId)
        {
            Symbol = symbol;
            Wartosc = value;
            Typ = type;
            OddzialId = sellerId;
        }
        public Ustawienie() { }
        public Ustawienie(Ustawienie baza) {this.KopiujPola(baza); }
        public object Clone()
        {
            return new Ustawienie( Symbol, Opis, Wartosc, Grupa, Nazwa, Typ,Widoczne, Slownik, NadpisywanyPracownik, OddzialId, OpisGrupy, Multiwartosc,WartoscDlaNiezalogowanych,Podgrupa.Value,PoprzedniaWartosc,PoprzedniaWartoscDlaNiezalogowanych,Dynamiczne,PodGrupaTekstowa, WartoscDomyslna, WartoscDomyslnaDlaNiezalogowanych);
        }

        public string NazwaPodgrupy()
        {
            return !string.IsNullOrEmpty(PodGrupaTekstowa) ? PodGrupaTekstowa : Podgrupa.ToString();
        }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
