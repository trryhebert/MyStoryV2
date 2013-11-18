$(document).ready(function () {
    $("#btnSubmit").click(function () {
        if (tinymce.get('txtComment').getContent() == "") {
            alert("Please enter a comment");
            return false;
        }
    });
});