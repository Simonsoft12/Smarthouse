

function toastrConfiguration(status, message) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };

        if (message !== null) {
            if (status === "Success") {
                toastr.success(message);
            } else if (status === "Error") {
                toastr.error(message);
            } else if (status === "Info") {
                toastr.info(message);
            } else if (status === "Warning") {
                toastr.warning(message);
            }
        }
    }


var interval = setInterval(function () {
    // Pobranie aktualnej godziny na dolnym pasku
    var momentNow = moment();
    $('#date-part').html(momentNow.format('DD.MM.YYYY').toUpperCase() + ' ' + momentNow.format('HH:mm'));
}, 1000); // co 1 sekundę