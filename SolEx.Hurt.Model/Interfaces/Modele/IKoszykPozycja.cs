using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces.Modele
{
    public interface IKoszykPozycja
    {
        [PrimaryKey]
        [UpdateColumnKey]
        [AutoIncrement]
        [FriendlyName("ID pozycji")]
        long Id { get; set; }

        long KoszykId { get; set; }
        [FriendlyName("Numer id produkt")]
        long ProduktId { get; set; }

        [FriendlyName("Ilość w koszyku")]
        decimal Ilosc { get; set; }
        
        [FriendlyName("Wybrana jednostka(liczba)")]
        long? JednostkaId { get; set; }

        decimal RabatDodatkowy { get; set; }

        string PowodDodatkowegoRabatu { get; set; }

        //string PrzedstawicielNazwa { get; set; }
        long? PrzedstawicielId { get; set; }

        decimal? WymuszonaCenaNettoPrzedstawiciel { get; set; }

        decimal? WymuszonaCenaNettoModul { get; set; }
        [FriendlyName("Data dodania")]
        DateTime DataDodania { get; set; }

        DateTime? DataZmiany { get; set; }

        Dictionary<long, string> Indywidualizacja { get; set; }

        int? DodajaceZadanie { get; set; }

        TypPozycjiKoszyka TypPozycji { get; set; }

        int? Hash { get; set; }
        string Opis { get; set; }
        long ProduktBazowyId { get; set; }
    }
}