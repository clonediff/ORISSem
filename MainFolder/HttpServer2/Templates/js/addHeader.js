function getHeader() {
    $.ajax({
        url: '/header',
        method: 'get',
        dataType: 'html',
        data: { },
        success: function (result) {
            $('#header_placer').append(result)
        }
        });
}

$(document).ready(function () {
    getHeader();
})