(function () {
    const backToTop = document.querySelector(".back-to-top");
    if (!backToTop) {
        return;
    }

    const showAfterPx = 400;

    function updateVisibility() {
        backToTop.classList.toggle("is-visible", window.scrollY > showAfterPx);
    }

    window.addEventListener("scroll", updateVisibility, { passive: true });
    updateVisibility();

    backToTop.addEventListener("click", function () {
        window.scrollTo({ top: 0, behavior: "smooth" });
    });
})();
