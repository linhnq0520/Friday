(function () {
    const lightbox = document.getElementById("gallery-lightbox");
    if (!lightbox) return;

    const image = document.getElementById("gallery-lightbox-image");
    const caption = document.getElementById("gallery-lightbox-caption");
    const closeBtn = lightbox.querySelector(".gallery-lightbox-close");
    const prevBtn = lightbox.querySelector(".gallery-lightbox-prev");
    const nextBtn = lightbox.querySelector(".gallery-lightbox-next");
    const photos = Array.from(document.querySelectorAll("[data-gallery] .gallery-photo"));
    let currentIndex = 0;

    function show(index) {
        if (!photos.length) return;
        currentIndex = (index + photos.length) % photos.length;
        const photo = photos[currentIndex];
        image.src = photo.dataset.gallerySrc || "";
        image.alt = photo.dataset.galleryTitle || "";
        caption.textContent = photo.dataset.galleryTitle || "";
        lightbox.hidden = false;
        document.body.classList.add("gallery-lightbox-open");
    }

    function hide() {
        lightbox.hidden = true;
        document.body.classList.remove("gallery-lightbox-open");
        image.removeAttribute("src");
    }

    photos.forEach((photo, index) => {
        photo.addEventListener("click", () => show(index));
    });

    closeBtn?.addEventListener("click", hide);
    prevBtn?.addEventListener("click", () => show(currentIndex - 1));
    nextBtn?.addEventListener("click", () => show(currentIndex + 1));

    lightbox.addEventListener("click", (event) => {
        if (event.target === lightbox) hide();
    });

    document.addEventListener("keydown", (event) => {
        if (lightbox.hidden) return;
        if (event.key === "Escape") hide();
        if (event.key === "ArrowLeft") show(currentIndex - 1);
        if (event.key === "ArrowRight") show(currentIndex + 1);
    });
})();
