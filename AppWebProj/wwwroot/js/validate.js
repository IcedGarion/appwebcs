function validatePass() {
    var color = "#d82b2b";
    var pass = document.forms["form1"]["pass"].value;
    var pass2 = document.forms["form1"]["pass2"].value;

    if (pass != pass2) {
        document.getElementById("pass").style.backgroundColor = color;
        document.getElementById("pass2").style.backgroundColor = color;

        return false;
    }
    else {
        form1.submit();
    }
}