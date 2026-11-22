// Onay istenen formlar: <form data-confirm="Emin misiniz?">
// Tarayıcının confirm() penceresi bazı ortamlarda otomatik iptal edildiği için Bootstrap modal kullanılır.
document.addEventListener('submit', function (e) {
    var form = e.target;
    var message = form.getAttribute && form.getAttribute('data-confirm');
    if (!message) return;

    e.preventDefault();

    var modalEl = document.getElementById('confirmModal');
    if (!modalEl || !window.bootstrap) {
        // Modal yoksa tarayıcı onayına geri dön
        if (window.confirm(message)) form.submit();
        return;
    }

    document.getElementById('confirmModalMessage').textContent = message;
    var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    var okBtn = document.getElementById('confirmModalOk');

    var onOk = function () {
        okBtn.removeEventListener('click', onOk);
        modal.hide();
        form.submit(); // submit olayını tetiklemez, döngü oluşmaz
    };
    okBtn.addEventListener('click', onOk);
    modalEl.addEventListener('hidden.bs.modal', function () {
        okBtn.removeEventListener('click', onOk);
    }, { once: true });

    modal.show();
});

// Başarı mesajlarını birkaç saniye sonra otomatik kapat
window.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.flash-area .alert-success').forEach(function (el) {
        setTimeout(function () {
            if (window.bootstrap && document.body.contains(el)) {
                bootstrap.Alert.getOrCreateInstance(el).close();
            }
        }, 5000);
    });

    // Karakter sayacı: <textarea data-counter="#hedef" maxlength="..">
    document.querySelectorAll('textarea[data-counter]').forEach(function (ta) {
        var target = document.querySelector(ta.getAttribute('data-counter'));
        if (!target) return;
        var update = function () { target.textContent = ta.value.length; };
        ta.addEventListener('input', update);
        update();
    });
});
