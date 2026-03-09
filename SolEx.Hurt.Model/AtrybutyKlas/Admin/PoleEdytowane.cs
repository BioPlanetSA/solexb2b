using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PoleEdytowane : Attribute
    {
        public int Kolejnosc { get; set; }
        public string Grupa { get; set; }
        public bool Wymagane { get; set; }
        public string TypKontrolki { get; set; }
    }
}