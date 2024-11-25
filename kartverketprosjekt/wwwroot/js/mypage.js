$(document).on('click', '.comment', function () {
    $(this).toggleClass('comment-expanded');
    if ($(this).hasClass('comment-expanded')) {
        $(this).text($(this).data('fullText')); // Vis hele teksten
    } else {
        $(this).text(truncateText($(this).data('fullText'), 50)); // Forkort igjen
    }
});
$(document).ready(function () {
    // Gå gjennom hver kommentarseksjon og hent kommentarer dynamisk
    $('[class^="commentsSection"]').each(function () {
        var sakID = $(this).data('sakid'); // Hent sakId fra data-attributt
        var commentsListId = '.commentsList#' + sakID; // Korriger selektoren for det spesifikke ul-elementet

        $.ajax({
            url: '/Bruker/GetComments',  // Controllerens GetComments-metode
            type: 'GET',
            data: { sakId: sakID },  // Send sakId til forespørselen
            success: function (result) {
                if (result.success) {
                    // Tøm kommentarliste før nye kommentarer legges til
                    $(commentsListId).empty();

                    // Legg til hver kommentar i listen
                    result.kommentarer.forEach(function (kommentar) {
                        var commentText = kommentar.tekst;
                        var commentDate = kommentar.dato;
                        var commentAuthor = kommentar.epost;
                        var truncatedText = truncateText(commentText, 50);

                        var commentInfoElement = '<li class="commentInfo">' + commentDate + ' av ' + commentAuthor + '</li>';
                        $(commentsListId).append(commentInfoElement);

                        var commentElement = $('<li class="comment comment-collapsed"></li>')
                            .text(truncatedText)
                            .data('fullText', commentText); // Lagre full tekst i data-fullText-attributt

                        $(commentsListId).append(commentElement);
                    });
                } else {
                    console.log('Kunne ikke hente kommentarer: ' + result.message);
                }
            },
            error: function () {
                console.log('En feil oppstod under henting av kommentarer.');
            }
        });
    });
});

// Funksjon for å forkorte tekst
function truncateText(text, length) {
    return text.length > length ? text.substring(0, length) + '...' : text;
}