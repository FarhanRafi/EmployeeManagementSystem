$(function () {
  loadDepartments("departmentFilter");

  var dataTable = new DataTable("#employee-table", {
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
      error: function (xhr, status, error) {
        const response = xhr.responseJSON;
        const statusCode = xhr.status;

        if (statusCode === 400) {
          alert("Bad request: " + response.Error);
        } else if (statusCode === 404) {
          alert("Resource not found: " + response.Error);
        } else if (statusCode === 500) {
          alert("Internal Server Error: " + response.Error);
        } else {
          alert("An unexpected error occurred. Please try again later.");
        }

        console.error(`Error ${statusCode}: ${response.Error}`);
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

// load departments and pre-select the current department
function loadDepartments(htmlId, selectedDepartmentId) {
  $.ajax({
    url: baseUrl + "/api/departments",
    method: "GET",
    success: function (res) {
      const departments = res.data;
      const departmentSelect = $(`#${htmlId}`);
      departmentSelect.empty();
      departmentSelect.append('<option value="">Select Department</option>');

      departments.forEach((department) => {
        if (selectedDepartmentId == department.id) {
          departmentSelect.append($("<option></option>").attr("value", department.id).text(department.name).attr("selected", "selected"));
        } else {
          departmentSelect.append($("<option></option>").attr("value", department.id).text(department.name));
        }
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

var isEditMode = false; // Flag to determine if we are in edit mode

// Function to open the modal in Add mode (clear form, change button text)
function openAddEmployeeModal() {
  isEditMode = false; // Set to Add mode
  loadDepartments("departmentId");
  $("#employeeForm")[0].reset(); // Reset the form

  $("#employeeModalLabel").text("Add Employee");
  $("#submitButton").text("Create");

  $("#employeeId").val(""); // Clear the hidden employee ID
  $("#employeeModal").modal("show");
}

// Function to open the modal in Edit mode (populate form, change button text)
function editEmployee(id) {
  isEditMode = true; // Set to Edit mode

  $.ajax({
    url: baseUrl + "/api/employees/" + id,
    method: "GET",
    success: function (res) {
      const employee = res.employee;

      $("#employeeId").val(res.id);
      $("#name").val(employee.name);
      $("#email").val(employee.email);
      $("#phone").val(employee.phone);
      $("#position").val(employee.position);

      const formattedDate = employee.joiningDate.split("T")[0];
      $("#joiningDate").val(formattedDate);
      $("#isActive").prop("checked", employee.isActive);

      $("#employeeModalLabel").text("Edit Employee");
      $("#submitButton").text("Update");

      loadDepartments("departmentId", employee.departmentId);

      $("#employeeModal").modal("show");
    },
    error: function (xhr, textStatus, errorThrown) {
      const response = xhr.responseJSON || {};
      const errorMessage = response.error || "An unexpected error occurred.";
      alert("Error loading employee: " + errorMessage);
    },
  });
}

$("#employeeForm").on("submit", function (e) {
  e.preventDefault();
  handleEmployeeFormSubmit();
});

// Handle the form submission (both Add and Edit)
function handleEmployeeFormSubmit() {
  const employeeData = {
    id: Number($("#employeeId").val()) || 0,
    name: $("#name").val(),
    email: $("#email").val(),
    phone: $("#phone").val(),
    position: $("#position").val(),
    joiningDate: $("#joiningDate").val(),
    departmentId: Number($("#departmentId").val()) || 0,
    isActive: $("#isActive").prop("checked"),
  };

  if (isEditMode) {
    $.ajax({
      url: baseUrl + "/api/employees/" + employeeData.id,
      method: "PUT",
      data: JSON.stringify(employeeData),
      contentType: "application/json",
      success: function (res) {
        alert("Employee updated successfully!");
        $("#employeeModal").modal("hide");
      },
      error: function (xhr, textStatus, errorThrown) {
        debugger;
        alert("Error updating employee: " + errorThrown);
      },
    });
  } else {
    $.ajax({
      url: baseUrl + "/api/employees",
      method: "POST",
      data: JSON.stringify(employeeData),
      contentType: "application/json",
      success: function (res) {
        alert("Employee added successfully!");
        $("#employeeModal").modal("hide");
      },
      error: function (xhr, textStatus, errorThrown) {
        alert("Error adding employee: " + errorThrown);
      },
    });
  }
  dataTable.ajax.reload();
}
