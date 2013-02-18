(function () {

    function withinUkBounds(coord) {
        if (coord && coord.length == 2) {
            return coord[0] > 48.95 && coord[0] < 60.8603 && coord[1] > -10.661283 && coord[1] < 1.7628;
        }
        return false;
    }

    var geocoder = new google.maps.Geocoder();

    jQuery.fn.extend({
        locationSelector: function (map) {
            
            if (map && typeof map == 'string' && map == 'val') {
                if (this.length == 1) {
                    var v = $(this[0]).select2('val');
                    if (v) {
                        return v.split(',');
                    }
                    return null;
                }
            }
            
            return this.each(function () {

                var $this = $(this);
                $this.select2({
                    placeholder: "Search for a location",
                    minimumInputLength: 3,
                    query: function (query) {
                        geocoder.geocode({
                            address: query.term,
                            region: 'uk'
                        }, function (results, status) {
                            var data = { results: [] };
                            if (results && results.length) {
                                $.each(results, function (i, result) {
                                    if (result && result.geometry && result.geometry.location) {
                                        var location = result.geometry.location;
                                        if (withinUkBounds([location.lat(), location.lng()])) {
                                            data.results.push({ id: location.lat() + ',' + location.lng(), text: result.formatted_address });
                                        }
                                    }
                                });
                            }
                            query.callback(data);
                        });
                    }
                });
                
                if (map) {
                    var button = $(this).closest('.location').find('.location-button a');

                    button.on('click', function () {
                        button.toggleClass('btn-success');
                    });

                    map.on('click', function (e) {
                        if (button.hasClass('btn-success')) {
                            $this.select2('data', {
                                id: e.latlng.lat + ',' + e.latlng.lng,
                                text: e.latlng.lat + ', ' + e.latlng.lng
                            });
                            $this.trigger('change');
                            button.removeClass('btn-success');
                        }
                    });
                }
            });
        }
    });

})();