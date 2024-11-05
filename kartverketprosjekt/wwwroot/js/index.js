function showModal() {
    // Show the login modal when "Til Kartet" is clicked and the user is not logged in
    document.getElementById('modal').style.display = 'block';
    document.getElementById('overlay').style.display = 'block';
}

function toggleForm() {
    // Toggle between login and register forms
    var loginForm = document.getElementById("login-form");
    var registerForm = document.getElementById("register-form");
    var modalTitle = document.getElementById("modal-title");

    if (loginForm.style.display === "none") {
        loginForm.style.display = "block";
        registerForm.style.display = "none";
        modalTitle.innerText = "Logg inn";
    } else {
        loginForm.style.display = "none";
        registerForm.style.display = "block";
        modalTitle.innerText = "Registrer";
    }
}

// Close modal when clicking outside the modal content
window.onclick = function (event) {
    var modal = document.getElementById("modal");
    var overlay = document.getElementById("overlay");
    if (event.target == modal || event.target == overlay) {
        modal.style.display = "none";
        overlay.style.display = "none";
    }
}