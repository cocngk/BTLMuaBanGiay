document.addEventListener("DOMContentLoaded", function () {
    var page = document.querySelector(".chi-tiet-trang");
    if (!page) return;

    var sizeField = page.querySelector("input[type='hidden'][id$='hfSize']");
    var quantity = page.querySelector("input.o-so-luong");
    var sizeButtons = page.querySelectorAll(".nut-size");

    sizeButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            sizeButtons.forEach(function (item) {
                item.classList.remove("dang-chon");
                item.setAttribute("aria-pressed", "false");
            });

            button.classList.add("dang-chon");
            button.setAttribute("aria-pressed", "true");
            if (sizeField) sizeField.value = button.getAttribute("data-size") || "";
        });
    });

    page.querySelectorAll("[data-qty-step]").forEach(function (button) {
        button.addEventListener("click", function () {
            if (!quantity || quantity.disabled) return;

            var minimum = parseInt(quantity.min, 10) || 1;
            var maximum = parseInt(quantity.max, 10) || 99;
            var current = parseInt(quantity.value, 10) || minimum;
            var step = parseInt(button.getAttribute("data-qty-step"), 10) || 0;
            quantity.value = Math.min(maximum, Math.max(minimum, current + step));
            quantity.dispatchEvent(new Event("change", { bubbles: true }));
        });
    });

    if (quantity) {
        quantity.addEventListener("change", function () {
            var minimum = parseInt(quantity.min, 10) || 1;
            var maximum = parseInt(quantity.max, 10) || 99;
            var value = parseInt(quantity.value, 10);
            if (!Number.isFinite(value)) value = minimum;
            quantity.value = Math.min(maximum, Math.max(minimum, value));
        });
    }

    var mainImage = page.querySelector(".anh-san-pham-lon");
    var imageWrap = page.querySelector(".anh-lon-wrap");
    var thumbnails = page.querySelectorAll(".anh-nho-nut");

    if (mainImage && thumbnails.length) {
        thumbnails.forEach(function (thumbnail) {
            thumbnail.addEventListener("click", function () {
                var largeImage = thumbnail.getAttribute("data-large");
                if (largeImage) mainImage.src = largeImage;

                thumbnails.forEach(function (item) {
                    item.classList.remove("dang-chon");
                    item.setAttribute("aria-pressed", "false");
                });
                thumbnail.classList.add("dang-chon");
                thumbnail.setAttribute("aria-pressed", "true");
            });
        });
    }

    if (mainImage && imageWrap) {
        imageWrap.addEventListener("mousemove", function (event) {
            var bounds = imageWrap.getBoundingClientRect();
            var x = ((event.clientX - bounds.left) / bounds.width) * 100;
            var y = ((event.clientY - bounds.top) / bounds.height) * 100;
            mainImage.style.transformOrigin = x + "% " + y + "%";
        });
    }
});
