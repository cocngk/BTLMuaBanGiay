(function () {
    'use strict';

    var fields = Array.prototype.slice.call(
        document.querySelectorAll('.auth-form .form-input-pill[data-validation]')
    );
    if (!fields.length) return;

    var form = fields[0].form;
    if (form) form.noValidate = true;

    function messageFor(input) {
        var value = input.value;
        var trimmed = value.trim();
        var rule = input.getAttribute('data-validation');

        if (!trimmed) {
            return input.closest('.form-group-item').querySelector('label').textContent.trim() + ' không được để trống.';
        }

        if (rule === 'username' && trimmed.length < 3) return 'Tên đăng nhập phải có ít nhất 3 ký tự.';
        if (rule === 'email' && !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(trimmed)) return 'Địa chỉ email không hợp lệ.';
        if (rule === 'phone' && !/^\d{9,11}$/.test(trimmed)) return 'Số điện thoại phải gồm từ 9 đến 11 chữ số.';
        if (rule === 'password' && value.length < 6) return 'Mật khẩu phải có ít nhất 6 ký tự.';
        if (rule === 'confirm-password') {
            var password = document.getElementById(input.getAttribute('data-match'));
            if (password && value !== password.value) return 'Hai lần nhập mật khẩu không giống nhau.';
        }
        if (rule === 'login-identifier' && trimmed.indexOf('@') >= 0 && !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(trimmed)) {
            return 'Vui lòng nhập địa chỉ email hợp lệ.';
        }
        return '';
    }

    function validate(input, force) {
        if (!force && input.dataset.touched !== 'true') return true;
        var group = input.closest('.form-group-item');
        var message = group.querySelector('.input-validation-message');
        var text = messageFor(input);
        message.textContent = text;
        input.setAttribute('aria-invalid', text ? 'true' : 'false');
        return !text;
    }

    fields.forEach(function (input) {
        var message = input.closest('.form-group-item').querySelector('.input-validation-message');
        if (message.textContent.trim()) input.setAttribute('aria-invalid', 'true');

        input.addEventListener('input', function () {
            input.dataset.touched = 'true';
            validate(input, true);
            if (input.getAttribute('data-validation') === 'password') {
                var confirmation = fields.find(function (candidate) {
                    return candidate.getAttribute('data-validation') === 'confirm-password';
                });
                if (confirmation && confirmation.dataset.touched === 'true') validate(confirmation, true);
            }
        });

        input.addEventListener('blur', function () {
            input.dataset.touched = 'true';
            validate(input, true);
        });
    });

    if (form) {
        form.addEventListener('submit', function (event) {
            var firstInvalid = null;
            fields.forEach(function (input) {
                input.dataset.touched = 'true';
                if (!validate(input, true) && !firstInvalid) firstInvalid = input;
            });
            if (firstInvalid) {
                event.preventDefault();
                firstInvalid.focus();
            }
        });
    }
}());
