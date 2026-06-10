(function () {
    const form = document.getElementById("warranty-form");
    const container = document.getElementById("warranty-sections");
    if (!form || !container) {
        return;
    }

    form.addEventListener("click", (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        if (target.classList.contains("js-add-warranty-section")) {
            event.preventDefault();
            addSection();
            reindexSections();
            return;
        }

        if (target.classList.contains("js-remove-warranty-section")) {
            event.preventDefault();
            const article = target.closest(".warranty-section-editor");
            if (article && container.querySelectorAll(".warranty-section-editor").length > 1) {
                article.remove();
                reindexSections();
            }
        }
    });

    function addSection() {
        const index = container.querySelectorAll(".warranty-section-editor").length;
        const article = document.createElement("article");
        article.className = "warranty-section-editor";
        article.dataset.sectionIndex = String(index);
        article.innerHTML =
            '<div class="warranty-section-editor-head">' +
            "<h3>Mục " +
            (index + 1) +
            "</h3>" +
            '<button type="button" class="btn btn-sm js-remove-warranty-section">Xóa mục</button>' +
            "</div>" +
            '<div class="form-group"><label>Tiêu đề mục</label>' +
            '<input name="Sections[' +
            index +
            '].Title" placeholder="VD: 1. Phạm vi bảo hành" /></div>' +
            '<div class="form-group"><label>Kiểu hiển thị</label>' +
            '<select name="Sections[' +
            index +
            '].Format">' +
            '<option value="paragraph">Đoạn văn</option>' +
            '<option value="list">Danh sách (mỗi dòng một ý)</option>' +
            "</select></div>" +
            '<div class="form-group"><label>Nội dung</label>' +
            '<textarea name="Sections[' +
            index +
            '].Body" rows="5"></textarea></div>';
        container.appendChild(article);
    }

    function reindexSections() {
        const sections = Array.from(container.querySelectorAll(".warranty-section-editor"));
        sections.forEach((section, sectionIndex) => {
            section.dataset.sectionIndex = String(sectionIndex);
            const heading = section.querySelector("h3");
            if (heading) {
                heading.textContent = "Mục " + (sectionIndex + 1);
            }

            const titleInput = section.querySelector('input[name$=".Title"]');
            const formatSelect = section.querySelector('select[name$=".Format"]');
            const bodyTextarea = section.querySelector('textarea[name$=".Body"]');

            if (titleInput) {
                titleInput.name = "Sections[" + sectionIndex + "].Title";
            }
            if (formatSelect) {
                formatSelect.name = "Sections[" + sectionIndex + "].Format";
            }
            if (bodyTextarea) {
                bodyTextarea.name = "Sections[" + sectionIndex + "].Body";
            }
        });
    }
})();
