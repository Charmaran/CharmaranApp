function changeToDefaultOption() {
    var selectElement = document.getElementById("employeeSelect");
    selectElement.selectedIndex = -1;
}

function selectNewEmployee(index) {
    var selectElement = document.getElementById("employeeSelect");
    selectElement.selectedIndex = index;
}