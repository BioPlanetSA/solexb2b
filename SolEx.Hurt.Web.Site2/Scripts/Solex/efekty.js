jQuery.fn.PodmieniajObrazkiPoNajechaniu = function () {
    return this.each(function () {
        PodmieniajObrazkiPoNajechaniu(this);
    });
}

function PodmieniajObrazkiPoNajechaniu(sender) {
    $(sender)
        .hover(
            function() {
                sciezkaHover = $(sender).data("hover");
                $(sender).attr("src", sciezkaHover);
            },
            function() {
                sciezkaNormalna = $(sender).data("src");
                $(sender).attr("src", sciezkaNormalna);
            }
        );
}