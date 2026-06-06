(function () {
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        return;
    }

    const container = document.querySelector(".float-actions");
    const bookWrap = document.querySelector(".float-book-wrap");
    const phoneBtn = document.querySelector(".float-social-phone");

    if (!container || !bookWrap) {
        return;
    }

    const INITIAL_DELAY_MS = 8000;
    const INTERVAL_MS = 12000;
    const ATTENTION_MS = 1600;
    const MAX_CYCLES = 5;

    let cycles = 0;
    let intervalId = null;
    let stopped = false;

    function triggerAttention() {
        if (stopped || cycles >= MAX_CYCLES) {
            stop();
            return;
        }

        cycles += 1;
        bookWrap.classList.add("is-attention");
        phoneBtn?.classList.add("is-attention");

        window.setTimeout(() => {
            bookWrap.classList.remove("is-attention");
            phoneBtn?.classList.remove("is-attention");
        }, ATTENTION_MS);
    }

    function stop() {
        stopped = true;
        if (intervalId !== null) {
            window.clearInterval(intervalId);
            intervalId = null;
        }
        bookWrap.classList.remove("is-attention");
        phoneBtn?.classList.remove("is-attention");
    }

    container.addEventListener("click", stop);

    window.setTimeout(() => {
        if (stopped) {
            return;
        }

        triggerAttention();
        intervalId = window.setInterval(triggerAttention, INTERVAL_MS);
    }, INITIAL_DELAY_MS);
})();
