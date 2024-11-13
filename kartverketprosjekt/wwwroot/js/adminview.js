document.querySelectorAll('.styled-table tr').forEach(row => {
    row.addEventListener('click', function () {
        const epost = this.getAttribute('data-epost');
        const tilgangsnivaa = this.getAttribute('data-tilgangsnivaa');

        // Populate modal fields
        document.getElementById('modalUserId').value = epost;
        document.getElementById('modalUserEmail').value = epost;
        document.getElementById('newAccessLevel').value = tilgangsnivaa;

        // Show modal and overlay
        document.getElementById('userModal').classList.add('open');
        document.getElementById('modalOverlay').classList.add('open');
    });
});

// Close modal function
function closeModal() {
    document.getElementById('userModal').classList.remove('open');
    document.getElementById('modalOverlay').classList.remove('open');
}

// Close modal when clicking the overlay
document.getElementById('modalOverlay').addEventListener('click', closeModal);


$(document).ready(function () {
    // Sort function
    function sortTable(columnIndex) {
        var rows = $('#brukerTable tbody tr').get();

        rows.sort(function (a, b) {
            var A = $(a).children('td').eq(columnIndex).text().trim();
            var B = $(b).children('td').eq(columnIndex).text().trim();

            // If the column is "Tilgangsnivaa" (assumed to be column 1), sort numerically from 1 to 4
            if (columnIndex === 1) { // Change based on the column index for "Tilgangsnivaa"
                return parseInt(A) - parseInt(B); // Sort as numbers (1 to 4)
            } else {
                // Default sorting: Sort strings alphabetically or numerically if both are numbers
                if ($.isNumeric(A) && $.isNumeric(B)) {
                    return A - B; // Sort numbers
                } else {
                    return A.localeCompare(B); // Sort strings
                }
            }
        });

        // Re-append the rows in sorted order
        $.each(rows, function (index, row) {
            $('#brukerTable').children('tbody').append(row);
        });
    }

    // Event listener for sorting dropdown
    $('#sortBy').change(function () {
        var sortBy = $(this).val();

        // Map sort option to column index
        var columnIndex = {
            'Tilgangsnivaa': 1,  // Column for Tilgangsnivaa
            'Organisasjon': 2    // Column for Organisasjon
        }[sortBy];

        if (columnIndex !== undefined) {
            sortTable(columnIndex);
        }
    });

    // Search function (works across all columns in each row)
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase(); // Get the search input and convert it to lowercase

        // Filter rows based on the search term
        $('#brukerTable tbody tr').filter(function () {
            // Check if any column in the row contains the search value
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });
});
