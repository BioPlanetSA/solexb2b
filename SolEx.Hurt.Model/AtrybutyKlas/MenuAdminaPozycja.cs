using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    [AttributeUsage(AttributeTargets.Class)]  // multiuse attribute
    public class MenuAdminaPozycja:Attribute
    {
        private ZakladkaAdmina _pozycja;
        private string _nazwaWMenu, _opis, _katalog;
        private Licencje _licencja;
        private int _kolejnosc;
        private bool _oddzialWidoczne;
        public ZakladkaAdmina Pozycja {
            get { return _pozycja; } 
        }
        public string NazwaWMenu
        {
            get { return _nazwaWMenu; }
        }
        public string Opis
        {
            get { return _opis; }
        }
        public int Kolejnosc
        {
            get { return _kolejnosc; }
        }
        public Licencje WymaganaLicencja
        {
            get { return _licencja; }
        }
        public string Katalog
        {
            get { return _katalog; }
        }
        public bool OddzialWidoczne
        {
            get { return _oddzialWidoczne; }
        }
        public MenuAdminaPozycja(ZakladkaAdmina pozycja, string nazwa, string opis,Licencje licencja=Licencje.Brak,string katalog="",bool oddzialWidoczne=true)
        {
            _pozycja = pozycja;
            _nazwaWMenu = nazwa;
            _opis = opis;
            _katalog = katalog;
            _licencja = licencja;
            _kolejnosc = (int) pozycja;
            _oddzialWidoczne = oddzialWidoczne;
        }
    }
}
