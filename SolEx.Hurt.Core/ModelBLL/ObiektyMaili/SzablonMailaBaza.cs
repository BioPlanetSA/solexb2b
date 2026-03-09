using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using Attachment = System.Net.Mail.Attachment;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
   public abstract class SzablonMailaBaza
   {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        public virtual long Id => this.GetType().FullName.WygenerujIDObiektuSHAWersjaLong();

       //    public abstract 

       public List<Attachment> Zalaczniki { get; } = new List<Attachment>();

       public IKlient Klient { get; set; }
       protected SzablonMailaBaza(IKlient klient)
       {
           Klient = klient;
       }

        public SzablonMailaBaza() { }

       public void DodajZalaczniki(string[] sciezkadozalacznika)
       {
           if (sciezkadozalacznika != null)
           {
               foreach (var z in sciezkadozalacznika)
               {
                   Zalaczniki.Add(new Attachment(z));
               }
           }
       }
       public string BrakZdjecia => $"{Konfiguracja.DomyslneZdjecieSciezka}?preset={Konfiguracja.ZdjecieRozmiarWPowiadomieniach}";

       public Jezyk Jezyk()
       {
           return Konfiguracja.JezykiWSystemie[this.JezykMaila()];
       }

       public int JezykMaila()
       {
           switch (DoKogoWysylany)
           {
               case TypyPowiadomienia.Opiekun:
                   if (Klient.Opiekun != null)
                   {
                        if (Klient.Opiekun.JezykId == 0)
                        {
                            throw new Exception($"Opiekun: {Klient.Opiekun.Nazwa}[{Klient.OpiekunId}], klienta: {Klient.Nazwa}({Klient.Email}), nie ma przypisanego języka.");
                        }
                        return Klient.Opiekun.JezykId;
                   }
                   break;
               case TypyPowiadomienia.Przedstawiciel:
                   if (Klient.Przedstawiciel != null)
                   {
                       if (Klient.Przedstawiciel.JezykId == 0)
                       {
                            throw new Exception($"Przedstawiciel: {Klient.Przedstawiciel.Nazwa}[{Klient.PrzedstawicielId}], klienta: {Klient.Nazwa}({Klient.Email}), nie ma przypisanego języka.");
                       }
                       return Klient.Przedstawiciel.JezykId;
                   }
                   break;
               case TypyPowiadomienia.DrugiOpiekun:
                   if (Klient.DrugiOpiekun != null)
                   {
                        if (Klient.DrugiOpiekun.JezykId == 0)
                        {
                            throw new Exception($"Drugi Opiekun: {Klient.DrugiOpiekun.Nazwa}[{Klient.DrugiOpiekunId}], klienta: {Klient.Nazwa}({Klient.Email}), nie ma przypisanego języka.");
                        }
                        return Klient.DrugiOpiekun.JezykId;
                   }
                   break;
               case TypyPowiadomienia.Klient:
                   return Klient.JezykId;

           }
          throw new Exception("Nie obsługiwany typ powiadomienia");
       }


       public IConfigBLL Konfiguracja
       {
           get;
           set;
       }

       public TypyPowiadomienia DoKogoWysylany
       {
           get;
           set;
       }
       public virtual string NazwaSzablonu()
       {
           return   GetType().Name;
       }

       public abstract string NazwaFormatu();
       public abstract string OpisFormatu();
        public abstract string OpisDlaKlienta();
        public virtual TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien => new[] { TypyPowiadomienia.Klient, TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel ,TypyPowiadomienia.DrugiOpiekun };

       public virtual TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne => new[] { TypyPowiadomienia.Klient, TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel, TypyPowiadomienia.DrugiOpiekun };

       public string Typ()
       {
           return GetType().PobierzOpisTypu();
       }

        public bool ZgodaNaZmianyPrzezKlienta { get; set; }
    }
}
