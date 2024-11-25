
//sjekker om darkmode ligger i localstorage slik at de som har valgt det får darkmode
document.addEventListener('DOMContentLoaded', (event) => {
    const navbar = document.getElementById('navbar');
    let lastScrollTop = 0;

    // gjemmer navbar når bruker scroller nedover
    window.addEventListener('scroll', () => {
        let scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        if (scrollTop > lastScrollTop && scrollTop > 100) {
            // Scroll down
            navbar.classList.add('navbar-hidden');
        } else {
            // Scroll up
            navbar.classList.remove('navbar-hidden');
        }

        lastScrollTop = scrollTop;
    });
});

if (localStorage.getItem('darkMode') === 'enabled') {
    document.body.classList.add('dark-mode');
    document.querySelector('.bx-toggle-left').classList.add('bxs-toggle-right');
}

// Darkmode funksjon
document.getElementById('darkModeToggle').addEventListener('click', function () {
    document.body.classList.toggle('dark-mode');
    document.querySelector('.bx-toggle-left').classList.toggle('bxs-toggle-right');

    // Update toggle-ikonet i layout
    if (document.body.classList.contains('dark-mode')) {
        localStorage.setItem('darkMode', 'enabled');
    } else {
        localStorage.setItem('darkMode', 'disabled');       
    }
});

 //hamburger meny elementer
const menuButton = document.getElementById('menuButton');
const dropdown = document.getElementById('dropdown-content');
const openIcon = menuButton.querySelector('.bx-menu');
const closeIcon = menuButton.querySelector('.bx-x'); 

// toggler dropdown menyen når hamburger menyen blir trykket på
menuButton.addEventListener('click', function (event) {
    event.stopPropagation(); 

    // Toggle dropdown synlighet
    const isOpen = dropdown.style.display === 'block';
    dropdown.style.display = isOpen ? 'none' : 'block'; 

    // Toggle ikoner
    openIcon.style.display = isOpen ? 'block' : 'none'; 
    closeIcon.style.display = isOpen ? 'none' : 'block'; 
});

// lukk dropdown når bruker klikker utenfor
window.addEventListener('click', function (event) {
    if (dropdown.style.display === 'block' && !event.target.matches('#menuButton') && !event.target.closest('#dropdown-content')) {
        dropdown.style.display = 'none'; 
        openIcon.style.display = 'block'; 
        closeIcon.style.display = 'none'; 
    }
});

const menuButtonFirstChild = document.getElementById('dropdown-content').firstElementChild;
$(document).ready(function () {
    // Funksjon som sjekker om det finnes en endret status
    function checkNotificationStatus() {
        $.getJSON('/Bruker/HarEndretStatus', function (result) {
            if (result) {
                $('#notificationMessage').show();
                $('#notificationDot').show();
                // Add class to menuButton's first child
                
                menuButtonFirstChild.classList.add('notificationAlertAnimation');

                    setTimeout(() => {
                        $('#notificationMessage').addClass('fade-out');
                    }, 2000);
            } else {
                $('#notificationMessage').hide(); // Fjern klassen hvis ingen notifikasjon
                $('#notificationDot').hide(); // Fjern klassen hvis ingen notifikasjon
            }
        });
    }

    // Funksjon som nullstiller notifikasjonen når brukeren trykker på menuButton
    function resetNotificationStatus() {
        $.post('/Bruker/ResetNotificationStatus', function () {
            console.log('Notifikasjonstatus er nullstilt');
        });
    }

    // Kjør sjekk på notifikasjoner ved sideinnlasting
    checkNotificationStatus();

    // Kjør sjekk på notifikasjoner hvert 20. sekund
    setInterval(checkNotificationStatus, 20000);

    // Når brukeren klikker på menuButton, resetter vi notifikasjonen
    $('#menuButton').on('click', function () {
        // Fjern den røde prikken
        $('#menuButton').removeClass('notification-active');

        $('#notificationMessage').hide();
        $('#notificationDot').hide();
        // Nullstill notifikasjonen på serveren
        resetNotificationStatus();
    });

        function removeNotificationAnimation() {
            menuButtonFirstChild.classList.remove('notificationAlertAnimation');
        }

        menuButtonFirstChild.addEventListener('click', removeNotificationAnimation);
});



