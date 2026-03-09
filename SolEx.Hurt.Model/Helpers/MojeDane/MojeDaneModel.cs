using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Model.Helpers.MojeDane
{
    public class MojeDaneModel
    {
        [FriendlyName("Nazwa firmy")]
        [PoleEdytowane]
        public string Firma { get; set; }
        [PoleEdytowane]
        public string NIP { get; set; }
        [PoleEdytowane]
        public string Adres { get; set; }
        [PoleEdytowane]
        public string Miasto { get; set; }
        [FriendlyName("Kod pocztowy")]
        [PoleEdytowane]
        public string KodPocztowy { get; set; }

        [FriendlyName("Kraj")]
        [PoleEdytowane]
        public string Kraj { get; set; }

        [FriendlyName("Telefon kontaktowy")]
        [PoleEdytowane]
        public string NrTelefonu { get; set; }

        [FriendlyName("Uwagi")]
        [PoleEdytowane(TypKontrolki = "Text")]
        public string Uwagi { get; set; }

        [FriendlyName("WiadomoscEmail")]
        [PoleEdytowane(Grupa = "Konto na B2B")]
        public string Email { get; set; }

        public bool Status { get; set; }
        public bool Rezultat { get; set; }

        public List<ParametryPola> Pola { get; set; }

        //Domyslnie wysyla email do opiekuna
        public string EmailNaJakiOdeslacOdpowiedz { get;set;}

        public bool BlokujFormularz { get; set; }
        public string Opis { get; set; }

        public MojeDaneModel(){}

        public MojeDaneModel(string firma, string nip, string adres, string miasto, string kodPocztowy, string kraj,
            string nrTelefonu, string uwagi, string email, bool status, bool rezultat)
        {
            Firma = firma;
            NIP = nip;
            Adres = adres;
            Miasto = miasto;
            KodPocztowy = kodPocztowy;
            Kraj = kraj;
            NrTelefonu = nrTelefonu;
            Uwagi = uwagi;
            Email = email;
            Status = status;
            Rezultat = rezultat;
        }
    }

    
}