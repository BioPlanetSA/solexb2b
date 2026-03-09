using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.DostepDane.DaneObiektu
{
    public class OpisPolaObiektuBaza
    {
        private readonly WidoczneListaAdminAttribute _widoczneAdmin;
        private readonly GrupaAtttribute _grupa;

        public TypEdytora PobierzTypEdytora()
        {
            if (WymuszonyTypEdytora != null)
            {
                return WymuszonyTypEdytora.Value;
            }
            if (TypPrzechowywanejWartosci.PobierzPodstawowyTyp().IsEnum)
            {
                return TypEdytora.PoleDropDown;
            }

            if (Slownik != null)
            {
                if (Slownik.Count < 1000)
                {
                    return TypEdytora.PoleDropDown;
                }
                return TypEdytora.PoleZeSlownikiem;
            }

            if (TypPrzechowywanejWartosci == typeof(AdresUrl))
            {
                return TypEdytora.AdresUrl;
            }

            if (TypPrzechowywanejWartosci == typeof(bool) || TypPrzechowywanejWartosci == typeof(bool?))
            {
                return TypEdytora.PoleBool;
            }

            if (TypPrzechowywanejWartosci == typeof(DateTime) || TypPrzechowywanejWartosci == typeof(DateTime?))
            {
                return TypEdytora.PoleDatoweZCzasem;
            }

            if (TypPrzechowywanejWartosci.GetInterfaces().Contains(typeof(IEnumerable)) && TypPrzechowywanejWartosci != typeof(string))
            {
                return TypEdytora.IEnumerable;
            }

            return TypEdytora.PoleTekstowe;
        }

        public GrupaAtttribute Grupa
        {
            get { return _grupa; }
        }

        public OpisPolaObiektuBaza()
        {
            _widoczneAdmin = new WidoczneListaAdminAttribute(false, false, false, false, false);
            _grupa = new GrupaAtttribute("Pozostałe", 99);
        }

        public OpisPolaObiektuBaza(OpisPolaObiektuBaza baza)
        {
            _widoczneAdmin = baza.ParamatryWidocznosciAdmin;
            IdentyfikatorObiektu = baza.IdentyfikatorObiektu;
            _grupa = baza.Grupa;
            LinkDoDokumentacji = baza.LinkDoDokumentacji;
            Multiselect = baza.Multiselect;
            NazwaPola = baza.NazwaPola;
            NazwaWyswietlana = baza.NazwaWyswietlana;
            NazwaWyswietlanaTlumaczona = NazwaWyswietlanaTlumaczona;
            OpisPola = baza.OpisPola;
            PobieraneErp = baza.PobieraneErp;
            PobieraneErpModul = baza.PobieraneErpModul;
            Property = baza.Property;
            TylkoDoOdczytu = baza.TylkoDoOdczytu;
            NazwaPola = baza.NazwaPola;
            TypSlownika = baza.TypSlownika;
            TypPrzechowywanejWartosci = baza.TypPrzechowywanejWartosci;
            WymuszonyTypEdytora = baza.WymuszonyTypEdytora;
            WalidatorDanych = baza.WalidatorDanych;
            Wymagane = baza.Wymagane;
            RodzajDanych = baza.RodzajDanych;
            RodzajDanychObiektuNadrzednego = baza.RodzajDanychObiektuNadrzednego;
            Slownik=baza.Slownik;
            AtrybutyPropertisa = baza.AtrybutyPropertisa;
            Tlumaczone = baza.Tlumaczone;
        }

        public OpisPolaObiektuBaza(WidoczneListaAdminAttribute widocznosc, GrupaAtttribute grupa)
        {
            _widoczneAdmin = widocznosc;
            _grupa = grupa;
        }

        public List<Attribute> AtrybutyPropertisa;

        public OpisPolaObiektuBaza(PropertyInfo p, string identyfikatorObiektu):this()
        {
            AdminDozwoloneFiltrowanieLiscie = true;
            AdminDozwoloneSortowanieLiscie = true;

            IdentyfikatorObiektu = identyfikatorObiektu;

            AtrybutyPropertisa = p.GetCustomAttributes().ToList();

            FriendlyNameAttribute atribut = AtrybutyPropertisa.FirstOrDefault(x => x is FriendlyNameAttribute) as FriendlyNameAttribute;
            string opis = string.Empty;
            if (atribut != null)
            {
                opis = atribut.FriendlyOpis;
            }
            
            WidoczneListaAdminAttribute w = AtrybutyPropertisa.FirstOrDefault(x => x is WidoczneListaAdminAttribute) as WidoczneListaAdminAttribute;

            if (AtrybutyPropertisa.Exists(x => x is IgnoreAttribute))
            {
                PoleIgnorowaneWBazieSQL = true;
            }
            else
            {
                PoleIgnorowaneWBazieSQL = false;
            }

            if (w != null)
            {
                _widoczneAdmin = w;
            }

            Type typslownika;
            Dictionary<string, object> slownik = p.ZbudujSlownikDlaPola(out typslownika);
            TypSlownika = typslownika;
            ObsoleteAttribute ob = AtrybutyPropertisa.FirstOrDefault(x => x is ObsoleteAttribute) as ObsoleteAttribute;
            string nazwaOpsolete;
            string nazwapola = atribut != null ? atribut.FriendlyName : p.Name;
            string opispola = opis;
            if (ob != null)
            {
                nazwaOpsolete = nazwapola + "Pole wycofane " + ob.Message;
            }
            else
            {
                nazwaOpsolete = nazwapola;
            }
            if (atribut != null)
            {
                NazwaWyswietlanaTlumaczona = atribut.Tlumaczony;
            }
            NazwaPola = p.Name;
            OpisPola = opispola;
            TypPrzechowywanejWartosci = p.PropertyType;
            NazwaWyswietlana = nazwaOpsolete;
            Slownik = slownik;
            Multiselect = CzyMultiselect(p);
            Tlumaczone = AtrybutyPropertisa.Exists(x => x is Lokalizowane);

            Wymagane = w.Edytowalne && (!AtrybutyPropertisa.Exists(x => x is Niewymagane) && !p.PropertyType.IsNullableType());
            Property = p;

            WymuszonyTypEdytora wymuszony = AtrybutyPropertisa.FirstOrDefault(x => x is WymuszonyTypEdytora) as WymuszonyTypEdytora;
            if (wymuszony != null)
            {
                WymuszonyTypEdytora = wymuszony.Typ;
                RodzajDanych = wymuszony.RodzajDanych;
            }
            WalidatorDanych walidator = AtrybutyPropertisa.FirstOrDefault(x => x is WalidatorDanych) as WalidatorDanych;
            if (walidator != null)
            {
                WalidatorDanych = walidator.Typ;
            }
            GrupaAtttribute grupaatr = AtrybutyPropertisa.FirstOrDefault(x => x is GrupaAtttribute) as GrupaAtttribute;
            if (grupaatr != null)
            {
                _grupa = grupaatr;
            }
        }

        public WidoczneListaAdminAttribute ParamatryWidocznosciAdmin
        {
            get { return _widoczneAdmin; }
        }
        private bool CzyMultiselect(PropertyInfo p)
        {
            return p.PropertyType.IsArray || (p.PropertyType.IsGenericType && p.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)));
        }
        /// <summary>
        /// Wymuszony typ edytora danych, jeśli null to edytor jest wyznaczany na podstawie parametrów
        /// </summary>
        public TypEdytora? WymuszonyTypEdytora { get; set; }
        public Type TypSlownika { get; set; }

        public PropertyInfo Property { get; set; }

        public bool Tlumaczone { get; set; }


        public string NazwaPola { get; set; }
        public string OpisPola { get; set; }
        public string NazwaWyswietlana { get; set; }
        public bool NazwaWyswietlanaTlumaczona { get; set; }
        public Type TypPrzechowywanejWartosci { get; set; }

        private Dictionary<string, string> _slownikOdwroconyWgWartosci = null;

        public bool PoleIgnorowaneWBazieSQL { get; set; }
        public bool AdminDozwoloneSortowanieLiscie { get; set; }
        public bool AdminDozwoloneFiltrowanieLiscie { get; set; }

        public Dictionary<string, string> SlownikWedlugWartosciStringowych()
        {
            if (_slownikOdwroconyWgWartosci == null)
            {
                _slownikOdwroconyWgWartosci = Slownik.ToDictionary(x => x.Value.ToString(), x => x.Key);
            }
            return _slownikOdwroconyWgWartosci;
        }

        public Dictionary<string, object> Slownik { get; set; }
        /// <summary>
        /// Czy pole jest wielokrotnego wyboru
        /// </summary>
        public bool Multiselect { get; set; }
        /// <summary>
        /// Czy pole musi mieć wpisaną wartość
        /// </summary>
        public bool Wymagane { get; set; }
        /// <summary>
        /// /Typ walidatora danych, jeśli null to wartość nie musi być walidowana
        /// </summary>
        public Type WalidatorDanych { get; set; }
        public Type RodzajDanych { get; set; }
        public Type RodzajDanychObiektuNadrzednego { get; set; }
        /// <summary>
        /// IDentyfikator obiektu do którego należy dane pole
        /// </summary>
        public string IdentyfikatorObiektu { get; set; }
        /// <summary>
        /// link pobrany z atrybutu LinkDokumentacji
        /// </summary>
        public string LinkDoDokumentacji { get; set; }
        /// <summary>
        /// Pole jest pobierane z erp i nie wolno go modyfikować w adminie
        /// </summary>
        public bool PobieraneErp { get; set; }
        /// <summary>
        /// Pole jest tylko do odczytu
        /// </summary>
        public bool TylkoDoOdczytu { get; set; }

        //public void PobierzWartoscPolaObiektu(object o)
        //{
        //    if (Property == null)
        //    {
        //        Property = o.GetType().GetProperty(NazwaPola);
        //    }
        //    if (Property == null)
        //    {
        //        throw new InvalidOperationException("Nie znaleziono pola o nazwie " + NazwaPola);
        //    }
        //    Wartosc = Property.Get(o);
        //}


        

        public int? PobieraneErpModul { get; set; }

        
    }
}
