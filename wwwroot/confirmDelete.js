function ConfirmDelete(id, isDeleteClicked) {
    var deleteSpan = 'deleteSpan_' + id;
    var confirmSpan = 'confirmSpan_' + id;
    if (isDeleteClicked) {
        $('#' + confirmSpan).show();
        $('#' + deleteSpan).hide();
    } else {
        $('#' + confirmSpan).hide();
        $('#' + deleteSpan).show();
    }
}