using System;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class OpcjaWGrupieBind
    {
        private readonly string _nazwa;
        private readonly string _link;
        private readonly int _kolejnosc;
        private readonly Type _type;
        private readonly Func<IKlient, bool> _walidator;

        public OpcjaWGrupieBind(string nazwa, string link, int kolejnosc, Func<IKlient, bool> walidator, Type t)
        {
            _nazwa = nazwa;
            _link = link;
            _kolejnosc = kolejnosc;
            _walidator = walidator;
            _type = t;
        }

        public string Nazwa
        {
            get { return _nazwa; }
        }

        public string Link
        {
            get { return _link; }
        }
        public Type Typ
        {
            get { return _type; }
        }

        public int Kolejnosc
        {
            get { return _kolejnosc; }
        }

        public Func<IKlient, bool> Walidator
        {
            get { return _walidator; }
        }
    }
}