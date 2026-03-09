using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.SolexPay.Model;

namespace SolEx.Hurt.SolexPay.Interfaces
{
    public interface ISolexPayLogic 
    {
        WartoscLiczbowa CalculateDefermentCost(decimal valueToPay, string currency, int defermentDays = 30);
        //   SolexPayUploadResult SendFile(byte[] plik, string fileName, string mime);
        void UpdatePaymentStatus(string content);
        //   bool ConvertPhoneNumberToString(string telefon, out string value);
        HistoriaDokumentuPlatnosciOnline GenerateDocumentOnlinePayment(byte[] plik, DokumentyBll dokument, string mimeType, string fileName, string adresId, out string hashDodanegoPliku, long clientId);
        string SendApplication(SolexPayDocumentModel documentModel, string shopUrl, string notificationUrl, string fileName, string hashDodanegoPliku, int defermentDays);
    }
}