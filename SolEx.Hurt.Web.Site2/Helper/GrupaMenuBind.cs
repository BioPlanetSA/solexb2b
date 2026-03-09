using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class GrupaMenuBind
    {
        private readonly string _nazwa;
        private readonly int _kolejnosc;
        private readonly Func<IKlient, bool> _walidator;
        private readonly Dictionary<string, OpcjaWGrupieBind> _pozycje;

        public GrupaMenuBind(string nazwa, int kolejnosc, Func<IKlient, bool> walidator)
        {
            _kolejnosc = kolejnosc;
            _walidator = walidator;
            _nazwa = nazwa;
            _pozycje = new Dictionary<string, OpcjaWGrupieBind>();
        }

        /// <summary>
        /// Nazwa zak³adki w menu
        /// </summary>
        public string Nazwa
        {
            get { return _nazwa; }
        }

        /// <summary>
        /// Metoda waliduj¹ca dostêp klient do menu
        /// </summary>
        public Func<IKlient, bool> Walidator
        {
            get { return _walidator; }
        }

        /// <summary>
        /// Kolejnoœæ w menu
        /// </summary>
        public int Kolejnosc
        {
            get { return _kolejnosc; }
        }

        /// <summary>
        /// Elementu w danej grupie
        /// </summary>
        public Dictionary<string, OpcjaWGrupieBind> Pozycje
        {
            get { return _pozycje; }
        }
    }
}