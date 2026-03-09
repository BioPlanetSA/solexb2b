using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
   /// <summary>
    /// Informuje, że dane pole ma być wykorzystywana jako klucz przy aktualizacji bazy danych
    /// </summary>
    public class UpdateColumnKey : Attribute
    {
    }

    public class CsvImport : Attribute
    {
        public Type KlasaImportujaca { get; set; }
    
        public CsvImport(Type importer)
        {
            KlasaImportujaca = importer;
        }
    }
}
