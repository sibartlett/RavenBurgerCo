$(function() {

    $(window).resize(function() {
        var h = $(window).height() - $('#header').outerHeight() - $('#controls').outerHeight();
        $('#map').height(h);
    }).trigger('resize');
    
});