$(function () {
  loadDepartments("departmentFilter");

  initEmployeeModal();

  const dataTable = new DataTable("#employee-table", {
    processing: true, // Show a loader while processing
    serverSide: true, // Enable server-side processing
    // paging: true, // Use DataTable's pagination
    ordering: false, // Enable column-based sorting
    searching: false, // Enable search functionality
    lengthMenu: [5, 10, 25], // Rows per page
    pageLength: 5, // Default rows per page
    ajax: {
      url: baseUrl + "/api/employees",
      type: "GET",
      data: function (d) {
        // Map DataTables params (start, length, search, etc.) to backend query
        const filters = {
          page: Math.floor(d.start / d.length) + 1,
          pageSize: d.length,
          name: $("#nameFilter").val(),
          position: $("#positionFilter").val(),
          departmentId: $("#departmentFilter").val() ? parseInt($("#departmentFilter").val()) : 0,
          minScore: $("#minScoreFilter").val() ? parseInt($("#minScoreFilter").val()) : 0,
          maxScore: $("#maxScoreFilter").val() ? parseInt($("#maxScoreFilter").val()) : 0,
        };
        return filters;
      },
      dataSrc: function (json) {
        // Map backend response to DataTables format
        return json.data;
      },
      error: function (xhr) {
        alert("Error loading employees");
        console.error(xhr.responseText);
      },
    },
    columns: [
      { data: "name" },
      { data: "email" },
      { data: "phone" },
      { data: "department" },
      { data: "joiningDate" },
      { data: "position" },
      { data: "performanceScore" },
      {
        data: "status",
        render: function (data) {
          return data ? "Active" : "Inactive";
        },
      },
      {
        data: "id",
        render: function (data) {
          return `
            <button class="btn btn-sm btn-warning" onclick="editEmployee(${data})">Edit</button>
            <button class="btn btn-sm btn-danger" onclick="deleteEmployee(${data})">Delete</button>`;
        },
      },
    ],
  });

  $("#filterButton").on("click", function () {
    dataTable.ajax.reload();
  });
});

function loadDepartments(id) {
  $.ajax({
    url: baseUrl + "/api/departments",
    method: "GET",
    success: function (res) {
      const departments = res.data;
      const departmentSelect = $(`#${id}`);
      departments.forEach((department) => {
        departmentSelect.append($("<option></option>").attr("value", department.id).text(department.name));
      });
    },
    error: function (jqXHR, textStatus, errorThrown) {
      console.error("Error loading departments:", textStatus, errorThrown);
      alert("Error loading departments");
      $(`#${id}`).empty();
      $(`#${id}`).append('<option value="">Failed to load departments</option>');
    },
  });
}

function initEmployeeModal() {
  loadDepartments("departmentId");

  // Attach form submission handler
  $("#employeeForm").on("submit", function (e) {
    e.preventDefault();
    handleEmployeeFormSubmit();
  });
}

function handleEmployeeFormSubmit() {
  const formData = {
    name: $("#name").val(),
    email: $("#email").val(),
    phone: $("#phone").val(),
    position: $("#position").val(),
    joiningDate: new Date($("#joiningDate").val()).toISOString(), // Convert to UTC
    departmentId: parseInt($("#departmentId").val(), 10),
    isActive: $("#isActive").is(":checked"),
  };

  $.ajax({
    url: baseUrl + "/api/employees",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(formData),
    success: function (res) {
      alert(`Employee added successfully!\nEmployee Name: ${res.name}\nEmployee Email: ${res.email}`);
      location.reload();
    },
    error: function (xhr) {
      alert("Failed to add employee");
      console.log("Failed to add employee: " + xhr.responseText);
    },
  });
}

function loadEmployees() {
  $.ajax({
    url: baseUrl + "/api/employees",
    method: "GET",
    success: function (res) {
      // console.log(res);
    },
    error: function (jqXHR, textStatus, errorThrown) {
      console.error("Error loading employess:", textStatus, errorThrown);
      alert("Error loading employees");
    },
  });
}
