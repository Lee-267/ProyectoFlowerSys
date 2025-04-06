// Selección de elementos
let list = document.querySelectorAll(".navigation li");
let toggle = document.querySelector(".toggle");
let navigation = document.querySelector(".navigation");
let main = document.querySelector(".main");
let menuIcon = toggle.querySelector("ion-icon"); // Obtiene el icono dentro de .toggle

// Función para resaltar el elemento activo en el menú
function activeLink() {
    list.forEach((item) => {
        item.classList.remove("hovered");
    });
    this.classList.add("hovered");
}

// Aplica la función cuando se pasa el mouse sobre un elemento
list.forEach((item) => item.addEventListener("mouseover", activeLink));

// Función para abrir/cerrar el menú y cambiar el ícono
toggle.onclick = function () {
    navigation.classList.toggle("active");
    main.classList.toggle("active");

    // Cambia el icono al abrir/cerrar el menú
    if (navigation.classList.contains("active")) {
        menuIcon.setAttribute("name", "close-outline"); // Cambia a "X"
    } else {
        menuIcon.setAttribute("name", "menu-outline"); // Vuelve al menú hamburguesa
    }
};
