var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("Cancelled")) {
        loadList("Cancelled");
    } else {
        if (url.includes("Completed")) {
            loadList("Completed");
        } else {
            loadList("Being Prepared");
        }
    }
});

function loadList(param) {
    dataTable = $("#DT_load").DataTable({
        "ajax": {
            "url": "/api/order?status="+param,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "orderHeader.id", "width": "20%" },
            { "data": "orderHeader.pickUpName", "width": "20%" },
            { "data": "orderHeader.applicationUser.email", "width": "20%" },
            { "data": "orderHeader.orderTotal", "width": "20%" },
            {
                "data": "orderHeader.id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Order/OrderDetails?id=${data}" class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                <i class="fas fa-info-circle"></i> Details
                                </a>
                        </div>`;
                },
                "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "No data was found."
        }, "width": "100%"
    });
}