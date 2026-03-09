using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model.Interfaces;
using Attachment = System.Net.Mail.Attachment;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IMaileBLL
    {
        /// <summary>
        /// zmienia maila tak aby zostal przechwycony - nie wyslany
        /// </summary>
        /// <param name="wiadomoscEmail"></param>
        /// <returns>True jesli przechwycony</returns>
        bool PrzechwycMaila(ref WiadomoscEmail wiadomoscEmail);

        bool WyslijEmaila(WiadomoscEmail wiadomoscEmail, IEnumerable<Attachment> files, TypyUstawieniaSkrzynek zJakiejSkrzynkiWyslac, out Exception blad, bool przechwytujWiadomosciEmail = true, bool logowac = true);

        void PrzedAktualizacjaNewslettera(IList<NewsletterKampania> obj);

        void UsunStaraHistorieMaili();

        Dictionary<string, IKlient> PobierzListeKlientowDoWysylki(NewsletterKampania k, out Dictionary<string, IKlient> ListaWszystkichEmailiKampani);

        void WyslijBledneMaile();

        Dictionary<TypyUstawieniaSkrzynek, UstawieniaSkrzynkiPocztowej> PobierzUstawieniaSkrzynki { get; }

        void UsunCache(IList<long> obj);

        void InicjalizacjaPowiadomien();
        string PrzetworzLinkiWTresciMaila(string message);

        List<SzablonMailaBaza> PobierzListeWszystkichPowiadomienMailowych();
    }
}