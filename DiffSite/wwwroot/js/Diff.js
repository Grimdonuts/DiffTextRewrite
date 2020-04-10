$(function () {
    $('#postDiff').on('click', function () {
        var formData = new FormData();
        formData.append('file1', $('#firstFile')[0].files[0]);
        formData.append('file2', $('#secondFile')[0].files[0]);
        $.ajax({
            url: 'http://localhost:59134/api/diff',
            type: 'POST',
            data: formData,
            success: function (data) {
                $('#leftDiff').html(data.left);
                $('#rightDiff').html(data.right);
            },
            cache: false,
            contentType: false,
            processData: false
        });
    });
});