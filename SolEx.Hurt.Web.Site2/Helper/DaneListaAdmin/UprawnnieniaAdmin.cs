using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class UprawnnieniaAdmin
    {
        private readonly Func<IKlient, bool> _uprawnieDodawanie;

        private readonly Func<IKlient, bool> _uprawnieEdycja;
        private readonly Func<IKlient, bool> _uprawnienaUsuwanie;

        public UprawnnieniaAdmin(Func<IKlient, bool> uprawnieEdycja, Func<IKlient, bool> uprawnieDodawanie, Func<IKlient, bool> walidatorUsuwania)
        {
            _uprawnieEdycja = uprawnieEdycja;
            _uprawnieDodawanie = uprawnieDodawanie;
            _uprawnienaUsuwanie = walidatorUsuwania;
        }

        public Func<IKlient, bool> UprawnieDodawanie
        {
            get { return _uprawnieDodawanie; }
        }

        public Func<IKlient, bool> UprawnieEdycja
        {
            get { return _uprawnieEdycja; }
        }

        public Func<IKlient, bool> UprawnieUsuwanie
        {
            get { return _uprawnienaUsuwanie; }
        }
    }

    public class EdycjaAdminPobraniePolObiektu
    {
        private readonly Func<object, object,  IList<OpisPolaObiektu>> _metoda;
        private readonly string _akcjaZapisz;
        private readonly string _akcjaDodaj;

        public EdycjaAdminPobraniePolObiektu(Func<object, object, IList<OpisPolaObiektu>> metoda, string akcjaZapisz, string akcjaDodaj)
        {
            _metoda = metoda;
            _akcjaZapisz = akcjaZapisz;
            _akcjaDodaj = akcjaDodaj;
        }

        public Func<object, object, IList<OpisPolaObiektu>> MetodaPobieraniaPol
        {
            get { return _metoda; }
        }

        public string AkcjaZapisz
        {
            get { return _akcjaZapisz; }
        }

        public string AkcjaDodaj
        {
            get { return _akcjaDodaj; }
        }
    }
}