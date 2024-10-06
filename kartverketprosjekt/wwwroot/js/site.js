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


