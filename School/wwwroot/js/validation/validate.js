function validatePass()
{
    var color = "#92fc71";
    var ret = true;
    var nome = document.forms["form"]["nome"].value;
    var cognome = document.forms["form"]["cognome"].value;

    if (nome != cognome)
    {
        document.getElementById("pass").style.backgroundColor = color;
        document.getElementById("pass2").style.backgroundColor = color;

        ret = false;
    }

    return ret;
}