using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class DzialaniaUzytkownikow : IHasIntId
    {
        [AutoIncrement]
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }


        [WidoczneListaAdmin(true, true, false, false)]
        [Ignore]
        public Dictionary<string,string> Parametry { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Adres Ip")]
        public string IpKlienta { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("E-mail Klienta")]
        public string EmailKlienta { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Grupa Zdarzeń")]
        public ZdarzenieGrupa Zdarzenie { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Zadrzenie")]
        public ZdarzenieGlowne ZdarzenieGlowne { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        public DateTime Data { get; set; }


        public DzialaniaUzytkownikow()
        {
            Data = DateTime.Now;
        }
        public DzialaniaUzytkownikow(DzialaniaUzytkownikow baza):this()
        {
            if (baza == null)
            {
                return;
            }
            Id =baza.Id;
            Parametry=baza.Parametry;
            IpKlienta = baza.IpKlienta;
            EmailKlienta=baza.EmailKlienta;
            ZdarzenieGlowne=baza.ZdarzenieGlowne;
            Data = baza.Data;
            Zdarzenie = baza.Zdarzenie;
        }

        public DzialaniaUzytkownikow(ZdarzenieGlowne zdarzenie, Dictionary<string, string>  parametry) : this()
        {
            if(parametry== null) parametry = new Dictionary<string, string>();
            ZdarzenieGlowne = zdarzenie;
            Parametry = parametry;

            Zdarzenie = Refleksja.PobierzAtrybutDlaEnuma<ZdarzenieGrupaAttribute>(ZdarzenieGlowne).grupa;
        }

        public object Klucz()
        {
            return Id;
        }

    }
}
