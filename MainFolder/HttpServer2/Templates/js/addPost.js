function addPost() {
    let contentVal = $('#post_content').val();
    if (checkContent(contentVal)) {
        $.ajax({
            url: '/post/addpost',
            method: 'post',
            dataType: 'html',
            data: { content: contentVal },
            success: function (result) {
                $('#posts').append(result)
            },
            statusCode: {
                400: function () {
                    $('#post_content').attr('style', "border: 2px solid red")
                }
            }
        });
        $('#post_content').removeAttr('style');
    } else {
        $('#post_content').attr('style', "border: 2px solid red")
    }
    $('#post_content').val('');
}

function addComment() {
    let contentVal = $('#comment_content').val();
    let postId = $('#comment_content').attr('postid');
    console.log(postId);
    if (checkContent(contentVal) && contentVal.length <= 500) {
        $.ajax({
            url: '/post/' + postId + '/addcomment',
            method: 'post',
            dataType: 'html',
            data: { content: contentVal },
            success: function (result) {
                $('#comments').append(result)
            },
            statusCode: {
                400: function () {
                    $('#comment_content').attr('style', "border: 2px solid red")
                }
            }
        });
        $('#comment_content').removeAttr('style');
    } else {
        $('#comment_content').attr('style', "border: 2px solid red")
    }
    $('#comment_content').val('');
}

$('#post_content').on("change focus", function () {
    $('#post_content').removeAttr('style');
});

$('#comment_content').on("change focus", function () {
    $('#comment_content').removeAttr('style');
});

function checkContent(str) {
    if (str.length > 1000)
        return false;
    for (var i = 0; i < str.length; i++)
        if (str.charAt(i) == '\'')
            return false;
       return true;
}