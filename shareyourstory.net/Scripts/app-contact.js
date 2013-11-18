$(document).ready(function () {
    $("#txtMessage").val("Message");

    $("#btnSubmit").click(function () {
        if ($("#txtSubject").val() == "Subject" || $("#txtSubject").val() == "") {
            alert("Please enter a subject");
            return false;
        }
        if ($("#txtMessage").html() == "Message" || $("#txtMessage").html() == "") {
            alert("Please enter a message");
            return false;
        }
    });

    //$("#txtMessage").click(function () {
    $("#txtMessage").focus(function () {
        if ($("#txtMessage").val() == "Message")
            $("#txtMessage").val("");

        $("#txtMessage").css("color", "#000000");
        $("#txtMessage").css("font-style", "normal");
    });

    $("#txtSubject").focus(function () {
        if ($("#txtSubject").val() == "Subject")
            $("#txtSubject").val("");

        $("#txtSubject").css("color", "#000000");
        $("#txtSubject").css("font-style", "normal");
    });
});