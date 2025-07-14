var dataTable;

$(document).ready(function () {

    var url = window.location.search;

    if (url.includes("inprocess")) {
        LoadDataTable("inprocess");
    }
    else if (url.includes("completed")) {
        LoadDataTable("completed");
    }
    else if (url.includes("approved")) {
        LoadDataTable("approved");
    }
    else if (url.includes("paymentpending")) {
        LoadDataTable("paymentpending");
    }
    else {
        LoadDataTable("all");
    }
});

function LoadDataTable(status) {
    dataTable = $('#myOrderTable').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status},

        "columns": [
            { data: 'id' , "width" : "15%"},
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "15%" },
            {
                data: 'id',
                    "render": function (data)
                    {
                        return `<div class="w-75 btn-group" role= "group">

                       <a href="/admin/order/delete?orderId=${data}" class="btn btn-primary mx-2"  > <i class="bi bi-pencil-square"></i> </a>

                        </div>`
                },
                "width": "15%"
            }
         ]
    });
}

