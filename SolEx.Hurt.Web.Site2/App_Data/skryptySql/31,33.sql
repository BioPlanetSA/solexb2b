--zmienilem forme zapisu w parametrach na liste serializowana - dlatego trzeba sksasować wszystko co stare
delete from [DzialaniaUzytkownikow];
delete from [ProfilKlienta] where [TypUstawienia] = 'KierunekSortowaniaDokumentow';
delete from [ProfilKlienta] where [TypUstawienia] = 'DokumentyPrzedzial';
