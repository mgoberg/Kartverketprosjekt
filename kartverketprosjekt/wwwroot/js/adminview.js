
// funksjon for å åpne modal med felter fra tabellen/databasen
document.querySelectorAll('.styled-table tr').forEach(row => {
    row.addEventListener('click', function () {
        const epost = this.getAttribute('data-epost');
        const tilgangsnivaa = this.getAttribute('data-tilgangsnivaa');

        document.getElementById('modalUserId').value = epost;
        document.getElementById('modalUserEmail').value = epost;
        document.getElementById('newAccessLevel').value = tilgangsnivaa;

        document.getElementById('userModal').classList.add('open');
        document.getElementById('modalOverlay').classList.add('open');
    });
});

// funksjon for å lukke modal
function closeModal() {
    document.getElementById('userModal').classList.remove('open');
    document.getElementById('modalOverlay').classList.remove('open');
}

document.getElementById('modalOverlay').addEventListener('click', closeModal);


    // Søk (fungerer på tvers av alle kolonner i hver rad)
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase(); 

        // Filtrer rader basert på søkeordet
        $('#brukerTable tbody tr').filter(function () {
            // Sjekk om noen kolonne i raden inneholder søkeverdien
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });
