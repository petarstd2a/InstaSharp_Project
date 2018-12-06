
$(function () {

    // Load modal window with content
    $('.modal-link').click(function (e) {
        e.preventDefault();

        // Modal and content
        var modal = $('.modal'),
            modalContent = $('.modal__content');

        // Ajax request data
        var link = $(this),
            url = link.attr('data-ajax-url'),
            method = link.attr('data-ajax-method');

        // Handle the ajax response, display the page in a modal
        var handleResponse = function (response) {
            modalContent.empty().append(response);
            modal.addClass('active');
        };

        // Make the ajax request
        if (method == 'post') {
            $.post(url, {}, handleResponse);
        } else {
            $.get(url, {}, handleResponse);
        }
    });

    // Close modal
    $('.modal__close').click(function (e) {
        if ($('.modal').hasClass('active')) {
            $('.modal').removeClass('active');
            $('.modal__content').empty();
        }
    });

    // Add comment to post
    $('.comment-form').submit(function (e) {
        e.preventDefault();

        var form = $(this),
            postId = form.find('#postId').eq(0).val(),
            comment = form.find('#comment').eq(0),
            comments = form.parent().find('.post__comments').eq(0),
            viewComments = form.parent().find('.post__view-comments').eq(0),
            postMeta = form.parent().find('.post__meta').eq(0),
            commentText = comment.val();

        $.post("/Comment/Add", { postId: postId, comment: commentText }, function (response) {
            comments.remove();
            viewComments.remove();
            $(response).insertAfter(postMeta);
            comment.val('');
        });
    });

    // Show extra comments
    $('.post__view-comments').click(function (e) {
        e.preventDefault();

        var hidden = $(this).next('.post__comments').find('li.hidden'),
            visible = $(this).next('.post__comments').find('li.visible');

        if (hidden.length > 0) {
            hidden.removeClass('hidden').addClass('visible');
        } else {
            visible.removeClass('visible').addClass('hidden');
        }
    });

    // Toggle like on post
    $('.like-button').click(function (e) {
        e.preventDefault();
        var btn = $(this),
            likes = btn.next('strong').find('.num-likes').eq(0),
            currentLikes = parseInt(likes.html()),
            postId = btn.attr('data-post-id');

        $.post('/Like/Toggle', { postId: postId }, function (response) {
            // Toggle class to liked
            btn.toggleClass('liked');

            // Increase/decrease likes by 1
            var likeIncrease = btn.hasClass('liked') ? 1 : -1;
            likes.html((currentLikes + likeIncrease));
        });
    });

    // Open search form
    $('.search-button').click(function (e) {
        e.preventDefault();
        var form = $('.search-form');

        if (form.css('display') == 'none') {
            $(this).find('i').removeClass('fa-search').addClass('fa-times');
            form.css('display', 'inline-block');
            form.find('#s').focus();
        } else {
            $(this).find('i').removeClass('fa-times').addClass('fa-search');
            form.css('display', 'none');
        }
    });
}());