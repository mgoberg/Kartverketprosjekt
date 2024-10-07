//SETUP

// enables fading in of body and navbar on initial load, remove styles on navbar and body if page appears blank
window.addEventListener('DOMContentLoaded', function () {
    document.getElementById('navbar').style.top = '0';
    document.querySelector('body').style.opacity = '1';
});

// function for darkmode
document.getElementById('darkModeToggle').addEventListener('click', function () {
    document.body.classList.toggle('dark-mode');
});

function showModal() {
    var modal = document.getElementById("modal");
    var overlay = document.getElementById("overlay");
    modal.classList.add("open");
    overlay.classList.add("open");
}

function hideModal() {
    var modal = document.getElementById("modal");
    var overlay = document.getElementById("overlay");
    modal.classList.remove("open");
    overlay.classList.remove("open");
}

document.getElementById('overlay').addEventListener('click', function () {
    hideModal(); // Close modal when overlay is clicked
});

function toggleForm() {
    var loginForm = document.getElementById("login-form");
    var registerForm = document.getElementById("register-form");
    var modalTitle = document.getElementById("modal-title");
    var toggleButton = document.getElementById("toggle-form");

    if (loginForm.style.display === "none") {
        loginForm.style.display = "block";
        registerForm.style.display = "none";
        modalTitle.innerText = "Logg inn";
        toggleButton.innerHTML = "<button class='btn-primary' onclick='toggleForm()'>Registrer deg</button>";
    } else {
        loginForm.style.display = "none";
        registerForm.style.display = "block";
        modalTitle.innerText = "Registrer deg";
        toggleButton.innerHTML = "<button class='btn-primary' onclick='toggleForm()'>Logg inn</button>";
    }
}

// Function to redirect on login submission for development purposes
function redirectToCorrectMap(event) {
    event.preventDefault(); // Prevent actual form submission
    window.location.href = '/Home/CorrectMap'; // Redirect to /CorrectMap
}

