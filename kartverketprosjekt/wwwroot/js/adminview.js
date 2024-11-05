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