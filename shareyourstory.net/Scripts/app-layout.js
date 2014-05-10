$(window).load(function () {
    footerPosition();
});

$(window).resize(function () {
    footerPosition();
});

function footerPosition() {
    var hWindow = $(window).height(); // get the height of your content
    var hContent = $(document).height();  // get the height of the visitor's browser window

    if (hContent <= hWindow) {
        $('.copyrights').css('position', 'fixed');
        $('.copyrights').css('bottom', '0');
    }
    else {
        $('.copyrights').css('position', 'relative');
        $('.copyrights').css('bottom', 'auto');
    }
}