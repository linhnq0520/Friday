(function () {
    const form = document.getElementById("price-list-form");
    if (!form) {
        return;
    }

    form.addEventListener("click", function (event) {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        if (target.classList.contains("js-add-length")) {
            addLengthRow();
            return;
        }

        if (target.classList.contains("js-remove-length")) {
            target.closest(".length-row")?.remove();
            reindexLengthGuide();
            return;
        }

        if (target.classList.contains("js-add-group")) {
            const column = Number.parseInt(target.dataset.column ?? "0", 10);
            addGroup(column);
            return;
        }

        if (target.classList.contains("js-remove-group")) {
            target.closest(".price-group-editor")?.remove();
            reindexGroups();
            return;
        }

        if (target.classList.contains("js-add-item")) {
            const group = target.closest(".price-group-editor");
            if (group) {
                addItemRow(group);
            }
            return;
        }

        if (target.classList.contains("js-remove-item")) {
            target.closest(".price-item-row")?.remove();
            reindexGroups();
        }
    });

    form.addEventListener("submit", function () {
        reindexLengthGuide();
        reindexGroups();
    });

    function addLengthRow() {
        const tbody = document.querySelector("#length-guide-table tbody");
        if (!tbody) {
            return;
        }

        const row = document.createElement("tr");
        row.className = "length-row";
        row.innerHTML =
            '<td><input name="" placeholder="S" /></td>' +
            '<td><input name="" placeholder="Mô tả" /></td>' +
            '<td><button type="button" class="btn btn-sm js-remove-length">Xóa</button></td>';
        tbody.appendChild(row);
        reindexLengthGuide();
    }

    function reindexLengthGuide() {
        const rows = form.querySelectorAll(".length-row");
        rows.forEach(function (row, index) {
            const inputs = row.querySelectorAll("input");
            if (inputs[0]) {
                inputs[0].name = "LengthGuide[" + index + "].Size";
            }
            if (inputs[1]) {
                inputs[1].name = "LengthGuide[" + index + "].Description";
            }
        });
    }

    function addGroup(columnIndex) {
        const column = form.querySelector('.price-list-column[data-column="' + columnIndex + '"]');
        if (!column) {
            return;
        }

        const article = document.createElement("article");
        article.className = "price-group-editor";
        article.innerHTML =
            '<input type="hidden" class="group-column-index" value="' + columnIndex + '" />' +
            '<input type="hidden" class="group-sort-order" value="0" />' +
            '<div class="price-group-editor-head">' +
            '<div class="form-group"><label>Tên nhóm</label>' +
            '<input class="group-title" placeholder="VD: Cắt tóc" /></div>' +
            '<button type="button" class="btn btn-sm js-remove-group">Xóa nhóm</button></div>' +
            '<table class="admin-table"><thead><tr><th>Dịch vụ</th><th>Giá</th><th></th></tr></thead>' +
            '<tbody class="price-items-body"></tbody></table>' +
            '<button type="button" class="btn btn-sm js-add-item">+ Thêm dòng giá</button>';
        column.appendChild(article);
        addItemRow(article);
        reindexGroups();
    }

    function addItemRow(group) {
        const tbody = group.querySelector(".price-items-body");
        if (!tbody) {
            return;
        }

        const row = document.createElement("tr");
        row.className = "price-item-row";
        row.innerHTML =
            '<td><input name="" placeholder="Tên dịch vụ" /></td>' +
            '<td><input name="" placeholder="VD: 350 hoặc S 800 · M 1.000" /></td>' +
            '<td><button type="button" class="btn btn-sm js-remove-item">Xóa</button></td>';
        tbody.appendChild(row);
        reindexGroups();
    }

    function reindexGroups() {
        const groups = form.querySelectorAll(".price-group-editor");
        groups.forEach(function (group, groupIndex) {
            const columnInput =
                group.querySelector('input[name*="ColumnIndex"]') ??
                group.querySelector(".group-column-index");
            const sortInput = group.querySelector(".group-sort-order");
            const titleInput =
                group.querySelector('input[name*="Title"]') ?? group.querySelector(".group-title");

            const columnSection = group.closest(".price-list-column");
            const columnIndex = columnSection
                ? Number.parseInt(columnSection.dataset.column ?? "0", 10)
                : 0;

            if (columnInput) {
                columnInput.name = "Groups[" + groupIndex + "].ColumnIndex";
                columnInput.value = String(columnIndex);
            }

            if (sortInput) {
                sortInput.name = "Groups[" + groupIndex + "].SortOrder";
                sortInput.value = String(
                    Array.from(columnSection?.querySelectorAll(".price-group-editor") ?? []).indexOf(
                        group
                    )
                );
            }

            if (titleInput) {
                titleInput.name = "Groups[" + groupIndex + "].Title";
            }

            const itemRows = group.querySelectorAll(".price-item-row");
            itemRows.forEach(function (row, itemIndex) {
                const inputs = row.querySelectorAll("input");
                if (inputs[0]) {
                    inputs[0].name = "Groups[" + groupIndex + "].Items[" + itemIndex + "].Name";
                }
                if (inputs[1]) {
                    inputs[1].name = "Groups[" + groupIndex + "].Items[" + itemIndex + "].Price";
                }
            });
        });
    }
})();
