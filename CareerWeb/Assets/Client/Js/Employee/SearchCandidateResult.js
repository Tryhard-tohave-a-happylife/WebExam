$(document).ready(function () {
    checkSaveFile();
})



var AreaID = 0, JobID = 0, Name = 0;

$(document).ready(function () {
    GetURLParameter();
    setSelectValue();

    $("#tim-nang-cao").click(function () {
        $("#search-more").slideToggle(500);
    });
});

function GetURLParameter() {
    var string = window.location.search.substring(1);

    var sPageURL = decodeURI(string);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == "AreaID") {
            AreaID = sParameterName[1];
        }
        else if (sParameterName[0] == "JobID") {
            JobID = sParameterName[1];
        }
        else if (sParameterName[0] == "Name") {
            NameID = sParameterName[1];

        }
    }
}

function setSelectValue() {
    if (AreaID != 0) {
        document.getElementById("dia-diem").value = AreaID;
    }
    if (JobID != 0) {
        document.getElementById("linh-vuc").value = JobID;
    }
    if (Name != 0) {
        document.getElementById("key-word").value = NameID;

        console.log(Name);
    }
}


function saveFile() {
    var element = $(event.target);
    var userId = element.attr("stt");
    $.ajax({
        data: { userId: userId },
        url: '/Employee/SaveCandidate',
        dataType: 'json',
        method: 'POST',
        beforeSend: function () {

        },
        success: function (res) {
            if (res.status == true) {
                element.text("Đã lưu hồ sơ");
            }
        }
    })
}

function checkSaveFile() {
    var element = $(".save-button");
    var userId = new Array();
    for (var i = 0; i < element.length; i++) {
        userId.push(element[i].getAttribute("stt"));
    }
    console.log(userId);
    if (userId != null) {
        $.ajax({
            data: { userId: userId },
            url: '/Employee/checkSavedCandidate',
            dataType: 'json',
            method: 'POST',
            beforeSend: function () {

            },
            success: function (res) {
                saveList = res.savedList;
                console.log(res.savedList.length);
                console.log(saveList.length);
                for (var i = 0; i < saveList.length; i++) {

                    if (saveList[i] == true) {
                        element[i].textContent = "Đã lưu hồ sơ";

                    }
                }
            }
        })
    }

}

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