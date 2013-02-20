$(function () {

    var gmapLayer = new L.Google('ROADMAP');
    var resultsLayer = L.layerGroup();

    var map = L.map('map', {
        layers: [gmapLayer, resultsLayer],
        center: [51.4775, -0.461389],
        zoom: 12,
        maxBounds: L.latLngBounds([49, 15], [60, -25])
    });

    var loadMarkers = function () {
        resultsLayer.clearLayers();
        if (map.getZoom() > 9) {
            var bounds = map.getBounds();
            $.get('/api/restaurants', {
                north: bounds.getNorthWest().lat,
                east: bounds.getSouthEast().lng,
                south: bounds.getSouthEast().lat,
                west: bounds.getNorthWest().lng,
            }).done(function(restaurants) {
                $.each(restaurants, function(index, value) {
                    var marker = L.marker([value.Latitude, value.Longitude])
                        .bindPopup(
                            '<p><strong>' + value.Name + '</strong><br />' +
                                value.Street + '<br />' +
                                value.City + '<br />' +
                                value.PostCode + '<br />' +
                                value.Phone + '</p>'
                        );
                    resultsLayer.addLayer(marker);
                });
            });
        }
    };

    loadMarkers();
    map.on('moveend', loadMarkers);
});