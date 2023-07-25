
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

    $("#DepartmentHeadName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#DepartmentHeadName").val();
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

    $("#DepartmentAltHeadName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#DepartmentAltHeadName").val();
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

    $("#UnitHeadName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#UnitHeadName").val();
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

    $("#UnitAltHeadName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#UnitAltHeadName").val();
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

    $("#LocationHeadName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#LocationHeadName").val();
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

    $("#LocationAltHeadName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#LocationAltHeadName").val();
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