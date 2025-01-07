$(function () {
  loadDepartments();

  new DataTable("#employee-table", {
    paging: true,
    ordering: true,
    searching: true,
    lengthMenu: [10, 25, 50],
    order: [],
  });
});

loadDepartments = () => {
  $.ajax({
    url: baseUrl + "/api/departments",
    method: "GET",
    success: function (data) {
      debugger;
      const departments = data.departments;
      const departmentSelect = $("#filter-department");
      departments.forEach((department) => {
        departmentSelect.append($("<option></option>").attr("value", department.id).text(department.name));
      });
    },
    error: function (jqXHR, textStatus, errorThrown) {
      console.error("Error loading departments:", textStatus, errorThrown);
      $("#filter-department").empty();
      $("#filter-department").append('<option value="">Failed to load departments</option>');
    },
  });
};
