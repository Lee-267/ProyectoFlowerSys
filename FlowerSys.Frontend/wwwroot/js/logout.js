function logout() {
    fetch('/Auth/Logout', { method: 'POST' })
        .then(response => response.json())  // Asegúrate de que la respuesta sea JSON
        .then(data => {
            if (data.success) {
                window.location.href = data.redirectUrl;  // Redirigir al login
            }
        })
        .catch(error => console.error("Error al cerrar sesión:", error));
}
