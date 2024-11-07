document.addEventListener('DOMContentLoaded', function () {
    // Initial kartposisjon og zoom-nivå
    var map = L.map('map').setView([65, 12], 4);

    // Legger til bakgrunnskart ved å bruke layerUrl
    L.tileLayer(layerUrl, { attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>' }).addTo(map);

    // Legger GeoJSON-dataen til kartet
    var layer = L.geoJSON(geoJsonData).bindPopup(description);
    layer.addTo(map);

    var coordinates = geoJsonData.geometry.coordinates;

    // Sjekker geometritypen og flytter kartet til riktig posisjon
    if (geoJsonData.geometry.type === 'LineString') {
        var lineCenter = calculateLineCenter(coordinates);
        map.setView([lineCenter[1], lineCenter[0]], 15); // Fly til linjens sentrum
    } else if (geoJsonData.geometry.type === 'Polygon') {
        var polygonCenter = calculatePolygonCenter(coordinates[0]);
        map.setView([polygonCenter[1], polygonCenter[0]], 15); // Fly til polygonets sentrum
    } else {
        map.setView([coordinates[1], coordinates[0]], 15); // Fly til punktet
    }

    // Funksjon for å beregne center av en linje
    function calculateLineCenter(coordinates) {
        var totalPoints = coordinates.length;
        var sumLat = 0;
        var sumLng = 0;

        // Summerer latituder og longituder for alle punkter
        for (var i = 0; i < totalPoints; i++) {
            sumLat += coordinates[i][1];
            sumLng += coordinates[i][0];
        }

        // Returnerer center som et array med longitude og latitude
        return [sumLng / totalPoints, sumLat / totalPoints];
    }

    // Funksjon for å beregne center av et polygon
    function calculatePolygonCenter(coordinates) {
        var totalPoints = coordinates.length;
        var sumLat = 0;
        var sumLng = 0;

        // Summerer latituder og longituder for alle punkter
        for (var i = 0; i < totalPoints; i++) {
            sumLat += coordinates[i][1];
            sumLng += coordinates[i][0];
        }

        // Returnerer center som et array med longitude og latitude
        return [sumLng / totalPoints, sumLat / totalPoints];
    }
});
