(function () {
    'use strict';

    window.moOTimKiemHeader = function (input) {
        var panel = input.parentElement;
        if (panel && panel.classList.contains('search-pill')) {
            panel.setAttribute('data-expanded', 'true');
        }
    };

    window.dongOTimKiemHeaderSauKhiMatFocus = function (input) {
        var panel = input.parentElement;
        if (!panel || !panel.classList.contains('search-pill')) return;

        window.setTimeout(function () {
            if (!panel.contains(document.activeElement)) {
                panel.removeAttribute('data-expanded');
            }
        }, 0);
    };

    window.bamKinhLupHeader = function (button) {
        var panel = button.parentElement;
        if (!panel || panel.getAttribute('data-expanded') === 'true') return true;

        var input = panel.querySelector('input[type="text"]');
        if (input) input.focus();
        return false;
    };
}());
