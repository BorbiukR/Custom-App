$('#EngineVolume').on('change', function () {
    $('#Submit').prop('disabled', !$(this).val());
}).trigger('change');