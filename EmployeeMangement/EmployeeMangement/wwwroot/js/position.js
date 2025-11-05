$(function () {
    const main_content = $("#content-main");
    let currentJobId = null;

    // Khi click menu Danh sách vị trí công tác
    $("#jobposition-list-link").on("click", function () {
        $.ajax({
            url: '/JobPosition/JobpositionList',
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .done(function (html) {
                main_content.html(html);
                 loadJobPosition();
            })
            .fail(function (xhr) {
                main_content.html('<div class="alert alert-danger">' + (xhr.responseText || 'Không tải được chi tiết') + '</div>');
            });
    });

    // Hàm load danh sách JobPosition
    function loadJobPosition(filters) {
        $.ajax({
            url: '/api/jobposition/name',
            type: 'GET',
            data: filters,
            success: function (res) {
                console.log("API Response:", res);
                if (res.success && res.data && res.data.length > 0) {
                    $("#noData").addClass("d-none");
                    renderJobPositionList(res.data);
                } else {
                    $("#jobPositionList").empty();
                    $("#noData").removeClass("d-none");
                }
            },
            error: function (xhr) {
                toastr.error("Server error: " + xhr.status);
            }
        });
    }

    // Hàm render ra bảng danh sách
    function renderJobPositionList(items) {
        const $list = $("#jobPositionList");
        $list.empty();

        items.forEach((item, index) => {
             const statusBadge = item.status === 1
                ? '<span class="badge bg-success">Kích hoạt</span>'
                : '<span class="badge bg-warning text-dark">Tạm khóa</span>';

            const html = `
            <tr>
                <td class="text-center"><input type="checkbox" class="form-check-input"></td>
                <td><span class="fw-bold text-primary">${String(index + 1).padStart(2, "0")}</span></td>
                <td><span class="fw-medium">${item.code || "VP00" + item.id}</span></td>
                <td><span class="fw-medium text-truncate d-inline-block" style="max-width:140px;">${item.name || ""}</span></td>
                <td><span class="fw-medium text-truncate d-inline-block" style="max-width:120px;">${item.keyword || "-"}</span></td>
                <td><span class="fw-medium text-truncate d-inline-block" style="max-width:150px;">${item.address || ""}</span></td>
                <td class="text-center">${statusBadge}</td>
                <td class="text-center">
                    <div class="dropup dropdown-action">
                        <a href="#" class="btn btn-soft-primary btn-sm dropdown" data-bs-toggle="dropdown">
                            <i class="ri-more-2-fill"></i>
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end shadow-sm">
                            <li>
                                <a href="#" class="dropdown-item viewjob-item-btn text-primary" data-id="${item.id}">
                                    <i class="ri-eye-fill fs-16 me-1"></i> Xem chi tiết
                                </a>
                            </li>
                            <li>
                                <a href="#" class="dropdown-item edit-item-btn text-warning" data-id="${item.id}">
                                    <i class="ri-edit-fill fs-16 me-1"></i> Chỉnh sửa
                                </a>
                            </li>
                            <li>
                                <a href="#" class="dropdown-item delete-jobposition-btn text-danger" data-id="${item.id}">
                                    <i class="ri-delete-bin-5-fill fs-16 me-1"></i> Xóa bỏ
                                </a>
                            </li>
                        </ul>
                    </div>
                </td>
            </tr>`;
            $list.append(html);
        });
    }



    $("#content-main").on("click", ".delete-jobposition-btn", function (e) {
        e.preventDefault();
        const id = $(this).data("id");
        console.log("Click delete, id=", id);

        if (!id) {
            toastr.error("Không có ID để xóa!");
            return;
        }

        if (confirm("Bạn có chắc chắn muốn xóa bài viết này không?")) {
            $.ajax({
                url: "/api/jobposition/delete",
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify({ id: id }),
                success: function (response) {
                    console.log("Response:", response);
                    if (response.success) {
                        toastr.success(response.message);
                        $(e.currentTarget).parents("tr").remove();
                    } else {
                        toastr.warning(response.message);
                    }
                },
                error: function (xhr) {
                    console.log("Error:", xhr);
                    toastr.error("Không thể kết nối tới server!");
                }
            });
        }
    });

    $("#content-main").on("click", ".viewjob-item-btn", function () {
        const modal = new bootstrap.Modal(document.getElementById('jobPositionDetailModal'), {
        });
        modal.show();
    });




     loadJobPosition();
});
