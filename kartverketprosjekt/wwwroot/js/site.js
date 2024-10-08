﻿//SETUP

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

$('#register-form').submit(function (event) {
    event.preventDefault(); // Prevent default form submission

    $.ajax({
        url: '/Bruker/Register',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Brukernavn: $('#brukernavn').val(),
            Passord: $('#passord').val(),
            Epost: $('#epost').val()
        }),
        success: function (response) {
            // Handle success response
            console.log(response);
        },
        error: function (xhr) {
            // Handle error response
            console.log(xhr.responseJSON.errors);
        }
    });
});


document.getElementById('register-form').onsubmit = async function (event) {
    event.preventDefault(); // Prevent default form submission

    // Create FormData from the form element
    const formData = new FormData(this);

    // Construct the JSON object from form data
    const jsonData = {
        Brukernavn: formData.get("Brukernavn"),
        Passord: formData.get("Passord"),
        Epost: formData.get("Epost")
    };

    try {
        const response = await fetch('/Bruker/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(jsonData) // Use jsonData here
        });

        // Check if the response is OK (status 200)
        if (!response.ok) {
            const errorResponse = await response.json(); // Get error response
            throw new Error(errorResponse.message || "Network response was not ok");
        }

        const result = await response.json(); // Parse JSON response
        if (result.success) {
            alert(result.message); // Display success message
            // Optionally redirect after registration
            window.location.href = "/Home/CorrectMap"; // Redirect after registration
        } else {
            alert("Errors: " + result.errors.join(', ')); // Show validation errors
        }
    } catch (error) {
        console.error("Error during registration:", error);
        alert("An error occurred during registration."); // Notify user
    }
};




