function validateUser()
{
    var redColor = "#d82b2b";
    var user = document.forms["form1"]["user"].value;
    var pass = document.forms["form1"]["pass"].value;
    var pass2 = document.forms["form1"]["pass2"].value;
    var pattern = /^[A-Za-z0-9\s]+@[A-Za-z0-9\s]+.[A-Za-z]+$/;

    //testa se nome utente e' una mail
    if (!pattern.test(user))
    {
        blankFields();
        document.getElementById("user").style.backgroundColor = redColor;
        alert("Username deve essere un indirizzo mail!");

        return false;
    }

    if (pass != pass2)
    {
        blankFields();
        document.getElementById("pass").style.backgroundColor = redColor;
        document.getElementById("pass2").style.backgroundColor = redColor;
        alert("Le password non corrispondono!");

        return false;
    }
    
    form1.submit();
}

function blankFields()
{
    var whiteColor = "#ffffffff";
    document.getElementById("user").style.backgroundColor = whiteColor;
    document.getElementById("pass").style.backgroundColor = whiteColor;
    document.getElementById("pass2").style.backgroundColor = whiteColor;
}