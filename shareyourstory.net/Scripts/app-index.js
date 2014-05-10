var story = {
    toggleSection: function (id) {
        $('.containerTitleMenu').each(function () {
            $(this).removeClass("containerTitleMenuActive");
        });
        $('.containerStory').each(function () {
            $(this).hide();
        });

        $('#menu' + id).addClass("containerTitleMenuActive");
        $('#section' + id).fadeIn('slow');
    }
};