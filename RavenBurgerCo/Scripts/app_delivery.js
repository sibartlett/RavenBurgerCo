$(function () {

    var gmapLayer = new L.Google('ROADMAP');
    var resultsLayer = L.layerGroup();
    var polygonLayer = L.featureGroup();
    

    var map = L.map('map', {
        layers: [gmapLayer, polygonLayer, resultsLayer],
        center: [54.14, -4.48],
        zoom: 6,
        maxBounds: L.latLngBounds([49, 15], [60, -25])
    });

    $('#location').locationSelector(map);

    $('#location').change(function () {
        var latlng = $('#location').locationSelector('val');
        map.setView(latlng, 12);
        
        polygonLayer.clearLayers();
        resultsLayer.clearLayers();
        
        resultsLayer.addLayer(L.circleMarker(latlng, { color: '#ff0000' }));
        
        var markerClick = function () {
            var polygon = L.geoJson(this.data.DeliveryArea);
            polygonLayer.clearLayers();
            polygonLayer.addLayer(polygon);
            map.fitBounds(polygon.getBounds());
        };
        
        $.get('/api/restaurants', {
            latitude: latlng[0],
            longitude: latlng[1],
            delivery: true
        }).done(function (restaurants) {
            $.each(restaurants, function (index, value) {
                var marker = L.geoJson(value.Location)
                    .bindPopup(
                        '<p><strong>' + value.Name + '</strong><br />' +
                        value.Street + '<br />' +
                        value.City + '<br />' +
                        value.PostCode + '<br />' +
                        value.Phone + '</p>'
                    )
                    .on('click', markerClick);
                marker.data = value;
                resultsLayer.addLayer(marker);
            });
            map.fitBounds(resultsLayer.getBounds());
        });
    });
});