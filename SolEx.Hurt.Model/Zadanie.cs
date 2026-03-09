using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using System.Text;
using SolEx.Hurt.Model.Interfaces;
using DateTimeHelper = SolEx.Hurt.Model.Helpers.DateTimeHelper;

namespace SolEx.Hurt.Model
{
    public class Zadanie : IHasIntId, IObiektPrzechowujacyKontrolke,IPolaIDentyfikujaceRecznieDodanyObiekt, IPoleJezyk
    {
        


        [AutoIncrement]
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false, false)]
        public int Id { get; set; }

        [Ignore]
        protected ILog Log => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Ignore]
        [FriendlyName("Nazwa Zadania synchronizacji")]
        [WidoczneListaAdmin(true, true, false, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core" })]
        public string NazwaZadania => TypZadaniaSynchronizacji?.ToString() ?? Id.ToString();

        [Ignore]
        [FriendlyName("Zadanie synchronizacji")]
        [WidoczneListaAdmin(true, true, false, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core" })]
        public ElementySynchronizacji? TypZadaniaSynchronizacji
        {
            get
            {
                if (!NumerElementuSynchronizacji.HasValue)
                {
                    return null;
                }
                return ((ElementySynchronizacji)NumerElementuSynchronizacji.Value);
            }
        }


        [WidoczneListaAdmin(true, false, false, false, false, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core"})]
        [FriendlyName("Czas ostatniego uruchomienia")]
        public DateTime? OstatnieUruchomienieStart { get; set; }

         [WidoczneListaAdmin(true, false, false, false, false, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core" })]
         [FriendlyName("Czas zakończenia działania")]
        public DateTime? OstatnieUruchomienieKoniec { get; set; }

        [FriendlyName("Działa od godziny")]
        [WidoczneListaAdmin(true, true, true, true, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core" })]
        
        public int MozeDzialacOdGodziny { get; set; }

        [FriendlyName("Działa do godziny")]
        [WidoczneListaAdmin(true, true, true, true, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core" })]
        public int MozeDzialacDoGodziny { get; set; }

        [FriendlyName("Przerwa")]
        [WidoczneListaAdmin(true, true, true, true, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core" })]
        public int IleMinutCzekacDoKolejnegoUruchomienia { get; set; }

        public int? NumerElementuSynchronizacji { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        public bool Aktywne { get; set; }

        [Lokalizowane]
        public string Parametry { get; set; }
        //modul odatkowy

        [FriendlyName("Nazwa typu")]
        //[WidoczneListaAdmin(true, false, false, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core" })]
        public string ModulFullTypeName { get; set; }
        
        [FriendlyName("Kolejność")]
        [WidoczneListaAdmin(true, true, true, true, true, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulKoszyka,SolEx.Hurt.Core" })]
        public int ModulKolejnosc { get; set; }

        [WidoczneListaAdmin(true, false, false, false, false)]
        public int? ZadanieNadrzedne { get; set; }
        public int? OddzialId { get; set; }
        public bool Centralne { get; set; }


        [WidoczneListaAdmin(true, true, false, false, false)]
        public bool ModulWymagany { get; set; }
        public bool Usuniente { get; set; }
        public Zadanie()
        {
            MozeDzialacOdGodziny = 0;
            MozeDzialacDoGodziny = 24;
            Aktywne = true;
        }

        public Zadanie(Zadanie z)
        {
            Id = z.Id;

            OstatnieUruchomienieStart = z.OstatnieUruchomienieStart;
            OstatnieUruchomienieKoniec = z.OstatnieUruchomienieKoniec;


            MozeDzialacOdGodziny = z.MozeDzialacOdGodziny;
            MozeDzialacDoGodziny = z.MozeDzialacDoGodziny;
            IleMinutCzekacDoKolejnegoUruchomienia = z.IleMinutCzekacDoKolejnegoUruchomienia;
            NumerElementuSynchronizacji = z.NumerElementuSynchronizacji;
            Aktywne = z.Aktywne;
            Parametry = z.Parametry;
            ModulFullTypeName = z.ModulFullTypeName;
            ModulKolejnosc = z.ModulKolejnosc;
            ZadanieNadrzedne = z.ZadanieNadrzedne;
            OddzialId = z.OddzialId;
            Centralne = z.Centralne;
            ModulWymagany = z.ModulWymagany;
            Usuniente = z.Usuniente;
        }

        public DateTimeHelper _biezacaData = DateTimeHelper.PobierzInstancje;

        [Ignore]
        public bool CzyPowinnoBycUruchomioneTeraz
        {
            get
            {
                if (!Aktywne)
                {
                    return false;
                }
                DateTime biezacaData = _biezacaData.BiezacaData();
                if ((MozeDzialacDoGodziny == 0 && MozeDzialacOdGodziny == 0) ||
                
                    (MozeDzialacDoGodziny > MozeDzialacOdGodziny && biezacaData.Hour >= MozeDzialacOdGodziny &&
                     biezacaData.Hour <= MozeDzialacDoGodziny)
                    ||
                    
                    (MozeDzialacDoGodziny < MozeDzialacOdGodziny &&
                     (biezacaData.Hour >= MozeDzialacOdGodziny || biezacaData.Hour < MozeDzialacDoGodziny))
                    )
                {
                    if (!OstatnieUruchomienieKoniec.HasValue || !OstatnieUruchomienieStart.HasValue)
                    {
                   
                        return true;
                    }

                    if (OstatnieUruchomienieKoniec < OstatnieUruchomienieStart)
                    {
                   
                        return true;
                    }

                    if ((biezacaData - OstatnieUruchomienieKoniec).Value.TotalMinutes >
                        IleMinutCzekacDoKolejnegoUruchomienia)
                    {
                    
                        return true;
                    }
                }
                return false;
            }
        }

        [Ignore]
        [WidoczneListaAdmin(true, true, false, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core" })]
        [FriendlyName("Ostatnie uruchomienie")]
        public string OstatnieUruchomieniePrzyjaznyOpis
        {
            get
            {
                string s = "";
                s = OstatnieUruchomienieStart.HasValue ? "od " + OstatnieUruchomienieStart.ToString() : "";
                if (OstatnieUruchomienieKoniec.HasValue)
                {
                    s += "\r\n do " + OstatnieUruchomienieKoniec.ToString();
                }
                return s;
            }
        }

        [FriendlyName("Czas trwania [s]", FriendlyOpis = "Czas trwania ostatniego uruchomienia")]
        [WidoczneListaAdmin(true, true, false, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.HarmonogramBll,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core" })]
        [Ignore]
        public int ostatniCzasTrwaniaMinuty
        {
            get
            {
                if (OstatnieUruchomienieStart.HasValue && OstatnieUruchomienieKoniec.HasValue && (OstatnieUruchomienieKoniec.Value > OstatnieUruchomienieStart.Value))
                {
                    return (int)(OstatnieUruchomienieKoniec.Value - OstatnieUruchomienieStart.Value).TotalSeconds;
                }
                return -1;
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.NazwaZadania);

            if (!OstatnieUruchomienieStart.HasValue || !OstatnieUruchomienieKoniec.HasValue)
            {
                //sb.Append("Zadanie dotychczas nie było uruchomione. \r\n");
            }
            else
            {
                sb.Append($". Ostatnie uruchomienie: {OstatnieUruchomienieStart}  -  {OstatnieUruchomienieKoniec} \r\n");
            }

            sb.Append($". Uruchamiane pomiędzy godzinami: {MozeDzialacOdGodziny} - {MozeDzialacDoGodziny} ");
            sb.Append($"z przerwą {IleMinutCzekacDoKolejnegoUruchomienia} minut");

            return sb.ToString();

        }

        public string PobierzGrupe()
        {
            if (TypZadaniaSynchronizacji != null)
            {
                MemberInfo memberInfo = typeof (ElementySynchronizacji).GetMember(TypZadaniaSynchronizacji.ToString())
                    .FirstOrDefault();

                if (memberInfo != null)
                {
                    GrupaZadanSynchronizacjiAttribute attribute = (GrupaZadanSynchronizacjiAttribute)
                        memberInfo.GetCustomAttributes(typeof (GrupaZadanSynchronizacjiAttribute), false)
                            .FirstOrDefault();
                    return attribute == null ? "Brak grupy" : attribute.NazwaGrupy;
                }
            }
            return "";
        }

        //[Ignore]
        //[WidoczneListaAdmin(true, true, false, false, true, new[] { "SolEx.Hurt.Core.ModelBLL.ModulSynchronizacji,SolEx.Hurt.Core", "SolEx.Hurt.Core.ModelBLL.ModulKoszyka,SolEx.Hurt.Core" })]
        //public string JakiejOperacjiSynchronizacjiDotyczyAdmin
        //{
        //    get
        //    {
        //        string operacje = string.Empty;
        //        var interfejsy = this.GetType().GetInterfaces();
        //        foreach (var inter in interfejsy)
        //        {
        //            List<SyncJakaOperacjaAttribute> syncAtr =
        //                inter.GetCustomAttributes(typeof(SyncJakaOperacjaAttribute), true).Select(a => a as SyncJakaOperacjaAttribute).ToList();
        //            foreach (SyncJakaOperacjaAttribute syncJakaOperacjaAttribute in syncAtr)
        //            {
        //                operacje += syncJakaOperacjaAttribute.operacja + koncowka;
        //            }
        //        }


        //        if (string.IsNullOrEmpty(operacje))
        //            throw new Exception(
        //            "Klasa modulu nie implementuje żadnego interfejsu ze zdefiniowanym atrybutem SyncJakaOperacjaAttribute");

        //        return operacje.TrimEnd(koncowka);
        //    }
        //}

        public string TypKontrolki()
        {
            return ModulFullTypeName;
        }

        public virtual string ParametrySerializowane()
        {
            return Parametry;
        }
        public virtual Dictionary<string, object> ParametryLokalizowane()
        {
            return null;
        }
        public void UstawParametrySerializowane(string parametry)
        {
            Parametry = parametry;
        }

        public bool RecznieDodany()
        {
            return true;
        }

        [Ignore]
        public int JezykId { get; set; }
    }
}
