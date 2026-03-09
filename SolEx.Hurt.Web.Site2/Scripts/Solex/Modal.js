
function PotwierdzenieWyslanieMaila(e, id) {
    //Sylwester musi dorobić tłumaczenie wie o co chodzi - problem w adminie przy probie wyslania maila powitalengo.
    var tl = PobierzTlumaczenia("Czy na pewno chcesz wysłać maila powitalnego?;Tak;Nie");
    PotwierdzenieAlert(id, '', tl["Czy na pewno chcesz wysłać maila powitalnego?"], tl["Tak"], tl["Nie"], function () {
        var a = e.getAttribute("data-link");
        window.location.href = a;
    });

}


function EdytujLokalizacje(fraza) {
    PokazModal(null, '/admin/tlumacz?fraza=' + fraza);
}
