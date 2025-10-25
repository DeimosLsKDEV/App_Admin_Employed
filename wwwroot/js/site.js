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

function isValid(value) {
    return value && value.trim() !== "" && value.toLowerCase() !== "none";
}


function CallDeleteObject(
    nameObject,
    valueRepeatDelete,
    urlDelete,
    contenedorOpcional
) {
    $(".modalTitleEmpleado").html(`Eliminar ${nameObject}.`)
    $('#contenedor-delete').empty().html(
        `
        <p>¿Deseas eliminar "${nameObject}"?. Este cambio es inreversible y eliminara toda la información del mismo</p>
        <p>Paa continua escribe exactamente <b class="text-danger">"${valueRepeatDelete}"</b> para continuar</p>
        <input 
            class="form-control" 
            id="input-delete-global"
            name="input-delete-global"
            type="text"
            data-valuecomplete="${valueRepeatDelete}"
            oninput = "ValidatorsInput(this),ValidatorInputDeleteGlobal(this)",
            onblur = "ValidatorsInput(this),ValidatorInputDeleteGlobal(this)" 
        />
        <label data-valmsg-for="input-delete-global" class="text-danger"></label>
        <br />
        <button id="btn-delete-global-object" data-url="${urlDelete}" data-contentforurl="${contenedorOpcional}" disabled class="btn btn-danger" style="width: 100%"><i class="fa fa-trash"></i></button>
        `
    )
    $('#modalDelete').modal('show');

     $("#btn-delete-global-object").click(function(){
            if (isValid(this.dataset.url)) {
                $.ajax({
                    url: this.dataset.url,
                    cache: false,
                    method: "POST",
                    dataType: "html",
                    beforeSend: function () {
                        $("#global-loader").fadeIn(200);
                    },
                    success: function (html) {
                        showNotification("Se ha eliminado el registro correctamente","success")
                        $('#modalDelete').modal('hide');
                    },
                    error: function (xhr, status, error) {
                        showNotification("Error en la petición AJAX:"+error,"error")
                        console.error("Error en la petición AJAX:", error);
                    },
                    complete: function () {
                        $("#global-loader").fadeOut(200);
                    }
                });
            } else {
                console.error(
                    `Alguno de los siguientes datos no contiene valores válidos: Url: ${this.dataset.url}, Contenedor: ${this.dataset.contentforurl}`
                );
            }
        })

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

function showNotification(message, type = "message", timeout = 5000) {
    const panel = document.getElementById("notification-panel");
    const notif = document.createElement("div");
    notif.classList.add("notification");
    notif.classList.add(`notification_${type}`);
    notif.style.padding = "10px";
    notif.style.marginBottom = "10px";
    notif.style.borderRadius = "5px";
    notif.style.opacity = "1";
    background = "";
    color = "#000000";

    switch(type){
        case "error":{
            message = `ERROR: ${message}`
            background = "#E06C6C"
        }
        case "message":{
            background = "#7A80FA"
        }
        case "success":{
            background = "#33DE81"
        }
    }

    notif.innerText = message;
    
    // Estilos básicos
    notif.style.background = background;
    notif.style.color = color;
    notif.style.boxShadow = "0 2px 6px rgba(0,0,0,0.2)";
    notif.style.transition = "opacity 0.5s";
    panel.appendChild(notif);
    // Quitar tras X segundos
    setTimeout(() => {
        notif.style.opacity = "0";
        setTimeout(() => panel.removeChild(notif), 500);
    }, timeout);
}