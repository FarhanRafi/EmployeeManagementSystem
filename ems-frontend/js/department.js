$(function () {
  //   loadDepartments();
  //   $("#search-department").on("keyup", function () {
  //     filterDepartments($(this).val());
  //   });
});

function loadDepartments() {
  $.ajax({
    url: "/api/departments",
    method: "GET",
    success: function (data) {
      displayDepartments(data);
    },
    error: function (error) {
      console.log("Error loading departments:", error);
    },
  });
}

// display departments in the table
function displayDepartments(departments) {
  const tbody = $("#department-table tbody");
  tbody.empty();

  departments.forEach(function (department) {
    const row = `<tr>
            <td>${department.name}</td>
            <td>${department.location}</td>
            <td><button class="btn btn-primary">Edit</button></td>
        </tr>`;
    tbody.append(row);
  });
}
