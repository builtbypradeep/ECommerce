var dataTable;

$(document).ready(function () {
    LoadDataTable();
});

function LoadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": { url: '/admin/companies/getall' },

        "columns": [
            { data: 'Id' , "width" : "15%"},
            { data: 'Name', "width": "15%" },
            { data: 'Address', "width": "15%" },
            { data: 'City', "width": "15%" },
            { data: 'State', "width": "15%" },
            { data: 'PostalCode', "width": "15%" },
            { data: 'PhoneNumber', "width": "15%" },
            {
                data: 'id',
                    "render": function (data)
                    {
                        return `<div class="w-75 btn-group role= "group">

                         <a href = "/admin/comapny/upsert?id=${data}" class="btn btn-primary mx-2"  > <i class="bi bi-pencil-square"></i> Edit </a>
                           <a onClick=Delete('/admin/comapny/delete/${data}') class="btn btn-danger mx-2"  > <i class="bi bi-trash-fill"></i> Delete </a>
                                
                        </div>`
                },
                "width": "15%"
            }
         ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            //Swal.fire({
            //    title: "Deleted!",
            //    text: "Your file has been deleted.",
            //    icon: "success"
            //});

            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}