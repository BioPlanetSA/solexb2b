
using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Tresci
{
    public class DaneDoMenu
    {
        public DaneDoMenu(List<TreeItem<ElementMenu>> drzewo, bool pokazujPodkategorie, bool pokazujReklamy, int? szerokosc = null)
        {
            Drzewo = drzewo;
            SzekoroscKolumny = szerokosc;

            PokazujPodkategorie = pokazujPodkategorie;
            PokazujReklamy = pokazujReklamy;
        }

        //nie mozna upublicznic dlatego ze w konstruktorze uzupelnianie
        public List<TreeItem<ElementMenu>> Drzewo { get; private set; }

        public int? SzekoroscKolumny { get; set; }

        public int idKontrolkiWywolujacej { get; set; }

        public bool PokazujPodkategorie { get; set; }
        public bool PokazujReklamy { get; set; }

        private List<DaneDoMenuOpcja> _listaOpcji = null;

        public List<DaneDoMenuOpcja> OpcjeMenu
        {
            get
            {
                if (_listaOpcji == null)
                {
                    _listaOpcji = new List<DaneDoMenuOpcja>( Drzewo.Count );
                    foreach (var elem in Drzewo)
                    {
                        _listaOpcji.Add(new DaneDoMenuOpcja(elem, PokazujPodkategorie, PokazujReklamy));
                    }
                }
                return _listaOpcji;
            }
        }
     
    }
}