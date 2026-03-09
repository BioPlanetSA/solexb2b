using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    /// <summary>
    /// interfejs dla profilu klienta gdzie deklarujemy jakie dodatkowe metory będzie posiadała klasa
    /// </summary>
    public interface IProfilKlientaBll//: IBllBazaPobieranieModyfikowanieDanych<ProfilKlienta, ProfilKlienta,long>
    {
        T PobierzWartoscDomyslna<T>(TypUstawieniaKlienta typ, string dodatkoweDane, AccesLevel dopisek = AccesLevel.Niezalogowani);
        T PobierzWartosc<T>(IKlient klient, TypUstawieniaKlienta typ, string dodatkoweDane = null);
        void UsunWartosc(IKlient klient, TypUstawieniaKlienta typ, string dodatkoweDane = null);
        void DodajWartosc<T>(IKlient klient, TypUstawieniaKlienta typ, T wartosc, string dodatkoweDane = null);
        IList<ProfilKlienta> PobierzProfilKlienta(IKlient klient);
        void InicjalizujDomyslneWartosci();
        Dictionary<int, HashSet<long>> PobierzStaleFiltry(IKlient aktualnyKlient);
        string PobierzStaleFiltryString(IKlient aktualnyKlient);
        void DodajStatyFiltr(IKlient aktualnyKlient, HashSet<long> filtry, bool zamien = false);
         void UsunStalyFiltr(IKlient aktualnyKlient, HashSet<long> filtry);
        Sortowanie PobierzSortowanie(IKlient klient, TypUstawieniaKlienta typ, string dodatkoweDane = null);
        void CzyszczenieUstawienWProfilu();

        IList<ProfilKlienta> SprawdzIPopraw(int jezykId, IKlient zadajacyKlient, IList<ProfilKlienta> listaProfili, object parametryDoMetodyPoSelect);
        bool CzyWStalychFiltrachSaUlubioneWybrane(IKlient aktualnyKlient);
        Dictionary<long, T> PobierzKlientowZWartosciaUstawienia<T>(TypUstawieniaKlienta typ, string dodatkoweDane = null, AccesLevel dopisek = AccesLevel.Niezalogowani);
    }
}