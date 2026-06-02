(function () {
    const slider = document.querySelector(".hero-slider");
    if (!slider) {
        return;
    }

    const slides = Array.from(slider.querySelectorAll(".hero-slide"));
    const dots = Array.from(slider.querySelectorAll(".hero-dot"));
    if (slides.length === 0) {
        return;
    }

    let current = 0;
    let timerId = null;
    const intervalMs = 5000;

    function show(index) {
        current = (index + slides.length) % slides.length;
        slides.forEach((slide, i) => slide.classList.toggle("is-active", i === current));
        dots.forEach((dot, i) => dot.classList.toggle("is-active", i === current));
    }

    function next() {
        show(current + 1);
    }

    function startAutoPlay() {
        stopAutoPlay();
        timerId = window.setInterval(next, intervalMs);
    }

    function stopAutoPlay() {
        if (timerId !== null) {
            window.clearInterval(timerId);
            timerId = null;
        }
    }

    dots.forEach((dot) => {
        dot.addEventListener("click", function () {
            const index = Number.parseInt(dot.dataset.slide ?? "0", 10);
            show(index);
            startAutoPlay();
        });
    });

    slider.addEventListener("mouseenter", stopAutoPlay);
    slider.addEventListener("mouseleave", startAutoPlay);

    show(0);
    startAutoPlay();
})();
