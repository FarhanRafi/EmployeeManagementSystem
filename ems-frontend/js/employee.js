$(function () {
  var table = $("#employee-table").DataTable({
    processing: true, // Show processing indicator
    serverSide: true, // Enable server-side processing
    ajax: function (data, callback, settings) {
      const searchTerm = $("#search-name").val();
      const departmentId = $("#filter-department").val();
      const position = $("#filter-position").val();
      const minScore = $("#min-score").val();
      const maxScore = $("#max-score").val();

      $.ajax({
        url: "/api/employees",
        method: "GET",
        data: {
          page: settings.page + 1, // DataTable is 0-indexed, so we increment by 1
          pageSize: settings.length,
          searchTerm: searchTerm,
          departmentId: departmentId,
          position: position,
          minScore: minScore,
          maxScore: maxScore,
        },
        success: function (data) {
          callback({
            draw: settings.draw,
            recordsTotal: data.TotalCount,
            recordsFiltered: data.TotalCount,
            data: data.Employees,
          });
        },
      });
    },
    columns: [
      { data: "name" },
      { data: "email" },
      { data: "phone" },
      { data: "departmentName" },
      { data: "position" },
      { data: "performanceScore" },
      {
        data: null,
        render: function (data, type, row) {
          return '<button class="btn btn-primary">Edit</button>';
        },
      },
    ],
    pageLength: 10, // Number of rows per page
    pagingType: "simple_numbers", // Pagination style
    dom: "lrtip", // Custom layout for DataTable
  });

  // Trigger table reload when filters change
  $("#search-name").on("keyup", function () {
    table.ajax.reload();
  });
  $("#filter-department, #filter-position, #min-score, #max-score").on("change", function () {
    table.ajax.reload();
  });
});
