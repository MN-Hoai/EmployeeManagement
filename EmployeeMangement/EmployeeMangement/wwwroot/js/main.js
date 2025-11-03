
$(function () {
    loadIndex();
    const main_content = $("#content-main");
    $("#employee-list-link").on("click", function () {
       
        $.ajax({
            url: '/Employee/EmployeeList',
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .done(function (html) {
                main_content.html(html);
                
            })
            .fail(function (xhr) {
                main_content.html('<div class="alert alert-danger">' + (xhr.responseText || 'Không tải được chi tiết') + '</div>');
               
            });


    });
    $("#department-list-link").on("click", function () {

        $.ajax({
            url: '/Department/DepartmentList',
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .done(function (html) {
                main_content.html(html);

            })
            .fail(function (xhr) {
                main_content.html('<div class="alert alert-danger">' + (xhr.responseText || 'Không tải được chi tiết') + '</div>');

            });




    });
    $("#jobposition-list-link").on("click", function () {

        $.ajax({
            url: '/JobPosition/JobpositionList',
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .done(function (html) {
                main_content.html(html);

            })
            .fail(function (xhr) {
                main_content.html('<div class="alert alert-danger">' + (xhr.responseText || 'Không tải được chi tiết') + '</div>');

            });
        


    });
   
});
function loadIndex() {
    getDepartments();
    getJobPosition();
    applyFilters(1);
}

// Lấy danh sách tên danh mục trong bộ lọc
function getDepartments() {
    $.ajax({
        url: 'api/departments/name',
        method: 'GET'

    })
        .done(function (res) {
            if (res.success) {
                rederDepartmentOptions(res);
            } else {
                alert("Không có dữ liệu!");
            }
        })
        .fail(function () {
            alert("lỗi");
        });
}

function rederDepartmentOptions(res) {
    var $select = $("#departmentFilter");
    $select.empty();

    $select.append('<option value="">Tất cả</option>');

    res.data.forEach(function (item) {
        $select.append(`<option value="${item.id}">${item.name}</option>`);
    });
}

// Lấy danh sách tên địa chỉ công tác trong bộ lọc
function getJobPosition() {
    $.ajax({
        url: 'api/jobposition/name',
        method: 'GET'

    })
        .done(function (res) {
            if (res.success) {
                rederJobPositionOptions(res);
            } else {
                toastr.warning("Không có dữ liệu!");
              
            }
        })
        .fail(function () {
            toastr.error("Lỗi kết nối server");
        });
}

function rederJobPositionOptions(res) {
    var $select = $("#jobPositionFilter");
    $select.empty();

    $select.append('<option value="">Tất cả</option>');

    res.data.forEach(function (item) {
        $select.append(`<option value="${item.id}">${item.name} - ${item.address}</option>`);
    });
}

// Lấy danh sách danh viên

function loadEmployees(filters) {
    $.ajax({
        url: 'api/employees',
        type: 'GET',
        data: filters,
        success: function (res) {
            console.log("API Response:", res);

            if (res.succsess && res.succsess.result && res.succsess.result.items.length > 0) {

                $("#noData").addClass("d-none"); // Ẩn thông báo "không có dữ liệu"

                const result = res.succsess.result;

                renderEmployeeList(result.items);

                renderPagination(
                    result.currentPage,
                    result.totalRecords,
                    result.pageSize,
                    filters.departmentId,
                    filters.keyword,
                    filters.position,
                    filters.status
                );

            } else {
                $("#employeeList").empty();
                $("#noData").removeClass("d-none");
            }
        
        },
        error: function (xhr) {
            toastr.error("Server error: " + xhr.status);
        }
    });
}


function renderEmployeeList(items) {
    var $list = $("#employeeList");
    $list.empty();

    items.forEach(function (item) {

        const avatar = `https://ui-avatars.com/api/?name=${encodeURIComponent(item.fullname)}&background=4361ee&color=fff&size=60&bold=true`;
        const statusBadge = item.status === 1
            ? '<span class="badge bg-success status-badge">Kích hoạt</span>'
            : '<span class="badge bg-danger status-badge">Không hoạt động</span>';

        const createDate = item.createDate
            ? new Date(item.createDate).toLocaleDateString("vi-VN")
            : "—";

        const keyword = item.keyword || "—";

        var html = `
        <div class="list-group-item employee-item">
            <div class="row align-items-center">
                
                <div class="col-auto">
                    <input type="checkbox" class="form-check-input">
                </div>

                <div class="col-1">
                    <span class="fw-bold text-primary">${item.id}</span>
                </div>

                <div class="col-2">
                    <img src="${avatar}" class="avatar" alt="Avatar">
                </div>

                <div class="col-2">
                    <div class="employee-name">${item.fullname}</div>
                    <div class="info-text">
                        <i class="ri-mail-line text-primary"></i> ${item.email}
                    </div>

                    <div class="d-flex align-items-center gap-2 mt-2 flex-wrap">
                        <i class="ri-phone-line text-success"></i> ${item.phone}
                        ${statusBadge}
                    </div>

                    <div class="d-flex align-items-center mt-2 flex-wrap">
                        <span class="badge bg-light text-dark border">Ngày tạo:</span>
                        <span class="text-muted fw-medium">${createDate}</span>

                        <span class="badge bg-light text-dark border mt-1">Keyword:</span>
                        <span class="text-muted fw-medium">${keyword}</span>
                    </div>
                </div>

                <div class="col-1 text-center fw-medium text-dark">${item.position}</div>
                <div class="col-2 text-center fw-medium text-dark">${item.departmentName}</div>
                <div class="col-2 text-center fw-medium text-dark">${item.jobPositionName}</div>

                <div class="col-1 text-center">
                    <div class="dropup dropdown-action">
                        <a href="#" class="btn btn-soft-primary btn-sm dropdown" data-bs-toggle="dropdown">
                            <i class="ri-more-2-fill"></i>
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end">
                            <li><a href="#" class="dropdown-item view-item-btn text-primary"><i class="ri-eye-fill fs-16"></i> Xem</a></li>
                            <li><a href="#" class="dropdown-item edit-item-btn text-warning"><i class="ri-edit-fill fs-16"></i> Sửa</a></li>
                            <li><a href="#" class="dropdown-item remove-item-btn text-danger"><i class="ri-delete-bin-5-fill fs-16"></i> Xóa</a></li>
                        </ul>
                    </div>
                </div>

            </div>
        </div>`;

        $list.append(html);
    });
}

function renderPagination(
    current,
    total,
    pageSize,
    departmentId,
    keyword,
    position,
    status
) {
    const totalPages = Math.ceil(total / pageSize);
    if (totalPages <= 1) {
        $("#pagination").html("");
        return;
    }

    let html = `<div class="btn-group" role="group">`;

    const isFirst = current === 1;
    const isLast = current === totalPages;

    // 🔹 First & Prev
    html += `
        <label class="btn btn-outline-primary btn-paging ${isFirst ? "disabled" : ""}" data-page="1">« First</label>
        <label class="btn btn-outline-primary btn-paging ${isFirst ? "disabled" : ""}" data-page="${current - 1}">‹ Prev</label>
    `;

    const maxVisible = 5;
    let startPage = Math.max(1, current - Math.floor(maxVisible / 2));
    let endPage = Math.min(totalPages, startPage + maxVisible - 1);
    if (endPage - startPage < maxVisible - 1) {
        startPage = Math.max(1, endPage - maxVisible + 1);
    }

    // 🔹 Nếu có trang đầu ẩn
    if (startPage > 1) {
        html += `
            <label class="btn btn-outline-primary btn-paging" data-page="1">1</label>
            <span class="btn btn-light disabled">...</span>
        `;
    }

    // 🔹 Các trang giữa
    for (let i = startPage; i <= endPage; i++) {
        html += `
            <label class="btn btn-outline-primary btn-paging ${i === current ? 'active' : ''}"
                data-page="${i}"
                data-department="${departmentId || ''}"
                data-keyword="${keyword || ''}"
                data-position="${position || ''}"
                data-status="${status || ''}">
                ${i}
            </label>`;
    }

    // 🔹 Nếu có trang cuối ẩn
    if (endPage < totalPages) {
        html += `
            <span class="btn btn-light disabled">...</span>
            <label class="btn btn-outline-primary btn-paging" data-page="${totalPages}">${totalPages}</label>
        `;
    }

    // 🔹 Next & Last
    html += `
        <label class="btn btn-outline-primary btn-paging ${isLast ? "disabled" : ""}" data-page="${current + 1}">Next ›</label>
        <label class="btn btn-outline-primary btn-paging ${isLast ? "disabled" : ""}" data-page="${totalPages}">Last »</label>
    `;

    html += `</div>`;
    $("#pagination").html(html);

    // ✅ Sự kiện click phân trang
    $("#pagination").off("click", ".btn-paging:not(.disabled)").on("click", ".btn-paging:not(.disabled)", function () {
        const page = $(this).data("page");
        const filterParams = getFilterParams(); // lấy toàn bộ filter hiện tại
        applyFilters(page, filterParams);
        window.scrollTo({ top: 0, behavior: "smooth" });
    });
}


function applyFilters(page = 1) {
    const filters = getFilterData();
    filters.page = page;

    loadEmployees(filters);
}
function getFilterData() {
    return {
        page: getCurrentPage(),
        pageSize: 2, // hoặc lấy từ dropdown nếu có

        keyword: $("#searchBox").val()?.trim() || "",
        departmentId: $("#departmentFilter").val() || "",
        position: $("#positionFilter").val() || "",
        status: $("#statusFilter").val() || ""
    };
}
function getFilterParams() {
    return {
        keyword: $("#keyword").val() || "",
        status: $("#statusFilter").val() || "",
        departmentCode: $("#departmentFilter").val() || "",
        jobPositionCode: $("#jobFilter").val() || ""
    };
}
function getCurrentPage() {
    const currentLabel = $("label[data-page].active, label[data-page].checked");

    if (currentLabel.length === 0) return 1;

    return parseInt(currentLabel.data("page"));
}