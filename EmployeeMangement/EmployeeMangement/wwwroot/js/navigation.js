$(function () {

    //Các nút chuyển trang
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