using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    /// <summary>
    /// Model szablonu katalogowego, wykorzystywany podczas drukowania oferty detalicznej
    /// </summary>
    [FriendlyName("Szablon wydruku katalogu")]
    [Alias("KatalogSzablon")]
    public class KatalogSzablonModelBLL: IKatalogSzablonModelBLL, IObiektWidocznyDlaOkreslonychGrupKlientow, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        private readonly IKlient _klient;
        private readonly int _jezyk;

        public KatalogSzablonModelBLL()
        {
            _jezyk =  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski;
        }

        public KatalogSzablonModelBLL(IKlient customer, int jezyk)
        {
            _klient = customer;
            _jezyk = jezyk;
        }

        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false, true)]
        public WidocznosciTypow Widocznosc { get; set; }

        [Ignore]
        public IObrazek Obrazek0
        {
            get
            {
                if (Obrazek0Id != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(Obrazek0Id.Value);
                }
                return null;
            }
        }
        [Ignore]
        public IObrazek Obrazek1
        {
            get
            {
                if (Obrazek1Id != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(Obrazek1Id.Value);
                }
                return null;
            }
        }
        [Ignore]
        public IObrazek Obrazek2
        {
            get
            {
                if (Obrazek2Id != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(Obrazek2Id.Value);
                }
                return null;
            }
        }
        [Ignore]
        public IObrazek Obrazek3
        {
            get
            {
                if (Obrazek3Id != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(Obrazek3Id.Value);
                }
                return null;
            }
        }
        [Ignore]
        public IObrazek Obrazek4
        {
            get
            {
                if (Obrazek4Id != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(Obrazek4Id.Value);
                }
                return null;
            }
        }
        [Ignore]
        public IObrazek Obrazek5
        {
            get
            {
                if (Obrazek5Id != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(Obrazek5Id.Value);
                }
                return null;
            }
        }

        [PrimaryKey]
        [UpdateColumnKey()]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }

        [Lokalizowane]
        [GrupaAtttribute("Podstawowe dane", 0)]
        [WidoczneListaAdmin(true, true, true, false)]
        public string Nazwa { get; set; }

        [GrupaAtttribute("Podstawowe dane", 0)]
        [FriendlyName("Plik szablonu")]
        [WidoczneListaAdmin(true, true, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PolePlik)]
        [WymuszoneRoszerzeniePliku( "'mrt'" )]
        public int? PlikSzablonuId { get; set; }

        [Ignore]
        public Plik PlikSzablonu
        {
            get
            {
                if (this.PlikSzablonuId != null)
                {
                    return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Plik>(this.PlikSzablonuId);
                }
                return null;
            }
        }


        [GrupaAtttribute("Podstawowe dane", 0)]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool Aktywny { get; set; }

        [Niewymagane]
        [FriendlyName("Parametr 1")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy1 { get; set; }

        [Niewymagane]
        [FriendlyName("Parametr 2")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy2 { get; set; }

        [Niewymagane]
        [FriendlyName("Parametr 3")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy3 { get; set; }

        [Niewymagane]
        [FriendlyName("Parametr 4")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy4 { get; set; }
        [Niewymagane]
        [FriendlyName("Parametr 5")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy5 { get; set; }
        [Niewymagane]
        [FriendlyName("Parametr 6")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy6 { get; set; }
        [Niewymagane]
        [FriendlyName("Parametr 7")]
        [GrupaAtttribute("Parametry", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string ParametrTekstowy7 { get; set; }
        [Niewymagane]
        [FriendlyName("Obrazek 0")]
        [GrupaAtttribute("Obrazki", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? Obrazek0Id { get; set; }
        [Niewymagane]
        [FriendlyName("Obrazek 1")]
        [GrupaAtttribute("Obrazki", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? Obrazek1Id { get; set; }
        [Niewymagane]
        [FriendlyName("Obrazek 2")]
        [GrupaAtttribute("Obrazki", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? Obrazek2Id { get; set; }
        [Niewymagane]
        [FriendlyName("Obrazek 3")]
        [GrupaAtttribute("Obrazki", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? Obrazek3Id { get; set; }
        [Niewymagane]
        [FriendlyName("Obrazek 4")]
        [GrupaAtttribute("Obrazki", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? Obrazek4Id { get; set; }
        [Niewymagane]
        [FriendlyName("Obrazek 5")]
        [GrupaAtttribute("Obrazki", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? Obrazek5Id { get; set; }

        [FriendlyName("Katalog można zapisać jako")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikFormatZapisu))]
        public HashSet<KatalogFormatZapisu> DozwoloneFormatyWydruku { get; set; }

        [FriendlyName("Role użytkowników z dostępem do szablonu")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikRol))]
        public HashSet<RoleType> DostepnyDla { get; set; }

        [Ignore]
        public int JezykId { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
    }


}
