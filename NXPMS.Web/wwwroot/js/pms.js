
//===== Function to delete a submission message ========//
function deleteSubmission(submission_id) {
    if (confirm('Are you sure your want to delete?')) {
        {
            $.ajax({
                type: 'POST',
                url: '/PMS/DeleteSubmission',
                dataType: "text",
                data: { sd: submission_id },
                success: function (result) {
                    if (result == "deleted") {
                        location.reload();
                    }
                    else {
                        alert('Deleting record failed!');
                        console.log(result);
                    }
                },
                error: function () {
                    alert('Sorry deleting operation could not be completed.');
                    console.log('Failed ');
                }
            })
        }
    }
}

//===== Function to mark a submission message as 'Actioned' ========//
function markDone(submission_id) {
    //const btnRevoke = document.getElementById("btn_revoke_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/PMS/MarkSubmissionAsDone',
        dataType: "text",
        data: { sd: submission_id },
        success: function (result) {
            if (result == "marked") {
                location.reload();
            }
            else {
                alert('Updating action status failed!');
            }
        },
        error: function () {
            alert('Sorry action status was not updated.');
            console.log('Failed ');
        }
    })
}

$("#nm").autocomplete(
    {
        minLength: 3,
        source: function (request, response) {
            var text = $("#nm").val();
            $.ajax({
                type: "GET",
                url: "/Employees/GetEmployeeNames?name=" + text,
                data: { text: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item, value: item }
                    }))
                }
            })
        }
    })