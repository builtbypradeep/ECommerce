var dataTable;

$(document).ready(function () {
    LoadDataTable();
});

function LoadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {url: '/admin/product/getall' },

        "columns": [
            { data: 'title' , "width" : "15%"},
            { data: 'description', "width": "15%" },
            { data: 'isbn', "width": "15%" },
            { data: 'author', "width": "15%" },
            { data: 'listPrice', "width": "15%" },
            { data: 'price', "width": "15%" },
            { data: 'price50', "width": "15%" },
            { data: 'price100', "width": "15%" },
            {
                data: 'id',
                    "render": function (data)
                    {
                        return `<div class="w-75 btn-group role= "group">

                         <a href = "/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"  > <i class="bi bi-pencil-square"></i> Edit </a>
                           <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"  > <i class="bi bi-trash-fill"></i> Delete </a>
                                
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