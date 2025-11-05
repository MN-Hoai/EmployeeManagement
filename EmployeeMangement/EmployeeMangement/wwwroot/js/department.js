$(function () {

    let currentDeptId = null;


   

  

    //Lấy tên
    function loadDepartment(filters) {
        $.ajax({
            url: '/api/departments/name',
            type: 'GET',
            data: filters,
            success: function (res) {
                console.log("API Response:", res);

                if (res.success && res.data && res.data.length > 0) {
                    $("#noData").addClass("d-none");
                    renderDepartmentList(res.data);
                } else {
                    $("#departmentList").empty();
                    $("#noData").removeClass("d-none");
                }
            },
            error: function (xhr) {
                toastr.error("Server error: " + xhr.status);
            }
        });
    }

    //Hiển thị
    function renderDepartmentList(items) {
        var $list = $("#departmentList");
        $list.empty();

        items.forEach(function (item, index) {
            // Phân biệt trạng thái
            const statusBadge = item.status === 1
                ? '<span class="badge bg-success">Kích hoạt</span>'
                : '<span class="badge bg-warning text-dark">Tạm khóa</span>';

            const html = `
            <tr>
                <td class="text-center"><input type="checkbox" class="form-check-input"></td>
                <td><span class="fw-bold text-primary">${index + 1}</span></td>
                <td><span class="fw-medium">PB00${item.id}</span></td>
                <td><span class="fw-medium">${item.name}</span></td>
             <td class="text-center">
                <span class="fw-medium text-dark">
                    ${item.managerName || "Chưa có"}
                </span>
            </td>

             <td class="text-center">
                <i class="ri-team-line text-primary me-1"></i>
                <span class="fw-semibold">${item.employeeCount ?? 0}</span>
            </td>
                <td class="text-center">
                    ${statusBadge}
                </td>
                <td class="text-center">
                    <div class="dropup dropdown-action">
                        <a href="#" class="btn btn-soft-primary btn-sm dropdown" data-bs-toggle="dropdown" aria-expanded="false">
                            <i class="ri-more-2-fill"></i>
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end">
                            <li>
                               <a href="#" class="dropdown-item view-item-btn text-primary" data-id="${item.id}">
                                    <i class="ri-eye-fill fs-16"></i> Xem chi tiết 
                                </a>

                            </li>
                            <li>
                                <a href="#" class="dropdown-item edit-item-btn text-warning" data-id="${item.id}">
                                    <i class="ri-edit-fill fs-16"></i> Chỉnh sửa
                                </a>
                            </li>
                            <li>
                                <a href="#" class="dropdown-item delete-item-btn text-danger" data-id="${item.id}">
                                     <i class="ri-delete-bin-5-fill fs-16"></i> Xóa bỏ
                                </a>
                            </li>
                        </ul>
                    </div>
                </td>
            </tr>
        `;
            $list.append(html);
        });
    }

    //Xem chi tiết
    $("#content-main").on("click", ".view-item-btn", async function (e) {
        e.preventDefault();
        const id = $(this).data("id");

        if (!id) {
            toas.error = "Không xác định được Id phòng ban";
            return;
        }

        try {
            const res = await $.ajax({
                url: `/api/department/view/${id}`,
                type: "GET",
                dataType: "json"
            });

            if (res.success && res.data) {
                const d = res.data;

                $("#detailDepartmentCode").val(d.code || "—");
                $("#detailDepartmentName").val(d.name || "-");
                $("#detailManagerName").val(d.manager?.fullname || "Chưa có");
                $("#detailJobPositionName").val(d.jobPosition?.name || "—");
                $("#detailStatusText").val(d.status === 1 ? "Kích hoạt" : "Tạm khóa");


                $("#detailCreatedBy").val(d.createBy ? d.createBy.fullName : "—");
                $("#detailCreatedDate").val(d.createDate ? new Date(d.createDate).toLocaleString() : "—");
                $("#detailUpdatedBy").val(d.updateBy ? d.updateBy.fullName : "-");
                $("#detailUpdatedDate").val(d.updatedDate ? new Date(d.updatedDate).toLocaleString() : "-");

                const modal = new bootstrap.Modal(document.getElementById("departmentDetailModal"));
                modal.show();

            }
            else {
                toas.warning(res.message || "Không tìm thấy phòng ban");
            }
        }
            catch (err) {
            console.error("Không thể tìm thấy phòng ban" + err);
            console.error("Không thể kết nối đến server" + err);

            }

        });

    $("#content-main").on("click", ".delete-item-btn", function (e) {
        e.preventDefault();
        const id = $(this).data("id");
        console.log("Click delete, id=", id);

        if (!id) {
            toastr.error("Không có ID để xóa!");
            return;
        }

        if (confirm("Bạn có chắc chắn muốn xóa bài viết này không?")) {
            $.ajax({
                url: "/api/department/delete",
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

    $("#content-main").on("click", "#btnAdd", function () {
        const modal = new bootstrap.Modal(document.getElementById('DepartmentCreateUpdate'), {
        });
        modal.show();
    });


    $("#content-main").on("click", ".view-item-btn", function (e) {
        e.preventDefault();
        currentDeptId = $(this).data("id");

        if (!currentDeptId) {
            toastr.error("Không xác định được ID phòng ban!");
            return;
        }

        const modalEl = document.getElementById("departmentDetailModal");
        const modal = new bootstrap.Modal(modalEl);


    });
















    // Gọi lần đầu
    loadDepartment();





});
