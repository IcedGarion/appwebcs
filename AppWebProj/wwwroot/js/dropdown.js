function showDiv() {
    document.getElementById('topbar').style.display = "block";
    document.getElementById('topbarbtn').style.display = "block";
}
function hide() {

    var bar = document.getElementById('topbar');

    bar.style.display = 'none';
    var btn = document.getElementById('topbarbtn');

    btn.style.display = 'none';

}