$(document).ready(function () {
    $("#tim-nang-cao").click(function () {
        $("#search-more").slideToggle(500);
    });

    function checkName() {
        var name = document.getElementById("key-word").value;
        var name1 = "";
        var i = 1;
        name1 = name.charAt(0).toUpperCase();
        if (name.length != 0) {
            name1 += name[0].toUpperCase();
            while (i < name.length) {
                if (name[i] == " " && name[i + 1] == " ") {
                    i++;
                    if (i == name.length) break;
                }
                else if (name[i - 1] == " ") {
                    name1 = name1 + name[i].toUpperCase();
                    i++;
                }
                else {
                    name1 = name1 + name[i];
                    i++;
                }
            }
        }
        document.getElementById("key-word").value = name1;
    }
})
function search() {
    var area = ($('#dia-diem :selected').val() != null) ? $('#dia-diem :selected').val() : 0;
    var job = ($('#linh-vuc option:selected').val() != null) ? $('#linh-vuc option:selected').val() : 0;
    var name = ($('#key-word:selected').val() != null) ? $('#key-word:selected').val() : "";
    console.log("here");
    var dbParam = "KeyWord=" + name + "&AreaID=" + area + "&JobID=" + job;
    window.location.href = "/SearchCandidateResult?" + dbParam;
}

function refresh() {
    location.reload();
}