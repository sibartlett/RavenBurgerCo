$(function () {

    var gmapLayer = new L.Google('ROADMAP');
    var resultsLayer = L.layerGroup();

    var map = L.map('map', {
        layers: [gmapLayer, resultsLayer],
        center: [54.14, -4.48],
        zoom: 6,
        maxBounds: L.latLngBounds([49, 15], [60, -25])
    });
    
    $('#location').locationSelector(map);

    $('#location').change(function () {
        var latlng = $('#location').locationSelector('val');
        map.setView(latlng, 12);
        
        resultsLayer.clearLayers();
        
        resultsLayer.addLayer(L.circle(latlng, 25000, { color: '#ff0000', fillOpacity: 0 }));
        resultsLayer.addLayer(L.circle(latlng, 15000, { color: '#ff0000', fillOpacity: 0.1 }));
        resultsLayer.addLayer(L.circle(latlng, 10000, { color: '#ff0000', fillOpacity: 0.3 }));
        resultsLayer.addLayer(L.circle(latlng, 5000, { color: '#ff0000', fillOpacity: 0.5 }));
        resultsLayer.addLayer(L.circleMarker(latlng, { color: '#ff0000', fillOpacity: 1, opacity: 1 }));
        
        $.get('/api/restaurants', {
            latitude: latlng[0],
            longitude: latlng[1]
        }).done(function (restaurants) {
            $.each(restaurants, function (index, value) {
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
    });
});