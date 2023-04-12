﻿
//======= Script to Grant the Permission (with RoleID) to User (with UserID) =======//
function grantPermission(user_id, role_id) {
    const btnGrant = document.getElementById("btn_grant_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/Security/GrantUserPermission',
        dataType: "text",
        data: { usd: user_id, rld: role_id },
        success: function (result) {
            if (result == "granted") {
                btnGrant.disabled = true;
                location.reload();
            }
            else {
                alert('Granting Permission failed!');
                console.log(result);
            }
        },
        error: function () {
            alert('Sorry Permission was not Granted.');
            console.log('Failed ');
        }
    })
}

//===== Function to Revoke the Permission (with PermissionID) from User ========//
function revokePermission(user_id, role_id) {
    const btnRevoke = document.getElementById("btn_revoke_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/Security/RevokeUserPermission',
        dataType: "text",
        data: { usd: user_id, rld: role_id },
        success: function (result) {
            if (result == "revoked") {
                btnRevoke.disabled = true;
                location.reload();
            }
            else {
                alert('Revoking Permission failed!');
            }
        },
        error: function () {
            alert('Sorry Permission was not Revoked.');
            console.log('Failed ');
        }
    })
}

//$(document).ready(function () {
//    console.log('Web page is ready!')
//    $("#SearchString").autocomplete(
//        {
//            minLength: 3,
//            source: function (request, response) {
//                var text = $("#SearchString").val();
//                $.ajax({
//                    type: "GET",
//                    url: "/UserAdministration/Home/GetNamesOfEmployeeUsers?text=" + text,
//                    data: { text: request.term },
//                    success: function (data) {
//                        response($.map(data, function (item) {
//                            return { label: item, value: item }
//                        }))
//                    }
//                })
//            }
//        })
//})
