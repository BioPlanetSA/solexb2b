using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model rejestracji klienta
    /// </summary>
    public class Rejestracja : IHasIntId, IBindowalny,IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public Rejestracja(Rejestracja baza)
        {
            this.KopiujPola(baza);
        }

        public Rejestracja(){}

        [PrimaryKey]
        [AutoIncrement]
        
        //Podstawowe informacje
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }

        [FriendlyName("Podstawowe - nazwa firmy")]
        [PoleEdytowane]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public virtual string Nazwa { get; set; }

        [FriendlyName("Podstawowe - NIP")]
        [PoleEdytowane]
        [Niewymagane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public virtual string Nip { get; set; }
        
        [FriendlyName("Podstawowe - opis działalności")]
        [Niewymagane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [WidoczneListaAdmin(true, false, true, false)]
        public virtual string RodzajDzialalnosci { get; set; }

        [FriendlyName("Zamawiający - imię i nazwisko")]
        [PoleEdytowane(Grupa = "Zamawiający")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, false)]
        public virtual string ImieNazwisko { get; set; }

        [PoleEdytowane(Grupa = "Zamawiający", TypKontrolki = "WiadomoscEmail")]
        [FriendlyName("Zamawiający - email")]
        [WidoczneListaAdmin(true, false, true, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public virtual string Email { get; set; }

        [FriendlyName("Zamawiający - telefon")]
        [PoleEdytowane(Grupa = "Zamawiający")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false)]
        public virtual string Telefon { get; set; }

        //Inne
        [FriendlyName("Zamawiający - hasło")]
        [PoleEdytowane(Grupa = "Zamawiający", TypKontrolki = "Haslo")]
        [Niewymagane]
        public virtual string HasloJednorazowe { get; set; }

        [FriendlyName("Podstawowe - osoba polacająca")]
        [Niewymagane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string Polecajcacy { get; set; }
        
        //Status klienta
        [GrupaAtttribute("Status klienta", 1)]
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Status eksportu")]
        public RegisterExportStatus StatusEksportu { get; set; }
        
        [GrupaAtttribute("Status klienta", 1)]
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false)]
        public RegisterStatus Status { get; set; }

        //Dane adresowe
        [FriendlyName("Adres firmy - ulica")]
        [Niewymagane]
        [PoleEdytowane(Grupa = "Adres firmy")]
        [GrupaAtttribute("Dane adresowe", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public virtual string Ulica { get; set; }

        [PoleEdytowane(Grupa = "Adres firmy")]
        [FriendlyName("Adres firmy - miasto")]
        [Niewymagane]
        [GrupaAtttribute("Dane adresowe", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public virtual string Miasto { get; set; }

        [FriendlyName("Adres firmy - kod pocztowy")]
        [PoleEdytowane(Grupa = "Adres firmy")]
        [Niewymagane]
        [GrupaAtttribute("Dane adresowe", 2)]
        [WidoczneListaAdmin(true, false, true, false)]
        public virtual string KodPocztowy { get; set; }

        [FriendlyName("Adres firmy - państwo")]
        [PoleEdytowane(Grupa = "Adres firmy", TypKontrolki = "DropDown")]
        [GrupaAtttribute("Dane adresowe", 2)]
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPanstwZSymbolem,SolEx.Hurt.Core")]
        public virtual string Panstwo { get; set; }

        //Załączniki
        [FriendlyName("Dokumenty rejestrowe firmy - załącznik rejestracji 1")]
        [PoleEdytowane(Grupa = "Dokumenty rejestrowe firmy", TypKontrolki = "Plik")]
        [GrupaAtttribute("Załączniki", 4)]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, false, false)]
        public virtual string Zalacznik1 { get; set; }

        [PoleEdytowane(Grupa = "Dokumenty rejestrowe firmy", TypKontrolki = "Plik")]
        [FriendlyName("Dokumenty rejestrowe firmy - załącznik rejestracji 2")]
        [GrupaAtttribute("Załączniki", 4)]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, false, false)]
        public string Zalacznik2 { get; set; }

        [PoleEdytowane(Grupa = "Dokumenty rejestrowe firmy", TypKontrolki = "Plik")]
        [FriendlyName("Dokumenty rejestrowe firmy - załącznik rejestracji 3")]
        [GrupaAtttribute("Załączniki", 4)]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, false, false)]
        public string Zalacznik3 { get; set; }

        //Opcjonalne dane do wysyłki
        [FriendlyName("Wysyłka - ulica")]
        [Niewymagane]
        [PoleEdytowane(Grupa = "Adres wysyłki")]
        [GrupaAtttribute("Opcjonalne dane do wysyłki", 3)]
        [WidoczneListaAdmin(false, false, true, false)]
        public virtual string WysylkaUlica { get; set; }

        [FriendlyName("Wysyłka - miasto")]
        [PoleEdytowane(Grupa = "Adres wysyłki")]
        [Niewymagane]
        [GrupaAtttribute("Opcjonalne dane do wysyłki", 3)]
        [WidoczneListaAdmin(false, false, true, false)]
        public virtual string WysylkaMiasto { get; set; }

        [FriendlyName("Wysyłka - kod pocztowy")]
        [PoleEdytowane(Grupa = "Adres wysyłki")]
        [Niewymagane]
        [GrupaAtttribute("Opcjonalne dane do wysyłki", 3)]
        [WidoczneListaAdmin(false, false, true, false)]
        public virtual string WysylkaKodPocztowy { get; set; }

        [FriendlyName("Wysyłka - państwo")]
        [PoleEdytowane(Grupa = "Adres wysyłki")]
        [Niewymagane]
        [GrupaAtttribute("Opcjonalne dane do wysyłki", 3)]
        [WidoczneListaAdmin(false, false, true, false)]
        public virtual string WysylkaPanstwo { get; set; }
        
        public int? KlientId { get; set; }
       
        public int? OddzialId { get; set; }

        [FriendlyName("Uwagi")]
        [Niewymagane]
        [PoleEdytowane]
        [WidoczneListaAdmin(true,false,true,false)]
        public string Uwagi { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        [FriendlyName("Adres ip klienta składającego rejestrację")]
        [Niewymagane]
        public virtual string AdresIp { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        [Niewymagane]
        [FriendlyName("Data złożenia")]
        public virtual DateTime DataRejestracji { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }

        [Ignore]
        public List<string> Zalaczniki
        {
            get
            {
                List<string> tmp = new List<string>();
                if (!string.IsNullOrEmpty(Zalacznik1))
                {
                    tmp.Add(Zalacznik1);
                }
                if (!string.IsNullOrEmpty(Zalacznik2))
                {
                    tmp.Add(Zalacznik2);
                }
                if (!string.IsNullOrEmpty(Zalacznik3))
                {
                    tmp.Add(Zalacznik3);
                }
                return tmp;
            }
        }
        [FriendlyName("Podstawowe - zgoda na faktury elektroniczne")]
        [PoleEdytowane]
        [Niewymagane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool FakturyElektroniczne { get; set; }

        [FriendlyName("Podstawowe - akceptuję regulamin")]
        [PoleEdytowane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool AkceptacjaRegulaminu { get; set; }

        [FriendlyName("Podstawowe - zgoda na przetwarzanie danych osobowych")]
        [PoleEdytowane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool PrzetwarzanieDanychOsobowych { get; set; }

        public static bool CzyRejestracjaJestPoprawna(Rejestracja rejestracja,int idPolski,  ref List<string> bledy)
        {
            if (string.IsNullOrEmpty(rejestracja.Nazwa))
            {
                bledy.Add("Proszę wpisać nazwę");
                return false;
            }
            
            try
            {
                rejestracja.Nip = WalidujNIP(rejestracja.Nip, rejestracja.Panstwo, idPolski);
            }
            catch (Exception)
            {
                bledy.Add("Błędny nip");
                if (!string.IsNullOrEmpty(rejestracja.Zalacznik1) || !string.IsNullOrEmpty(rejestracja.Zalacznik2) || !string.IsNullOrEmpty(rejestracja.Zalacznik3))
                {
                    bledy.Add("Proszę dodać ponownie załaczniki");
                }
                return false;
            }
            return true;
        }

        protected static string WalidujNIP(string nip, string panstwo, int idPolski)
        {
            string niepoprawnyNip = "Niepoprawny nip";

            if (panstwo == null)
            {
                return nip;
            }

            if (panstwo.Equals(idPolski.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                nip = (nip ?? "").Replace("/", "").Replace("-", "").Replace("\\", "").Replace(" ", "");
                if (nip.Length != 10) throw new Exception(niepoprawnyNip);

                try { Int64.Parse(nip); }
                catch
                {
                    throw new Exception(niepoprawnyNip);
                }

                int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
                int sum = 0;
                for (int i = 0; i < weights.Length; i++)
                    sum += int.Parse(nip.Substring(i, 1)) * weights[i];

                bool result = (sum % 11) == int.Parse(nip.Substring(9, 1));
                if (result) return nip;
                throw new Exception(niepoprawnyNip);
            }
            return nip;
        }

      
    }
}