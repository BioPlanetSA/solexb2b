/// <reference path="../../SolEx.Hurt.Web.Site2/Scripts/Typo/_menu.ts" />

describe('testowe testy', () => {

    //testowe tylko srpawdzenie czy jasmine dziala dobrze
    it('test czy 1 == 1', () => {
        expect(true).toBe(true);
    });

    it('test czy 1 == 1 - test ma oblac', () => {
        expect(true).toBe(false);
    });

    it ('uruchomienie metody z klasy _menu, poza solucja', () => {
        let menu = new MenuHamburger();
        expect(menu.testowaMetodaZwracaZawsze1()).toBe(1);
    });

    it('uruchomienie metody z klasy _menu, poza solucja - ma NIE przejsc', () => {
        let menu = new MenuHamburger();
        expect(menu.testowaMetodaZwracaZawsze1()).toBe(0);
    });




    //let htmlMenuHamburgerowe = '<div class="menu-hamburger" data-menu-ajax="@Model">' +
    //    '< button class="hamburger is-closed animated fadeInRight" type= "button" >' +
    //    '<span></span>' +
    //    '<span> </span>' +
    //    '< span > </span>' +
    //    '< span > </span>' +
    //    '< /button>' +
    //    '< div class="navbar navbar-fixed-top" >@*tu idzie ajaxem doladowane menu jesli jest potrzeba*@</div>' +
    //    '</div>';

    //if ('test inicjalizacj menu hamburgerowego', () => {
    //    let menu = new MenuHamburger();



    //});
   



});