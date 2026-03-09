using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.RegulyPunktowe;
using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Zadanie")]
    [FriendlyName("Zadanie", FriendlyOpis = "")]
    public class ZadanieBll : Zadanie, IPoleJezyk
    {
        private ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        public override Dictionary<string, object> ParametryLokalizowane()
        {
            Dictionary<string, object> nadpisywane = null;
            if (JezykId != Calosc.Konfiguracja.JezykIDDomyslny)
            {
                string typ =typeof (ModulStowrzonyNaPodstawieZadania).PobierzOpisTypu();
                nadpisywane = Calosc.DostepDane.Pobierz<Tlumaczenie>(null, x => x.ObiektId == Id && x.JezykId == JezykId && x.Typ == typ).ToDictionary(x => x.Pole, x => (object)x.Wpis);
            }
            return nadpisywane;
        }
        [Ignore]
        public int JezykId { get; set; }

        private ModulStowrzonyNaPodstawieZadania _kontrolka;

        public ZadanieBll()
        {
            //JezykId = Calosc.Konfiguracja.JezykIDDomyslny;
        }

        public ZadanieBll(ZadanieBll x):base(x)
        {
            JezykId = x.JezykId;
        }
        public ModulStowrzonyNaPodstawieZadania Modul()
        {
                if (_kontrolka == null)
                {
                    _kontrolka = this.StworzKontrolke<ModulStowrzonyNaPodstawieZadania>();
                    _kontrolka.ZadanieBazowe = this;
                }
                return _kontrolka;
        }

        [Ignore]
        public Type Typ { get; set; } // => string.IsNullOrEmpty(this.ModulFullTypeName) ? null : Type.GetType(this.ModulFullTypeName);

        [Ignore]
        [FriendlyName("Nazwa")]
        [WidoczneListaAdmin(true, true, false, false, false, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulKoszyka,SolEx.Hurt.Core" })]
        public string NazwaZadanie
        {
            get
            {
                if (this.ModulFullTypeName == null)
                {
                    return null;
                }
                FriendlyNameAttribute opisy = Modul().GetType().GetCustomAttribute<FriendlyNameAttribute>();
                return (opisy != null && !string.IsNullOrEmpty(opisy.FriendlyName)) ? opisy.FriendlyName : Modul().Nazwa;
            }
        }

        [Ignore]
        [FriendlyName("Opis")]
        [WidoczneListaAdmin(true, false, false, false, false, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulKoszyka,SolEx.Hurt.Core" })]
        public string OpisZadanie
        {
            get
            {
                if (this.ModulFullTypeName == null)
                {
                    return null;
                }
                FriendlyNameAttribute opisy = Modul().GetType().GetCustomAttribute<FriendlyNameAttribute>();
                return (opisy != null) ? opisy.FriendlyOpis : Modul().Opis; ;
            }
        }

        [Ignore]
        [WidoczneListaAdmin(true, true, false, false, false, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulKoszyka,SolEx.Hurt.Core" })]
        [FriendlyName("Lista parametrów")]
        public List<OpisPolaObiektu> ListaParametrów
        {
            get
            {
                if (this.ModulFullTypeName == null)
                {
                    return null;
                }

                return OpisObiektu.PobranieParametowObiektu(Modul(), 0, true);
            }
        }

        public HashSet<TypZadania> PobierzGrupyDoJakichPasujeZadanie()
        {
            HashSet<TypZadania> wynik = new HashSet<TypZadania>();
            if (string.IsNullOrEmpty(ModulFullTypeName))
            {
                return wynik;
            }
            Type typ = Modul().GetType();
            var interfejscy = typ.GetInterfaces();
            if (interfejscy.Contains(typeof(IZadanieSynchronizacji)))
            {
                wynik.Add(TypZadania.Synchronizacja);
            }
            if (interfejscy.Contains(typeof(IWarunekRegulyCalegoDokumentu)))
            {
                wynik.Add(TypZadania.WarunekRegulyPunktowej);
            }
            if (interfejscy.Contains(typeof(IWarunekRegulyPozycjiDokumentu)))
            {
                wynik.Add(TypZadania.WarunekRegulyPunktowej);
            }
            if (interfejscy.Contains(typeof(IRegulaCalegoKoszyka)))
            {
                wynik.Add(TypZadania.WarunekRegulyKoszyka);
            }
            if (interfejscy.Contains(typeof(IRegulaPozycji)))
            {
                wynik.Add(TypZadania.WarunekRegulyKoszyka);
            }
            if (typ.IsDerivativeOf(typeof(ZadanieKoszyka)))
            {
                wynik.Add(TypZadania.RegulaKoszyka);
            }
            if (typ.IsDerivativeOf(typeof(RegulaPunktowa)))
            {
                wynik.Add(TypZadania.RegulaPunktowa);
            }
            return wynik;
        }
    }
}
