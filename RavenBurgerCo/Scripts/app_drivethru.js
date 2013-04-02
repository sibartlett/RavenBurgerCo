$(function () {

    var directionsService = new google.maps.DirectionsService();

    var gmapLayer = new L.Google('ROADMAP');
    var resultsLayer = L.layerGroup();
    var routeLayer = L.layerGroup();

    var map = L.map('map', {
        layers: [gmapLayer, routeLayer, resultsLayer],
        center: [54.14, -4.48],
        zoom: 6,
        maxBounds: L.latLngBounds([49, 15], [60, -25])
    });
    
    $('#from, #to').locationSelector(map);

    var markerClick = function () {
        routeLayer.clearLayers();
        var latlng = this.getLatLng();
        var from = $('#from').locationSelector('val');
        var to = $('#to').locationSelector('val');

        var request = {
            origin: new google.maps.LatLng(from[0], from[1]),
            destination: new google.maps.LatLng(to[0], to[1]),
            waypoints: [{
                location: new google.maps.LatLng(latlng.lat, latlng.lng),
                stopover: true
            }],
            travelMode: google.maps.TravelMode.DRIVING
        };
        
        directionsService.route(request, function (result, status) {
            if (result && result.routes && result.routes.length) {

                var zoom = map.getZoom() > 9 ? map.getZoom() : 9;
                map.setView(latlng, zoom);

                var polyline = [];
                $.each(result.routes[0].overview_path, function (i, val) {
                    polyline[polyline.length] = [val.lat(), val.lng()];
                });
                routeLayer.addLayer(L.polyline(polyline, { color: 'blue' }));
            }
        });
    };

    $('#from, #to').change(function (e) {
        resultsLayer.clearLayers();
        var from = $('#from').locationSelector('val');
        var to = $('#to').locationSelector('val');
        if (from && to) {
            var request = {
                origin: new google.maps.LatLng(from[0], from[1]),
                destination: new google.maps.LatLng(to[0], to[1]),
                travelMode: google.maps.TravelMode.DRIVING
            };
            directionsService.route(request, function (result, status) {
                if (result && result.routes && result.routes.length) {
                    var bounds = result.routes[0].bounds;
                    map.fitBounds(L.latLngBounds(
                        [bounds.getSouthWest().lat(), bounds.getSouthWest().lng()],
                        [bounds.getNorthEast().lat(), bounds.getNorthEast().lng()]
                    ));

                    var polyline = [];
                    $.each(result.routes[0].overview_path, function(i, val) {
                        polyline[polyline.length] = [val.lat(), val.lng()];
                    });
                    resultsLayer.addLayer(L.polyline(polyline, { color: 'red' }));
                    resultsLayer.addLayer(L.circleMarker(polyline[0], { color: '#00ff00', fillOpacity: 0.7, opacity: 1 }));
                    resultsLayer.addLayer(L.circleMarker(polyline[polyline.length-1], { color: '#ff0000', fillOpacity: 0.7, opacity: 1 }));


                    $.get('/api/restaurants', {
                        polyline: result.routes[0].overview_polyline.points
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
                            resultsLayer.addLayer(marker);
                        });
                    });
                }
            });
        } else if (from || to) {
            var center = from || to;
            map.setView(center, 8);
        }
    });
});