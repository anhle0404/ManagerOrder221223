$(document).ready(function () {
    GetAllProduct();
});

//Sự kiện get danh sách sản phẩm
function GetAllProduct() {
    $.ajax({
        url: '/RegisterProduct/GetAll',
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
                var htmlAction = `<button class="btn btn-info btn-sm p-2" onclick="return onEditProduct(${item.Id});"><span class="mdi mdi-pencil mdi-14px"></span></button>
                                <button class="btn btn-danger btn-sm p-2" onclick="return onDeleteProduct(${item.Id},'${item.ProductCode}');"><span class="mdi mdi-delete mdi-14px"></span></button>`;

                html += `<tr tabindex="0">
                            <td class="sticky-left-table bg-light text-dark fw-bold">${item.ProductCode}</td>
                            <td>${item.ProductName}</td>
                            <td>${item.UnitName}</td>
                            <td class="text-end">${new Intl.NumberFormat().format(item.QtyInventory)}</td>
                            <td class="text-end">${new Intl.NumberFormat().format(item.QtyImport)}</td>
                            <td class="text-end">${new Intl.NumberFormat().format(item.QtyExport)}</td>
                            <td class="text-end">${new Intl.NumberFormat().format(item.ProductImportPrice)} vnđ</td>
                            <td class="text-end">${new Intl.NumberFormat().format(item.WholesalePrice)} vnđ</td>
                            <td class="sticky-right-table bg-light">
                                ${htmlAction}
                            </td>
                        </tr>`;


                htmlCard += `<div class="card mb-3">
                                <div class="card-hearder border-bottom bg-light d-flex justify-content-between p-2">
                                    <h5 class="card-title text-uppercase p-0 m-0 " onclick="return onEditProduct(${item.Id});">${item.ProductCode}</h5>
                                    <span class="mdi mdi-delete mdi-14px text-danger" onclick="return onDeleteProduct(${item.Id},'${item.ProductCode}');"></span>
                                </div>
                                <div class="card-body p-2">
                                    ${item.ProductName}
                                </div>
                                <div class="card-footer d-flex justify-content-between flex-wrap p-2">
                                    <small>Đơn vị: <span class="text-dark fw-bold">${item.UnitName}</span></small>
                                    <small>Số lượng: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.QtyInventory)}</span></small>
                                    <small>Giá bán: <span class="text-danger fw-bold">${new Intl.NumberFormat().format(item.WholesalePrice)} vnđ</span></small>
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
        Id: parseInt($('#RegisterProduct_Id').val()),
        ProductCode: $('#RegisterProduct_ProductCode').val(),
        ProductName: $('#RegisterProduct_ProductName').val(),
        Unit: parseInt($('#RegisterProduct_Unit').val()),
        GiaNhap: parseFloat($('#RegisterProduct_ProductImportPrice').val()),
        GiaBanChung: parseFloat($('#RegisterProduct_WholesalePrice').val())
    };
    //console.log(obj);
    if (CheckValidate(obj)) {
        $.ajax({
            url: '/RegisterProduct/SaveData',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(obj),
            contentType: 'application/json',
            success: function (result) {
                //console.log(result);
                //alert(result.message);
                if (parseInt(result.status) == 1) {
                    GetAllProduct();

                    $('#RegisterProduct_Id').val(0);
                    document.getElementById('frm_RegisterProduct').reset();

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
function onAddProduct() {
    document.getElementById('frm_RegisterProduct').reset();
    $('#modalCenterTitle').text('Thêm mới sản phẩm');
    $('#modalCenter').modal('show');
}

//Sự kiện click sửa
function onEditProduct(id) {
    $.ajax({
        url: '/RegisterProduct/GetByID',
        type: 'GET',
        dataType: 'json',
        data: {
            id: id
        },
        contentType: 'application/json',
        success: function (result) {
            if (result != null) {
                $('#RegisterProduct_Id').val(result.Id);
                $('#RegisterProduct_ProductCode').val(result.ProductCode);
                $('#RegisterProduct_ProductName').val(result.ProductName);
                $('#RegisterProduct_Unit').val(result.Unit);
                $('#RegisterProduct_ProductImportPrice').val(result.GiaNhap);
                $('#RegisterProduct_WholesalePrice').val(result.GiaBanChung);

                $('#modalCenterTitle').text('Cập nhật sản phẩm');
                $('#modalCenter').modal('show');
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện click xóa
function onDeleteProduct(id, code) {
    var ans = confirm(`Bạn có chắc muốn xóa sản phẩm [${code}] không!`);
    if (ans) {
        $.ajax({
            url: '/RegisterProduct/Delete',
            type: 'GET',
            dataType: 'json',
            data: {
                id: id
            },
            contentType: 'application/json',
            success: function (result) {
                //alert(result.message);
                if (parseInt(result.status) > 0) {
                    GetAllProduct();
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
    if (obj.ProductCode.trim() == '') {
        alert('Vui lòng nhập Mã sản phẩm!');
        return false;
    }

    if (obj.ProductName.trim() == '') {
        alert('Vui lòng nhập Tên sản phẩm!');
        return false;
    }

    if (obj.Unit <= 0) {
        alert('Vui lòng nhập Đơn vị!');
        return false;
    }

    if (obj.ProductImportPrice <= 0) {
        alert('Vui lòng nhập Giá nhập!');
        return false;
    }

    if (obj.WholesalePrice <= 0) {
        alert('Vui lòng nhập Giá bán lẻ!');
        return false;
    }

    return true;
}

//Sự kiện nhập giá
function onInput(event) {
    //var value = $(event.target).val();
    //$(event.target).val(new Intl.NumberFormat().format(value));
}