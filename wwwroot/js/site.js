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

function ValidatorsInput(input) {
    let value = input.value.trim();
    let errors = [];

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