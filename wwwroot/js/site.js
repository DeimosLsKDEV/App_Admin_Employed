// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function RedirectContentUrl(container, url){
    $.ajax({
        url: url,
        cache: false,
        method: "GET",
        dataType: "html",
        beforeSend: function () {
            $("#global-loader").fadeIn(200);
        },
        success: function (html) {
            $(container).empty().html(html);
        },
        error: function (xhr, status, error) {
            console.error("Error en la petición AJAX:", error);
            $(container).html('<div class="error">❌ Error al cargar</div>');
        },
        complete: function () {
            $("#global-loader").fadeOut(200);
        }
    });
}

function CallDeleteObject(
    valueDelete,
    valueRepeatDelete,
    URLDelete
) {
    $(".modalTitleEmpleado").html("Actualizar Empleado.")
    $('#contenedor-delete').empty().html(

    )

}

function ValidatorInputDeleteGlobal(input) {
    if ($(input).hasClass("is-invalid")) {
        $("#btn-delete-global-object").prop("disabled", true);
    } else {
        $("#btn-delete-global-object").prop("disabled", false);
    }
}

function ValidatorsInput(input) {
    let value = input.value;
    let errors = [];

    // Error with info in data
    if (input.dataset.pattern) {
        let regex = new RegExp(input.dataset.pattern);
        let match = input.dataset.pattern.match(/\[([^\]]+)\]/);
        if (match) {
            let cleanRegex = new RegExp(`[^${match[1]}]`, "g");
            input.value = value.replace(cleanRegex, "");
            value = input.value;
        }

        if (!regex.test(value)) {
            errors.push("El formato no es válido.");
        }
    }

    if (input.dataset.valuecomplete) {
        if (input.value != input.dataset.valuecomplete) {
            errors.push(`El contenido no es valido debe ser "${input.dataset.valuecomplete}"`);
        }
    }

    // Error with info in attributes in html
    if (input.hasAttribute("maxlength")) {
        let max = parseInt(input.getAttribute("maxlength"));
        if (value.length > max) {
            errors.push(`No puede tener más de ${max} caracteres.`);
        }
    }
    if (input.hasAttribute("required") && value === "") {
        errors.push("Este campo es obligatorio.");
    }

    if (input.hasAttribute("minlength")) {
        let min = parseInt(input.getAttribute("minlength"));
        if (value.length < min) {
            errors.push(`Debe tener al menos ${min} caracteres.`);
        }
    }
    if (input.hasAttribute("maxlength")) {
        let max = parseInt(input.getAttribute("maxlength"));
        if (value.length > max) {
            errors.push(`No puede tener más de ${max} caracteres.`);
        }
    }

    let validationMessage = document.querySelector(
        `[data-valmsg-for="${input.name}"]`
    );

    if (errors.length > 0) {
        input.classList.add("is-invalid");
        input.classList.remove("is-valid");

        if (validationMessage) {
            validationMessage.innerHTML = `<ul>${errors.map(e => `<li>${e}</li>`).join("")}</ul>`;
        }

        return { valid: false, errors };
    } else {
        input.classList.remove("is-invalid");
        input.classList.add("is-valid");

        if (validationMessage) {
            validationMessage.innerHTML = "";
        }

        return { valid: true };
    }
}

function showNotification(message, timeout = 5000) {
        const panel = document.getElementById("notification-panel");

        const notif = document.createElement("div");
        notif.className = "notification";
        notif.innerText = message;

        // Estilos básicos
        notif.style.background = "#2196F3";
        notif.style.color = "white";
        notif.style.padding = "10px";
        notif.style.marginBottom = "10px";
        notif.style.borderRadius = "5px";
        notif.style.boxShadow = "0 2px 6px rgba(0,0,0,0.2)";
        notif.style.opacity = "1";
        notif.style.transition = "opacity 0.5s";

        panel.appendChild(notif);

        // Quitar tras X segundos
        setTimeout(() => {
            notif.style.opacity = "0";
            setTimeout(() => panel.removeChild(notif), 500); // esperar la animación
        }, timeout);
    }