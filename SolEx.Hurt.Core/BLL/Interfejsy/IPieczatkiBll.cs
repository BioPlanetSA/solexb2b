using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IPieczatkiBll
    {
        PieczatkiTyp PobierzTypPieczatkiPoSymbolu(string symbol);

        List<PieczatkiSzablony> PobierzSzablonyPoSymbolu(string symbol);

        PieczatkiTyp PobierzKoloryPieczatkiPoIdTypu(string symbol);

        PieczatkiTyp PobierzKoloryPieczatkiPoIdTypu(int idTypu);

        PieczatkiTyp PobierzTypPieczatkiPoId(int id);

        List<PieczatkiSzablony> PobierzSzablonyPoIdTypu(int id);

        PieczatkiSzablony PobierzSzablonPoId(int? id);

        void DodajTyp(PieczatkiTyp typ);

        void UsunTyp(int? id);

        void DodajSzablon(PieczatkiSzablony szablon);
    }
}