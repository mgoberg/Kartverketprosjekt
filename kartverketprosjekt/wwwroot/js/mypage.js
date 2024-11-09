$(document).on('click', '.comment', function () {
    $(this).toggleClass('comment-expanded');
    if ($(this).hasClass('comment-expanded')) {
        $(this).text($(this).data('fullText')); // Vis hele teksten
    } else {
        $(this).text(truncateText($(this).data('fullText'), 50)); // Forkort igjen
    }
});
$(document).ready(function () {
    // Loop through each comment section and fetch comments dynamically
    $('[class^="commentsSection"]').each(function () {
        var sakID = $(this).data('sakid'); // Get sakId from data attribute
        var commentsListId = '.commentsList#' + sakID; // Correct the selector for the specific ul element

        $.ajax({
            url: '/Bruker/GetComments',  // Controller's GetComments method
            type: 'GET',
            data: { sakId: sakID },  // Pass sakId to the request
            success: function (result) {
                if (result.success) {
                    // Clear the comment list before adding new comments
                    $(commentsListId).empty();

                    // Add each comment to the list
                    result.kommentarer.forEach(function (kommentar) {
                        var commentText = kommentar.tekst;
                        var commentDate = kommentar.dato;
                        var commentAuthor = kommentar.epost;
                        var truncatedText = truncateText(commentText, 50);


                        var commentInfoElement = '<li class="commentInfo">' + commentDate + ' av ' + commentAuthor + '</li>';
                        $(commentsListId).append(commentInfoElement);

                        var commentElement = $('<li class="comment comment-collapsed"></li>')
                            .text(truncatedText)
                            .data('fullText', commentText); // Store full text in data-fullText attribute

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

// Truncate text function
function truncateText(text, length) {
    return text.length > length ? text.substring(0, length) + '...' : text;
}