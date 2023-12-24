$(document).ready(function () {
    GetAllOrder();
})

//Sự kiện get danh sách đơn hàng
function GetAllOrder() {
    $.ajax({
        url: '/HistoryOrder/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            dateStart: $('#datestart_history_order').val(),
            dateEnd: $('#dateend_history_order').val(),
            deliverystatus: parseInt($('#isapproved_history_order').val()),
            keyword: $('#keyword_history_order').val(),
        },
        contentType: 'application/json',
        success: function (result) {
            //console.log(result);
            if (parseInt(result) != 0) {

                var html = '';
                var htmlCard = '';
                $.each(result, function (key, item) {
                    var bgStatus = item.DeliveryStatus == 1 ? 'bg-success text-white' : 'bg-warning text-dark';
                    var colorStatus = item.DeliveryStatus == 1 ? 'text-white' : '';

                    var isApprovedText = item.IsApproved == 0 ? "Chờ duyệt đơn hàng" : "Duyệt đơn hàng";
                    var deliveryStatusText = item.DeliveryStatus == 0 ? "Chờ giao hàng" : "Giao hàng thành công";
                    var isFullPaymentText = item.IsFullPayment == 0 ? "Chờ thanh toán" : (item.IsFullPayment == 1 ? "Thanh toán 1 phần" : "Thanh toán toàn bộ");

                    html += `<tr tabindex="0">
                            <td class="text-nowrap text-center"><input type="checkbox" name="IsApproved" value="${item.Id}" onclick="return onSelected(event);"/></td>
                            <td class="text-nowrap text-center">${key + 1}</td>
                            <td class="text-nowrap">${isApprovedText}</td>
                            <td class="text-nowrap">${deliveryStatusText}</td>
                            <td class="text-nowrap">${isFullPaymentText}</td>
                            <td class="text-nowrap fw-bold sticky-left-table ${bgStatus}" onclick="return GetOrderDetail(${item.Id});">${item.OrderCode}</td>
                            <td class="text-nowrap">${item.CreatedDate}</td>
                            <td style="white-space:pre-line">${item.CustomerCode}</td>
                            <td style="white-space:pre-line">${item.CustomerName}</td>
                            <td style="white-space:pre-line">${item.CustomerAddress}</td>
                            <td style="white-space:pre-line"><a href="tel:${item.CustomerPhone}">${item.CustomerPhone}</a></td>
                            <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.TotalIntoMoney)} vnđ</td>
                            <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.CustomerPayment)} vnđ</td>
                            <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.MoneyOwedCustomer)} vnđ</td>
                        </tr>`;

                    htmlCard += `<div class="card mb-3">
                                <div class="card-hearder border-bottom p-2 justify-content-between ${bgStatus}">
                                    <h5 class="card-title text-uppercase p-0 m-0 ${colorStatus}" onclick="return GetOrderDetail(${item.Id});">${item.OrderCode}</h5>
                                    ${item.CreatedDate}
                                </div>
                                <div class="card-body p-2">
                                    <p class="mb-2">Mã khách hàng: <span class="text-dark">${item.CustomerCode}</span></p>
                                    <p class="mb-2">Tên khách hàng: <span class="text-dark">${item.CustomerName}</span></p>
                                    <p class="mb-2">Địa chỉ: <span class="text-dark">${item.CustomerAddress}</span></p>
                                    <p class="mb-0">Stt: <a href="tel:${item.CustomerPhone}">${item.CustomerPhone}</a></p>
                                    
                                </div>
                                <div class="card-footer d-flex justify-content-between flex-wrap p-2">
                                    <small>Tổng tiền: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.TotalIntoMoney)} vnđ</span></small>
                                    <small>Tiền khách trả: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.CustomerPayment)} vnđ</span></small>
                                    <small>Tiền khách nợ: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.MoneyOwedCustomer)} vnđ</span></small>
                                </div>
                            </div>`;
                });

                $('.tbody').html(html);
                $('.list-card').html(htmlCard);

                $('#searchModal').modal('hide');
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện get chi tiết đơn hàng
function GetOrderDetail(idOrder) {

    $.ajax({
        url: '/HistoryOrderDetailRepo/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            historyOrderId: idOrder
        },
        contentType: 'application/json',
        success: function (result) {
            //console.log(result);
            if (parseInt(result.status) == 1) {

                var htmlbody = '';

                var htmlfooter = `<p class="text-dark">Tổng tiền: <span class="text-danger">${new Intl.NumberFormat().format(result.order.TongTien)} vnđ</span></p>
                                    <button type="button" class="btn btn-primary btn-sm" onclick="return onClickDelivered(${result.order.Id}, ${result.order.TongTien},${result.order.IsApproved},${result.order.DeliveryStatus});">Giao hàng</button>`;
                if (result.order.DeliveryStatus == 1) {
                    htmlfooter = `<p class="text-dark">Tổng tiền: <span class="text-danger">${new Intl.NumberFormat().format(result.order.TongTien)} vnđ</span></p>
                                    <button type="button" class="btn btn-success btn-sm">Giao hàng thành công</button>`;
                } else if (result.order.IsApproved == 0) {
                    htmlfooter = `<p class="text-dark">Tổng tiền: <span class="text-danger">${new Intl.NumberFormat().format(result.order.TongTien)} vnđ</span></p>
                                    <button type="button" class="btn btn-secondary btn-sm">Chờ duyệt đơn hàng</button>`;
                }

                $.each(result.detail, function (key, item) {
                    htmlbody += `<div class="card border-top border-primary border-2 m-1 p-0">
                                <div class="card-body p-1">
                                    <h5 class="card-title p-0 m-0">${item.ProductCode}</h5>
                                    ${item.ProductName}
                                </div>
                                <div class="card-footer d-flex justify-content-between flex-wrap p-2">
                                    <small>Đơn giá: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.Price)} vnđ</span></small>
                                    <small>Số lượng: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.Quantity)}</span></small>
                                    <small>Thành tiền: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.TotalPrice)} vnđ</span></small>
                                </div>
                            </div>`;
                });

                $('.modal-title').text(result.order.OrderCode);
                $('.list-group').html(htmlbody);
                $('.modal-footer').html(htmlfooter);

                $('#basicModal').modal('show');
            } else {
                alert(result.message);
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện click giao hàng
function onClickDelivered(id, totalmoney, isApproved, deliveryStatus) {
    if (isApproved == 1) {
        alert('Đơn hàng đã được duyệt.\nBạn không thể giao hàng!')
    } else if (deliveryStatus == 1) {
        alert('Đơn hàng đã được giao thành công.\nBạn không thể giao hàng!')
    } else {
        let money = prompt("Vui lòng nhập số tiền khách trả để xác nhận giao hàng:", totalmoney);
        if (parseFloat(money) > 0) {
            var obj = {
                Id: id,
                CustomerPayment: parseFloat(money)
            };

            $.ajax({
                url: '/HistoryOrder/Update',
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(obj),
                contentType: 'application/json',
                success: function (result) {
                    if (parseInt(result) > 0) {
                        GetAllOrder();
                        $('#basicModal').modal('hide');
                    } else {
                        alert(result);
                    }
                },
                error: function (err) {
                    alert(err.responseText);
                }
            })
        }
    }
}


//Sự kiện click chọn tất cả
function onSelecteAll(event) {
    var checked = $(event.target).is(':checked');
    //console.log(checked);
    $('input[name="IsApproved"]').each((i, el) => {
        //console.log(el);
        $(el).prop("checked", checked);

        var checkedChild = $(el).is(':checked');
        var trElement = $($(el).parent()).parent();
        $(trElement).css("background-color", checkedChild ? "#ffb400" : "transparent");
    })
}

//Sự kiện click chọn đơn hàng
function onSelected(event) {
    var checked = $(event.target).is(':checked');
    var trElement = $($(event.target).parent()).parent();
    $(trElement).css("background-color", checked ? "#ffb400" : "transparent");
    if (!checked) {
        $('#selectall').prop('checked', false);
    }

}

//Sự kiện duyệt đơn hàng
function onApproved(isAppove) {
    var listSelected = $('input[name="IsApproved"]:checked');
    //console.log(listSelected.length);
    if (listSelected.length <= 0) {
        alert(`Vui lòng chọn hóa đơn muốn ${isAppove == 1 ? "duyệt" : "hủy duyệt"}!`);
    } else {
        //console.log(listSelected);
        var listOrder = $('input[name="IsApproved"]:checked').map(function () {
            var obj = {
                Id: parseInt(this.value),
                IsApproved: isAppove
            }
            return obj;
        }).get();

        var ans = confirm(`Bạn có chắc muốn ${isAppove == 1 ? "duyệt" : "hủy duyệt"} đơn hàng đã chọn không?`);
        if (ans) {
            $.ajax({
                url: '/HistoryOrder/Approved',
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(listOrder),
                contentType: 'application/json',
                success: function (result) {
                    if (parseInt(result) > 0) {
                        GetAllOrder();
                        $('#selectall').prop('checked', false);
                    } else {
                        alert(result);
                    }
                },
                error: function (err) {
                    alert(err.responseText);
                }
            })
        }
        //console.log(listOrder);
    }
}

//Sự kiện xác nhận giao hàng
function onDelivery(deliveryStatus) {
    var listSelected = $('input[name="IsApproved"]:checked');
    //console.log(listSelected.length);
    if (listSelected.length <= 0) {
        alert(`Vui lòng chọn hóa đơn muốn ${deliveryStatus == 1 ? "giao hàng" : "hủy giao hàng"}!`);
    } else {
        //console.log(listSelected);
        var listOrder = $('input[name="IsApproved"]:checked').map(function () {
            var obj = {
                Id: parseInt(this.value),
                DeliveryStatus: deliveryStatus
            }
            return obj;
        }).get();

        //console.log(listOrder);
        $.ajax({
            url: '/HistoryOrder/GetListDelivery',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(listOrder),
            contentType: 'application/json',
            success: function (result) {
                //console.log(result);
                var html = '';
                $.each(result, (key, item) => {
                    var htmlDetail = '';
                    $.each(item.detail, (i, val) => {
                        htmlDetail += `<div class="card border-top border-primary border-2 m-1 p-0">
                                            <div class="card-body p-1">
                                                <h5 class="card-title p-0 m-0">${val.ProductCode}</h5>
                                                ${val.ProductName}
                                            </div>
                                            <div class="card-footer d-flex justify-content-between p-2 flex-wrap" style="flex-wrap:wrap;">
                                                <small>Đơn giá: <span class="text-dark fw-bold">${val.Price} vnđ</span></small>
                                                <small>Số lượng: <span class="text-dark fw-bold">${val.Quantity}</span></small>
                                                <small>Thành tiền: <span class="text-dark fw-bold">${val.TotalPrice} vnđ</span></small>
                                            </div>
                                        </div>`;
                    });

                    html += `<div class="card">
                                <div class="card-header border-bottom bg-light d-flex justify-content-between flex-nowrap p-2">
                                    <h5 class="card-title text-uppercase p-0 m-0 fw-bold">${item.order.OrderCode}</h5>
                                    <span class="mdi mdi-delete text-danger" style="cursor:pointer;" onclick="return onDeleteFromlist(event,'${item.order.OrderCode}');"></span>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-sm-6 p-1">
                                            <div class="form-floating form-floating-outline">
                                                <input type="text" id="HistoryOrder_TongTien_${item.order.Id}" class="form-control" placeholder="" value="${new Intl.NumberFormat().format(item.order.TongTien)}" disabled />
                                                <label for="dateend_history_order">Tổng tiền</label>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 p-1">
                                            <div class="form-floating form-floating-outline">
                                                <input type="number" id="HistoryOrder_TienKhachTra_${item.order.Id}" class="form-control text-end" placeholder="" value="${deliveryStatus == 1 ? item.order.TongTien : item.order.TienKhachTra}" ${deliveryStatus == 1 ? "" :"disabled"}/>
                                                <label for="dateend_history_order">Tiền khách trả</label>
                                            </div>
                                        </div>
                                        <div class="accordion-item p-0" id="accordion_${item.order.Id}">
                                            <h2 class="accordion-header text-body d-flex justify-content-between bg-info w-100 p-2">
                                                <button type="button" class="accordion-button collapsed text-dark" data-bs-toggle="collapse" data-bs-target="#accordionIcon-${item.order.Id}" aria-controls="accordionIcon-${item.order.Id}" aria-expanded="false">
                                                    Xem chi tiết
                                                </button>
                                            </h2>

                                            <div id="accordionIcon-${item.order.Id}" class="accordion-collapse collapse" data-bs-parent="#accordion_${item.order.Id}" style="">
                                                <div class="accordion-body">
                                                    ${htmlDetail}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
                });

                var htmlFooter = '';
                if (!deliveryStatus) {
                    $('.delivery-title').text('Xác nhận hủy giao hàng');
                    htmlFooter = `<button type="button" class="btn btn-warning btn-sm text-dark w-100" id="btnDelevery" onclick="return Delivery(${deliveryStatus});">Xác nhận hủy giao hàng</button>`;
                } else {
                    $('.delivery-title').text('Xác nhận giao hàng');
                    htmlFooter = `<button type="button" class="btn btn-primary btn-sm w-100" id="btnDelevery" onclick="return Delivery(${deliveryStatus});">Xác nhận giao hàng</button>`;
                }

                $('#deliveryModal').modal('show');
                $('#deliveryModal').find('.modal-footer').html(htmlFooter);
                $('.list-group-delivery').html(html);
            },
            error: function (err) {
                alert(err.responseText);
            }
        })

        //var ans = confirm(`Bạn có chắc muốn ${deliveryStatus == 1 ? "giao hàng" : "hủy giao hàng"} đơn hàng đã chọn không?`);
        //if (ans) {
        //    
        //}
        //console.log(listOrder);
    }
}


//Sự kiện click xóa đơn hàng
function onDeleteFromlist(event, code) {
    var ans = confirm(`Bạn có chắc muốn xóa đơn hàng [${code}] khỏi danh sách giao hàng không?`);
    if (ans) {
        var el = $($(event.target).parent()).parent();
        console.log(el);
        $(el).remove();
    }
}

//Sự kiện xác nhận giao hàng
function Delivery(deliveryStatus) {
    var ans = confirm(`Bạn có chắc muốn ${deliveryStatus == 1 ? "giao hàng" : "hủy giao hàng"} danh sách đơn hàng không?`);
    if (ans) {
        var listOrder = $('[id^="HistoryOrder_TienKhachTra_"]').map(function () {
            var idOrder = $(this).attr('id').substring($(this).attr('id').lastIndexOf('_') + 1);
            var obj = {
                Id: parseInt(idOrder),
                DeliveryStatus: deliveryStatus,
                TienKhachTra: parseFloat($(this).val())
            };
            return obj;
        }).get();

        //console.log(listOrder);
        $.ajax({
            url: '/HistoryOrder/Delivery',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(listOrder),
            contentType: 'application/json',
            success: function (result) {
                if (parseInt(result) > 0) {
                    GetAllOrder();
                    $('#selectall').prop('checked', false);
                    $('#deliveryModal').modal('hide');
                } else {
                    alert(result);
                }
            },
            error: function (err) {
                alert(err.responseText);
            }
        })
    }

}