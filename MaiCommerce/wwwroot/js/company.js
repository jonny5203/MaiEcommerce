var dataTable
$(document).ready(function () {
    loadDataTable();
});

//loads data json data from ajax call and then putting it into
//an array with a datastructure of the json obj, to be used to populate
//the products table in Product/Index. Sent along with some html to create the buttons
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "render": function (data)
                {
                    return `
                        <div class="w-55 btn-group" role="group">
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a onClick=Delete('/admin/company/delete?id=${data}') class="btn btn-danger mx-2">
                                <i class="bi bi-trash-fill"></i> Delete
                            </a>
                        </div>
                    `
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url)
{
    //creaing alert window confirming if you really want to delete or not
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
            //do an ajax request after the user has confirmed yes
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data)
                {
                    dataTable.ajax.reload()
                    //toastr is already imported in _Layout
                    toastr.success(data.message)
                }
            })
        }
    });
}