document.addEventListener('DOMContentLoaded', function () {
    var forms = document.querySelectorAll('.delete-form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            var confirmed = window.confirm(
                'Внимание!\n\nУдаление данных является необратимой операцией. ' +
                'Связанные сведения о поставщике будут удалены без возможности восстановления.\n\n' +
                'Продолжить удаление?');
            if (confirmed) {
                form.submit();
            }
        });
    });
});
