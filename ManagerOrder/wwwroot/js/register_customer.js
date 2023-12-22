$(document).ready(function () {
    GetAllCustomer();
});

//Sự kiện get danh sách sản phẩm
function GetAllCustomer() {
    $.ajax({
        url: '/RegisterCustomer/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            keyword: $('#keyword').val()
        },
        contentType: 'application/json',
        success: function (result) {
            var html = '';
            var htmlCard = '';
            $.each(result, function (key, item) {
                var htmlAction = `<button class="btn btn-info btn-sm p-2" onclick="return onEditCustomer(${item.Id});"><span class="mdi mdi-pencil mdi-14px"></span></button>
                                <button class="btn btn-danger btn-sm p-2" onclick="return onDeleteCustomer(${item.Id},'${item.CustomerCode}');"><span class="mdi mdi-delete mdi-14px"></span></button>`;

                html += `<tr tabindex="0">
                            <td class="sticky-left-table bg-light text-dark fw-bold">${item.CustomerCode}</td>
                            <td>${item.CustomerName}</td>
                            <td>${item.CustomerInitials}</td>
                            <td>${item.CustomerPhone}</td>
                            <td>${item.CustomerAddress}</td>
                            <td class="sticky-right-table bg-light">
                                ${htmlAction}
                            </td>
                        </tr>`;


                htmlCard += `<div class="card mb-3">
                                <div class="card-hearder border-bottom bg-light d-flex justify-content-between p-2">
                                    <h5 class="card-title text-uppercase p-0 m-0 " onclick="return onEditCustomer(${item.Id});">${item.CustomerCode}</h5>
                                    <span class="mdi mdi-delete mdi-14px text-danger" onclick="return onDeleteCustomer(${item.Id},'${item.CustomerCode}');"></span>
                                </div>
                                <div class="card-body p-1">
                                    <p>Tên khách hàng: <span class="text-dark">${item.CustomerName}</span></p>
                                    <p>Tên viết tắt: <span class="text-dark">${item.CustomerInitials}</span></p>
                                    <p>Sđt: <a href="tel:${item.CustomerPhone}">${item.CustomerPhone}</a></p>
                                    <p>Địa chỉ: <span class="text-dark">${item.CustomerAddress}</span></p>
                                </div>
                            </div>`;
            });

            $('.table-border-bottom-0').html(html);
            $('.list-card').html(htmlCard);
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện lưu thông tin sẩn phẩm
function SaveDataProduct() {
    var obj = {
        Id: parseInt($('#RegisterCustomer_Id').val()),
        CustomerCode: $('#RegisterCustomer_CustomerCode').val(),
        CustomerName: $('#RegisterCustomer_CustomerName').val(),
        CustomerInitials: $('#RegisterCustomer_CustomerInitials').val(),
        CustomerPhone: $('#RegisterCustomer_CustomerPhone').val(),
        CustomerAddress: $('#RegisterCustomer_CustomerAddress').val(),
    };
    //console.log(obj);
    if (CheckValidate(obj)) {
        $.ajax({
            url: '/RegisterCustomer/SaveData',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(obj),
            contentType: 'application/json',
            success: function (result) {
                //console.log(result);
                //alert(result.message);
                if (parseInt(result.status) == 1) {
                    GetAllCustomer();

                    $('#RegisterCustomer_Id').val(0);
                    document.getElementById('frm_RegisterCustomer').reset();
                } else {
                    alert(result.message)
                }
            },
            error: function (err) {
                alert(err.responseText);
            }
        })
    }
}

//Sự kiện click thêm mới
function onAddCustomer() {
    document.getElementById('frm_RegisterCustomer').reset();
    $('#modalCenterTitle').text('Thêm mới khách hàng');
    $('#modalCenter').modal('show');
}

//Sự kiện click sửa
function onEditCustomer(id) {
    $.ajax({
        url: '/RegisterCustomer/GetByID',
        type: 'GET',
        dataType: 'json',
        data: {
            id: id
        },
        contentType: 'application/json',
        success: function (result) {
            if (result != null) {
                $('#RegisterCustomer_Id').val(result.Id);
                $('#RegisterCustomer_CustomerCode').val(result.CustomerCode);
                $('#RegisterCustomer_CustomerName').val(result.CustomerName);
                $('#RegisterCustomer_CustomerInitials').val(result.CustomerInitials);
                $('#RegisterCustomer_CustomerPhone').val(result.CustomerPhone);
                $('#RegisterCustomer_CustomerAddress').val(result.CustomerAddress);

                $('#modalCenterTitle').text('Cập nhật khách hàng');
                $('#modalCenter').modal('show');
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện click xóa
function onDeleteCustomer(id, code) {
    var ans = confirm(`Bạn có chắc muốn xóa khách hàng [${code}] không!`);
    if (ans) {
        $.ajax({
            url: '/RegisterCustomer/Delete',
            type: 'GET',
            dataType: 'json',
            data: {
                id: id
            },
            contentType: 'application/json',
            success: function (result) {
                //alert(result.message);
                if (parseInt(result.status) > 0) {
                    GetAllCustomer();
                } else {
                    alert(result.message)
                }
            },
            error: function (err) {
                alert(err.responseText);
            }
        })
    }

}


//Sự kiện check validate
function CheckValidate(obj) {
    if (obj.CustomerCode.trim() == '') {
        alert('Vui lòng nhập Mã khách hàng!');
        return false;
    }

    if (obj.CustomerName.trim() == '') {
        alert('Vui lòng nhập Tên khách hàng!');
        return false;
    }

    if (obj.CustomerPhone.trim() == '') {
        alert('Vui lòng nhập Số điện thoại!');
        return false;
    }

    if (obj.CustomerAddress.trim() == '') {
        alert('Vui lòng nhập Địa chỉ!');
        return false;
    }

    return true;
}