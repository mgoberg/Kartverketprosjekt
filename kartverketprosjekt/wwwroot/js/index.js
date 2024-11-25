function showModal() {
    // vis modal og overlay når bruker klikker på Til kartet-knappen
    document.getElementById('modal').style.display = 'block';
    document.getElementById('overlay').style.display = 'block';
}

function toggleForm() {
    // toggle mellom logg inn og registrer skjema
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

// lukke modal og overlay når bruker klikker utenfor modalen
window.onclick = function (event) {
    var modal = document.getElementById("modal");
    var overlay = document.getElementById("overlay");
    if (event.target == modal || event.target == overlay) {
        modal.style.display = "none";
        overlay.style.display = "none";
    }
}