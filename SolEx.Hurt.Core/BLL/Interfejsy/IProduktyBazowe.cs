using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IProduktyBazowe
    {
        /// <summary>
        /// termin najblizszej dostawy CYKLICZNEJ. nie mylic z terminem dostawy erp
        /// </summary>
        DateTime? NajblizszaDostawa(ProduktBazowy produkt);

   
        bool MoznaDodacDoKoszyka(IProduktKlienta p, out string info, out bool pokazujCene);

        bool MoznaDodacDoPoinformujODostepnosci(IProduktKlienta produkt);

        ProduktBazowy Pobierz(string symbol, int jezykId, IKlient kontekst = null);


        void ZapiszJednostkiProduktow(List<ProduktBazowy> listaProduktow);

        void ZapiszZdjecieProduktu(IList<ProduktBazowy> obj);

        //List<Podpowiedz> Znajdz(string searchString, int count, int lang, string provider);
        List<ProduktBazowy> ZnajdzProdukty(string searchString, int? lang = null);

        IEnumerable<long> PobierzProduktyPasujaceDoszukania(int jezyk, string szukane);

        IEnumerable<long> PobierzProduktyPasujaceDoszukania(int jezyk, HashSet<long> produktyIds, string szukane);

        TypStanu WyliczTypStanu(IProduktBazowy produkt);

       // bool MoznaDodacDoKoszyka(IProduktKlienta p);
        IList<ProduktBazowy>  MetodaPrzetwarzajacaPoSelect_UzupelnijProdutkBazowy(int jezykID, IKlient klient, IList<ProduktBazowy> listaProduktow, object opcjonalmnyParametr);

        IList<Indywidualizacja> BindPoSelectIndywidualizacji(int jezykId, IKlient zadajacy, IList<Indywidualizacja> obj,object parametrDoMetodyPoSelect = null);


        void BindPoAktualizacjiIndywidualizacji(IList<Indywidualizacja> obj);

    }
}