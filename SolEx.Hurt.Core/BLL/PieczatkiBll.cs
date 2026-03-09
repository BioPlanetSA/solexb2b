using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL
{
    public class PieczatkiBll : LogikaBiznesBaza, IPieczatkiBll
    {
        public PieczatkiBll(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public List<PieczatkiSzablony> PobierzSzablonyPoSymbolu(string symbol)
        {
            PieczatkiTyp element = Calosc.DostepDane.PobierzPojedynczy<PieczatkiTyp>(x => x.SymbolTypu == symbol, null);
            List<PieczatkiSzablony> listaSzablonow = new List<PieczatkiSzablony>();
            if (element != null)
            {
                listaSzablonow = Calosc.DostepDane.Pobierz<PieczatkiSzablony>(null, x => x.TypId == element.Id).ToList();
            }
            return listaSzablonow;
        }

        // *************** TYP ************************ //

        public void DodajTyp(PieczatkiTyp typ)
        {
            Calosc.DostepDane.AktualizujPojedynczy(new PieczatkiTyp(typ));
        }

        public void UsunTyp(int? id)
        {
            if (id != null)
            {
                var listaSzablonow = PobierzSzablonyPoIdTypu(id.Value);

                foreach (var szablon in listaSzablonow)
                {
                    Calosc.DostepDane.UsunPojedynczy<PieczatkiSzablony>(szablon.Id);
                }
                Calosc.DostepDane.UsunPojedynczy<PieczatkiTyp>(id);
            }
        }

        /// <summary>
        /// Pobiera obiekt pieczatki po symbolu
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>Pieczatki typ</returns>
        public PieczatkiTyp PobierzTypPieczatkiPoSymbolu(string symbol)
        {
            var element = Calosc.DostepDane.PobierzPojedynczy<PieczatkiTyp>(x => x.SymbolTypu == symbol, null);
            return element;
        }

        /// <summary>
        /// Pobiera obiekt typu pieczatątki
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PieczatkiTyp</returns>
        public PieczatkiTyp PobierzTypPieczatkiPoId(int id)
        {
            var element = Calosc.DostepDane.PobierzPojedynczy<PieczatkiTyp>(x => x.Id == id, null);
            return element;
        }

        // *************** Szablony ************************ //

        public void DodajSzablon(PieczatkiSzablony szablon)
        {
            Calosc.DostepDane.AktualizujPojedynczy(new PieczatkiSzablony(szablon));
        }

        public List<PieczatkiSzablony> PobierzSzablonyPoIdTypu(int id)
        {
            PieczatkiTyp element = Calosc.DostepDane.PobierzPojedynczy<PieczatkiTyp>(x => x.Id == id, null);
            List<PieczatkiSzablony> listaSzablonow = new List<PieczatkiSzablony>();
            if (element != null)
            {
                listaSzablonow = Calosc.DostepDane.Pobierz<PieczatkiSzablony>(null, x => x.TypId == element.Id).ToList();
            }
            return listaSzablonow;
        }

        /// <summary>
        /// Pobiera szablon po id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PieczatkiSzablony</returns>
        public PieczatkiSzablony PobierzSzablonPoId(int? id)
        {
            if (id != null)
            {
                return Calosc.DostepDane.PobierzPojedynczy<PieczatkiSzablony>(x => x.Id == id, null);
            }
            return null;
        }

        public PieczatkiTyp PobierzKoloryPieczatkiPoIdTypu(string idTypu)
        {
            return Calosc.DostepDane.PobierzPojedynczy<PieczatkiTyp>(x => x.Id == Convert.ToInt32(idTypu), null);
        }

        public PieczatkiTyp PobierzKoloryPieczatkiPoIdTypu(int idTypu)
        {
            return Calosc.DostepDane.PobierzPojedynczy<PieczatkiTyp>(x => x.Id == idTypu, null);
        }
    }
}