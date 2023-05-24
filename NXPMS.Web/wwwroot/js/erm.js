
$(document).ready(function () {
    $("#est").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#est").val();
                $.ajax({
                    type: "GET",
                    url: "/Employees/GetEmployeeNames?name="+text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })

    $("#FullName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#FullName").val();
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

    $("#ReportsToName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ReportsToName").val();
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

});