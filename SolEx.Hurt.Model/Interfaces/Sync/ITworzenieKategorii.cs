using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    
    public interface ITworzenieKategorii
    {
        /// <inheritdoc/>
        /// <remarks>
        /// wypada³o by po implementacji tej metody dodaæ do modu³u DodajBrakujaceKategorie w uwagach jaki erp obs³uguje
        /// </remarks>
        void PrzetworzKategorie(List<Model.Grupa> grupyPRoduktow);
    }

}
