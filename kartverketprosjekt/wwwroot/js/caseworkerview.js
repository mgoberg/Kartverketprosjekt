var map = L.map('map').setView([58.075045, 7.909303], 10);

var marker;
var geoJsonLayer; // Lagrer GeoJSON-laget
var tileLayer; // Lagrer tile-laget

// Funskjon for å slette sak i db
$('#slettSakKnapp').click(function () {
    var sakID = $('#dashboardSakID').text(); // Hent SakID fra dashboardet

    if (sakID === "-") {
        alert("Velg en sak før du sletter!");
        return;
    }

    // Bekreft før sletting
    if (confirm("Er du sikker på at du vil slette denne saken?")) {
        $.ajax({
            url: '/Saksbehandler/Delete',  // URL til controllerens Delete-funksjon
            type: 'POST',
            data: { id: sakID },
            success: function (result) {
                if (result.success) {
                    console.log(result.message);  // Viser en melding til brukeren
                    location.reload();      // Oppdaterer siden for å fjerne slettet sak fra visningen
                } else {
                    console.log(result.message);  // Viser en feilmelding 
                }
            },
            error: function () {
                console.log("En feil oppstod under sletting.");
            }
        });
    }
});

// Legge til ny kommentar i listen
$('#saveComment').click(function () {
    var commentText = $('.caseComment').val();  // Hente innholdet fra tekstområdet

    if (commentText.trim() !== "") {
        // Hent sakID fra dashboardet
        var sakID = $('#dashboardSakID').text();

        if (sakID !== "-") {
            // Legg til ny kommentar til listen på klientsiden
            $('.commentsList').append('<li>' + commentText + '</li>');

            // Tømme kommentarfeltet etter at kommentaren er lagt til
            $('.caseComment').val('');

            // Send kommentar til backend for å lagre den
            $.ajax({
                url: '/Saksbehandler/AddComment',  // URL til controllerens AddComment-funksjon
                type: 'POST',
                data: {
                    sakID: sakID,
                    kommentar: commentText
                },
                success: function (result) {
                    if (result.success) {
                        alert('Kommentar lagret!');
                    } else {
                        alert('Kunne ikke lagre kommentar.');
                    }
                },
                error: function () {
                    alert('En feil oppstod under lagring.');
                }
            });
        } else {
            alert("Velg en sak før du legger til kommentar.");
        }
    } else {
        alert("Kommentarfeltet er tomt.");
    }
});




$(document).ready(function () {
    $('#casesTable tbody tr').click(function () {
        // Hente data fra tabellen
        var sakID = $(this).data('sakid');  // Hent sakID fra data-attribute
        var epost = $(this).find('td:eq(0)').text();
        var typeFeil = $(this).find('td:eq(1)').text();
        var beskrivelse = $(this).data('beskrivelse');
        var vedlegg = $(this).find('td:eq(2)').text();
        var geojson = $(this).data('geojson');
        var fylke = $(this).find('td:eq(3)').text();
        var kommune = $(this).find('td:eq(4)').text();
        var status = $(this).find('td:eq(5)').text();
        var dato = $(this).find('td:eq(6)').text();
        var kartlagUrl = $(this).data('layerurl');

        // Konvertere GeoJSON-strengen til et objekt
        if (typeof geojson === "string") {
            try {
                geojson = JSON.parse(geojson);  // Ingen erstatning nødvendig hvis formatet er riktig
            } catch (error) {
                console.error("Error parsing GeoJSON: ", error);
                return; // Avslutt funksjonen hvis det oppstod en feil
            }
        }

        // Oppdater dashboard med data fra valgt rad
        $('#dashboardSakID').text(sakID);
        $('#dashboardEpost').text(epost);
        $('#dashboardTypeFeil').text(typeFeil);
        $('#dashboardBeskrivelse').text(beskrivelse);
        $('#dashboardVedlegg').text(vedlegg);
        $('#dashboardFylke').text(fylke);
        $('#dashboardKommune').text(kommune);
        $('#dashboardStatus').text(status);
        $('#dashboarddato').text(dato);

        $(document).on('click', '.comment', function () {
            $(this).toggleClass('comment-expanded');
            if ($(this).hasClass('comment-expanded')) {
                $(this).text($(this).data('fullText')); // Vis hele teksten
            } else {
                $(this).text(truncateText($(this).data('fullText'), 50)); // Forkort igjen
            }
        });

        // Funksjon for å forkorte tekst til en spesifisert lengde
        function truncateText(text, maxLength) {
            return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
        }

        $.ajax({
            url: '/Saksbehandler/GetComments',  // URL til kontrollerens GetComments metode
            type: 'GET',
            data: { sakId: sakID },  // Passer sakID til forespørselen
            success: function (result) {
                if (result.success) {
                    // Tøm kommentarlisten før nye kommentarer legges til
                    $('.commentsList').empty();
                    // Legg til hver kommentar i listen
                    result.kommentarer.forEach(function (kommentar) {
                        var commentText = kommentar.tekst;
                        var commentDate = kommentar.dato;
                        var commentAuthor = kommentar.epost;
                        var truncatedText = truncateText(commentText, 50);

                        var commentElement = $('<li class="comment comment-collapsed"></li>')
                            .text(truncatedText)
                            .data('fullText', commentText); // Lagre full tekst i data-fullText-attributtet
                        $('.commentsList').append(commentElement);

                        var commentInfoElement = '<li class="commentInfo">' + commentDate + '  av ' + commentAuthor + '</li>';

                        $('.commentsList').append(commentInfoElement);

                        $('.commentsList').append(commentElement);
                    });
                } else {
                    alert('Kunne ikke hente kommentarer: ' + result.message);
                }
            },
            error: function () {
                alert('En feil oppstod under henting av kommentarer.');
            }
        });

        // Sjekk kartlag-URL og vis tilsvarende tekst i dashboardet
        var kartlagTekst = "";
        if (kartlagUrl === "https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png") {
            kartlagTekst = "Topografisk";
        } else if (kartlagUrl === "https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png") {
            kartlagTekst = "Topografisk Gråtone";
        } else if (kartlagUrl === "https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png") {
            kartlagTekst = "Sjøkart";
        } else if (kartlagUrl === "https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png") {
            kartlagTekst = "Turkart";
        } else {
            kartlagTekst = "Ukjent kartlag";
        }

        // Oppdater kartlaget i dashboardet
        $('#dashboardKartlag').text(kartlagTekst);


        // Scroll ned til dashbordet
        document.querySelector('#map').scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });


        // endre status
        $('#changeStatus').val(status);


        // Oppdatere kart med GeoJSON data og nytt kartlag basert på URL
        if (geojson && kartlagUrl) {
            updateMap(geojson, kartlagUrl, beskrivelse); // Send beskrivelse med
        }
    });
});


// Change status event
$('#changeStatus').change(function () {
    var sakID = $('#dashboardSakID').text();  // Get the SakID from the dashboard
    var newStatus = $(this).val(); // Get the new status from the dropdown

    if (sakID !== "-") {
        $.ajax({
            url: '/Saksbehandler/UpdateStatus', // URL to your controller's update method
            type: 'POST',
            data: {
                id: sakID,
                status: newStatus
            },
            success: function (result) {
                if (result.success) {
                    alert('Status oppdatert!'); // Notify the user
                    location.reload(); // Reload the page to reflect changes
                } else {
                    alert('Kunne ikke oppdatere status.');
                }
            },
            error: function () {
                alert('En feil oppstod under oppdatering av status.');
            }
        });
    } else {
        alert("Velg en sak før du endrer status.");
    }
});


function updateMap(geojson, layerUrl, beskrivelse) {
    // Kontroller om geojson er en gyldig GeoJSON-streng
    if (typeof geojson === 'string') {
        try {
            geojson = JSON.parse(geojson); // Prøv å analysere JSON-strengen
        } catch (e) {
            console.error("Invalid GeoJSON string:", geojson);
            return; // Avbryt hvis parsing mislykkes
        }
    }

    // Fjerne eksisterende GeoJSON-lag og markører
    if (marker) {
        map.removeLayer(marker);
    }

    if (geoJsonLayer) {
        map.removeLayer(geoJsonLayer);
    }

    // Legg til GeoJSON-laget til kartet
    try {
        geoJsonLayer = L.geoJSON(geojson).addTo(map); // Lagre laget i en variabel

        // Hent koordinatene fra GeoJSON
        var coordinates = geojson.geometry.coordinates;

        // Bestem hvilken type geometri vi har
        if (geojson.geometry.type === "Point") {
            var latLng = [coordinates[1], coordinates[0]]; // [latitude, longitude]
            marker = L.marker(latLng).addTo(map);
            marker.bindPopup(beskrivelse).openPopup();
            map.setView(latLng, 13); // Zoom inn på markøren
        } else if (geojson.geometry.type === "Polygon") {
            var firstSet = coordinates[0]; // Hent den første sett med koordinater
            if (firstSet.length > 0) {
                var latLng = [(firstSet[0][1] + firstSet[firstSet.length - 1][1]) / 2,
                (firstSet[0][0] + firstSet[firstSet.length - 1][0]) / 2]; // Midtpunkt av første og siste koordinat
                marker = L.polygon(firstSet.map(coord => [coord[1], coord[0]])).addTo(map);
                marker.bindPopup(beskrivelse).openPopup();
                map.setView(latLng, 13); // Zoom inn på midtpunktet
            }
        } else if (geojson.geometry.type === "LineString") {
            // For LineString, vi konverterer alle koordinater til latLng format
            var latLngs = coordinates.map(coord => [coord[1], coord[0]]);
            marker = L.polyline(latLngs).addTo(map); // Legger til linjen til kartet
            marker.bindPopup(beskrivelse).openPopup();

            // Sentrer kartet på midtpunktet av linjen
            var midPointIndex = Math.floor(latLngs.length / 2);
            var midLatLng = latLngs[midPointIndex];
            map.setView(midLatLng, 13); // Zoom inn på midtpunktet av linjen
        } else if (geojson.geometry.type === "Rectangle") {
            var bounds = L.latLngBounds(
                [coordinates[0][1], coordinates[0][0]],
                [coordinates[1][1], coordinates[1][0]]
            );
            marker = L.rectangle(bounds).addTo(map);
            marker.bindPopup(beskrivelse).openPopup();
            map.fitBounds(bounds); // Juster kartet til rektangelet
        } else {
            console.error("Unsupported GeoJSON geometry type:", geojson.geometry.type);
        }

        // Fjerne eksisterende tile layer før vi legger til et nytt
        if (tileLayer) {
            map.removeLayer(tileLayer);
        }

        // Legge til nytt tile lag basert på kartlag-URL
        tileLayer = L.tileLayer(layerUrl, {
            attribution: '&copy; <a href="https://www.kartverket.no">Kartverket</a>'
        }).addTo(map);

    } catch (e) {
        console.error("Error adding GeoJSON to map:", e);
    }


}

$(document).ready(function () {
    // Sort function
    function sortTable(columnIndex) {
        var rows = $('#casesTable tbody tr').get();

        rows.sort(function (a, b) {
            var A = $(a).children('td').eq(columnIndex).text().toUpperCase();
            var B = $(b).children('td').eq(columnIndex).text().toUpperCase();

            if ($.isNumeric(A) && $.isNumeric(B)) {
                return A - B; // Sort numbers
            } else {
                return A.localeCompare(B); // Sort strings
            }
        });

        $.each(rows, function (index, row) {
            $('#casesTable').children('tbody').append(row);
        });
    }

    // Event listener for sorting dropdown
    $('#sortBy').change(function () {
        var sortBy = $(this).val();

        // Map sort option to column index
        var columnIndex = {
            'TypeFeil': 1,
            'Vedlegg': 2,
            'Fylke': 3,
            'Kommune': 4,
            'Status': 5,
            'Dato': 6,
            'Kartlag': 7
        }[sortBy];

        sortTable(columnIndex);
    });

    // Search function
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase(); // Get the search input and convert it to lowercase
        $('#casesTable tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1) // Show/hide rows based on the search input
        });
    });
});

function playSound() {
    var audio = new Audio('/images/viktigIkkeSlett.mp3'); // Replace with the path to your sound file
    audio.play();
}