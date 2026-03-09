using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class WFMagXml : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument) => true;

        public override Licencje? WymaganaLicencja => Licencje.DokumentyXML;

        public override Encoding Kodowanie => Encoding.UTF8;

        public override string Nazwa => "WF-Mag XML";

        public override string PobierzNazwePliku(DokumentyBll dokument) => $"{NazwaPliku(dokument)}-wfmag.xml";

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            IKlient platnik = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(dokument.DokumentPlatnikId);
            int iloscDniDoZaplaty = dokument.TerminPlatnosci != null ? (dokument.DataUtworzenia - dokument.TerminPlatnosci.Value).Days : 0;
            string dataUtworzenia = dokument.DataUtworzenia.ToString("yyyy-MM-dd");
            string terminPlatnosci = string.Empty;
            IKlient odbiorca = dokument.Klient;
            string wartoscVat = "";

            if (dokument.Id > 0)
            {
                if (dokument.OdbiorcaId.HasValue)
                {
                    odbiorca = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(dokument.OdbiorcaId.Value);
                    if (odbiorca == null)
                    {
                        odbiorca = dokument.Klient;
                    }
                }
                terminPlatnosci = dokument.TerminPlatnosci?.ToString("yyyy-MM-dd") ?? "";
                wartoscVat = dokument.DokumentWartoscVat.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            }

            List<DokumentyPozycje> pozycje = dokument.PobierzPozycjeDokumentu().ToList();
            Dictionary<decimal, List<DokumentyPozycje>> slownikStawekVat = pozycje.GroupBy(x => x.Vat).ToDictionary(x => x.Key, x => x.ToList());

            Dictionary<string, string> jednostkiDoPodmiany = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"szt","PCE"},
                {"szt.","PCE"},
                {"kg","KGM" },
                {"TONA","TNE" },
                {"m2","MTK" }
            };

            XmlDocument doc = new XmlDocument();

            //Document-Invoice
            XmlElement szablon = doc.CreateElement("Document-Invoice");
            doc.AppendChild(szablon);

            //Invoice-Header
            XmlElement header = doc.CreateElement("Invoice-Header");
            szablon.AppendChild(header);
            XmlElement invoiceNumber = doc.CreateElement("InvoiceNumber");
            invoiceNumber.InnerText = dokument.NazwaDokumentu;
            header.AppendChild(invoiceNumber);
            XmlElement invoiceDate = doc.CreateElement("InvoiceDate");
            invoiceDate.InnerText = dataUtworzenia;
            header.AppendChild(invoiceDate);
            XmlElement salesDate = doc.CreateElement("SalesDate");
            salesDate.InnerText = dataUtworzenia;
            header.AppendChild(salesDate);
            XmlElement invoiceCurrency = doc.CreateElement("InvoiceCurrency");
            invoiceCurrency.InnerText = dokument.DokumentWartoscBrutto.Waluta;
            header.AppendChild(invoiceCurrency);
            XmlElement invoicePaymentDueDate = doc.CreateElement("InvoicePaymentDueDate");
            invoicePaymentDueDate.InnerText = terminPlatnosci;
            header.AppendChild(invoicePaymentDueDate);
            XmlElement invoicePaymentTerms = doc.CreateElement("InvoicePaymentTerms");
            invoicePaymentTerms.InnerText = iloscDniDoZaplaty.ToString();
            header.AppendChild(invoicePaymentTerms);
            XmlElement invoicePaymentMeans = doc.CreateElement("InvoicePaymentMeans");
            invoicePaymentMeans.InnerText = " ";
            header.AppendChild(invoicePaymentMeans);
            XmlElement invoicePostDate = doc.CreateElement("InvoicePostDate");
            invoicePostDate.InnerText = dataUtworzenia;
            header.AppendChild(invoicePostDate);
            XmlElement documentFunctionCode = doc.CreateElement("DocumentFunctionCode");
            documentFunctionCode.InnerText = dokument.WartoscNetto < 0 ? "C" : "O";
            header.AppendChild(documentFunctionCode);
            XmlElement remarks = doc.CreateElement("Remarks");
            remarks.AppendChild(doc.CreateCDataSection(""));
            header.AppendChild(remarks);

            //Delivery
            XmlElement delivery = doc.CreateElement("Delivery");
            header.AppendChild(delivery);
            XmlElement deliveryLocationNumber = doc.CreateElement("DeliveryLocationNumber");
            deliveryLocationNumber.InnerText = " ";
            delivery.AppendChild(deliveryLocationNumber);
            XmlElement deliveryDate = doc.CreateElement("DeliveryDate");
            deliveryDate.InnerText = " ";
            delivery.AppendChild(deliveryDate);
            XmlElement despatchNumber = doc.CreateElement("DespatchNumber");
            despatchNumber.InnerText = " ";
            delivery.AppendChild(despatchNumber);

            //Invoice-Parties
            XmlElement parties = doc.CreateElement("Invoice-Parties");
            szablon.AppendChild(parties);

            //Buyer
            XmlElement buyer = doc.CreateElement("Buyer");
            parties.AppendChild(buyer);
            XmlElement ilnB = doc.CreateElement("ILN");
            ilnB.InnerText = odbiorca.PoleTekst2;
            buyer.AppendChild(ilnB);
            XmlElement taxIdB = doc.CreateElement("TaxID");
            taxIdB.InnerText = string.IsNullOrEmpty(odbiorca.Nip) ? " " : odbiorca.Nip;
            buyer.AppendChild(taxIdB);
            XmlElement accountNumberB = doc.CreateElement("AccountNumber");
            accountNumberB.InnerText = " ";
            buyer.AppendChild(accountNumberB);
            XmlElement nameB = doc.CreateElement("Name");
            nameB.InnerText = odbiorca.Nazwa;
            buyer.AppendChild(nameB);
            XmlElement streetAndNumberB = doc.CreateElement("StreetAndNumber");
            streetAndNumberB.InnerText = odbiorca.DomyslnyAdres.UlicaNr;
            buyer.AppendChild(streetAndNumberB);
            XmlElement cityNameB = doc.CreateElement("CityName");
            cityNameB.InnerText = odbiorca.DomyslnyAdres.Miasto;
            buyer.AppendChild(cityNameB);
            XmlElement postalCodeB = doc.CreateElement("PostalCode");
            postalCodeB.InnerText = odbiorca.DomyslnyAdres.KodPocztowy;
            buyer.AppendChild(postalCodeB);
            XmlElement countryB = doc.CreateElement("Country");
            countryB.InnerText = odbiorca.DomyslnyAdres.Kraj;
            buyer.AppendChild(countryB);

            //Seller
            XmlElement sellerS = doc.CreateElement("Seller");
            parties.AppendChild(sellerS);
            XmlElement ilnS = doc.CreateElement("ILN");
            ilnS.InnerText = "5907814660008";
            sellerS.AppendChild(ilnS);
            XmlElement taxIdS = doc.CreateElement("TaxID");
            taxIdS.InnerText = seller.NIP;
            sellerS.AppendChild(taxIdS);
            XmlElement accountNumberS = doc.CreateElement("AccountNumber");
            accountNumberS.InnerText = " ";
            sellerS.AppendChild(accountNumberS);
            XmlElement codeByBuyerS = doc.CreateElement("CodeByBuyer");
            codeByBuyerS.InnerText = " ";
            sellerS.AppendChild(codeByBuyerS);
            XmlElement nameS = doc.CreateElement("Name");
            nameS.InnerText = seller.Name;
            sellerS.AppendChild(nameS);
            XmlElement streetAndNumberS = doc.CreateElement("StreetAndNumber");
            streetAndNumberS.InnerText = seller.Address.UlicaNr;
            sellerS.AppendChild(streetAndNumberS);
            XmlElement cityNameS = doc.CreateElement("CityName");
            cityNameS.InnerText = seller.Address.Miasto;
            sellerS.AppendChild(cityNameS);
            XmlElement postalCodeS = doc.CreateElement("PostalCode");
            postalCodeS.InnerText = seller.Address.KodPocztowy;
            sellerS.AppendChild(postalCodeS);
            XmlElement countryS = doc.CreateElement("Country");
            countryS.InnerText = string.IsNullOrEmpty(seller.Address.Kraj) ? " " : seller.Address.Kraj;
            sellerS.AppendChild(countryS);

            //Payee
            XmlElement payee = doc.CreateElement("Payee");
            parties.AppendChild(payee);
            XmlElement ilnP = doc.CreateElement("ILN");
            ilnP.InnerText = platnik.PoleTekst2;
            payee.AppendChild(ilnP);
            XmlElement taxIdP = doc.CreateElement("TaxID");
            taxIdP.InnerText = string.IsNullOrEmpty(platnik.Nip) ? " " : platnik.Nip;
            payee.AppendChild(taxIdP);
            XmlElement accountNumberP = doc.CreateElement("AccountNumber");
            accountNumberP.InnerText = " ";
            payee.AppendChild(accountNumberP);
            XmlElement nameP = doc.CreateElement("Name");
            nameP.InnerText = platnik.Nazwa;
            payee.AppendChild(nameP);
            XmlElement streetAndNumberP = doc.CreateElement("StreetAndNumber");
            streetAndNumberP.InnerText = string.IsNullOrEmpty(platnik.DomyslnyAdres.UlicaNr) ? " " : platnik.DomyslnyAdres.UlicaNr;
            payee.AppendChild(streetAndNumberP);
            XmlElement cityNameP = doc.CreateElement("CityName");
            cityNameP.InnerText = string.IsNullOrEmpty(platnik.DomyslnyAdres.Miasto) ? " " : platnik.DomyslnyAdres.Miasto;
            payee.AppendChild(cityNameP);
            XmlElement postalCodeP = doc.CreateElement("PostalCode");
            postalCodeP.InnerText = string.IsNullOrEmpty(platnik.DomyslnyAdres.KodPocztowy) ? " " : platnik.DomyslnyAdres.KodPocztowy;
            payee.AppendChild(postalCodeP);
            XmlElement countryP = doc.CreateElement("Country");
            countryP.InnerText = string.IsNullOrEmpty(platnik.DomyslnyAdres.Kraj) ? " " : platnik.DomyslnyAdres.Kraj;
            payee.AppendChild(countryP);

            //SellerHeadquarters
            XmlElement sellerHeadquarters = doc.CreateElement("SellerHeadquarters");
            parties.AppendChild(sellerHeadquarters);
            XmlElement ilnSh = doc.CreateElement("ILN");
            ilnSh.InnerText = " ";
            sellerHeadquarters.AppendChild(ilnSh);
            XmlElement nameSh = doc.CreateElement("Name");
            nameSh.InnerText = " ";
            sellerHeadquarters.AppendChild(nameSh);
            XmlElement streetAndNumberSh = doc.CreateElement("StreetAndNumber");
            streetAndNumberSh.InnerText = " ";
            sellerHeadquarters.AppendChild(streetAndNumberSh);
            XmlElement cityNameSh = doc.CreateElement("CityName");
            cityNameSh.InnerText = " ";
            sellerHeadquarters.AppendChild(cityNameSh);
            XmlElement postalCodeSh = doc.CreateElement("PostalCode");
            postalCodeSh.InnerText = " ";
            sellerHeadquarters.AppendChild(postalCodeSh);
            XmlElement countrySh = doc.CreateElement("Country");
            countrySh.InnerText = " ";
            sellerHeadquarters.AppendChild(countrySh);

            //Invoice-Lines
            XmlElement invoiceLines = doc.CreateElement("Invoice-Lines");
            szablon.AppendChild(invoiceLines);

            for (int i = 0; i < pozycje.Count; i++)
            {
                ProduktBazowy produkt = pozycje[i].ProduktBazowy;

                //Line
                XmlElement line = doc.CreateElement("Line");
                invoiceLines.AppendChild(line);

                //Line-Item
                XmlElement lineItem = doc.CreateElement("Line-Item");
                line.AppendChild(lineItem);
                XmlElement lineNumber = doc.CreateElement("LineNumber");
                lineNumber.InnerText = $"{i + 1}";
                lineItem.AppendChild(lineNumber);
                XmlElement ean = doc.CreateElement("EAN");
                ean.InnerText = produkt != null ? produkt.KodKreskowy : " ";
                lineItem.AppendChild(ean);
                XmlElement buyerItemCode = doc.CreateElement("BuyerItemCode");
                buyerItemCode.InnerText = produkt != null ? produkt.KodKreskowy : " ";
                lineItem.AppendChild(buyerItemCode);
                XmlElement supplierItemCode = doc.CreateElement("SupplierItemCode");
                supplierItemCode.InnerText = produkt != null ? produkt.KodKreskowy : " ";
                lineItem.AppendChild(supplierItemCode);
                XmlElement itemDescription = doc.CreateElement("ItemDescription");
                itemDescription.AppendChild(doc.CreateCDataSection($"{(produkt != null ? produkt.Nazwa : "")}"));
                lineItem.AppendChild(itemDescription);
                XmlElement itemType = doc.CreateElement("ItemType");
                itemType.InnerText = "CU";
                lineItem.AppendChild(itemType);
                XmlElement invoiceQuantity = doc.CreateElement("InvoiceQuantity");
                invoiceQuantity.InnerText = Math.Round(pozycje[i].Ilosc, base.Zaokraglenia[pozycje[i].JednostkaMiary]).ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                lineItem.AppendChild(invoiceQuantity);
                XmlElement unitOfMeasure = doc.CreateElement("UnitOfMeasure");
                if (!jednostkiDoPodmiany.TryGetValue(pozycje[i].Jednostka, out string jednostka))
                {
                    jednostka = pozycje[i].Jednostka;
                }
                unitOfMeasure.InnerText = jednostka;
                lineItem.AppendChild(unitOfMeasure);
                XmlElement invoiceUnitNetPrice = doc.CreateElement("InvoiceUnitNetPrice");
                invoiceUnitNetPrice.InnerText = pozycje[i]?.CenaNettoPoRabacie.ToString("F2").Replace(",", ".") ?? pozycje[i].CenaNetto.ToString("F2").Replace(",", ".");
                lineItem.AppendChild(invoiceUnitNetPrice);
                XmlElement invoiceUnitGrossPrice = doc.CreateElement("InvoiceUnitGrossPrice");
                invoiceUnitGrossPrice.InnerText = pozycje[i]?.CenaBruttoPoRabacie.ToString("F2").Replace(",", ".") ?? pozycje[i].CenaNetto.ToString("F2").Replace(",", ".");
                lineItem.AppendChild(invoiceUnitGrossPrice);
                XmlElement taxRate = doc.CreateElement("TaxRate");
                taxRate.InnerText = pozycje[i].Vat.ToString("F2").Replace(",", ".");
                lineItem.AppendChild(taxRate);
                XmlElement taxCategoryCode = doc.CreateElement("TaxCategoryCode");
                taxCategoryCode.InnerText = dokument.Klient.IndywidualnaStawaVat == null ? "S" : "E";
                lineItem.AppendChild(taxCategoryCode);
                XmlElement taxReference = doc.CreateElement("TaxReference");
                lineItem.AppendChild(taxReference);
                XmlElement referenceType = doc.CreateElement("ReferenceType");
                referenceType.InnerText = "PKWiU";
                taxReference.AppendChild(referenceType);
                XmlElement referenceNumber = doc.CreateElement("ReferenceNumber");
                referenceNumber.InnerText = produkt?.PKWiU ?? " ";
                taxReference.AppendChild(referenceNumber);
                XmlElement taxAmount = doc.CreateElement("TaxAmount");
                taxAmount.InnerText = (pozycje[i].PozycjaDokumentuWartoscBrutto - pozycje[i].PozycjaDokumentuWartoscNetto).ToString("F2").Replace(",", ".");
                lineItem.AppendChild(taxAmount);
                XmlElement netAmount = doc.CreateElement("NetAmount");
                netAmount.InnerText = pozycje[i].PozycjaDokumentuWartoscNetto.Wartosc.ToString("F2").Replace(",", ".");
                lineItem.AppendChild(netAmount);

                //Line-Order
                XmlElement lineOrder = doc.CreateElement("Line-Order");
                line.AppendChild(lineItem);
                XmlElement buyerOrderNumber = doc.CreateElement("BuyerOrderNumber");
                buyerOrderNumber.AppendChild(doc.CreateCDataSection(""));
                lineOrder.AppendChild(buyerOrderNumber);
                XmlElement supplierOrderNumber = doc.CreateElement("SupplierOrderNumber");
                supplierOrderNumber.AppendChild(doc.CreateCDataSection(""));
                lineOrder.AppendChild(supplierOrderNumber);
                XmlElement buyerOrderDate = doc.CreateElement("BuyerOrderDate");
                buyerOrderDate.InnerText = dataUtworzenia;
                lineOrder.AppendChild(buyerOrderDate);
            }

            //Invoice-Summary
            XmlElement invoiceSummary = doc.CreateElement("Invoice-Summary");
            szablon.AppendChild(invoiceSummary);
            XmlElement totalLines = doc.CreateElement("TotalLines");
            totalLines.InnerText = pozycje.Count.ToString();
            invoiceSummary.AppendChild(totalLines);
            XmlElement totalNetAmount = doc.CreateElement("TotalNetAmount");
            totalNetAmount.InnerText = dokument.WartoscNetto.ToString("F2").Replace(",", ".");
            invoiceSummary.AppendChild(totalNetAmount);
            XmlElement totalTaxableBasis = doc.CreateElement("TotalTaxableBasis");
            totalTaxableBasis.InnerText = dokument.WartoscNetto.ToString("F2").Replace(",", ".");
            invoiceSummary.AppendChild(totalTaxableBasis);
            XmlElement totalTaxAmount = doc.CreateElement("TotalTaxAmount");
            totalTaxAmount.InnerText = wartoscVat;
            invoiceSummary.AppendChild(totalTaxAmount);
            XmlElement totalGrossAmount = doc.CreateElement("TotalGrossAmount");
            totalGrossAmount.InnerText = dokument.WartoscBrutto.ToString("F2").Replace(",", ".");
            invoiceSummary.AppendChild(totalGrossAmount);
            XmlElement grossAmountInWords = doc.CreateElement("GrossAmountInWords");
            grossAmountInWords.AppendChild(doc.CreateCDataSection(""));
            invoiceSummary.AppendChild(grossAmountInWords);

            //Tax-Summary
            XmlElement taxSummary = doc.CreateElement("Tax-Summary");
            invoiceSummary.AppendChild(taxSummary);
            foreach (var stawkaVat in slownikStawekVat)
            {
                XmlElement taxSummaryLine = doc.CreateElement("Tax-Summary-Line");
                taxSummary.AppendChild(taxSummaryLine);
                XmlElement TaxRate = doc.CreateElement("TaxRate");
                TaxRate.InnerText = stawkaVat.Key.ToString("F2").Replace(",", ".");
                taxSummaryLine.AppendChild(TaxRate);
                XmlElement TaxCategoryCode = doc.CreateElement("TaxCategoryCode");
                TaxCategoryCode.InnerText = dokument.Klient.IndywidualnaStawaVat == null ? "E" : "S";
                taxSummaryLine.AppendChild(TaxCategoryCode);
                XmlElement TaxAmount = doc.CreateElement("TaxAmount");
                TaxAmount.InnerText = stawkaVat.Value.Sum(x => x.WartoscVat).ToString("F2").Replace(",", ".");
                taxSummaryLine.AppendChild(TaxAmount);
                XmlElement taxableBasis = doc.CreateElement("TaxableBasis");
                taxableBasis.InnerText = stawkaVat.Value.Sum(x => x.PozycjaDokumentuWartoscNetto).ToString("F2").Replace(",", ".");
                taxSummaryLine.AppendChild(taxableBasis);
                XmlElement taxableAmount = doc.CreateElement("TaxableAmount");
                taxableAmount.InnerText = stawkaVat.Value.Sum(x => x.PozycjaDokumentuWartoscNetto).ToString("F2").Replace(",", ".");
                taxSummaryLine.AppendChild(taxableAmount);
                XmlElement grossAmount = doc.CreateElement("GrossAmount");
                grossAmount.InnerText = stawkaVat.Value.Sum(x => x.PozycjaDokumentuWartoscBrutto).ToString("F2").Replace(",", ".");
                taxSummaryLine.AppendChild(grossAmount);
            }

            return Kodowanie.GetBytes(doc.OuterXml.Replace("> <", "><"));
        }
    }
}
