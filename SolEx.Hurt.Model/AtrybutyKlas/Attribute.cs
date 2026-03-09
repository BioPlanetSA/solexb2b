using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class Niewymagane : Attribute
    {

    }
    public class Lokalizowane :Attribute
    {

    }
    public class ImportyNieObslugiwaneAttribute : Attribute
        {
        }
    /// <summary>
    /// Oznacza że atrybut w cryteriach nie jest przetwarzany automatycznie, należy obsłużyć go w metodzie DodatowySql()
    /// </summary>
    public class PomijajAtrybut : Attribute
    {
    }
    //dla modułów które mogą synchronizować/nadpisywać wybrane pola
    public class SynchronizowanePola : Attribute
    {
        public Type Typ { get; set; }
        public SynchronizowanePola(Type typ)
        {
            Typ = typ;
        }
    }

    public class ModulStandardowy : Attribute
    {
    }

    public class DomyslnieAktywny : Attribute
    {
    }
    public class SymbolKontrolki : Attribute
    {
        public string Symbol { get; set; }
        public SymbolKontrolki(string symbol)
        {
            Symbol = symbol;
        }
    }

    public class RealSortColumnName : Attribute
    {
        public RealSortColumnName(string name)
        {
            RealSortColumn = name;
        }
        public string RealSortColumn{get;set;}
    }
}
